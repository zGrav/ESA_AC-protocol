using System;
using System.Windows.Forms;

namespace ESA_AC
{
    public partial class ErrorForm : Form
    {
        public ErrorForm()
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


    }
}
