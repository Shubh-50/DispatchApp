using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;

namespace BarcodeBartenderApp
{
    public partial class AdminForm : Form
    {
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "BarcodeApp");

        private string sopFolder;

        public AdminForm()
        {
            InitializeComponent();

            sopFolder = Path.Combine(baseFolder, "SOP");

            if (!Directory.Exists(sopFolder))
                Directory.CreateDirectory(sopFolder);

            LoadUsers();
            LoadEmailSettings();
            LoadShiftSettings();
            LoadPdf();
        }

        // ================= USER =================

        private void LoadUsers()
        {
            lstUsers.Items.Clear();

            using (var con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                var cmd = new SQLiteCommand("SELECT Username FROM Users", con);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string user = reader["Username"]?.ToString() ?? "";
                    lstUsers.Items.Add(user);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            if (user == "" || pass == "")
            {
                MessageBox.Show("Enter username & password");
                return;
            }

            bool success = DatabaseHelper.AddUser(user, pass);

            if (success)
            {
                MessageBox.Show("User Added");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("User already exists ❌");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem == null) return;

            string user = lstUsers.SelectedItem?.ToString() ?? "";

            if (user == "admin")
            {
                MessageBox.Show("Admin cannot be deleted");
                return;
            }

            using (var con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                var cmd = new SQLiteCommand(
                    "DELETE FROM Users WHERE Username=@u", con);

                cmd.Parameters.AddWithValue("@u", user);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("User Deleted");
            LoadUsers();
        }

        // ================= EMAIL =================

        private void LoadEmailSettings()
        {
            using (var con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                string query = "CREATE TABLE IF NOT EXISTS EmailSettings (Sender TEXT, Password TEXT, Receiver TEXT)";
                new SQLiteCommand(query, con).ExecuteNonQuery();

                var cmd = new SQLiteCommand("SELECT Sender, Password, Receiver FROM EmailSettings LIMIT 1", con);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtSender.Text = reader["Sender"]?.ToString() ?? "";
                    txtPassword.Text = reader["Password"]?.ToString() ?? "";
                    txtReceiver.Text = reader["Receiver"]?.ToString() ?? "";
                }
            }
        }

        private void btnSaveEmail_Click(object sender, EventArgs e)
        {
            using (var con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                new SQLiteCommand("DELETE FROM EmailSettings", con).ExecuteNonQuery();

                var cmd = new SQLiteCommand(
                    "INSERT INTO EmailSettings(Sender,Password,Receiver) VALUES(@s,@p,@r)", con);

                cmd.Parameters.AddWithValue("@s", txtSender.Text);
                cmd.Parameters.AddWithValue("@p", txtPassword.Text);
                cmd.Parameters.AddWithValue("@r", txtReceiver.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Email Settings Saved ✅");
        }

        // ================= SHIFT =================

        private void LoadShiftSettings()
        {
            var shifts = DatabaseHelper.GetShifts();

            foreach (var s in shifts)
            {
                if (s.shift == "A")
                {
                    txtAStart.Text = s.start.ToString(@"hh\:mm");
                    txtAEnd.Text = s.end.ToString(@"hh\:mm");
                }
                else if (s.shift == "B")
                {
                    txtBStart.Text = s.start.ToString(@"hh\:mm");
                    txtBEnd.Text = s.end.ToString(@"hh\:mm");
                }
                else if (s.shift == "C")
                {
                    txtCStart.Text = s.start.ToString(@"hh\:mm");
                    txtCEnd.Text = s.end.ToString(@"hh\:mm");
                }
            }
        }

        private void btnSaveShift_Click(object sender, EventArgs e)
        {
            try
            {
                string aStart = txtAStart.Text.Replace('.', ':');
                string aEnd = txtAEnd.Text.Replace('.', ':');

                string bStart = txtBStart.Text.Replace('.', ':');
                string bEnd = txtBEnd.Text.Replace('.', ':');

                string cStart = txtCStart.Text.Replace('.', ':');
                string cEnd = txtCEnd.Text.Replace('.', ':');

                // 🔥 Validate format
                TimeSpan.Parse(aStart);
                TimeSpan.Parse(aEnd);
                TimeSpan.Parse(bStart);
                TimeSpan.Parse(bEnd);
                TimeSpan.Parse(cStart);
                TimeSpan.Parse(cEnd);

                DatabaseHelper.UpdateShift("A", aStart, aEnd);
                DatabaseHelper.UpdateShift("B", bStart, bEnd);
                DatabaseHelper.UpdateShift("C", cStart, cEnd);

                MessageBox.Show("Shift timings updated ✅");
            }
            catch
            {
                MessageBox.Show("Enter time in HH:mm format (Example: 15:30)");
            }
        }

        // ================= PDF =================

        private void LoadPdf()
        {
            using (var con = new SQLiteConnection("Data Source=users.db"))
            {
                con.Open();

                string create = "CREATE TABLE IF NOT EXISTS PdfSettings (FilePath TEXT)";
                new SQLiteCommand(create, con).ExecuteNonQuery();

                var cmd = new SQLiteCommand("SELECT FilePath FROM PdfSettings LIMIT 1", con);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string path = reader["FilePath"]?.ToString() ?? "";
                    lblPdfName.Text = Path.GetFileName(path);
                }
            }
        }

        private void btnUploadPdf_Click(object sender, EventArgs e)
        {
            // 🔥 FIX: Use local dialog (no designer dependency)
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PDF Files (*.pdf)|*.pdf";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string destPath = Path.Combine(sopFolder, Path.GetFileName(ofd.FileName));

                File.Copy(ofd.FileName, destPath, true);

                using (var con = new SQLiteConnection("Data Source=users.db"))
                {
                    con.Open();

                    new SQLiteCommand("DELETE FROM PdfSettings", con).ExecuteNonQuery();

                    var cmd = new SQLiteCommand("INSERT INTO PdfSettings VALUES (@p)", con);
                    cmd.Parameters.AddWithValue("@p", destPath);
                    cmd.ExecuteNonQuery();
                }

                lblPdfName.Text = Path.GetFileName(destPath);

                MessageBox.Show("PDF Uploaded Successfully ✅");

                // 🔥 INSTANT REFRESH FORM1
                // 🔥 NEW CLEAN CODE
                foreach (Form f in Application.OpenForms)
                {
                    if (f is Form1 mainForm)
                    {
                        mainForm.BeginInvoke(new Action(() =>
                        {
                            mainForm.LoadPDF();
                        }));
                        break;
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}