using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace BarcodeBartenderApp
{
    public partial class Form1 : Form
    {
        private string printerShareName = "TSC_TE244";

        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "BarcodeApp");

        private string prnPath;

        private int totalCount = 0;
        private int todayCount = 0;
        private int serialNumber;

        private string currentUser;
        private string currentShift = "";
        private string lastShift = "";

        private bool shiftMailSent = false;

        private System.Windows.Forms.Timer timerClock;

        public Form1(string user)
        {
            InitializeComponent();

            currentUser = user;
            prnPath = Path.Combine(baseFolder, "label.prn");

            // 🔥 Load serial from file
            serialNumber = LoadSerial();

            timerClock = new System.Windows.Forms.Timer();
            timerClock.Interval = 1000;

            timerClock.Tick += (s, e) =>
            {
                lblDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                string newShift = ShiftHelper.GetCurrentShift();

                if (newShift != lastShift)
                {
                    if (!shiftMailSent)
                    {
                        SendShiftReport();
                        shiftMailSent = true;
                    }

                    lastShift = newShift;
                    lblShift.Text = "Shift: " + newShift;
                }
                else
                {
                    shiftMailSent = false;
                }
            };

            timerClock.Start();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "READY";
            lblTotal.Text = "Total: 0";
            lblToday.Text = "Today: 0";

            // 🔥 Load parts dynamically
            LoadParts();

            txtScan.Focus();

            currentShift = ShiftHelper.GetCurrentShift();
            lastShift = currentShift;
            lblShift.Text = "Shift: " + currentShift;

            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            await webView21.EnsureCoreWebView2Async();

            this.BeginInvoke(new Action(() =>
            {
                LoadPDF();
            }));
        }

        // ================= LOAD PARTS =================
        private void LoadParts()
        {
            cmbPart.Items.Clear();

            var parts = DatabaseHelper.GetParts();

            foreach (var part in parts)
            {
                cmbPart.Items.Add(part);
            }

            if (cmbPart.Items.Count > 0)
                cmbPart.SelectedIndex = 0;
        }

        // ================= PDF =================
        public void LoadPDF()
        {
            try
            {
                if (webView21.CoreWebView2 == null)
                    return;

                string path = DatabaseHelper.GetPdfPath();

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    string uri = new Uri(path).AbsoluteUri;
                    webView21.CoreWebView2.Navigate(uri + "?v=" + DateTime.Now.Ticks);
                }
                else
                {
                    webView21.NavigateToString("<h3>No SOP Found</h3>");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF Error: " + ex.Message);
            }
        }

        // ================= SHIFT MAIL =================
        private void SendShiftReport()
        {
            string file = Path.Combine(baseFolder, "log.csv");

            if (File.Exists(file))
                EmailHelper.SendEmail(file);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            string file = Path.Combine(baseFolder, "log.csv");

            if (File.Exists(file))
                EmailHelper.SendEmail(file);

            MessageBox.Show("Logged out successfully");
            Application.Restart();
        }

        // ================= SCANNER =================
        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string barcode = txtScan.Text.Trim();
            txtScan.Clear();

            if (barcode == "") return;

            currentShift = ShiftHelper.GetCurrentShift();
            lblShift.Text = "Shift: " + currentShift;

            PrintLabel(barcode);
            SaveToCsv(barcode);

            totalCount++;
            todayCount++;

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            lblStatus.Text = "PRINTED";

            lstStatus.Items.Add($"Printed: {barcode} | User: {currentUser} | Shift: {currentShift}");
        }

        // ================= CSV =================
        private void SaveToCsv(string barcode)
        {
            string file = Path.Combine(baseFolder, "log.csv");

            if (!File.Exists(file))
                File.WriteAllText(file, "SrNo,DateTime,Barcode,Part,User,Shift\n");

            int sr = File.ReadAllLines(file).Length;
            if (sr > 0) sr--; // remove header

            File.AppendAllText(file,
                $"{sr + 1},{DateTime.Now},{barcode},{cmbPart.Text},{currentUser},{currentShift}\n");
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            string file = Path.Combine(baseFolder, "log.csv");

            if (!File.Exists(file))
            {
                MessageBox.Show("CSV not created yet ❌");
                return;
            }

            EmailHelper.SendEmail(file);
        }

        // ================= PRINT =================
        private void PrintLabel(string barcode)
        {
            string prn = $@"
SIZE 24 mm,8 mm
GAP 1 mm,0 mm
CLS
QRCODE 5,5,L,3,A,0,""{barcode}""
TEXT 45,5,""2"",0,1,1,""{barcode}""
TEXT 45,18,""2"",0,1,1,""{cmbPart.Text}""
TEXT 45,31,""2"",0,1,1,""{serialNumber}""
PRINT 1
";

            File.WriteAllText(prnPath, prn);

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c copy /b \"{prnPath}\" \"\\\\localhost\\{printerShareName}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            serialNumber++;
            SaveSerial(serialNumber);
        }

        // ================= SERIAL =================
        private int LoadSerial()
        {
            string path = Path.Combine(baseFolder, "serial.txt");

            if (File.Exists(path))
                return int.Parse(File.ReadAllText(path));

            return 500;
        }

        private void SaveSerial(int value)
        {
            string path = Path.Combine(baseFolder, "serial.txt");
            File.WriteAllText(path, value.ToString());
        }

        // ================= BUTTONS =================
        private void btnClear_Click(object sender, EventArgs e)
        {
            lstStatus.Items.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            totalCount = 0;
            todayCount = 0;
        }

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", baseFolder);
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (currentUser != "admin")
            {
                MessageBox.Show("Only admin allowed ❌");
                return;
            }

            AdminForm admin = new AdminForm();
            admin.ShowDialog();

            // 🔥 Refresh after admin changes
            LoadParts();
            LoadPDF();
        }
    }
}