using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using LPO.Global;
using LPO.Utillity;
using ESA_AC;
using System.Text;
using System.Threading;

namespace LPO
{

    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            int countac = 0;

            try
            {
                Process[] processlist = Process.GetProcesses();

                foreach (Process process in processlist)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        if (process.MainWindowTitle.Contains("ESA Anti"))
                        {
                            countac++;
                            if (countac > 1)
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            if (File.Exists(Core.AppPath + "mymail.txt") == true)
            {
                TextBox1.Text = File.ReadAllText(Core.AppPath + "mymail.txt");
            }
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

        private void Button1_Click(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.login_active;

            if (TextBox1.Text.Equals("e-mail") || TextBox1.Text.Length == 0 || TextBox1.Text.Equals("") || TextBox1.Text.Equals(" ")) {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Please insert a e-mail!";
                ef.Show();
                return;
            }

            if (TextBox2.Text.Equals("password") || TextBox2.Text.Length == 0 || TextBox2.Text.Equals("") || TextBox2.Text.Equals(" ")) {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Please insert a password!";
                ef.Show();
                return;
            }

            if (File.Exists(Core.AppPath + "mymail.txt") == true)
            {
                File.Delete(Core.AppPath + "mymail.txt");
            }

            File.WriteAllText(Core.AppPath + "mymail.txt", TextBox1.Text);

            if (checkBox1.Checked == true) 
            {
                byte[] tobytes = ASCIIEncoding.ASCII.GetBytes(TextBox2.Text);
                string tobase64 = System.Convert.ToBase64String(tobytes);
                File.WriteAllText(Core.AppPath + "mypassword.txt", tobase64);
            }

            try
            {
                WebProxy proxy = WebProxy.GetDefaultProxy();
                proxy.UseDefaultCredentials = true;
                System.Net.WebClient WC = new System.Net.WebClient();
                WC.Proxy = proxy;
                string hash = Core.SHA1Encoder.EncodeHash(TextBox2.Text);
                string requestString = GlobalSettings.Website_URL + "/login.php?email=" + TextBox1.Text + "&password=" + hash;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WC_DownloadStringCompleted);
                WC.DownloadStringAsync(new Uri(requestString));
            }
            catch (Exception)
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Unable to login!";
                ef.Show();
            }
        }

        void WC_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                switch (TextHandling.Parse(e.Result)) // todo again later on
                {
                    case 0:
                        {
                            ErrorForm ef = new ErrorForm();
                            ef.label2.Text = "Invalid username/password!";
                            ef.Show();
                            break;
                        }
                    case 1: // Login successful
                        {
                            WebProxy proxy = WebProxy.GetDefaultProxy();
                            proxy.UseDefaultCredentials = true;
                            System.Net.WebClient WC = new System.Net.WebClient();
                            string requestString = GlobalSettings.Website_URL + "/getuser.php?email=" + TextBox1.Text;
                            WC.Proxy = proxy;
                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                            WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WC_DownloadStringCompleted2);
                            WC.DownloadStringAsync(new Uri(requestString));
                            break;
                        }
                    case 3:
                        {
                            ErrorForm ef = new ErrorForm();
                            ef.label2.Text = "Database not found.";
                            ef.Show();
                            break;
                        }
                    case 4:
                        {
                            ErrorForm ef = new ErrorForm();
                            ef.label2.Text = "User table not found.";
                            ef.Show();
                            break;
                        }
                }
            }

            catch (Exception)
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "A unknown error occurred!";
                ef.Show();
            }
        }

        void WC_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                switch (e.Result) // todo again later on
                {
                    default:
                    File.WriteAllText(Core.AppPath + "myusername.txt", e.Result);
                    Core.DisplayMainForm();
                    this.Hide();
                    break;
                }
            }

            catch (Exception)
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "A unknown error occurred!";
                ef.Show();
            }
        }

        private void DisplayLoginError(string reason)
        {
            MessageBox.Show(reason, "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start(GlobalSettings.Website_URL);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Button3.Image = ESA_AC.Properties.Resources.create_active;

            Process.Start("http://esagamer.com/register");

            Thread.Sleep(1000);

            Button3.Image = ESA_AC.Properties.Resources.create_normal;
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void TextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (TextBox1.Text.Equals("e-mail"))
            {
                TextBox1.Text = "";
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox2.UseSystemPasswordChar = true;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            if (File.Exists(Core.AppPath + "mypassword.txt") == true)
            {
                byte[] frombyte = System.Convert.FromBase64String(File.ReadAllText(Core.AppPath + "mypassword.txt"));
                string frombase64 = System.Text.ASCIIEncoding.ASCII.GetString(frombyte);
                TextBox2.UseSystemPasswordChar = true;
                TextBox2.Text = frombase64;
                checkBox1.Checked = true;
                Button1.PerformClick();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://esagamer.com/dashboard/password");
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            Process.Start("http://esagamer.com");
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Button1.PerformClick();
                e.Handled = true;
            }
        }

        private void Button1_MouseHover(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.login_hover;
        }

        private void Button1_MouseLeave(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.login_normal;
        }

        private void Button3_MouseHover(object sender, EventArgs e)
        {
            Button3.Image = ESA_AC.Properties.Resources.create_hover;
        }

        private void Button3_MouseLeave(object sender, EventArgs e)
        {
            Button3.Image = ESA_AC.Properties.Resources.create_normal;
        }

        private void TextBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (TextBox2.Text.Equals("password"))
            {
                TextBox2.Text = "";
                TextBox2.UseSystemPasswordChar = true;
            }
        }

    }
}
