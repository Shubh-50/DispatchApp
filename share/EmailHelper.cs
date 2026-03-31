using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.Data.SQLite;

namespace BarcodeBartenderApp
{
    public static class EmailHelper
    {
        public static void SendEmail(string filePath)
        {
            try
            {
                // 🔥 FILE CHECK
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("CSV file not found");
                    return;
                }

                string sender = "";
                string password = "";
                string receiver = "";

                // 🔥 STEP 3 — GET EMAIL FROM DATABASE (NULL SAFE)
                using (var con = new SQLiteConnection("Data Source=users.db"))
                {
                    con.Open();

                    var cmd = new SQLiteCommand(
                        "SELECT Sender, Password, Receiver FROM EmailSettings LIMIT 1", con);

                    var reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        MessageBox.Show("Email settings not configured ❌");
                        return;
                    }

                    sender = reader["Sender"]?.ToString() ?? "";
                    password = reader["Password"]?.ToString() ?? "";
                    receiver = reader["Receiver"]?.ToString() ?? "";
                }

                // 🔥 STEP 4 — VALIDATION (VERY IMPORTANT)
                if (string.IsNullOrWhiteSpace(sender) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(receiver))
                {
                    MessageBox.Show("Please configure email settings in Admin Panel ❌");
                    return;
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(sender);
                mail.To.Add(receiver);

                mail.Subject = "Shift Report";
                mail.Body = "CSV report attached.";

                // 🔥 SAFE FILE ACCESS (NO LOCK ISSUE)
                FileStream fs = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite);

                mail.Attachments.Add(new Attachment(fs, "report.csv", "text/csv"));

                // 🔥 GMAIL SMTP
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(sender, password);
                smtp.EnableSsl = true;

                smtp.Send(mail);

                fs.Close();

                MessageBox.Show("Email Sent Successfully ✅");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email Error: " + ex.Message);
            }
        }
    }
}