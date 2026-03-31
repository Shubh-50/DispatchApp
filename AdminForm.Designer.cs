namespace BarcodeBartenderApp
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabUsers;
        private System.Windows.Forms.TabPage tabEmail;
        private System.Windows.Forms.TabPage tabShift;
        private System.Windows.Forms.TabPage tabParts;
        private System.Windows.Forms.TabPage tabSOP;
        private System.Windows.Forms.TabPage tabConfig;

        private System.Windows.Forms.TextBox txtUser, txtPass;
        private System.Windows.Forms.Button btnAdd, btnDelete;
        private System.Windows.Forms.ListBox lstUsers;

        private System.Windows.Forms.TextBox txtSender, txtPassword, txtReceiver;
        private System.Windows.Forms.Button btnSaveEmail;

        private System.Windows.Forms.TextBox txtAStart, txtAEnd;
        private System.Windows.Forms.TextBox txtBStart, txtBEnd;
        private System.Windows.Forms.TextBox txtCStart, txtCEnd;
        private System.Windows.Forms.TextBox txtTargetA, txtTargetB, txtTargetC;
        private System.Windows.Forms.Button btnSaveShift;

        private System.Windows.Forms.TextBox txtNewPart;
        private System.Windows.Forms.Button btnAddPart, btnDeletePart;
        private System.Windows.Forms.ListBox lstParts;

        private System.Windows.Forms.ComboBox cmbPartSop;
        private System.Windows.Forms.Button btnUploadPdf;
        private System.Windows.Forms.Label lblPdfName;

        private System.Windows.Forms.TextBox txtPrinterName;
        private System.Windows.Forms.Button btnSavePrinter;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Panel — " + DatabaseHelper.AppVersion;
            this.Size = new System.Drawing.Size(700, 520);
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor =
                System.Drawing.Color.FromArgb(245, 247, 250);

            tabControl = new System.Windows.Forms.TabControl();
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Font = new System.Drawing.Font("Segoe UI", 10);

            tabUsers = new System.Windows.Forms.TabPage("Users");
            tabEmail = new System.Windows.Forms.TabPage("Email");
            tabShift = new System.Windows.Forms.TabPage("Shifts & Targets");
            tabParts = new System.Windows.Forms.TabPage("Parts");
            tabSOP = new System.Windows.Forms.TabPage("SOP");
            tabConfig = new System.Windows.Forms.TabPage("Config");

            tabControl.TabPages.AddRange(new System.Windows.Forms.TabPage[]
                { tabUsers, tabEmail, tabShift, tabParts, tabSOP, tabConfig });

            BuildUsersTab();
            BuildEmailTab();
            BuildShiftTab();
            BuildPartsTab();
            BuildSOPTab();
            BuildConfigTab();

            this.Controls.Add(tabControl);
        }

        private void BuildUsersTab()
        {
            txtUser = MakeBox("Username", 20, 20, 250);
            txtPass = MakeBox("Password", 20, 60, 250);
            txtPass.PasswordChar = '*';

            btnAdd = MakeBtn("Add User", 20, 100,
                System.Drawing.Color.FromArgb(40, 167, 69));
            btnAdd.Click += btnAdd_Click;

            btnDelete = MakeBtn("Delete User", 140, 100,
                System.Drawing.Color.FromArgb(220, 53, 69));
            btnDelete.Click += btnDelete_Click;

            lstUsers = new System.Windows.Forms.ListBox
            { Location = new System.Drawing.Point(20, 145), Size = new System.Drawing.Size(300, 260) };

            tabUsers.Controls.AddRange(new System.Windows.Forms.Control[]
                { txtUser, txtPass, btnAdd, btnDelete, lstUsers });
        }

        private void BuildEmailTab()
        {
            tabEmail.Controls.Add(MakeLabel("Sender Gmail:", 20, 20));
            txtSender = MakeBox("", 20, 45, 400);

            tabEmail.Controls.Add(MakeLabel("App Password:", 20, 85));
            txtPassword = MakeBox("", 20, 110, 400);
            txtPassword.PasswordChar = '*';

            tabEmail.Controls.Add(MakeLabel("Receiver Email:", 20, 150));
            txtReceiver = MakeBox("", 20, 175, 400);

            btnSaveEmail = MakeBtn("Save Email Settings", 20, 220,
                System.Drawing.Color.FromArgb(0, 120, 215));
            btnSaveEmail.Width = 200;
            btnSaveEmail.Click += btnSaveEmail_Click;

            tabEmail.Controls.AddRange(new System.Windows.Forms.Control[]
                { txtSender, txtPassword, txtReceiver, btnSaveEmail });
        }

        private void BuildShiftTab()
        {
            var headers = new[] { "Shift A", "Shift B", "Shift C" };
            int[] tops = { 50, 120, 190 };

            tabShift.Controls.Add(MakeLabel("Shift", 20, 20));
            tabShift.Controls.Add(MakeLabel("Start", 120, 20));
            tabShift.Controls.Add(MakeLabel("End", 240, 20));
            tabShift.Controls.Add(MakeLabel("Target (pcs)", 360, 20));

            TextBox[] starts = new TextBox[3];
            TextBox[] ends = new TextBox[3];
            TextBox[] targets = new TextBox[3];

            for (int i = 0; i < 3; i++)
            {
                tabShift.Controls.Add(MakeLabel(headers[i], 20, tops[i] + 3));
                starts[i] = MakeBox("00:00", 120, tops[i], 90);
                ends[i] = MakeBox("00:00", 240, tops[i], 90);
                targets[i] = MakeBox("0", 360, tops[i], 90);
                tabShift.Controls.AddRange(new System.Windows.Forms.Control[]
                    { starts[i], ends[i], targets[i] });
            }

            txtAStart = starts[0]; txtAEnd = ends[0]; txtTargetA = targets[0];
            txtBStart = starts[1]; txtBEnd = ends[1]; txtTargetB = targets[1];
            txtCStart = starts[2]; txtCEnd = ends[2]; txtTargetC = targets[2];

            btnSaveShift = MakeBtn("Save All Shifts & Targets", 20, 260,
                System.Drawing.Color.FromArgb(0, 120, 215));
            btnSaveShift.Width = 250;
            btnSaveShift.Click += btnSaveShift_Click;
            tabShift.Controls.Add(btnSaveShift);
        }

        private void BuildPartsTab()
        {
            txtNewPart = MakeBox("New part name...", 20, 20, 250);

            btnAddPart = MakeBtn("Add Part", 20, 60,
                System.Drawing.Color.FromArgb(40, 167, 69));
            btnAddPart.Click += btnAddPart_Click;

            btnDeletePart = MakeBtn("Delete Part", 140, 60,
                System.Drawing.Color.FromArgb(220, 53, 69));
            btnDeletePart.Click += btnDeletePart_Click;

            lstParts = new System.Windows.Forms.ListBox
            { Location = new System.Drawing.Point(20, 105), Size = new System.Drawing.Size(300, 300) };

            tabParts.Controls.AddRange(new System.Windows.Forms.Control[]
                { txtNewPart, btnAddPart, btnDeletePart, lstParts });
        }

        private void BuildSOPTab()
        {
            tabSOP.Controls.Add(MakeLabel("Select Part:", 20, 20));
            cmbPartSop = new System.Windows.Forms.ComboBox
            {
                Location = new System.Drawing.Point(120, 17),
                Size = new System.Drawing.Size(220, 28),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };

            btnUploadPdf = MakeBtn("Upload PDF for Selected Part", 20, 60,
                System.Drawing.Color.FromArgb(0, 120, 215));
            btnUploadPdf.Width = 250;
            btnUploadPdf.Click += btnUploadPdf_Click;

            lblPdfName = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(20, 105),
                Size = new System.Drawing.Size(500, 25),
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };

            tabSOP.Controls.AddRange(new System.Windows.Forms.Control[]
                { cmbPartSop, btnUploadPdf, lblPdfName });
        }

        private void BuildConfigTab()
        {
            tabConfig.Controls.Add(MakeLabel("Printer Share Name:", 20, 20));
            txtPrinterName = MakeBox("TSC_TE244", 20, 50, 300);

            btnSavePrinter = MakeBtn("Save Printer Name", 20, 95,
                System.Drawing.Color.FromArgb(0, 120, 215));
            btnSavePrinter.Click += btnSavePrinter_Click;
            btnSavePrinter.Width = 180;

            tabConfig.Controls.AddRange(new System.Windows.Forms.Control[]
                { txtPrinterName, btnSavePrinter });
        }

        // ===== HELPERS =====

        private System.Windows.Forms.TextBox MakeBox(
            string placeholder, int x, int y, int w)
        {
            return new System.Windows.Forms.TextBox
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, 28),
                Font = new System.Drawing.Font("Segoe UI", 10),
                PlaceholderText = placeholder
            };
        }

        private System.Windows.Forms.Button MakeBtn(
            string text, int x, int y,
            System.Drawing.Color color)
        {
            return new System.Windows.Forms.Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(110, 32),
                BackColor = color,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9),
                Cursor = System.Windows.Forms.Cursors.Hand
            };
        }

        private System.Windows.Forms.Label MakeLabel(
            string text, int x, int y)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9,
                    System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(50, 50, 50)
            };
        }
    }
}