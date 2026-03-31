namespace BarcodeBartenderApp
{
    partial class LoginForm
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;

        private void InitializeComponent()
        {
            txtUser = new TextBox();
            txtPass = new TextBox();
            btnLogin = new Button();

            txtUser.Location = new System.Drawing.Point(30, 30);
            txtUser.Width = 150;
            txtUser.PlaceholderText = "Username";

            txtPass.Location = new System.Drawing.Point(30, 70);
            txtPass.Width = 150;
            txtPass.PasswordChar = '*';
            txtPass.PlaceholderText = "Password";

            btnLogin.Text = "Login";
            btnLogin.Location = new System.Drawing.Point(30, 110);
            btnLogin.Click += btnLogin_Click;

            Controls.Add(txtUser);
            Controls.Add(txtPass);
            Controls.Add(btnLogin);

            Text = "Login";
            Size = new System.Drawing.Size(250, 200);
        }
    }
}