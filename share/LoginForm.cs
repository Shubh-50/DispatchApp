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

            if (DatabaseHelper.ValidateUser(txtUser.Text, txtPass.Text, out firstLogin))
            {
                LoggedUser = txtUser.Text;

                if (firstLogin)
                {
                    string newPass = Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter New Password", "First Login");

                    if (!string.IsNullOrEmpty(newPass))
                        DatabaseHelper.UpdatePassword(LoggedUser, newPass);
                }

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid Username or Password");
            }
        }
    }
}