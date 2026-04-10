namespace BarcodeBartenderApp
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Panel panelCard;
        // REQ-2: Forgot Password link
        private System.Windows.Forms.LinkLabel lnkForgotPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelCard = new System.Windows.Forms.Panel();
            txtUser = new System.Windows.Forms.TextBox();
            txtPass = new System.Windows.Forms.TextBox();
            btnLogin = new System.Windows.Forms.Button();
            lblTitle = new System.Windows.Forms.Label();
            lblVersion = new System.Windows.Forms.Label();
            lnkForgotPassword = new System.Windows.Forms.LinkLabel();

            // Form
            this.Text = "Login — Dispatch System";
            this.Size = new System.Drawing.Size(380, 350);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);

            // Card panel
            panelCard.Size = new System.Drawing.Size(300, 260);
            panelCard.Location = new System.Drawing.Point(30, 30);
            panelCard.BackColor = System.Drawing.Color.White;
            panelCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Title
            lblTitle.Text = "Dispatch System";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 13, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);
            lblTitle.Location = new System.Drawing.Point(20, 18);
            lblTitle.AutoSize = true;

            // Version
            lblVersion.Text = "Version " + DatabaseHelper.AppVersion;
            lblVersion.Font = new System.Drawing.Font("Segoe UI", 8);
            lblVersion.ForeColor = System.Drawing.Color.Gray;
            lblVersion.Location = new System.Drawing.Point(20, 46);
            lblVersion.AutoSize = true;

            // Username
            txtUser.Location = new System.Drawing.Point(20, 80);
            txtUser.Size = new System.Drawing.Size(255, 27);
            txtUser.Font = new System.Drawing.Font("Segoe UI", 10);
            txtUser.PlaceholderText = "Username";

            // Password
            txtPass.Location = new System.Drawing.Point(20, 116);
            txtPass.Size = new System.Drawing.Size(255, 27);
            txtPass.Font = new System.Drawing.Font("Segoe UI", 10);
            txtPass.PasswordChar = '*';
            txtPass.PlaceholderText = "Password";
            txtPass.KeyDown += txtPass_KeyDown;

            // Login button
            btnLogin.Text = "Login";
            btnLogin.Location = new System.Drawing.Point(20, 158);
            btnLogin.Size = new System.Drawing.Size(255, 36);
            btnLogin.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            btnLogin.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnLogin.ForeColor = System.Drawing.Color.White;
            btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            btnLogin.Click += btnLogin_Click;

            // REQ-2: Forgot Password link
            lnkForgotPassword.Text = "Forgot Password? Send recovery email";
            lnkForgotPassword.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            lnkForgotPassword.Location = new System.Drawing.Point(20, 204);
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.LinkColor = System.Drawing.Color.FromArgb(0, 100, 200);
            lnkForgotPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            lnkForgotPassword.LinkClicked += lnkForgotPassword_LinkClicked;

            panelCard.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblTitle, lblVersion, txtUser, txtPass, btnLogin, lnkForgotPassword });

            this.Controls.Add(panelCard);
        }
    }
}