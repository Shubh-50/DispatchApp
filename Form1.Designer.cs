namespace BarcodeBartenderApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelPdf;

        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.ComboBox cmbPart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblToday;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblShift;
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
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            panelTop = new Panel();
            panelPdf = new Panel();

            txtScan = new TextBox();
            cmbPart = new ComboBox();
            lblStatus = new Label();
            lblTotal = new Label();
            lblToday = new Label();
            lblDateTime = new Label();
            lblShift = new Label();
            lstStatus = new ListBox();

            btnClear = new Button();
            btnReset = new Button();
            btnOpenCsv = new Button();
            btnTestMail = new Button();
            btnAdmin = new Button();
            btnLogout = new Button();

            btnZoomIn = new Button();
            btnZoomOut = new Button();

            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();

            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();

            SuspendLayout();

            // ================= SPLIT =================
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.SplitterDistance = 1100;

            // ================= LEFT PANEL =================
            splitContainer1.Panel1.Controls.Add(txtScan);
            splitContainer1.Panel1.Controls.Add(cmbPart);
            splitContainer1.Panel1.Controls.Add(lblStatus);
            splitContainer1.Panel1.Controls.Add(lblTotal);
            splitContainer1.Panel1.Controls.Add(lblToday);
            splitContainer1.Panel1.Controls.Add(lblDateTime);
            splitContainer1.Panel1.Controls.Add(lblShift);
            splitContainer1.Panel1.Controls.Add(lstStatus);
            splitContainer1.Panel1.Controls.Add(btnClear);
            splitContainer1.Panel1.Controls.Add(btnReset);
            splitContainer1.Panel1.Controls.Add(btnOpenCsv);
            splitContainer1.Panel1.Controls.Add(btnTestMail);
            splitContainer1.Panel1.Controls.Add(btnAdmin);
            splitContainer1.Panel1.Controls.Add(btnLogout);

            // ================= RIGHT PANEL =================
            splitContainer1.Panel2.Controls.Add(panelPdf);
            splitContainer1.Panel2.Controls.Add(panelTop);

            // ================= TOP PANEL (ZOOM BAR) =================
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 40;
            panelTop.BackColor = System.Drawing.Color.LightGray;

            panelTop.Controls.Add(btnZoomIn);
            panelTop.Controls.Add(btnZoomOut);

            btnZoomIn.Text = "+";
            btnZoomIn.Location = new System.Drawing.Point(10, 8);
            btnZoomIn.Size = new System.Drawing.Size(40, 25);

            btnZoomOut.Text = "-";
            btnZoomOut.Location = new System.Drawing.Point(60, 8);
            btnZoomOut.Size = new System.Drawing.Size(40, 25);

            // ================= PDF PANEL =================
            panelPdf.Dock = DockStyle.Fill;
            panelPdf.Controls.Add(webView21);

            webView21.Dock = DockStyle.Fill;

            // ================= INPUT =================
            txtScan.Location = new System.Drawing.Point(20, 50);
            txtScan.Size = new System.Drawing.Size(300, 27);
            txtScan.KeyDown += txtScan_KeyDown;

            cmbPart.Location = new System.Drawing.Point(340, 50);
            cmbPart.Size = new System.Drawing.Size(150, 28);

            // ================= LABELS =================
            lblStatus.Location = new System.Drawing.Point(20, 100);
            lblTotal.Location = new System.Drawing.Point(20, 130);
            lblToday.Location = new System.Drawing.Point(20, 160);

            lblDateTime.Location = new System.Drawing.Point(500, 10);
            lblShift.Location = new System.Drawing.Point(500, 40);
            lblShift.Text = "Shift: A";

            // ================= LIST =================
            lstStatus.Location = new System.Drawing.Point(20, 200);
            lstStatus.Size = new System.Drawing.Size(400, 200);

            // ================= BUTTONS =================
            btnClear.Location = new System.Drawing.Point(20, 420);
            btnClear.Text = "Clear";
            btnClear.Click += btnClear_Click;

            btnReset.Location = new System.Drawing.Point(100, 420);
            btnReset.Text = "Reset";
            btnReset.Click += btnReset_Click;

            btnOpenCsv.Location = new System.Drawing.Point(180, 420);
            btnOpenCsv.Text = "Open CSV";
            btnOpenCsv.Click += btnOpenCsv_Click;

            btnTestMail.Location = new System.Drawing.Point(300, 420);
            btnTestMail.Text = "Send Mail";
            btnTestMail.Click += btnTestMail_Click;

            btnAdmin.Location = new System.Drawing.Point(450, 420);
            btnAdmin.Text = "Admin Panel";
            btnAdmin.Click += btnAdmin_Click;

            btnLogout.Location = new System.Drawing.Point(600, 420);
            btnLogout.Text = "Logout";
            btnLogout.Click += btnLogout_Click;

            // ================= FORM =================
            ClientSize = new System.Drawing.Size(1400, 729);
            Controls.Add(splitContainer1);
            Text = "Packaging EOL System";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;

            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);

            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);

            ResumeLayout(false);
        }
    }
}