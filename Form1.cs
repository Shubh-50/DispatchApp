using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class Form1 : Form
    {
        // ── state ────────────────────────────────────────────────────────
        private string printerShareName = "";
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeApp");

        // REQ-4: Removed shift/total counters from stats panel.
        // These are kept only for CSV export compatibility.
        private int totalCount = 0;

        public string CurrentUser = "";
        private string currentShift = "";
        private string lastShift = "";
        private string mailSentForShift = "";
        private string currentInspector = "";

        // ── dispatch state ───────────────────────────────────────────────
        private DispatchOrder? activeOrder = null;
        private readonly Stopwatch scanTimer = new Stopwatch();

        private System.Windows.Forms.Timer timerClock = new System.Windows.Forms.Timer();

        public Form1(string user)
        {
            InitializeComponent();
            CurrentUser = user;
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            timerClock.Interval = 1000;
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
        }

        // ── timer ────────────────────────────────────────────────────────
        private void TimerClock_Tick(object? sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string newShift = ShiftHelper.GetCurrentShift();
            if (newShift != lastShift)
            {
                if (!string.IsNullOrEmpty(lastShift) && mailSentForShift != newShift)
                {
                    SendShiftReport();
                    mailSentForShift = newShift;
                }
                lastShift = newShift;
                currentShift = newShift;
                lblShift.Text = "Shift: " + newShift;
            }

            // REQ-5: Update status bar dynamically every tick
            UpdateTokenStatusBar();
        }

        // ── load ─────────────────────────────────────────────────────────
        private async void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);

            this.Text = $"Dispatch System — {DatabaseHelper.AppVersion}";
            lblUser.Text = "User: " + CurrentUser;
            SetDispatchStatus("READY", Color.FromArgb(0, 120, 215));

            currentShift = ShiftHelper.GetCurrentShift();
            lastShift = currentShift;
            mailSentForShift = currentShift;
            lblShift.Text = "Shift: " + currentShift;

            totalCount = DatabaseHelper.GetTotalCount();

            // REQ-4: Dashboard shows no-token state on load
            UpdateTokenDashboard(null);
            LoadDispatchTokens();

            // WebView2 setup (unchanged)
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Settings.IsZoomControlEnabled = false;
            webView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            string js = @"
