using System;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                DatabaseHelper.Initialize();
                LoginForm login = new LoginForm();
                if (login.ShowDialog() == DialogResult.OK)
                    Application.Run(new Form1(login.LoggedUser));
                else
                    Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup Error: " + ex.Message);
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] Startup Error: {ex.Message}\n");
            }
        }
    }
}