using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using LPO.Global;
using LPO.Utillity;
using LPO.GameRuntimeCheck;
using System.ComponentModel;
using System.Diagnostics;
using ESA_AC;

namespace LPO
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();

            UpdateProgressStatus("Starting application...");

            if (File.Exists("c:\\ac_updated.exe") == true)
            {
                File.Delete("c:\\ac_updated.exe");
            }

            if (Directory.Exists(Core.AppPath) == false)
            {
                Directory.CreateDirectory(Core.AppPath);
            }

            if (Directory.Exists(Core.AppPath + "acscreens\\") == true)
            {
                DirectoryInfo di = new DirectoryInfo(Core.AppPath + "acscreens\\");
                di.Delete(true);
            }

            if (Directory.Exists(Core.AppPath + "acscreens\\") == false)
            {
                Directory.CreateDirectory(Core.AppPath + "acscreens\\");
            }

            if (File.Exists(Core.AppPath + "myusername.txt"))
            {
                File.Delete(Core.AppPath + "myusername.txt");
            }

                UpdateProgressStatus("Downloading files needed...");
                ContinueApplicationstartup();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        private const int WM_NCLBUTTONDBLCLK = 0x00A3;

        private void UpdateDllFiles()
        {
            ContinueApplicationstartup();
        }


        private void DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            UpdateProgressStatus("Installing files...");

            UpdateDllFiles();
        }

        private void ContinueApplicationstartup()
        {
            UpdateProgressStatus("Starting application...");
            string username = string.Empty;
            string getID = string.Empty;
            if (File.Exists(Core.AppPath + "mymail.txt"))
            {
                username = File.ReadAllText(Core.AppPath + "mymail.txt");
            }
            if (File.Exists(Core.AppPath + "matchid.txt"))
            {
                getID = File.ReadAllText(Core.AppPath + "matchid.txt");
            }

            UpdateProgressStatus("Checking for updates..");

            WebProxy proxy = WebProxy.GetDefaultProxy();
            proxy.UseDefaultCredentials = true;
            WebClient webClient = new WebClient();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            webClient.Proxy = proxy;
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(UpdateResponse);
            webClient.DownloadFileAsync(new Uri(GlobalSettings.Website_URL + "/update/" + "ac.exe.newver"), "c:\\ac_updated.exe");
        }

        private void UpdateResponse(object sender, AsyncCompletedEventArgs ae)
        {
            UpdateProgressStatus("Installing updates...");

            UpdateProgressStatus("Checking files...");

            //autoupdate code
            
            string lul = null;
            string lulupdate = null;
            string arewedebugging = "no";

            //lolcode ftw
            lul = Core.MD5Encoder.EncodeHashFromFile(Application.ExecutablePath);
            lulupdate = Core.MD5Encoder.EncodeHashFromFile("c:\\ac_updated.exe");
            if (lul != lulupdate)
            {
                if (arewedebugging.Equals("no"))
                {
                    FileInfo updateExe = new FileInfo("C:\\ac_updated.exe");
                    long getSize = updateExe.Length;

                    if (getSize == 0)
                    {
                        ReportForm ef = new ReportForm();
                        ef.label2.Text = "Auto-update failed!" + Environment.NewLine + "Program terminated.";
                        ef.Show();

                        updateExe.Delete();
                        Environment.Exit(0);
                    }

                    if (File.Exists(Application.StartupPath + "\\" + "ac_updated.exe"))
                    {
			File.Delete(Application.StartupPath + "\\" + "ac_updated.exe");
                        File.Copy("c:\\ac_updated.exe", Application.StartupPath + "\\" + "ac_updated2.exe");
                        File.Delete("c:\\ac_updated.exe");
                        Process.Start("cmd.exe",
        "/C choice /C Y /N /D Y /T 2 & ren ac_updated2.exe ac_updated.exe");
                        Process.Start("cmd.exe",
"/C choice /C Y /N /D Y /T 3 & Del " + Application.ExecutablePath);
                        Process.Start("cmd.exe", "/K echo Updating, please wait... & timeout 5 & exit");
                        Application.Exit();
                    }

                    else
                    {
                        File.Copy("c:\\ac_updated.exe", Application.StartupPath + "\\" + "ac_updated.exe");
                        File.Delete("c:\\ac_updated.exe");
                        Process.Start("cmd.exe",
        "/C choice /C Y /N /D Y /T 2 & Del " + Application.ExecutablePath);
                        Process.Start("cmd.exe", "/K echo Updating, please wait... & timeout 5 & exit");
                        Application.Exit();
                    }
                }

                else
                {
                    if (File.Exists("c:\\ac_updated.exe"))
                    {
                        File.Delete("c:\\ac_updated.exe");
                    }
                }

            }

            else
            {
                if (File.Exists("c:\\ac_updated.exe")) {
                File.Delete("c:\\ac_updated.exe");
                }
            }

            ShowMainFormCallback callback = new ShowMainFormCallback(ShowMainForm);
            this.BeginInvoke(callback, new object[] { });
        }

        delegate void ShowMainFormCallback();

        delegate void UpdateProgressStatusCallback(string text);

        private void ShowMainForm()
        {
            Core.DisplayLoginForm();
            this.Hide();
        }

        private void UpdateProgressStatus(string text)
        {
            if (InvokeRequired)
            {
                UpdateProgressStatusCallback d = new UpdateProgressStatusCallback(UpdateProgressStatus);
                this.BeginInvoke(d, new object[] { text });
            }
            else
            {
                label1.Text = text;
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.abort_active;

            Environment.Exit(0);
        }

        private void Button1_MouseHover(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.abort_hover;
        }

        private void Button1_MouseLeave(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.abort_normal;
        }
    }
}
