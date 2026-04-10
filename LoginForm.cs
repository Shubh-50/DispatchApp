using System;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class LoginForm : Form
    {
        public string LoggedUser = "";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();
            string password = txtPass.Text.Trim();

            // REQ-2: Master access — works even if DB is missing or corrupted
            // Username: Admin (case-insensitive), Password: 5050
            if (username.Equals("Admin", StringComparison.OrdinalIgnoreCase) && password == "5050")
            {
                LoggedUser = "admin";
                this.DialogResult = DialogResult.OK;
                return;
            }

            try
            {
                bool firstLogin;
                if (DatabaseHelper.ValidateUser(username, password, out firstLogin))
                {
                    LoggedUser = username;
                    if (firstLogin)
                    {
                        string newPass = Microsoft.VisualBasic.Interaction.InputBox(
                            "First login detected!\nPlease set a new password:", "Set Password");
                        if (!string.IsNullOrEmpty(newPass))
                            DatabaseHelper.UpdatePassword(LoggedUser, newPass);
                    }
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                // DB error — only master access can proceed
                MessageBox.Show(
                    $"Database error: {ex.Message}\n\nUse Admin / 5050 for emergency access.",
                    "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPass.Clear();
                txtPass.Focus();
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, e);
        }

        // REQ-2: Forgot Password — email admin credentials to registered address
        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var (senderEmail, _, receiverEmail) = DatabaseHelper.GetEmailSettings();

                if (string.IsNullOrWhiteSpace(receiverEmail))
                {
                    MessageBox.Show(
                        "No email address configured.\nAsk your system administrator for access.",
                        "No Email Configured", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Retrieve admin credentials from DB
                string adminInfo = DatabaseHelper.GetAdminCredentialsInfo();

                EmailHelper.SendPasswordRecoveryEmail(receiverEmail, adminInfo);

                MessageBox.Show(
                    $"Admin credentials have been sent to:\n{receiverEmail}\n\nPlease check your inbox.",
                    "Recovery Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not send recovery email: {ex.Message}\n\nUse emergency access: Admin / 5050",
                    "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}