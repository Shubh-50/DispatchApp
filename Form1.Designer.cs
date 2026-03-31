namespace BarcodeBartenderApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelPdf;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.ComboBox cmbPart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblToday;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblShift;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressShift;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOpenCsv;
        private System.Windows.Forms.Button btnTestMail;
        private System.Windows.Forms.Button btnAdmin;
        private System.Windows.Forms.Button btnLogout;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            txtScan = new TextBox();
            cmbPart = new ComboBox();
            panelStatus = new Panel();
            lblStatus = new Label();
            lblTotal = new Label();
            lblToday = new Label();
            lblDateTime = new Label();
            lblShift = new Label();
            lblUser = new Label();
            progressShift = new ProgressBar();
            lblProgress = new Label();
            lstStatus = new ListBox();
            btnClear = new Button();
            btnReset = new Button();
            btnOpenCsv = new Button();
            btnTestMail = new Button();
            btnAdmin = new Button();
            btnLogout = new Button();
            panelPdf = new Panel();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            panelTop = new Panel();
            btnZoomIn = new Button();
            btnZoomOut = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panelStatus.SuspendLayout();
            panelPdf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            panelTop.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = Color.FromArgb(245, 247, 250);
            splitContainer1.Panel1.Controls.Add(txtScan);
            splitContainer1.Panel1.Controls.Add(cmbPart);
            splitContainer1.Panel1.Controls.Add(panelStatus);
            splitContainer1.Panel1.Controls.Add(lblShift);
            splitContainer1.Panel1.Controls.Add(lblUser);
            splitContainer1.Panel1.Controls.Add(progressShift);
            splitContainer1.Panel1.Controls.Add(lblProgress);
            splitContainer1.Panel1.Controls.Add(lstStatus);
            splitContainer1.Panel1.Controls.Add(btnClear);
            splitContainer1.Panel1.Controls.Add(btnReset);
            splitContainer1.Panel1.Controls.Add(btnOpenCsv);
            splitContainer1.Panel1.Controls.Add(btnTestMail);
            splitContainer1.Panel1.Controls.Add(btnAdmin);
            splitContainer1.Panel1.Controls.Add(btnLogout);
            splitContainer1.Panel1.Paint += splitContainer1_Panel1_Paint;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panelPdf);
            splitContainer1.Panel2.Controls.Add(panelTop);
            splitContainer1.Size = new Size(1400, 750);
            splitContainer1.SplitterDistance = 779;
            splitContainer1.TabIndex = 0;
            // 
            // txtScan
            // 
            txtScan.Font = new Font("Segoe UI", 11F);
            txtScan.Location = new Point(15, 15);
            txtScan.Name = "txtScan";
            txtScan.PlaceholderText = "Scan barcode here...";
            txtScan.Size = new Size(290, 32);
            txtScan.TabIndex = 0;
            txtScan.KeyDown += txtScan_KeyDown;
            // 
            // cmbPart
            // 
            cmbPart.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPart.Font = new Font("Segoe UI", 10F);
            cmbPart.Location = new Point(315, 15);
            cmbPart.Name = "cmbPart";
            cmbPart.Size = new Size(200, 31);
            cmbPart.TabIndex = 1;
            cmbPart.SelectedIndexChanged += cmbPart_SelectedIndexChanged;
            // 
            // panelStatus
            // 
            panelStatus.BackColor = Color.White;
            panelStatus.BorderStyle = BorderStyle.FixedSingle;
            panelStatus.Controls.Add(lblStatus);
            panelStatus.Controls.Add(lblTotal);
            panelStatus.Controls.Add(lblToday);
            panelStatus.Controls.Add(lblDateTime);
            panelStatus.Location = new Point(15, 58);
            panelStatus.Name = "panelStatus";
            panelStatus.Size = new Size(520, 60);
            panelStatus.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            lblStatus.Location = new Point(10, 10);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(200, 40);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "READY";
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotal.Location = new Point(220, 8);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(0, 23);
            lblTotal.TabIndex = 1;
            // 
            // lblToday
            // 
            lblToday.AutoSize = true;
            lblToday.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblToday.Location = new Point(340, 8);
            lblToday.Name = "lblToday";
            lblToday.Size = new Size(0, 23);
            lblToday.TabIndex = 2;
            // 
            // lblDateTime
            // 
            lblDateTime.AutoSize = true;
            lblDateTime.Font = new Font("Segoe UI", 9F);
            lblDateTime.ForeColor = Color.Gray;
            lblDateTime.Location = new Point(220, 32);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Size = new Size(0, 20);
            lblDateTime.TabIndex = 3;
            // 
            // lblShift
            // 
            lblShift.AutoSize = true;
            lblShift.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblShift.ForeColor = Color.FromArgb(0, 120, 215);
            lblShift.Location = new Point(15, 128);
            lblShift.Name = "lblShift";
            lblShift.Size = new Size(0, 23);
            lblShift.TabIndex = 3;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 10F);
            lblUser.ForeColor = Color.Gray;
            lblUser.Location = new Point(200, 128);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(0, 23);
            lblUser.TabIndex = 4;
            // 
            // progressShift
            // 
            progressShift.Location = new Point(15, 158);
            progressShift.Name = "progressShift";
            progressShift.Size = new Size(520, 22);
            progressShift.Style = ProgressBarStyle.Continuous;
            progressShift.TabIndex = 5;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Font = new Font("Segoe UI", 9F);
            lblProgress.ForeColor = Color.Gray;
            lblProgress.Location = new Point(15, 185);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(0, 20);
            lblProgress.TabIndex = 6;
            // 
            // lstStatus
            // 
            lstStatus.BorderStyle = BorderStyle.FixedSingle;
            lstStatus.Font = new Font("Consolas", 8F);
            lstStatus.ItemHeight = 15;
            lstStatus.Location = new Point(15, 215);
            lstStatus.Name = "lstStatus";
            lstStatus.Size = new Size(520, 242);
            lstStatus.TabIndex = 7;
            // 
            // btnClear
            // 
            SetBtn(btnClear, "Clear Log", 15, 468, 120, 36, Color.FromArgb(108, 117, 125));
            btnClear.TabIndex = 8;
            btnClear.Click += btnClear_Click;
            // 
            // btnReset
            // 
            SetBtn(btnReset, "Reset", 145, 468, 120, 36, Color.FromArgb(220, 53, 69));
            btnReset.TabIndex = 9;
            btnReset.Click += btnReset_Click;
            // 
            // btnOpenCsv
            // 
            SetBtn(btnOpenCsv, "Open CSV", 275, 468, 120, 36, Color.FromArgb(23, 162, 184));
            btnOpenCsv.TabIndex = 10;
            btnOpenCsv.Click += btnOpenCsv_Click;
            // 
            // btnTestMail
            // 
            SetBtn(btnTestMail, "Test Mail", 405, 468, 120, 36, Color.FromArgb(255, 153, 0));
            btnTestMail.TabIndex = 11;
            btnTestMail.Click += btnTestMail_Click;
            // 
            // btnAdmin
            // 
            SetBtn(btnAdmin, "Admin Panel", 15, 514, 185, 36, Color.FromArgb(0, 120, 215));
            btnAdmin.TabIndex = 12;
            btnAdmin.Click += btnAdmin_Click;
            // 
            // btnLogout
            // 
            SetBtn(btnLogout, "Logout", 210, 514, 120, 36, Color.FromArgb(52, 58, 64));
            btnLogout.TabIndex = 13;
            btnLogout.Click += btnLogout_Click;
            // 
            // panelPdf
            // 
            panelPdf.Controls.Add(webView21);
            panelPdf.Dock = DockStyle.Fill;
            panelPdf.Location = new Point(0, 40);
            panelPdf.Name = "panelPdf";
            panelPdf.Size = new Size(617, 710);
            panelPdf.TabIndex = 0;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Dock = DockStyle.Fill;
            webView21.Location = new Point(0, 0);
            webView21.Name = "webView21";
            webView21.Size = new Size(617, 710);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // panelTop
            // 
            panelTop.BackColor = Color.FromArgb(230, 230, 230);
            panelTop.Controls.Add(btnZoomIn);
            panelTop.Controls.Add(btnZoomOut);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(617, 40);
            panelTop.TabIndex = 1;
            // 
            // btnZoomIn
            // 
            btnZoomIn.FlatStyle = FlatStyle.Flat;
            btnZoomIn.Location = new Point(10, 7);
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Size = new Size(70, 26);
            btnZoomIn.TabIndex = 0;
            btnZoomIn.Text = "Zoom +";
            btnZoomIn.Click += btnZoomIn_Click;
            // 
            // btnZoomOut
            // 
            btnZoomOut.FlatStyle = FlatStyle.Flat;
            btnZoomOut.Location = new Point(90, 7);
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Size = new Size(70, 26);
            btnZoomOut.TabIndex = 1;
            btnZoomOut.Text = "Zoom -";
            btnZoomOut.Click += btnZoomOut_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(1400, 750);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "Packaging EOL System";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            panelStatus.PerformLayout();
            panelPdf.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            panelTop.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void SetBtn(System.Windows.Forms.Button btn,
            string text, int x, int y, int w, int h,
            System.Drawing.Color color)
        {
            btn.Text = text;
            btn.Location = new System.Drawing.Point(x, y);
            btn.Size = new System.Drawing.Size(w, h);
            btn.BackColor = color;
            btn.ForeColor = System.Drawing.Color.White;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new System.Drawing.Font("Segoe UI", 9);
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
        }
    }
}