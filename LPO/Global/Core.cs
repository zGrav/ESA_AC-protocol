using System.Windows.Forms;
using LPO.Cryptography;
using LPO.Cryptography.ComputerID;
using LPO.Utillity;
using System;
using ESA_AC;
using ESA_AC.Utillity;
using ESA_AC.Cryptography;
using System.Diagnostics;

namespace LPO.Global
{
    class Core // Class where some "main" variables are stored
    {
        internal static MD5CryptoService MD5Encoder;

        internal static SHA1CryptoService SHA1Encoder;

        private static string computerHardwareID;

        internal static string AC_Ver = "1.0c";

        internal static string AppPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\esagamerac\\";

        internal static string ComputerHWID
        {
            get
            {
                return computerHardwareID;
            }
        }

        public static void Initialize()
        {
            try
            {
                MD5Encoder = new MD5CryptoService();
                SHA1Encoder = new SHA1CryptoService();

                string hwid = ComputerIDFactory.GenerateComputerID();
                computerHardwareID = MD5Encoder.EncodeHash(hwid);
                ProcessManagement.TriggerTaskmanager(true);

                Application.EnableVisualStyles();
                AeroControl ac = new AeroControl();
                ac.ControlAero(false);
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LoadingForm());
            }
            catch (Exception)
            {
                ReportForm ef = new ReportForm();
                ef.label2.Text = "An error has occurred on initialization!";
                ef.Show();
            }
        }

        internal static void DisplayLoginForm()
        {
            DisplayForm(new LoginForm());
        }

        internal static void DisplayMainForm()
        {
            DisplayForm(new MainForm());
        }

        private static void DisplayForm(Form form)
        {
            form.Show();
        }

    }
}
