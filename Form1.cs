using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace BarcodeBartenderApp
{
    public partial class Form1 : Form
    {
        private string printerShareName = "";
        private string baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeApp");
        private string prnPath = "";
        private int totalCount = 0;
        private int todayCount = 0;
        private int shiftCount = 0;
        private int shiftTarget = 0;
        private int serialNumber = 500;
        private string currentUser = "";
        private string currentShift = "";
        private string lastShift = "";
        private bool shiftMailSent = false;
        private System.Windows.Forms.Timer timerClock = new System.Windows.Forms.Timer();

        public Form1(string user)
        {
            InitializeComponent();
            currentUser = user;
            printerShareName = DatabaseHelper.GetConfig("PrinterShareName");
            prnPath = Path.Combine(baseFolder, "label.prn");
            serialNumber = DatabaseHelper.GetSerial();
            timerClock.Interval = 1000;
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
        }

        private void TimerClock_Tick(object? sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string newShift = ShiftHelper.GetCurrentShift();
            if (newShift != lastShift)
            {
                lastShift = newShift;
                lblShift.Text = "Shift: " + newShift;
                shiftMailSent = false;
                shiftCount = DatabaseHelper.GetShiftCount(newShift);
                shiftTarget = DatabaseHelper.GetShiftTarget(newShift);
                UpdateProgress();
            }
            if (!shiftMailSent)
            {
                SendShiftReport();
                shiftMailSent = true;
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            this.Text = $"Packaging EOL System — {DatabaseHelper.AppVersion}";
            lblUser.Text = "User: " + currentUser;
            lblStatus.Text = "READY";
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(0, 120, 215);

            totalCount = DatabaseHelper.GetTotalCount();
            todayCount = DatabaseHelper.GetTodayCount();
            currentShift = ShiftHelper.GetCurrentShift();
            lastShift = currentShift;
            shiftCount = DatabaseHelper.GetShiftCount(currentShift);
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            lblShift.Text = "Shift: " + currentShift;

            UpdateProgress();
            LoadParts();

            txtScan.Focus();

            await webView21.EnsureCoreWebView2Async();
            this.BeginInvoke(new Action(() => LoadPDF()));
        }

        // ================= PARTS =================

        private void LoadParts()
        {
            cmbPart.Items.Clear();
            foreach (var part in DatabaseHelper.GetParts())
                cmbPart.Items.Add(part);
            if (cmbPart.Items.Count > 0)
                cmbPart.SelectedIndex = 0;
        }

        // ================= PDF =================

        public void LoadPDF()
        {
            try
            {
                if (webView21.CoreWebView2 == null) return;
                string path = DatabaseHelper.GetPdfPath(cmbPart.Text);
                if (string.IsNullOrEmpty(path))
                    path = DatabaseHelper.GetPdfPath();
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    webView21.CoreWebView2.Navigate(
                        new Uri(path).AbsoluteUri + "?v=" + DateTime.Now.Ticks);
                else
                    webView21.NavigateToString(
                        "<h2 style='font-family:Segoe UI;color:gray;text-align:center;margin-top:60px'>No SOP Found</h2>");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] PDF Error: {ex.Message}\n");
            }
        }

        // ================= PROGRESS =================

        private void UpdateProgress()
        {
            if (shiftTarget > 0)
            {
                int pct = Math.Min((shiftCount * 100) / shiftTarget, 100);
                progressShift.Value = pct;
                lblProgress.Text = $"Shift: {shiftCount} / {shiftTarget} ({pct}%)";
                progressShift.ForeColor = pct >= 100
                    ? System.Drawing.Color.Green
                    : System.Drawing.Color.FromArgb(0, 120, 215);
            }
            else
            {
                progressShift.Value = 0;
                lblProgress.Text = $"Shift: {shiftCount} (No target set)";
            }
        }

        // ================= SCAN =================

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string barcode = txtScan.Text.Trim();
            txtScan.Clear();
            txtScan.Focus();

            if (string.IsNullOrEmpty(barcode)) return;

            bool isReprint = false;
            string reprintReason = "";

            if (DatabaseHelper.IsDuplicate(barcode))
            {
                var result = MessageBox.Show(
                    $"Barcode '{barcode}' already printed!\nDo you want to REPRINT?",
                    "Duplicate Detected",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;

                reprintReason = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter reprint reason:", "Reprint Reason");
                if (string.IsNullOrWhiteSpace(reprintReason)) return;
                isReprint = true;
            }

            currentShift = ShiftHelper.GetCurrentShift();
            lblShift.Text = "Shift: " + currentShift;

            PrintLabel(barcode);
            SaveToCsv(barcode, isReprint, reprintReason);
            DatabaseHelper.SaveScanLog(barcode, cmbPart.Text,
                currentUser, currentShift, isReprint, reprintReason);

            totalCount++;
            todayCount++;
            shiftCount++;

            lblTotal.Text = $"Total: {totalCount}";
            lblToday.Text = $"Today: {todayCount}";
            UpdateProgress();

            lblStatus.Text = isReprint ? "REPRINT" : "PRINTED";
            lblStatus.ForeColor = isReprint
                ? System.Drawing.Color.Orange
                : System.Drawing.Color.Green;

            lstStatus.Items.Insert(0,
                $"[{DateTime.Now:HH:mm:ss}] {barcode} | {cmbPart.Text} | {currentUser} | {currentShift}"
                + (isReprint ? " | REPRINT" : ""));

            PlayBeep(isReprint);
        }

        // ================= BEEP =================

        private void PlayBeep(bool isReprint)
        {
            try
            {
                if (isReprint)
                    SystemSounds.Exclamation.Play();
                else
                    SystemSounds.Beep.Play();
            }
            catch { }
        }

        // ================= CSV =================

        private string GetCsvPath()
        {
            string fileName = $"log_{currentShift}_{DateTime.Now:yyyy-MM-dd}.csv";
            return Path.Combine(baseFolder, fileName);
        }

        private void SaveToCsv(string barcode, bool isReprint = false, string reason = "")
        {
            try
            {
                string file = GetCsvPath();
                if (!File.Exists(file))
                    File.WriteAllText(file,
                        "SrNo,DateTime,Barcode,Part,User,Shift,Reprint,Reason\n");
                int sr = File.ReadAllLines(file).Length;
                using (var sw = new StreamWriter(file, true))
                    sw.WriteLine(
                        $"{sr},{DateTime.Now:yyyy-MM-dd HH:mm:ss},{barcode}," +
                        $"{cmbPart.Text},{currentUser},{currentShift}," +
                        $"{(isReprint ? "YES" : "NO")},{reason}");
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.log", $"[{DateTime.Now}] CSV Error: {ex.Message}\n");
            }
        }

        // ================= PRINT =================

        private void PrintLabel(string barcode)
        {
            try
            {
                if (!Directory.Exists(baseFolder))
                    Directory.CreateDirectory(baseFolder);

                string prn = $@"SIZE 24 mm,8 mm
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
                DatabaseHelper.SaveSerial(serialNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Error: " + ex.Message);
                File.AppendAllText("error.log",
                    $"[{DateTime.Now}] Print Error: {ex.Message}\n");
            }
        }

        // ================= SHIFT MAIL =================

        private void SendShiftReport()
        {
            string file = GetCsvPath();
            if (File.Exists(file))
                EmailHelper.SendEmailAsync(file,
                    $"Shift {currentShift} Report — {DateTime.Now:dd-MM-yyyy}");
        }

        // ================= BUTTONS =================

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstStatus.Items.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset today's count?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                totalCount = 0; todayCount = 0; shiftCount = 0;
                lblTotal.Text = "Total: 0";
                lblToday.Text = "Today: 0";
                UpdateProgress();
            }
        }

        private void btnOpenCsv_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", baseFolder);
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            string file = GetCsvPath();
            if (!File.Exists(file)) { MessageBox.Show("No CSV yet!"); return; }
            EmailHelper.SendEmailAsync(file, "Test Report");
            MessageBox.Show("Sending in background! ✅");
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            if (currentUser != "admin")
            {
                MessageBox.Show("Only admin allowed!", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var admin = new AdminForm();
            admin.ShowDialog();
            LoadParts();
            LoadPDF();
            shiftTarget = DatabaseHelper.GetShiftTarget(currentShift);
            UpdateProgress();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                EmailHelper.SendEmailAsync(GetCsvPath(),
                    $"Logout Report — {currentUser} — {DateTime.Now:dd-MM-yyyy HH:mm}");
                timerClock.Stop();
                LoginForm login = new LoginForm();
                this.Hide();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var newForm = new Form1(login.LoggedUser);
                    newForm.Show();
                }
                this.Close();
            }
        }

        private void cmbPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPDF();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            webView21.ZoomFactor = Math.Min(webView21.ZoomFactor + 0.1, 3.0);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            webView21.ZoomFactor = Math.Max(webView21.ZoomFactor - 0.1, 0.5);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}