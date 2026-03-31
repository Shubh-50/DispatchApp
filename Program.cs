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

                // 🔥 CRITICAL: Initialize database (creates all tables)
                DatabaseHelper.Initialize();

                // 🔥 LOGIN FLOW
                LoginForm login = new LoginForm();

                if (login.ShowDialog() == DialogResult.OK)
                {
                    // 🔥 OPEN MAIN FORM WITH USER
                    Application.Run(new Form1(login.LoggedUser));
                }
                else
                {
                    // 🔥 SAFE EXIT
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup Error: " + ex.Message);
            }
        }
    }
}