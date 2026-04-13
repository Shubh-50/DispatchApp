namespace BarcodeBartenderApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainerMain = new SplitContainer();
            splitContainerLeft = new SplitContainer();
            panelTopBar = new Panel();
            lblDateTime = new Label();
            lblUser = new Label();
            lblShift = new Label();
            btnAdmin = new Button();
            btnLogout = new Button();
            picLogo = new PictureBox();

            panelStatusBar = new Panel();
            lblTokenStatusBar = new Label();

            panelScan = new Panel();
            lblScanTitle = new Label();
            txtScan = new TextBox();
            lblInspectorTitle = new Label();
            txtInspector = new TextBox();
            lblStatus = new Label();

            panelActiveOrder = new Panel();
            lblActiveOrderTitle = new Label();
            lblActiveOrder = new Label();
            lblActiveCustomer = new Label();
            lblActivePart = new Label();
            lblActiveQty = new Label();
            progressDispatch = new ProgressBar();
            btnCancelOrder = new Button();

            panelTokenTitle = new Panel();
            lblTokenTitle = new Label();
            btnRefreshTokens = new Button();
            flpTokens = new FlowLayoutPanel();

            splitContainerRight = new SplitContainer();
            panelPartSop = new Panel();
            panelSopToolbar = new Panel();
            lblPartLabel = new Label();
            cmbPart = new ComboBox();
            btnZoomIn = new Button();
            btnZoomOut = new Button();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            panelLog = new Panel();
            lblLogTitle = new Label();
            lstStatus = new ListBox();
            btnClear = new Button();
            btnOpenCsv = new Button();
            btnTestMail = new Button();

            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).BeginInit();
            splitContainerLeft.Panel1.SuspendLayout();
            splitContainerLeft.Panel2.SuspendLayout();
            splitContainerLeft.SuspendLayout();
            panelTopBar.SuspendLayout();
            panelStatusBar.SuspendLayout();
            panelScan.SuspendLayout();
            panelActiveOrder.SuspendLayout();
            panelTokenTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
            splitContainerRight.Panel1.SuspendLayout();
            splitContainerRight.Panel2.SuspendLayout();
            splitContainerRight.SuspendLayout();
            panelPartSop.SuspendLayout();
            panelSopToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            panelLog.SuspendLayout();
            SuspendLayout();

            // ── splitContainerMain ───────────────────────────────────────
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 0);
            splitContainerMain.Name = "splitContainerMain";
            splitContainerMain.Panel1.Controls.Add(splitContainerLeft);
            splitContainerMain.Panel2.Controls.Add(splitContainerRight);
            splitContainerMain.Size = new Size(1400, 860);
            splitContainerMain.SplitterDistance = 760;
            splitContainerMain.TabIndex = 0;

            // ── splitContainerLeft ───────────────────────────────────────
            // FIX: SplitterDistance reduced so Panel1 (top controls) is compact
            // and Panel2 (tokens) gets ample space with no gap.
            splitContainerLeft.Dock = DockStyle.Fill;
            splitContainerLeft.Location = new Point(0, 0);
            splitContainerLeft.Name = "splitContainerLeft";
            splitContainerLeft.Orientation = Orientation.Horizontal;
            splitContainerLeft.Panel1.Controls.Add(panelTopBar);
            splitContainerLeft.Panel1.Controls.Add(panelStatusBar);
            splitContainerLeft.Panel1.Controls.Add(panelScan);
            splitContainerLeft.Panel1.Controls.Add(panelActiveOrder);
            // FIX: flpTokens must be added FIRST, panelTokenTitle SECOND.
            // WinForms Dock=Top processes in reverse add order — last added sits at top.
            // panelTokenTitle (Dock=Top) added last = renders at top correctly,
            // flpTokens (Dock=Fill) added first = fills remaining space below title.
            splitContainerLeft.Panel2.Controls.Add(flpTokens);
            splitContainerLeft.Panel2.Controls.Add(panelTokenTitle);
            splitContainerLeft.Size = new Size(760, 860);
            // FIX: Panel1 = 44(topbar) + 28(statusbar) + 148(scan) + 210(active order) = 430
            // But panelActiveOrder Dock=Fill causes layout bloat. Setting to exact stack height
            // removes the gap. We remove Dock=Fill from panelActiveOrder and use fixed height.
            splitContainerLeft.SplitterDistance = 370;
            splitContainerLeft.IsSplitterFixed = true;   // FIX: prevent accidental drag
            splitContainerLeft.TabIndex = 0;

            // ── panelTopBar ──────────────────────────────────────────────
            panelTopBar.BackColor = Color.FromArgb(24, 48, 96);
            panelTopBar.Controls.Add(lblDateTime);
            panelTopBar.Controls.Add(lblUser);
            panelTopBar.Controls.Add(lblShift);
            panelTopBar.Controls.Add(picLogo);
            panelTopBar.Controls.Add(btnAdmin);
            panelTopBar.Controls.Add(btnLogout);
            panelTopBar.Dock = DockStyle.Top;
            panelTopBar.Name = "panelTopBar";
            panelTopBar.Size = new Size(760, 44);
            panelTopBar.TabIndex = 0;

            // ── picLogo — Hodek logo centered in top bar ──────────────────
            picLogo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            picLogo.Location = new Point(430, 4);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(120, 36);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.BackColor = Color.Transparent;
            picLogo.TabStop = false;

            lblDateTime.AutoSize = true;
            lblDateTime.Font = new Font("Segoe UI", 9.5F);
            lblDateTime.ForeColor = Color.White;
            lblDateTime.Location = new Point(10, 14);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Text = "00-00-0000 00:00:00";

            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9.5F);
            lblUser.ForeColor = Color.FromArgb(180, 210, 255);
            lblUser.Location = new Point(185, 14);
            lblUser.Name = "lblUser";
            lblUser.Text = "User: —";

            lblShift.AutoSize = true;
            lblShift.Font = new Font("Segoe UI", 9.5F);
            lblShift.ForeColor = Color.FromArgb(180, 210, 255);
            lblShift.Location = new Point(310, 14);
            lblShift.Name = "lblShift";
            lblShift.Text = "Shift: —";

            btnAdmin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdmin.BackColor = Color.Transparent;
            btnAdmin.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 255);
            btnAdmin.FlatStyle = FlatStyle.Flat;
            btnAdmin.Font = new Font("Segoe UI", 8.5F);
            btnAdmin.ForeColor = Color.White;
            btnAdmin.Location = new Point(566, 8);
            btnAdmin.Name = "btnAdmin";
            btnAdmin.Size = new Size(80, 28);
            btnAdmin.Text = "Admin";
            btnAdmin.UseVisualStyleBackColor = false;
            btnAdmin.Click += btnAdmin_Click;

            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.BackColor = Color.Transparent;
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 255);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 8.5F);
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(654, 8);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(80, 28);
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;

            // ── panelStatusBar ────────────────────────────────────────────
            panelStatusBar.BackColor = Color.FromArgb(230, 238, 255);
            panelStatusBar.Controls.Add(lblTokenStatusBar);
            panelStatusBar.Dock = DockStyle.Top;
            panelStatusBar.Name = "panelStatusBar";
            panelStatusBar.Padding = new Padding(10, 4, 8, 4);
            panelStatusBar.Size = new Size(760, 28);
            panelStatusBar.TabIndex = 4;

            lblTokenStatusBar.AutoSize = false;
            lblTokenStatusBar.Dock = DockStyle.Fill;
            lblTokenStatusBar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTokenStatusBar.ForeColor = Color.FromArgb(100, 100, 120);
            lblTokenStatusBar.Name = "lblTokenStatusBar";
            lblTokenStatusBar.Text = "No active token — select a token to begin dispatch";
            lblTokenStatusBar.TextAlign = ContentAlignment.MiddleLeft;

            // ── panelScan ────────────────────────────────────────────────
            panelScan.BackColor = Color.White;
            panelScan.Controls.Add(lblScanTitle);
            panelScan.Controls.Add(txtScan);
            panelScan.Controls.Add(lblInspectorTitle);
            panelScan.Controls.Add(txtInspector);
            panelScan.Controls.Add(lblStatus);
            panelScan.Dock = DockStyle.Top;
            panelScan.Name = "panelScan";
            panelScan.Size = new Size(760, 148);
            panelScan.TabIndex = 2;

            lblScanTitle.AutoSize = true;
            lblScanTitle.Font = new Font("Segoe UI", 8.5F);
            lblScanTitle.ForeColor = Color.FromArgb(100, 100, 120);
            lblScanTitle.Location = new Point(10, 6);
            lblScanTitle.Name = "lblScanTitle";
            lblScanTitle.Text = "Scan barcode:";

            txtScan.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtScan.BackColor = Color.FromArgb(240, 255, 240);
            txtScan.Font = new Font("Segoe UI", 13F);
            txtScan.Location = new Point(10, 24);
            txtScan.MaxLength = 200;
            txtScan.Name = "txtScan";
            txtScan.PlaceholderText = "Scan barcode here...";
            txtScan.Size = new Size(580, 36);
            txtScan.TabIndex = 1;
            txtScan.KeyDown += txtScan_KeyDown;
            txtScan.KeyPress += txtScan_KeyPress;

            lblInspectorTitle.AutoSize = true;
            lblInspectorTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblInspectorTitle.ForeColor = Color.FromArgb(160, 60, 0);
            lblInspectorTitle.Location = new Point(10, 68);
            lblInspectorTitle.Name = "lblInspectorTitle";
            lblInspectorTitle.Text = "Inspector Name *:";

            txtInspector.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInspector.BackColor = Color.FromArgb(255, 250, 235);
            txtInspector.Font = new Font("Segoe UI", 10.5F);
            txtInspector.Location = new Point(10, 86);
            txtInspector.MaxLength = 80;
            txtInspector.Name = "txtInspector";
            txtInspector.PlaceholderText = "Enter inspector name before scanning...";
            txtInspector.Size = new Size(580, 30);
            txtInspector.TabIndex = 3;

            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.AutoSize = false;
            lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(0, 120, 215);
            lblStatus.Location = new Point(10, 122);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(730, 22);
            lblStatus.Text = "READY";

            // ── panelActiveOrder (Token Dashboard) ────────────────────────
            // FIX: Changed Dock from Fill to Top with fixed height = 150px.
            // This eliminates the gap between status bar and token panel.
            panelActiveOrder.BackColor = Color.FromArgb(245, 250, 255);
            panelActiveOrder.Controls.Add(lblActiveOrderTitle);
            panelActiveOrder.Controls.Add(lblActiveOrder);
            panelActiveOrder.Controls.Add(lblActiveCustomer);
            panelActiveOrder.Controls.Add(lblActivePart);
            panelActiveOrder.Controls.Add(lblActiveQty);
            panelActiveOrder.Controls.Add(progressDispatch);
            panelActiveOrder.Controls.Add(btnCancelOrder);
            panelActiveOrder.Dock = DockStyle.Top;     // FIX: was Fill
            panelActiveOrder.Height = 150;             // FIX: fixed height, no wasted space
            panelActiveOrder.Name = "panelActiveOrder";
            panelActiveOrder.Padding = new Padding(12, 8, 12, 4);
            panelActiveOrder.TabIndex = 3;

            lblActiveOrderTitle.AutoSize = true;
            lblActiveOrderTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblActiveOrderTitle.ForeColor = Color.FromArgb(30, 50, 100);
            lblActiveOrderTitle.Location = new Point(12, 6);
            lblActiveOrderTitle.Name = "lblActiveOrderTitle";
            lblActiveOrderTitle.Text = "Active Token";

            lblActiveOrder.AutoSize = true;
            lblActiveOrder.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblActiveOrder.ForeColor = Color.FromArgb(0, 80, 180);
            lblActiveOrder.Location = new Point(12, 24);
            lblActiveOrder.Name = "lblActiveOrder";
            lblActiveOrder.Text = "No token selected";

            lblActiveCustomer.AutoSize = true;
            lblActiveCustomer.Font = new Font("Segoe UI", 9.5F);
            lblActiveCustomer.ForeColor = Color.FromArgb(60, 60, 80);
            lblActiveCustomer.Location = new Point(12, 48);
            lblActiveCustomer.Name = "lblActiveCustomer";
            lblActiveCustomer.Text = "—";

            lblActivePart.AutoSize = true;
            lblActivePart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblActivePart.ForeColor = Color.FromArgb(30, 70, 150);
            lblActivePart.Location = new Point(12, 66);
            lblActivePart.Name = "lblActivePart";
            lblActivePart.Text = "—";

            lblActiveQty.AutoSize = true;
            lblActiveQty.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblActiveQty.ForeColor = Color.FromArgb(0, 120, 60);
            lblActiveQty.Location = new Point(12, 84);
            lblActiveQty.Name = "lblActiveQty";
            lblActiveQty.Text = "Dispatched: — / —  |  Remaining: —";

            progressDispatch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressDispatch.Location = new Point(12, 104);
            progressDispatch.Name = "progressDispatch";
            progressDispatch.Size = new Size(700, 10);
            progressDispatch.Style = ProgressBarStyle.Continuous;
            progressDispatch.TabIndex = 5;

            btnCancelOrder.BackColor = Color.FromArgb(255, 240, 240);
            btnCancelOrder.FlatAppearance.BorderColor = Color.FromArgb(200, 100, 100);
            btnCancelOrder.FlatStyle = FlatStyle.Flat;
            btnCancelOrder.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnCancelOrder.ForeColor = Color.FromArgb(180, 30, 30);
            btnCancelOrder.Location = new Point(12, 118);
            btnCancelOrder.Name = "btnCancelOrder";
            btnCancelOrder.Size = new Size(160, 28);
            btnCancelOrder.Text = "⊘  Cancel / Release";
            btnCancelOrder.UseVisualStyleBackColor = false;
            btnCancelOrder.Click += btnCancelOrder_Click;

            // ── panelTokenTitle & flpTokens ──────────────────────────────
            panelTokenTitle.BackColor = Color.FromArgb(235, 240, 250);
            panelTokenTitle.Controls.Add(lblTokenTitle);
            panelTokenTitle.Controls.Add(btnRefreshTokens);
            panelTokenTitle.Dock = DockStyle.Top;
            panelTokenTitle.Name = "panelTokenTitle";
            panelTokenTitle.Size = new Size(760, 38);
            panelTokenTitle.TabIndex = 0;

            lblTokenTitle.AutoSize = true;
            lblTokenTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblTokenTitle.ForeColor = Color.FromArgb(30, 30, 80);
            lblTokenTitle.Location = new Point(10, 10);
            lblTokenTitle.Name = "lblTokenTitle";
            lblTokenTitle.Text = "Dispatch Tokens";

            btnRefreshTokens.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshTokens.BackColor = Color.FromArgb(220, 230, 245);
            btnRefreshTokens.FlatStyle = FlatStyle.Flat;
            btnRefreshTokens.FlatAppearance.BorderColor = Color.FromArgb(160, 190, 230);
            btnRefreshTokens.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnRefreshTokens.ForeColor = Color.FromArgb(30, 60, 120);
            btnRefreshTokens.Location = new Point(660, 5);
            btnRefreshTokens.Name = "btnRefreshTokens";
            btnRefreshTokens.Size = new Size(94, 28);
            btnRefreshTokens.Text = "↻  Refresh";
            btnRefreshTokens.UseVisualStyleBackColor = false;
            btnRefreshTokens.Click += btnRefreshTokens_Click;

            // FIX: flpTokens Dock=Fill fills ALL of Panel2 below panelTokenTitle.
            // Padding top=4 ensures first card row is not clipped by splitter bar.
            flpTokens.AutoScroll = true;
            flpTokens.AutoScrollMargin = new Size(0, 6);
            flpTokens.BackColor = Color.FromArgb(245, 246, 250);
            flpTokens.Dock = DockStyle.Fill;
            flpTokens.FlowDirection = FlowDirection.LeftToRight;
            flpTokens.WrapContents = true;
            flpTokens.Name = "flpTokens";
            flpTokens.Padding = new Padding(6, 6, 6, 6);
            flpTokens.TabIndex = 1;

            // ── splitContainerRight ──────────────────────────────────────
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.Location = new Point(0, 0);
            splitContainerRight.Name = "splitContainerRight";
            splitContainerRight.Orientation = Orientation.Horizontal;
            splitContainerRight.Panel1.Controls.Add(panelPartSop);
            splitContainerRight.Panel2.Controls.Add(panelLog);
            splitContainerRight.Size = new Size(636, 860);
            // FIX-5: PDF viewer (Panel1) = ~70%, Log (Panel2) = ~30%
            // At 860px height: 602 = 70%. We set this here and recalculate on resize.
            splitContainerRight.SplitterDistance = 602;
            splitContainerRight.IsSplitterFixed = false; // allow user to drag if needed
            splitContainerRight.TabIndex = 0;

            // ── panelPartSop ─────────────────────────────────────────────
            panelPartSop.BackColor = Color.White;
            // FIX-3: panelSopToolbar holds label/combo/zoom buttons (Dock=Top, h=40)
            // webView21 fills ALL remaining space via Dock=Fill — no hardcoded width
            panelPartSop.Controls.Add(webView21);
            panelPartSop.Controls.Add(panelSopToolbar);
            panelPartSop.Dock = DockStyle.Fill;
            panelPartSop.Name = "panelPartSop";
            panelPartSop.TabIndex = 0;

            panelSopToolbar.Controls.Add(lblPartLabel);
            panelSopToolbar.Controls.Add(cmbPart);
            panelSopToolbar.Controls.Add(btnZoomIn);
            panelSopToolbar.Controls.Add(btnZoomOut);
            panelSopToolbar.Dock = DockStyle.Top;
            panelSopToolbar.Height = 40;
            panelSopToolbar.Name = "panelSopToolbar";
            panelSopToolbar.BackColor = Color.White;

            lblPartLabel.AutoSize = true;
            lblPartLabel.Font = new Font("Segoe UI", 8.5F);
            lblPartLabel.ForeColor = Color.FromArgb(80, 80, 100);
            lblPartLabel.Location = new Point(8, 10);
            lblPartLabel.Name = "lblPartLabel";
            lblPartLabel.Text = "Part / SOP:";

            cmbPart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbPart.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPart.Font = new Font("Segoe UI", 9F);
            cmbPart.Location = new Point(90, 6);
            cmbPart.Name = "cmbPart";
            cmbPart.Size = new Size(400, 28);
            cmbPart.TabIndex = 1;
            cmbPart.SelectedIndexChanged += cmbPart_SelectedIndexChanged;

            btnZoomIn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnZoomIn.FlatStyle = FlatStyle.Flat;
            btnZoomIn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnZoomIn.Location = new Point(500, 6);
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Size = new Size(32, 28);
            btnZoomIn.Text = "+";
            btnZoomIn.Click += btnZoomIn_Click;

            btnZoomOut.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnZoomOut.FlatStyle = FlatStyle.Flat;
            btnZoomOut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnZoomOut.Location = new Point(538, 6);
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Size = new Size(32, 28);
            btnZoomOut.Text = "−";
            btnZoomOut.Click += btnZoomOut_Click;

            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            // FIX-3: Dock=Fill makes webView fill entire panelPartSop below toolbar — no gap
            webView21.Dock = DockStyle.Fill;
            webView21.Name = "webView21";
            webView21.ZoomFactor = 1D;

            // ── panelLog ─────────────────────────────────────────────────
            panelLog.BackColor = Color.White;
            panelLog.Controls.Add(lblLogTitle);
            panelLog.Controls.Add(lstStatus);
            panelLog.Controls.Add(btnClear);
            panelLog.Controls.Add(btnOpenCsv);
            panelLog.Controls.Add(btnTestMail);
            panelLog.Dock = DockStyle.Fill;
            panelLog.Name = "panelLog";
            panelLog.Padding = new Padding(6);
            panelLog.TabIndex = 0;

            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblLogTitle.ForeColor = Color.FromArgb(80, 80, 100);
            lblLogTitle.Location = new Point(6, 6);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Text = "Dispatch Scan Log";

            lstStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                                     AnchorStyles.Left | AnchorStyles.Right;
            lstStatus.BorderStyle = BorderStyle.FixedSingle;
            lstStatus.Font = new Font("Consolas", 8.5F);
            lstStatus.ItemHeight = 17;
            lstStatus.Location = new Point(6, 26);
            lstStatus.Name = "lstStatus";
            lstStatus.Size = new Size(624, 168);
            lstStatus.TabIndex = 1;

            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 8.5F);
            btnClear.Location = new Point(6, 202);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(90, 28);
            btnClear.Text = "Clear Log";
            btnClear.Click += btnClear_Click;

            btnOpenCsv.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenCsv.FlatStyle = FlatStyle.Flat;
            btnOpenCsv.Font = new Font("Segoe UI", 8.5F);
            btnOpenCsv.Location = new Point(102, 202);
            btnOpenCsv.Name = "btnOpenCsv";
            btnOpenCsv.Size = new Size(90, 28);
            btnOpenCsv.Text = "Open CSV";
            btnOpenCsv.Click += btnOpenCsv_Click;

            btnTestMail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnTestMail.FlatStyle = FlatStyle.Flat;
            btnTestMail.Font = new Font("Segoe UI", 8.5F);
            btnTestMail.Location = new Point(198, 202);
            btnTestMail.Name = "btnTestMail";
            btnTestMail.Size = new Size(90, 28);
            btnTestMail.Text = "Test Mail";
            btnTestMail.Click += btnTestMail_Click;

            // ── Form1 ────────────────────────────────────────────────────
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(1400, 860);
            Controls.Add(splitContainerMain);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1100, 700);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dispatch System";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;

            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainerLeft.Panel1.ResumeLayout(false);
            splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).EndInit();
            splitContainerLeft.ResumeLayout(false);
            panelTopBar.ResumeLayout(false);
            panelTopBar.PerformLayout();
            panelStatusBar.ResumeLayout(false);
            panelScan.ResumeLayout(false);
            panelScan.PerformLayout();
            panelActiveOrder.ResumeLayout(false);
            panelActiveOrder.PerformLayout();
            panelTokenTitle.ResumeLayout(false);
            panelTokenTitle.PerformLayout();
            splitContainerRight.Panel1.ResumeLayout(false);
            splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).EndInit();
            splitContainerRight.ResumeLayout(false);
            panelPartSop.ResumeLayout(false);
            panelPartSop.PerformLayout();
            panelSopToolbar.ResumeLayout(false);
            panelSopToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            panelLog.ResumeLayout(false);
            panelLog.PerformLayout();
            ResumeLayout(false);
        }

        // ── control declarations ─────────────────────────────────────────
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblShift;
        private System.Windows.Forms.Button btnAdmin;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelStatusBar;
        private System.Windows.Forms.Label lblTokenStatusBar;
        private System.Windows.Forms.Panel panelScan;
        private System.Windows.Forms.Label lblScanTitle;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.Label lblInspectorTitle;
        private System.Windows.Forms.TextBox txtInspector;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelActiveOrder;
        private System.Windows.Forms.Label lblActiveOrderTitle;
        private System.Windows.Forms.Label lblActiveOrder;
        private System.Windows.Forms.Label lblActiveCustomer;
        private System.Windows.Forms.Label lblActivePart;
        private System.Windows.Forms.Label lblActiveQty;
        private System.Windows.Forms.ProgressBar progressDispatch;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.Panel panelTokenTitle;
        private System.Windows.Forms.Label lblTokenTitle;
        private System.Windows.Forms.Button btnRefreshTokens;
        private System.Windows.Forms.FlowLayoutPanel flpTokens;
        private System.Windows.Forms.Panel panelPartSop;
        private System.Windows.Forms.Panel panelSopToolbar;
        private System.Windows.Forms.Label lblPartLabel;
        private System.Windows.Forms.ComboBox cmbPart;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Panel panelLog;
        private System.Windows.Forms.Label lblLogTitle;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnOpenCsv;
        private System.Windows.Forms.Button btnTestMail;
    }
}