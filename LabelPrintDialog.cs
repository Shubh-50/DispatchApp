using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    /// <summary>
    /// Modal dialog shown when a dispatch order is completed or when reprinting a customer label.
    /// Operator fills in required fields; values replace tokens in the CustomerPrn template.
    /// 
    /// Token map (PRN token → source):
    ///   {PART_NUMBER}       → user enters
    ///   {PART_NAME}         → auto from order
    ///   {CUSTOMER_NAME}     → auto from order
    ///   {CUSTOMER_LOCATION} → user enters
    ///   {QUANTITY}          → auto from order (QtyScanned)
    ///   {OPERATOR_NAME}     → auto from order (operator who dispatched)
    ///   {INSPECTOR_NAME}    → auto from Form1 inspector field
    ///   {INVOICE_NO}        → user enters
    ///   {HODEK_PART_NO}     → user enters
    ///   {MFG_DATE}          → auto = today in DD/MM/YYYY
    /// </summary>
    public class LabelPrintDialog : Form
    {
        // ── inputs ───────────────────────────────────────────────────────
        private TextBox txtPartNumber = new TextBox();
        private TextBox txtPartName = new TextBox();
        private TextBox txtCustomerName = new TextBox();
        private TextBox txtCustLocation = new TextBox();
        private TextBox txtQuantity = new TextBox();
        private TextBox txtOperatorName = new TextBox();
        private TextBox txtInspectorName = new TextBox();
        private TextBox txtInvoiceNo = new TextBox();
        private TextBox txtHodekPartNo = new TextBox();
        // MFG Date shown as read-only label (auto = today)
        private Label lblMfgDateValue = new Label();

        private Button btnPrint = new Button();
        private Button btnCancel = new Button();

        private readonly string _printerShareName;
        private readonly string _baseFolder;
        private readonly string _prnContent;
        private readonly string _orderNo;
        private readonly string _mfgDate;   // pre-computed today DD/MM/YYYY

        // ── constructor ──────────────────────────────────────────────────
        public LabelPrintDialog(DispatchOrder order,
            string operatorName, string inspectorName,
            string printerShareName, string baseFolder)
        {
            _printerShareName = printerShareName;
            _baseFolder = baseFolder;
            _orderNo = order.OrderNo;
            _mfgDate = DateTime.Today.ToString("dd/MM/yyyy");  // auto MFG date

            _prnContent = DatabaseHelper.GetCustomerPrnContent(order.CustomerName);
            if (string.IsNullOrWhiteSpace(_prnContent))
            {
                MessageBox.Show(
                    $"No PRN template configured for customer '{order.CustomerName}'.\n" +
                    "Go to Admin → Customer PRN to set up the template.",
                    "PRN Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Load += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
                return;
            }

            // ── Pre-fill auto fields (read-only) ─────────────────────────
            // These come directly from the order token — user cannot change them
            txtPartName.Text = order.PartName ?? "";
            txtCustomerName.Text = order.CustomerName ?? "";
            txtQuantity.Text = order.QtyScanned.ToString();
            txtOperatorName.Text = operatorName ?? "";
            txtInspectorName.Text = inspectorName ?? "";

            BuildUI();
        }

        // ── UI builder ───────────────────────────────────────────────────
        private void BuildUI()
        {
            Text = $"Print Customer Label — {_orderNo}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(248, 250, 255);
            Font = new Font("Segoe UI", 9.5F);

            // ── title banner ─────────────────────────────────────────────
            var banner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(24, 48, 96)
            };
            var lblTitle = new Label
            {
                Text = $"🖨  Customer Label — Order: {_orderNo}",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            banner.Controls.Add(lblTitle);
            Controls.Add(banner);

            // ── help label ───────────────────────────────────────────────
            var lblHelp = new Label
            {
                Text = "Fields marked * are required. Grey fields are auto-filled from the order token.",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(90, 90, 110),
                Location = new Point(16, 54),
                AutoSize = true
            };
            Controls.Add(lblHelp);

            // ── row builder ──────────────────────────────────────────────
            int y = 80;
            const int LBL_X = 16, LBL_W = 160, TXT_X = 182, TXT_W = 320;

            // source tag shown beside auto-filled fields
            void AddRow(string caption, Control ctrl,
                        bool required = false,
                        bool readOnly = false,
                        string sourceNote = "")
            {
                var lbl = new Label
                {
                    Text = caption + (required ? " *" : ""),
                    Location = new Point(LBL_X, y + 5),
                    Size = new Size(LBL_W, 22),
                    Font = new Font("Segoe UI", 9F,
                                    required ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = required ? Color.FromArgb(160, 30, 0) : Color.FromArgb(50, 50, 70)
                };

                ctrl.Location = new Point(TXT_X, y);
                ctrl.Size = new Size(TXT_W, 28);
                ctrl.Font = new Font("Segoe UI", 9F);

                if (ctrl is TextBox tb && readOnly)
                {
                    tb.ReadOnly = true;
                    tb.BackColor = Color.FromArgb(230, 232, 238);
                    tb.ForeColor = Color.FromArgb(70, 70, 90);
                }

                Controls.Add(lbl);
                Controls.Add(ctrl);

                // Small source note to the right
                if (!string.IsNullOrEmpty(sourceNote))
                {
                    var note = new Label
                    {
                        Text = sourceNote,
                        Location = new Point(TXT_X + TXT_W + 6, y + 7),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(130, 130, 160)
                    };
                    Controls.Add(note);
                }

                y += 36;
            }

            // Row 1: Part Number — USER ENTERS (required)
            AddRow("Part Number", txtPartNumber, required: true,
                   sourceNote: "← enter manually");

            // Row 2: Part Name — AUTO from order token (read-only)
            AddRow("Part Name", txtPartName, readOnly: true,
                   sourceNote: "← from order");

            // Row 3: Customer Name — AUTO from order token (read-only)
            AddRow("Customer Name", txtCustomerName, readOnly: true,
                   sourceNote: "← from order");

            // Row 4: Customer Location — USER ENTERS (required)
            AddRow("Customer Location", txtCustLocation, required: true,
                   sourceNote: "← enter manually");

            // Row 5: Quantity — AUTO from order (read-only)
            AddRow("Quantity", txtQuantity, readOnly: true,
                   sourceNote: "← from order");

            // Row 6: Operator Name — AUTO from order (read-only)
            AddRow("Operator Name", txtOperatorName, readOnly: true,
                   sourceNote: "← from order");

            // Row 7: Inspector Name — AUTO from Form1 inspector field (editable if needed)
            AddRow("Inspector Name", txtInspectorName, required: true,
                   sourceNote: "← from inspector field");

            // Row 8: Invoice No — USER ENTERS (required)
            AddRow("Invoice No", txtInvoiceNo, required: true,
                   sourceNote: "← enter manually");

            // Row 9: Hodek Part No — USER ENTERS
            AddRow("Hodek Part No", txtHodekPartNo,
                   sourceNote: "← enter manually");

            // Row 10: MFG Date — AUTO = today (read-only label, not picker)
            var lblDateCaption = new Label
            {
                Text = "MFG Date",
                Location = new Point(LBL_X, y + 5),
                Size = new Size(LBL_W, 22),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(50, 50, 70)
            };
            lblMfgDateValue.Text = _mfgDate;
            lblMfgDateValue.Location = new Point(TXT_X, y + 4);
            lblMfgDateValue.AutoSize = true;
            lblMfgDateValue.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblMfgDateValue.ForeColor = Color.FromArgb(0, 100, 0);
            var lblDateNote = new Label
            {
                Text = "← today (auto)",
                Location = new Point(TXT_X + TXT_W + 6, y + 7),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(130, 130, 160)
            };
            Controls.Add(lblDateCaption);
            Controls.Add(lblMfgDateValue);
            Controls.Add(lblDateNote);
            y += 36;

            // ── buttons ──────────────────────────────────────────────────
            y += 10;
            btnPrint.Text = "🖨  Print Label";
            btnPrint.Location = new Point(16, y);
            btnPrint.Size = new Size(160, 40);
            btnPrint.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPrint.BackColor = Color.FromArgb(0, 120, 215);
            btnPrint.ForeColor = Color.White;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Cursor = Cursors.Hand;
            btnPrint.Click += BtnPrint_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(186, y);
            btnCancel.Size = new Size(100, 40);
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Cursor = Cursors.Hand;

            Controls.Add(btnPrint);
            Controls.Add(btnCancel);
            AcceptButton = btnPrint;
            CancelButton = btnCancel;

            // ── placeholder texts ─────────────────────────────────────────
            txtPartNumber.PlaceholderText = "e.g. PN-12345";
            txtCustLocation.PlaceholderText = "e.g. Pune, MH";
            txtInvoiceNo.PlaceholderText = "Invoice / challan number";
            txtHodekPartNo.PlaceholderText = "Hodek drawing / part no (optional)";

            // ── form size ─────────────────────────────────────────────────
            ClientSize = new Size(660, y + 60);
        }

        // ── print handler ────────────────────────────────────────────────
        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtPartNumber.Text))
            { Warn("Part Number is required."); txtPartNumber.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtCustLocation.Text))
            { Warn("Customer Location is required."); txtCustLocation.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtInspectorName.Text))
            { Warn("Inspector Name is required."); txtInspectorName.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txtInvoiceNo.Text))
            { Warn("Invoice No is required."); txtInvoiceNo.Focus(); return; }

            try
            {
                // Strip // comments from PRN
                string prn = string.Join("\r\n",
                    _prnContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                               .Where(l => !l.TrimStart().StartsWith("//"))) + "\r\n";

                // ── Token replacement ─────────────────────────────────────
                // Matches BOTH the uppercase-underscore tokens in the PRN
                // (e.g. {PART_NUMBER}) AND any CamelCase variants for compatibility.
                prn = prn
                    // Uppercase underscore tokens (your current PRN format)
                    .Replace("{PART_NUMBER}", txtPartNumber.Text.Trim())
                    .Replace("{PART_NAME}", txtPartName.Text.Trim())
                    .Replace("{CUSTOMER_NAME}", txtCustomerName.Text.Trim())
                    .Replace("{CUSTOMER_LOCATION}", txtCustLocation.Text.Trim())
                    .Replace("{QUANTITY}", txtQuantity.Text.Trim())
                    .Replace("{OPERATOR_NAME}", txtOperatorName.Text.Trim())
                    .Replace("{INSPECTOR_NAME}", txtInspectorName.Text.Trim())
                    .Replace("{INVOICE_NO}", txtInvoiceNo.Text.Trim())
                    .Replace("{HODEK_PART_NO}", txtHodekPartNo.Text.Trim())
                    .Replace("{MFG_DATE}", _mfgDate)

                    // CamelCase variants (backwards compatibility)
                    .Replace("{PartNumber}", txtPartNumber.Text.Trim())
                    .Replace("{PartName}", txtPartName.Text.Trim())
                    .Replace("{CustomerName}", txtCustomerName.Text.Trim())
                    .Replace("{CustomerLocation}", txtCustLocation.Text.Trim())
                    .Replace("{Quantity}", txtQuantity.Text.Trim())
                    .Replace("{QtyOrdered}", txtQuantity.Text.Trim())
                    .Replace("{QtyScanned}", txtQuantity.Text.Trim())
                    .Replace("{OperatorName}", txtOperatorName.Text.Trim())
                    .Replace("{InspectorName}", txtInspectorName.Text.Trim())
                    .Replace("{InvoiceNo}", txtInvoiceNo.Text.Trim())
                    .Replace("{HodekPartNo}", txtHodekPartNo.Text.Trim())
                    .Replace("{MFGDate}", _mfgDate)
                    .Replace("{MfgDate}", _mfgDate)

                    // Order reference
                    .Replace("{OrderNo}", _orderNo)
                    .Replace("{ORDER_NO}", _orderNo);

                // Write and send to printer
                string tempPrn = Path.Combine(_baseFolder, "customer_label.prn");
                File.WriteAllText(tempPrn, prn, Encoding.ASCII);

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{_printerShareName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(psi)?.WaitForExit(3000);

                MessageBox.Show(
                    $"Label sent to printer ✅\n\n" +
                    $"Part: {txtPartNumber.Text.Trim()}\n" +
                    $"Customer: {txtCustomerName.Text.Trim()}\n" +
                    $"Invoice: {txtInvoiceNo.Text.Trim()}\n" +
                    $"MFG Date: {_mfgDate}",
                    "Printed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] LabelPrintDialog Error: {ex.Message}\n");
            }
        }

        private static void Warn(string msg) =>
            MessageBox.Show(msg, "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}