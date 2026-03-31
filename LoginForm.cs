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
            bool firstLogin;
            if (DatabaseHelper.ValidateUser(txtUser.Text.Trim(), txtPass.Text.Trim(), out firstLogin))
            {
                LoggedUser = txtUser.Text.Trim();
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

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, e);
        }
    }
}