(function(){
    let zoomed=false,isPanning=false,startX=0,startY=0;
    document.addEventListener('wheel',function(e){
        if(e.ctrlKey){e.preventDefault();
        let c=parseFloat(document.body.style.zoom)||1.0;
        let n=e.deltaY<0?c+0.1:c-0.1;
        document.body.style.zoom=Math.min(Math.max(n,0.5),3.0).toFixed(1);}
    },{passive:false});
    document.addEventListener('dblclick',function(){zoomed=!zoomed;document.body.style.zoom=zoomed?'1.5':'1.0';});
    document.addEventListener('mousedown',function(e){if(e.button===2){isPanning=true;startX=e.clientX+window.scrollX;startY=e.clientY+window.scrollY;e.preventDefault();}});
    document.addEventListener('mousemove',function(e){if(isPanning)window.scrollTo(startX-e.clientX,startY-e.clientY);});
    document.addEventListener('mouseup',function(e){if(e.button===2)isPanning=false;});
    document.addEventListener('contextmenu',function(e){e.preventDefault();});
})();";
            webView21.CoreWebView2.DOMContentLoaded += async (s2, e2) =>
                await webView21.CoreWebView2.ExecuteScriptAsync(js);

            this.BeginInvoke(new Action(() => LoadPDF()));
            txtScan.Focus();
        }

        // ── REQ-4: Token Dashboard Update ────────────────────────────────
        /// <summary>
        /// Updates the active token dashboard panel with live token progress.
        /// Pass null to show the "no active token" state.
        /// </summary>
        private void UpdateTokenDashboard(DispatchOrder? order)
        {
            if (order == null)
            {
                lblActiveOrder.Text = "No token selected";
                lblActiveCustomer.Text = "—";
                lblActivePart.Text = "—";
                lblActiveQty.Text = "Dispatched: — / —  |  Remaining: —";
                progressDispatch.Value = 0;
                progressDispatch.ForeColor = Color.FromArgb(0, 120, 215);
                return;
            }

            int remaining = Math.Max(order.QtyOrdered - order.QtyScanned, 0);
            int pct = order.QtyOrdered > 0
                ? Math.Min((order.QtyScanned * 100) / order.QtyOrdered, 100)
                : 0;

            lblActiveOrder.Text = $"Token: {order.OrderNo}";
            lblActiveCustomer.Text = $"Customer: {order.CustomerName}";
            lblActivePart.Text = $"Part No: {order.QRReference}";
            lblActiveQty.Text = $"Dispatched: {order.QtyScanned} / {order.QtyOrdered}  |  Remaining: {remaining}";

            progressDispatch.Maximum = Math.Max(order.QtyOrdered, 1);
            progressDispatch.Value = Math.Min(order.QtyScanned, order.QtyOrdered);

            // REQ-4: Highlight green when dispatch complete
            if (order.QtyScanned >= order.QtyOrdered)
            {
                progressDispatch.ForeColor = Color.Green;
                lblActiveQty.ForeColor = Color.FromArgb(0, 140, 0);
            }
            else
            {
                progressDispatch.ForeColor = Color.FromArgb(0, 120, 215);
                lblActiveQty.ForeColor = Color.FromArgb(0, 120, 60);
            }
        }

        // ── REQ-5: Token Status Bar ───────────────────────────────────────
        /// <summary>
        /// Updates the status bar text dynamically.
        /// Format: "Running Token: [No] | Customer: [Name] | Progress: X/Y"
        /// </summary>
        private void UpdateTokenStatusBar()
        {
            if (activeOrder != null)
            {
                lblTokenStatusBar.Text =
                    $"Running Token: {activeOrder.OrderNo}  |  " +
                    $"Customer: {activeOrder.CustomerName}  |  " +
                    $"Progress: {activeOrder.QtyScanned}/{activeOrder.QtyOrdered}";
                lblTokenStatusBar.ForeColor = Color.FromArgb(0, 100, 0);
            }
            else
            {
                lblTokenStatusBar.Text = "No active token — select a token to begin dispatch";
                lblTokenStatusBar.ForeColor = Color.FromArgb(100, 100, 120);
            }
        }

        // ── dispatch tokens ──────────────────────────────────────────────
        public void LoadDispatchTokens()
        {
            // REQ-3: Fix token panel layout — ensure flpTokens fills properly
            flpTokens.Controls.Clear();
            flpTokens.Padding = new Padding(6, 6, 6, 6);
            flpTokens.AutoScroll = true;

            var orders = DatabaseHelper.GetDispatchOrders();

            foreach (var o in orders)
            {
                var card = BuildTokenCard(o);
                flpTokens.Controls.Add(card);
            }

            // Refresh active order reference from DB
            if (activeOrder != null)
            {
                var refreshed = DatabaseHelper.GetDispatchOrder(activeOrder.OrderNo);
                if (refreshed != null)
                {
                    activeOrder = refreshed;
                    UpdateTokenDashboard(activeOrder);
                }
            }

            UpdateTokenStatusBar();
        }

        private Panel BuildTokenCard(DispatchOrder o)
        {
            Color borderCol = GetDueColour(o.DueDate, o.Status);
            bool isActive = activeOrder?.OrderNo == o.OrderNo;
            bool isDone = o.Status == "Done";

            // REQ-3: Consistent card height to prevent clipping
            var card = new Panel
            {
                Width = 230,
                Height = isDone ? 185 : 160,
                Margin = new Padding(6),
                Cursor = Cursors.Hand,
                Tag = o.OrderNo
            };

            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var bg = new SolidBrush(isActive ? Color.FromArgb(235, 248, 255) : Color.White);
                using var pen = new Pen(borderCol, isActive ? 3 : 2);
                g.FillRoundedRectangle(bg, 1, 1, card.Width - 2, card.Height - 2, 10);
                g.DrawRoundedRectangle(pen, 1, 1, card.Width - 2, card.Height - 2, 10);
            };

            Color badgeCol = o.Status switch
            {
                "Done" => Color.FromArgb(60, 160, 60),
                "InProgress" => Color.FromArgb(0, 120, 215),
                _ => Color.FromArgb(150, 150, 150)
            };

            var lblOrder = new Label
            {
                Text = o.OrderNo,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(10, 10),
                AutoSize = true
            };
            var lblCustomer = new Label
            {
                Text = o.CustomerName,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(10, 28),
                AutoSize = true
            };
            // REQ-1: Show Part Number (QRReference) prominently as the token's key field
            var lblPart = new Label
            {
                Text = $"Part: {o.QRReference}",
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 130),
                Location = new Point(10, 46),
                AutoSize = true
            };
            var lblQty = new Label
            {
                Text = $"Dispatched: {o.QtyScanned} / {o.QtyOrdered}",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(10, 64),
                AutoSize = true
            };
            var lblDue = new Label
            {
                Text = $"Due: {o.DueDate[..10]}",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = borderCol,
                Location = new Point(10, 82),
                AutoSize = true
            };

            // Mini progress bar
            var pb = new ProgressBar
            {
                Location = new Point(10, 100),
                Size = new Size(210, 8),
                Minimum = 0,
                Maximum = Math.Max(o.QtyOrdered, 1),
                Value = Math.Min(o.QtyScanned, o.QtyOrdered),
                Style = ProgressBarStyle.Continuous
            };

            var lblStatus = new Label
            {
                Text = o.Status,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = badgeCol,
                Location = new Point(10, 114),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[]
                { lblOrder, lblCustomer, lblPart, lblQty, lblDue, pb, lblStatus });

            // Reprint button — only for completed orders
            if (isDone)
            {
                var btnReprint = new Button
                {
                    Text = "🖨 Reprint Label",
                    Location = new Point(10, 138),
                    Size = new Size(210, 28),
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    BackColor = Color.FromArgb(220, 235, 255),
                    ForeColor = Color.FromArgb(0, 70, 180),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnReprint.FlatAppearance.BorderColor = Color.FromArgb(150, 190, 255);
                btnReprint.Click += (s, e) =>
                {
                    currentInspector = txtInspector.Text.Trim();
                    using var dlg = new LabelPrintDialog(
                        o, CurrentUser, currentInspector, printerShareName, baseFolder);
                    dlg.ShowDialog(this);
                };
                card.Controls.Add(btnReprint);
            }

            // Click to select (only non-done orders)
            if (!isDone)
            {
                EventHandler clickHandler = (s, e) => SelectOrder(o.OrderNo);
                card.Click += clickHandler;
                foreach (Control c in card.Controls) c.Click += clickHandler;
            }

            return card;
        }

        private void SelectOrder(string orderNo)
        {
            var order = DatabaseHelper.GetDispatchOrder(orderNo);
            if (order == null) return;

            if (order.Status == "Done")
            {
                MessageBox.Show($"Token {orderNo} is already completed.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Release previous lock
            if (activeOrder != null && activeOrder.OrderNo != orderNo)
                DatabaseHelper.UnlockDispatchOrder(activeOrder.OrderNo);

            // Check if locked by another operator
            if (!string.IsNullOrEmpty(order.LockedBy) && order.LockedBy != CurrentUser)
            {
                MessageBox.Show($"Token {orderNo} is currently being processed by {order.LockedBy}.",
                    "Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseHelper.LockDispatchOrder(orderNo, CurrentUser);
            activeOrder = order;

            // REQ-4: Update dashboard with live token info
            UpdateTokenDashboard(activeOrder);

            SetDispatchStatus("TOKEN SELECTED — Start scanning parts", Color.FromArgb(0, 120, 215));

            // REQ-5: Update status bar immediately
            UpdateTokenStatusBar();

            LoadDispatchTokens();
            txtScan.Focus();
        }

        // ── scan ─────────────────────────────────────────────────────────
        private void txtScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtScan.Text.Length == 0) scanTimer.Restart();
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            string barcode = txtScan.Text.Trim();
            long elapsed = scanTimer.ElapsedMilliseconds;
            txtScan.Clear();
            scanTimer.Reset();

            if (string.IsNullOrEmpty(barcode)) { txtScan.Focus(); return; }

            // Inspector check (compulsory)
            currentInspector = txtInspector.Text.Trim();
            if (string.IsNullOrEmpty(currentInspector))
            {
                SetDispatchStatus("⚠ Enter Inspector Name first before scanning!", Color.FromArgb(200, 80, 0));
                txtInspector.BackColor = Color.FromArgb(255, 220, 200);
                txtInspector.Focus();
                return;
            }
            txtInspector.BackColor = Color.FromArgb(240, 255, 230);

            // REQ-1: Dispatch mode — must have an active token
            if (activeOrder != null)
            {
                ProcessDispatchScan(barcode, elapsed);
                txtScan.Focus();
                return;
            }

            // No token selected
            SetDispatchStatus("⚠ No token selected — click a dispatch token to begin", Color.FromArgb(180, 80, 0));
            PlayBeep(true);
            txtScan.Focus();
        }

        // ── REQ-1: Core Dispatch Scan Logic ──────────────────────────────
        private void ProcessDispatchScan(string barcode, long elapsedMs)
        {
            if (activeOrder == null) return;

            // Speed check — reject manual typing
            if (barcode.Length > 1 && elapsedMs > 500)
            {
                SetDispatchStatus("Too slow — looks like manual typing. Rescan.", Color.Orange);
                PlayBeep(true);
                return;
            }

            // REQ-1: Check if required quantity already reached — stop accepting scans
            if (activeOrder.QtyScanned >= activeOrder.QtyOrdered)
            {
                SetDispatchStatus($"✅ Dispatch complete for token {activeOrder.OrderNo}. No more scans accepted.", Color.Green);
                PlayBeep(true);
                return;
            }

            // Duplicate within this order
            var scans = DatabaseHelper.GetDispatchScans(activeOrder.OrderNo);
            if (scans.Any(s => s.Barcode == barcode && s.Result == "OK"))
            {
                SetDispatchStatus($"DUPLICATE — already scanned: {barcode}", Color.Orange);
                PlayBeep(true);
                DatabaseHelper.SaveDispatchScan(activeOrder.OrderNo, barcode, CurrentUser, "Duplicate");
                lstStatus.Items.Insert(0,
                    $"[{DateTime.Now:HH:mm:ss}] DUPLICATE | {barcode} | Token:{activeOrder.OrderNo}");
                return;
            }

            // REQ-1: Part match check — validate scanned part against token's QRReference (Part Number)
            bool matched = IsPartMatch(barcode, activeOrder.QRReference);
            if (!matched)
            {
                // REQ-1: Clear error message for wrong part
                SetDispatchStatus($"❌ INVALID PART — Expected: {activeOrder.QRReference}", Color.Red);
                PlayBeep(true);
                DatabaseHelper.SaveDispatchScan(activeOrder.OrderNo, barcode, CurrentUser, "WrongPart");
                lstStatus.Items.Insert(0,
                    $"[{DateTime.Now:HH:mm:ss}] INVALID PART | Expected:{activeOrder.QRReference} | Scanned:{barcode}");
                return;
            }

            // ── SUCCESS ──────────────────────────────────────────────────
            DatabaseHelper.IncrementDispatchScan(activeOrder.OrderNo);
            DatabaseHelper.SaveDispatchScan(activeOrder.OrderNo, barcode, CurrentUser, "OK");

            // Refresh active order from DB
            activeOrder = DatabaseHelper.GetDispatchOrder(activeOrder.OrderNo)!;

            int remaining = activeOrder.QtyOrdered - activeOrder.QtyScanned;

            // REQ-4: Update dashboard live on every successful scan
            UpdateTokenDashboard(activeOrder);

            // REQ-5: Update status bar immediately
            UpdateTokenStatusBar();

            SetDispatchStatus($"✅ OK — {remaining} remaining", Color.Green);
            PlayBeep(false);

            lstStatus.Items.Insert(0,
                $"[{DateTime.Now:HH:mm:ss}] OK | {barcode} | Token:{activeOrder.OrderNo} | Remaining:{remaining}");

            LoadDispatchTokens();

            // REQ-1: Stop when quantity reached → trigger order complete
            if (activeOrder.QtyScanned >= activeOrder.QtyOrdered)
                CompleteOrder();
        }

        private bool IsPartMatch(string scanned, string qrRef)
        {
            if (string.IsNullOrEmpty(qrRef)) return false;
            if (scanned == qrRef) return true;
            if (scanned.Contains(qrRef, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private void CompleteOrder()
        {
            if (activeOrder == null) return;

            DatabaseHelper.CompleteDispatchOrder(activeOrder.OrderNo);
            activeOrder = DatabaseHelper.GetDispatchOrder(activeOrder.OrderNo)!;

            SetDispatchStatus($"🎉 DISPATCH COMPLETE — Token: {activeOrder.OrderNo}", Color.Green);
            SystemSounds.Exclamation.Play();

            // REQ-4: Final dashboard update — highlight complete state
            UpdateTokenDashboard(activeOrder);

            var completedOrder = activeOrder;
            activeOrder = null;

            // Reset dashboard to idle
            UpdateTokenDashboard(null);
            UpdateTokenStatusBar();

            // Export CSV report
            string csvPath = ExportOrderCsv(completedOrder);
            EmailHelper.SendEmailAsync(csvPath,
                $"Dispatch Complete — {completedOrder.OrderNo} — {completedOrder.CustomerName}");

            MessageBox.Show(
                $"Token {completedOrder.OrderNo} dispatch complete!\n" +
                $"Customer: {completedOrder.CustomerName}\n" +
                $"Part No: {completedOrder.QRReference}\n" +
                $"Qty Dispatched: {completedOrder.QtyScanned}\n\n" +
                "Printing customer label now. Report emailed.",
                "Dispatch Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // REQ-1: Single customer label print after order completion (not per-scan)
            currentInspector = txtInspector.Text.Trim();
            using var printDlg = new LabelPrintDialog(
                completedOrder, CurrentUser, currentInspector,
                printerShareName, baseFolder);
            printDlg.ShowDialog(this);

            LoadDispatchTokens();
            txtScan.Focus();
        }

        // ── CSV export per order ─────────────────────────────────────────
        private string ExportOrderCsv(DispatchOrder order)
        {
            string fileName = $"dispatch_{order.OrderNo}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(baseFolder, fileName);

            var sb = new StringBuilder();
            sb.AppendLine("OrderNo,CreatedDate,DueDate,CustomerName,PartNo,QtyOrdered,QtyDispatched,Status,CompletedAt");
            sb.AppendLine(
                $"{order.OrderNo},{order.CreatedDate},{order.DueDate}," +
                $"{order.CustomerName},{order.QRReference}," +
                $"{order.QtyOrdered},{order.QtyScanned}," +
                $"{order.Status},{order.CompletedAt}");

            sb.AppendLine();
            sb.AppendLine("Sr,Barcode,ScanTime,Operator,Result");
            int sr = 1;
            foreach (var scan in DatabaseHelper.GetDispatchScans(order.OrderNo))
            {
                sb.AppendLine($"{sr},{scan.Barcode},{scan.ScanTime},{scan.Operator},{scan.Result}");
                sr++;
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            return filePath;
        }

        // ── shift CSV ────────────────────────────────────────────────────
        public string GetCsvPath()
        {
            string fileName = $"dispatch_shift_{currentShift}_{DateTime.Now:yyyy-MM-dd}.csv";
            return Path.Combine(baseFolder, fileName);
        }

        private void ExportShiftDispatchCsv()
        {
            try
            {
                var orders = DatabaseHelper.GetDispatchOrders();
                if (!orders.Any()) return;

                string filePath = GetCsvPath();
                var sb = new StringBuilder();
                sb.AppendLine("OrderNo,CustomerName,PartNo,QtyOrdered,QtyDispatched,QtyPending,Status,DueDate,CompletedAt");
                foreach (var o in orders)
                    sb.AppendLine($"{o.OrderNo},{o.CustomerName},{o.QRReference}," +
                                  $"{o.QtyOrdered},{o.QtyScanned},{o.QtyPending}," +
                                  $"{o.Status},{o.DueDate},{o.CompletedAt}");

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                EmailHelper.SendEmailAsync(filePath,
                    $"Shift {currentShift} Dispatch Summary — {DateTime.Now:dd-MM-yyyy}");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] Shift CSV Error: {ex.Message}\n");
            }
        }

        // ── dispatch label (direct print, used internally) ───────────────
        private void PrintDispatchLabel(DispatchOrder order)
        {
            try
            {
                string prnContent = DatabaseHelper.GetCustomerPrnContent(order.CustomerName);

                if (string.IsNullOrWhiteSpace(prnContent))
                {
                    MessageBox.Show(
                        $"No PRN configured for customer '{order.CustomerName}'!\n" +
                        "Go to Admin → Customer PRN to set up the label format.",
                        "PRN Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                prnContent = string.Join("\r\n",
                    prnContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                              .Where(l => !l.TrimStart().StartsWith("//"))) + "\r\n";

                prnContent = prnContent
                    .Replace("{OrderNo}", order.OrderNo)
                    .Replace("{CustomerName}", order.CustomerName)
                    .Replace("{PartName}", order.PartName)
                    .Replace("{PartNumber}", order.QRReference)
                    .Replace("{QtyOrdered}", order.QtyOrdered.ToString())
                    .Replace("{QtyScanned}", order.QtyScanned.ToString())
                    .Replace("{DueDate}", order.DueDate)
                    .Replace("{CompletedAt}", order.CompletedAt)
                    .Replace("{CreatedDate}", order.CreatedDate);

                prnContent = ResolveMultilineTokens(prnContent, order.OrderNo);

                string tempPrn = Path.Combine(baseFolder, "dispatch_label.prn");
                File.WriteAllText(tempPrn, prnContent, Encoding.ASCII);

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{printerShareName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(psi)?.WaitForExit(3000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dispatch Label Print Error: " + ex.Message);
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] Dispatch Label Error: {ex.Message}\n");
            }
        }

        // ── existing EOL label print (kept for SOP/reprint use) ─────────
        private void PrintLabel(string barcode)
        {
            try
            {
                if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);
                string partName = cmbPart.Text.Trim();
                string prnContent = DatabaseHelper.GetPrnContent(partName);

                if (string.IsNullOrWhiteSpace(prnContent))
                {
                    string filePath = DatabaseHelper.GetPrnPath(partName);
                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        prnContent = File.ReadAllText(filePath, Encoding.ASCII);
                }

                if (!string.IsNullOrWhiteSpace(prnContent))
                    prnContent = string.Join("\r\n",
                        prnContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                                  .Where(l => !l.TrimStart().StartsWith("//"))) + "\r\n";

                if (string.IsNullOrWhiteSpace(prnContent?.Replace("\r\n", "").Trim()))
                {
                    MessageBox.Show(
                        $"No PRN configured for part '{partName}'!\nPlease set PRN in Admin → PRN Editor.",
                        "PRN Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int serialNumber = DatabaseHelper.GetSerial();
                prnContent = prnContent!
                    .Replace("{barcode}", barcode)
                    .Replace("{PartName}", partName)
                    .Replace("{serialNumber}", serialNumber.ToString());

                prnContent = ResolveMultilineTokens(prnContent, barcode);

                string tempPrn = Path.Combine(baseFolder, "active_label.prn");
                File.WriteAllText(tempPrn, prnContent, Encoding.ASCII);

                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c copy /b \"{tempPrn}\" \"\\\\localhost\\{printerShareName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(psi)?.WaitForExit(3000);

                DatabaseHelper.SaveSerial(serialNumber + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Error: " + ex.Message);
                File.AppendAllText("error.log", $"[{DateTime.Now}] Print Error: {ex.Message}\n");
            }
        }

        // ── multiline PRN helpers ────────────────────────────────────────
        private static IEnumerable<string> SplitIntoChunks(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }

        private static string BuildMultilineTextCommands(
            string fullString, int x, int startY, int lineHeight,
            string font, int rotation, int xMul, int yMul, int chunkSize)
        {
            var sb = new StringBuilder();
            int y = startY;
            var chunks = SplitIntoChunks(fullString, chunkSize).ToList();
            for (int i = 0; i < chunks.Count; i++)
            {
                string chunk = chunks[i];
                int lineX = x;
                if (i == chunks.Count - 1 && chunk.Length < chunkSize)
                {
                    int charWidth = (font == "1" ? 8 : font == "2" ? 10 : 12) * xMul;
                    int fullW = chunkSize * charWidth;
                    int lastW = chunk.Length * charWidth;
                    lineX = x + (fullW - lastW) / 2;
                }
                sb.Append($"TEXT {lineX},{y},\"{font}\",{rotation},{xMul},{yMul},\"{chunk}\"\r\n");
                y += lineHeight;
            }
            return sb.ToString();
        }

        private static string ResolveMultilineTokens(string prnContent, string value)
        {
            var pattern = new Regex(@"\{MULTILINE_TEXT:([^}]+)\}", RegexOptions.IgnoreCase);
            return pattern.Replace(prnContent, match =>
            {
                var args = match.Groups[1].Value
                    .Split(',').Select(p => p.Trim().Split('='))
                    .Where(p => p.Length == 2)
                    .ToDictionary(p => p[0].Trim().ToUpper(), p => p[1].Trim(),
                        StringComparer.OrdinalIgnoreCase);
                int x = args.TryGetValue("X", out var vx) && int.TryParse(vx, out var ix) ? ix : 10;
                int y = args.TryGetValue("Y", out var vy) && int.TryParse(vy, out var iy) ? iy : 50;
                int lh = args.TryGetValue("LH", out var vlh) && int.TryParse(vlh, out var ilh) ? ilh : 25;
                int cs = args.TryGetValue("CS", out var vcs) && int.TryParse(vcs, out var ics) ? ics : 10;
                int rot = args.TryGetValue("ROT", out var vr) && int.TryParse(vr, out var ir) ? ir : 0;
                int xm = args.TryGetValue("XM", out var vxm) && int.TryParse(vxm, out var ixm) ? ixm : 1;
                int ym = args.TryGetValue("YM", out var vym) && int.TryParse(vym, out var iym) ? iym : 1;
                string font = args.TryGetValue("FONT", out var vf) ? vf : "3";
                return BuildMultilineTextCommands(value, x, y, lh, font, rot, xm, ym, cs);
            });
        }

        // ── helpers ──────────────────────────────────────────────────────
        private void SetDispatchStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private Color GetDueColour(string dueDateStr, string status)
        {
            if (status == "Done") return Color.FromArgb(60, 160, 60);
            if (!DateTime.TryParse(dueDateStr, out DateTime due)) return Color.Gray;
            int daysLeft = (due.Date - DateTime.Today).Days;
            int yellow = int.TryParse(DatabaseHelper.GetConfig("YellowDaysBeforeDue"), out int y) ? y : 1;
            int red = int.TryParse(DatabaseHelper.GetConfig("RedDaysBeforeDue"), out int r) ? r : 0;
            if (daysLeft <= red) return Color.FromArgb(210, 50, 50);
            if (daysLeft <= yellow) return Color.FromArgb(200, 140, 0);
            return Color.FromArgb(30, 130, 30);
        }

        private void PlayBeep(bool isError)
        {
            try { if (isError) SystemSounds.Exclamation.Play(); else SystemSounds.Beep.Play(); }
            catch { }
        }

        private void SendShiftReport()
        {
            ExportShiftDispatchCsv();
        }

        // ── parts / PDF ──────────────────────────────────────────────────
        private void LoadParts()
        {
            string previousPart = cmbPart.SelectedItem?.ToString() ?? "";
            cmbPart.Items.Clear();
            foreach (var part in DatabaseHelper.GetParts()) cmbPart.Items.Add(part);
            if (!string.IsNullOrEmpty(previousPart) && cmbPart.Items.Contains(previousPart))
                cmbPart.SelectedItem = previousPart;
            else if (cmbPart.Items.Count > 0)
                cmbPart.SelectedIndex = 0;
        }

        public void LoadPDF()
        {
            try
            {
                if (webView21.CoreWebView2 == null) return;
                string path = DatabaseHelper.GetPdfPath(cmbPart.Text);
                if (string.IsNullOrEmpty(path)) path = DatabaseHelper.GetPdfPath();
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    webView21.CoreWebView2.Navigate(
                        new Uri(path).AbsoluteUri + "?v=" + DateTime.Now.Ticks);
                else
                    webView21.NavigateToString(
                        "<h2 style='font-family:Segoe UI;color:gray;text-align:center;margin-top:60px'>No SOP Found</h2>");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] PDF Error: {ex.Message}\n");
            }
        }

        // ── buttons ──────────────────────────────────────────────────────
        private void btnClear_Click(object sender, EventArgs e) => lstStatus.Items.Clear();

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (CurrentUser.ToLower() != "admin")
            {
                MessageBox.Show("Only admin allowed!", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var admin = new AdminForm();
            admin.ShowDialog();
            LoadParts(); LoadPDF();
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            LoadDispatchTokens();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (activeOrder != null)
                {
                    DatabaseHelper.UnlockDispatchOrder(activeOrder.OrderNo);
                    activeOrder = null;
                }

                ExportShiftDispatchCsv();
                EmailHelper.SendEmailAsync(GetCsvPath(),
                    $"Logout Report — {CurrentUser} — {DateTime.Now:dd-MM-yyyy HH:mm}");

                timerClock.Stop();
                LoginForm login = new LoginForm();
                this.Hide();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var newForm = new Form1(login.LoggedUser);
                    newForm.Show();
                }
                this.Close();
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (activeOrder == null) return;
            if (MessageBox.Show($"Release token {activeOrder.OrderNo}?\nProgress will be saved.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.UnlockDispatchOrder(activeOrder.OrderNo);
                activeOrder = null;
                UpdateTokenDashboard(null);
                SetDispatchStatus("READY", Color.FromArgb(0, 120, 215));
                UpdateTokenStatusBar();
                LoadDispatchTokens();
                txtScan.Focus();
            }
        }

        private void btnRefreshTokens_Click(object sender, EventArgs e)
        {
            LoadDispatchTokens();
            txtScan.Focus();
        }

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            string path = GetCsvPath();
            if (File.Exists(path))
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            EmailHelper.SendEmailAsync(GetCsvPath(), "Test Mail — Dispatch App");
            MessageBox.Show("Test email queued.", "Mail", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmbPart_SelectedIndexChanged(object sender, EventArgs e) => LoadPDF();

        private void btnZoomIn_Click(object sender, EventArgs e) =>
            webView21.ZoomFactor = Math.Min(webView21.ZoomFactor + 0.1, 3.0);

        private void btnZoomOut_Click(object sender, EventArgs e) =>
            webView21.ZoomFactor = Math.Max(webView21.ZoomFactor - 0.1, 0.5);

        // REQ-2: Refresh targets when admin closes (for any shift config changes)
        public void RefreshShiftTarget() { /* no-op: shift target UI removed per REQ-4 */ }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) { }
    }

    // ── Graphics extension for rounded rectangles ─────────────────────────
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this System.Drawing.Graphics g,
            Brush brush, float x, float y, float w, float h, float r)
        {
            using var path = GetRoundedPath(x, y, w, h, r);
            g.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this System.Drawing.Graphics g,
            Pen pen, float x, float y, float w, float h, float r)
        {
            using var path = GetRoundedPath(x, y, w, h, r);
            g.DrawPath(pen, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedPath(
            float x, float y, float w, float h, float r)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(x, y, r * 2, r * 2, 180, 90);
            path.AddArc(x + w - r * 2, y, r * 2, r * 2, 270, 90);
            path.AddArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}