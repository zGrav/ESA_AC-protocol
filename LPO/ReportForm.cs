using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using LPO;

namespace ESA_AC
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_MouseHover(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.report_hover;
        }

        private void Button1_MouseLeave(object sender, EventArgs e)
        {
            Button1.Image = ESA_AC.Properties.Resources.report_normal;
        }

        private int checkforID()
        {
            return MainForm.getID;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int getid = checkforID();

            string getlink = string.Empty;

            if (getid == 0)
            {
                getlink = "https://esagamer.com/support/31/Bug-Report";
            }
            else
            {
                getlink = "https://esagamer.com/matches/details/" + MainForm.getID;
                label3.Text = "Please screenshot this window and" + Environment.NewLine + "report on match comments.";
            }

            Button1.Image = ESA_AC.Properties.Resources.report_active;

            Process.Start(getlink);

            Thread.Sleep(1000);

            Button1.Image = ESA_AC.Properties.Resources.report_normal;
        }
    }
}
