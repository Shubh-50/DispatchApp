namespace BarcodeBartenderApp
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelUser;
        private Panel panelEmail;
        private Panel panelShift;
        private Panel panelSOP;
        private Panel panelPart;

        private Label lblUserTitle;
        private Label lblEmailTitle;
        private Label lblShiftTitle;
        private Label lblSOPTitle;
        private Label lblPartTitle;

        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnAdd;
        private Button btnDelete;
        private ListBox lstUsers;

        private TextBox txtSender;
        private TextBox txtPassword;
        private TextBox txtReceiver;
        private Button btnSaveEmail;

        private TextBox txtAStart, txtAEnd;
        private TextBox txtBStart, txtBEnd;
        private TextBox txtCStart, txtCEnd;
        private Button btnSaveShift;

        private Button btnUploadPdf;
        private Label lblPdfName;

        private ComboBox cmbPart;
        private Button btnAddPart;

        private void InitializeComponent()
        {
            this.Text = "Admin Panel";
            this.Size = new System.Drawing.Size(1100, 700);

            // ================= COMMON STYLE =================
            System.Drawing.Color cardColor = System.Drawing.Color.White;
            System.Drawing.Color titleColor = System.Drawing.Color.FromArgb(0, 120, 215);

            // ================= USER PANEL =================
            panelUser = CreateCard(20, 20, 300, 250);
            lblUserTitle = CreateTitle("User Management", titleColor);

            txtUser = new TextBox() { Top = 40, Left = 20, Width = 200, PlaceholderText = "Username" };
            txtPass = new TextBox() { Top = 80, Left = 20, Width = 200, PlaceholderText = "Password" };

            btnAdd = new Button() { Text = "Add", Top = 120, Left = 20 };
            btnAdd.Click += btnAdd_Click;

            btnDelete = new Button() { Text = "Delete", Top = 120, Left = 100 };
            btnDelete.Click += btnDelete_Click;

            lstUsers = new ListBox() { Top = 160, Left = 20, Width = 250, Height = 70 };

            panelUser.Controls.AddRange(new Control[] { lblUserTitle, txtUser, txtPass, btnAdd, btnDelete, lstUsers });

            // ================= EMAIL PANEL =================
            panelEmail = CreateCard(350, 20, 300, 250);
            lblEmailTitle = CreateTitle("Email Configuration", titleColor);

            txtSender = new TextBox() { Top = 40, Left = 20, Width = 250, PlaceholderText = "Sender Email" };
            txtPassword = new TextBox() { Top = 80, Left = 20, Width = 250, PlaceholderText = "App Password" };
            txtReceiver = new TextBox() { Top = 120, Left = 20, Width = 250, PlaceholderText = "Receiver Email" };

            btnSaveEmail = new Button() { Text = "Save Email", Top = 160, Left = 20 };

            panelEmail.Controls.AddRange(new Control[] { lblEmailTitle, txtSender, txtPassword, txtReceiver, btnSaveEmail });

            // ================= SHIFT PANEL =================
            panelShift = CreateCard(680, 20, 350, 250);
            lblShiftTitle = CreateTitle("Shift Management", titleColor);

            txtAStart = CreateTimeBox(40, 20);
            txtAEnd = CreateTimeBox(40, 120);

            txtBStart = CreateTimeBox(80, 20);
            txtBEnd = CreateTimeBox(80, 120);

            txtCStart = CreateTimeBox(120, 20);
            txtCEnd = CreateTimeBox(120, 120);

            btnSaveShift = new Button() { Text = "Save Shift", Top = 170, Left = 20 };
            btnSaveShift.Click += btnSaveShift_Click;

            panelShift.Controls.AddRange(new Control[]
            {
                lblShiftTitle, txtAStart, txtAEnd,
                txtBStart, txtBEnd,
                txtCStart, txtCEnd,
                btnSaveShift
            });

            // ================= SOP PANEL =================
            panelSOP = CreateCard(20, 300, 500, 200);
            lblSOPTitle = CreateTitle("SOP Management", titleColor);

            btnUploadPdf = new Button() { Text = "Upload PDF", Top = 50, Left = 20 };
            btnUploadPdf.Click += btnUploadPdf_Click;

            lblPdfName = new Label() { Top = 90, Left = 20, Width = 400 };

            panelSOP.Controls.AddRange(new Control[] { lblSOPTitle, btnUploadPdf, lblPdfName });

            // ================= PART PANEL =================
            panelPart = CreateCard(550, 300, 480, 200);
            lblPartTitle = CreateTitle("Part Management", titleColor);

            cmbPart = new ComboBox() { Top = 50, Left = 20, Width = 200 };
            btnAddPart = new Button() { Text = "Add Part", Top = 50, Left = 240 };

            panelPart.Controls.AddRange(new Control[] { lblPartTitle, cmbPart, btnAddPart });

            // ================= ADD TO FORM =================
            Controls.Add(panelUser);
            Controls.Add(panelEmail);
            Controls.Add(panelShift);
            Controls.Add(panelSOP);
            Controls.Add(panelPart);
        }

        // ================= HELPERS =================

        private Panel CreateCard(int x, int y, int w, int h)
        {
            return new Panel
            {
                Left = x,
                Top = y,
                Width = w,
                Height = h,
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Label CreateTitle(string text, System.Drawing.Color color)
        {
            return new Label
            {
                Text = text,
                Top = 10,
                Left = 20,
                ForeColor = color,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
        }

        private TextBox CreateTimeBox(int top, int left)
        {
            return new TextBox()
            {
                Top = top,
                Left = left,
                Width = 80,
                Text = "00:00"
            };
        }
    }
}