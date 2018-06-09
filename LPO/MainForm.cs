using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LPO.GameRuntimeCheck;
using LPO.Global;
using LPO.Utillity;
using ESA_AC;
using System.Threading;
using Microsoft.Win32;
using System.IO.Compression;

namespace LPO
{
    public partial class MainForm : Form
    {
        private string username;
        public static int getID;
        private string selectedGame = "";

        public MainForm()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(MainForm_FormClosed);

            this.KeyPreview = true;

            try
            {
            username = File.ReadAllText(Core.AppPath + "myusername.txt");
            
            label2.Text = username;

            connectedAsusernameToolStripMenuItem.Text = "Connected as: " + username;

            try
            {
                FileManagement fm = new FileManagement();
                fm.createFolder(0, username, null, null);
            }
            catch (Exception)
            {
                ReportForm ef = new ReportForm();
                    ef.label2.Text = "Failed to connect to server." + Environment.NewLine + "Program terminating.";
                    ef.Show();

                    string grabProc;
                    try
                    {
                        grabProc = File.ReadAllText(Core.AppPath + "chosengameexec.txt");
                        grabProc = grabProc.Substring(0, grabProc.Length - 4);

                        RuntimeChecker.IsGameRunning(grabProc);
                    }
                    catch (Exception)
                    {
                        ReportForm ef2 = new ReportForm();
                        ef2.label2.Text = "Upload failed.";
                        ef2.Show();
                    }
                    Environment.Exit(0);
                }

            if (Directory.Exists(Core.AppPath + "acscreens") == false)
            {
                Directory.CreateDirectory(Core.AppPath + "acscreens");
            }

            
            }

            catch (FileNotFoundException)
            {
                /* user does not need to know about this error */
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

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                ProcessManagement.TriggerTaskmanager(true);
                notifyIcon1.Visible = false;
                //Environment.Exit(0);
            }
            catch (Exception)
            {
                Environment.Exit(0);
            }
        }

        private void startRoutine(DialogResult getdr)
        {
            if (label4.Text.Equals("Scanning"))
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Routine is already running!";
                ef.Show();
                return;
            }

            if (label4.Text.Equals("Uploading"))
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Please wait until uploading ends!";
                ef.Show();
                return;
            }

            else
            {
                try
                {
                    getID = int.Parse(TextBox1.Text);
					
					if (getID == 0) 
					{
					ErrorForm ef = new ErrorForm();
                    ef.label2.Text = "Match ID cannot be 0.";
                    ef.Show();
                    return;
					}
					
                    File.WriteAllText(Core.AppPath + "matchid.txt", TextBox1.Text);
                }
                catch (Exception)
                {
                    ErrorForm ef = new ErrorForm();
                    ef.label2.Text = "Invalid Match ID.";
                    ef.Show();
                    return;
                }

                DialogResult dr = getdr;

                if (dr == DialogResult.Cancel)
                {
                    //we continue
                }

                else if (dr == DialogResult.OK)
                {
                    noGameSelectedToolStripMenuItem.Text = "Scanning game: " + selectedGame;

                    label4.Text = "Scanning";

                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;

                    string grabName;
                    grabName = File.ReadAllText(Core.AppPath + "chosengameexec.txt");

                    if (grabName.Equals("steam://run/730"))
                    {
                        grabName = "csgo.exe";
                    }

                    else if (grabName.Equals("steam://run/440"))
                    {
                        grabName = "hl2.exe";
                    }

                    else if (grabName.Equals("steam://run/570"))
                    {
                        grabName = "dota.exe";
                    }

                    else if (grabName.Equals("Shootmania"))
                    {
                        string read = string.Empty;

                        try
                        {
                            read = File.ReadAllText(Core.AppPath + "chosengamefullexec.txt");
                        }
                        catch (Exception) { }

                        if (!read.Contains("ManiaPlanet"))
                        {
                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.Filter = "maniaplanet.exe|maniaplanet.exe| Exe Files (.exe)|*.exe";
                            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                string fullPath = ofd.InitialDirectory + ofd.FileName;
                                string execName = ofd.SafeFileName;
                                File.WriteAllText(Core.AppPath + "chosengamefullexec.txt", fullPath);
                                File.WriteAllText(Core.AppPath + "chosengameexec.txt", execName);
                            }
                            else
                            {
                                label4.Text = "Idle";

                                this.Show();
                                this.WindowState = FormWindowState.Normal;
                                this.Activate();

                                return;
                            }
                        }

                        File.WriteAllText(Core.AppPath + "chosengameexec.txt", "ManiaPlanet.exe");
                    }

                    if (ProcessManagement.ProcessIsRunning(grabName))
                    {
                        grabName = grabName.Substring(0, grabName.Length - 4);
                        RuntimeChecker.IsGameRunning(grabName); //kills old process and opens new one with anticheat
                        ProcessManagement.TriggerTaskmanager(true);
                        Thread.Sleep(1000);
                    }

                    bool primaryFail = false;

                    try
                   { 
                        RuntimeChecker checker = new RuntimeChecker(ServerType.PrimaryServer, username);
                        checker.OnStopped += new CheckerStoppedEvent(checker_OnStopped);
                    }
                    catch (Exception)
                    {
                        primaryFail = true;
                    }

                    if (primaryFail == true)
                    {
                        this.Show();
                        this.WindowState = FormWindowState.Normal;

                        ReportForm ef = new ReportForm();
                        ef.label2.Text = "A error has occurred!";
                        ef.Show();

                        checker_OnStopped();

                        ProcessManagement.TriggerTaskmanager(false);
                        ProcessManagement.TaskManagerIsRunning();

                        button5.Image = ESA_AC.Properties.Resources.startscan_normal;

                        label4.Text = "Uploading";

                        try
                        {
                            if (File.Exists(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip"))
                            {
                                using (FileStream fsread = new FileStream(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip", FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    if (File.Exists(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip"))
                                    {
                                        FileManagement fm = new FileManagement();
                                        fm.postFile(username, selectedGame, getID.ToString(), Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip");

                                        fsread.Close();

                                        File.Delete(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip");
                                    }
                                    else
                                    {
                                        File.WriteAllText(Core.AppPath + "acscreens\\warning.txt", "Anticheat zip log has been deleted.");

                                        FileManagement fm = new FileManagement();
                                        fm.postFile(username, selectedGame, getID.ToString(), Core.AppPath + "acscreens\\warning.txt");

                                        File.Delete(Core.AppPath + "acscreens\\warning.txt");
                                    }

                                }
                            }
                            else
                            {
                                File.WriteAllText(Core.AppPath + "acscreens\\warning.txt", "Anticheat zip log has been deleted.");

                                FileManagement fm = new FileManagement();
                                fm.postFile(username, selectedGame, getID.ToString(), Core.AppPath + "acscreens\\warning.txt");

                                File.Delete(Core.AppPath + "acscreens\\warning.txt");
                            }
                        }
                        catch (Exception)
                        {
                            ReportForm ef2 = new ReportForm();
                            ef2.label2.Text = "Upload failed.";
                            ef2.Show();
                        }

                        ProcessManagement.TriggerTaskmanager(true);

                        noGameSelectedToolStripMenuItem.Text = "No game currently selected";

                        selectedGame = "";

                        label4.Text = "Idle";
                    }
                }
            }
        }

        private void checker_OnStopped()
        {
            CheckForIllegalCrossThreadCalls = false;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            button5.Image = ESA_AC.Properties.Resources.startscan_normal;

            ProcessManagement.TriggerTaskmanager(false);
            ProcessManagement.TaskManagerIsRunning();

            label4.Text = "Uploading";

            try
            {
                if (File.Exists(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip"))
                {
                    using (FileStream fsread = new FileStream(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (File.Exists(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip"))
                        {
                            FileManagement fm = new FileManagement();
                            fm.postFile(username, selectedGame, getID.ToString(), Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip");

                            fsread.Close();

                            File.Delete(Core.AppPath + "acscreens\\log_" + getID + "_" + selectedGame + "_" + RuntimeChecker.getTimeZip + ".zip");
                        }
                        else
                        {
                            File.WriteAllText(Core.AppPath + "acscreens\\warning.txt", "Anticheat zip log has been deleted.");

                            FileManagement fm = new FileManagement();
                            fm.postFile(username, selectedGame, getID.ToString(), Core.AppPath + "acscreens\\warning.txt");

                            File.Delete(Core.AppPath + "acscreens\\warning.txt");
                        }

                    }
                }
                else
                {
                    File.WriteAllText(Core.AppPath + "acscreens\\warning.txt", "Anticheat zip log has been deleted.");

                    FileManagement fm = new FileManagement();
                    fm.postFile(username, selectedGame, getID.ToString(), Core.AppPath + "acscreens\\warning.txt");

                    File.Delete(Core.AppPath + "acscreens\\warning.txt");
                }
            }
            catch (Exception)
            {
                ReportForm ef = new ReportForm();
                ef.label2.Text = "Upload failed.";
                ef.Show();
            }
        
            ProcessManagement.TriggerTaskmanager(true);

            noGameSelectedToolStripMenuItem.Text = "No game currently selected";

            selectedGame = "";

            label4.Text = "Idle";
        }

        private void MainForm_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            try
            {
                if (ProcessManagement.ProcessIsRunning(File.ReadAllText(Core.AppPath + "chosengameexec.txt")))
                {
                    int gameclosedtimestamp = TextHandling.GetUnixTimestamp();

                    GameReport report = new GameReport();

                    report.WriteLine("Anticheat closed while game was running! at " + DateTime.Now.TimeOfDay + " timestamp: " + gameclosedtimestamp + Environment.NewLine + "On match ID: " + getID + Environment.NewLine);

                    File.WriteAllText(Core.AppPath + "acscreens\\gameclosed.txt", report.toFile());

                    try
                    {
                        FileManagement fm = new FileManagement();
                        fm.postFile(username, null, null, Core.AppPath + "acscreens\\gameclosed.txt");
                    }

                    catch (Exception)
                    {
                        ReportForm ef = new ReportForm();
                        ef.label2.Text = "Upload failed.";
                        ef.Show();
                    }

                    File.Delete(Core.AppPath + "acscreens\\gameclosed.txt");

                    string grabName;
                    grabName = File.ReadAllText(Core.AppPath + "chosengameexec.txt");
                    string grabProc;
                    grabProc = grabName;
                    grabProc = grabProc.Substring(0, grabProc.Length - 4);

                    RuntimeChecker.IsGameRunning(grabProc);
                    ProcessManagement.TriggerTaskmanager(true);

                    notifyIcon1.Visible = false;

                    Environment.Exit(0);
                }
            }

            catch (FileNotFoundException)
            {
                /* user does not need to know about this error */
                try
                {
                    string grabName;
                    grabName = File.ReadAllText(Core.AppPath + "chosengameexec.txt");
                    string grabProc;
                    grabProc = grabName;
                    grabProc = grabProc.Substring(0, grabProc.Length - 4);

                    RuntimeChecker.IsGameRunning(grabProc);
                    ProcessManagement.TriggerTaskmanager(true);
                    notifyIcon1.Visible = false;
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    ProcessManagement.TriggerTaskmanager(true);
                    notifyIcon1.Visible = false;
                    Environment.Exit(0);
                }
            }
            ProcessManagement.TriggerTaskmanager(true);
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        
        }

        private void MainForm_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Alt == true & e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessManagement.TriggerTaskmanager(false);

            ProcessManagement.TaskManagerIsRunning();

            if (label4.Text.Equals("Scanning"))
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Game is running!";
                ef.Show();
                return;
            }

            if (label4.Text.Equals("Uploading"))
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Please wait until uploading ends!";
                ef.Show();
                return;
            }

            ProcessManagement.TriggerTaskmanager(true);

            notifyIcon1.Visible = false;

            Environment.Exit(0);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
                ProcessManagement.TriggerTaskmanager(false);

                ProcessManagement.TaskManagerIsRunning();

                if (label4.Text.Equals("Scanning"))
                {
                    ErrorForm ef = new ErrorForm();
                    ef.label2.Text = "Game is running!";
                    ef.Show();
                    return;
                }

                if (label4.Text.Equals("Uploading"))
                {
                    ErrorForm ef = new ErrorForm();
                    ef.label2.Text = "Please wait until uploading ends!";
                    ef.Show();
                    return;
                }

                ProcessManagement.TriggerTaskmanager(true);

                notifyIcon1.Visible = false;

                if (Directory.Exists(Core.AppPath + "acscreens\\") == true)
                {
                    DirectoryInfo di = new DirectoryInfo(Core.AppPath + "acscreens\\");
                    di.Delete(true);
                }

                Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ProcessManagement.TriggerTaskmanager(false);

            ProcessManagement.TaskManagerIsRunning();

            if (label4.Text.Equals("Scanning"))
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Game is running!";
                ef.Show();
                return;
            }

            if (label4.Text.Equals("Uploading"))
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "Please wait until uploading ends!";
                ef.Show();
                return;
            }

            ProcessManagement.TriggerTaskmanager(true);

            pictureBox2.Image = ESA_AC.Properties.Resources.ac_button_back_active;

            if (File.Exists(Core.AppPath + "mypassword.txt"))
            {
                File.Delete(Core.AppPath + "mypassword.txt");
            }

            notifyIcon1.Visible = false;

            if (Directory.Exists(Core.AppPath + "acscreens\\") == true)
            {
                DirectoryInfo di = new DirectoryInfo(Core.AppPath + "acscreens\\");
                di.Delete(true);
            }

            Core.DisplayLoginForm();
            this.Hide();
        }

        private ToolTip toolTip = new ToolTip();

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            pictureBox2.Image = ESA_AC.Properties.Resources.ac_button_back_hover;

            toolTip.SetToolTip(sender as Control, "Logout"); 
            toolTip.IsBalloon = true;
        }

        private void TextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (TextBox1.Text.Equals("Match ID"))
            {
                TextBox1.Text = "";
            }
        }

        public string gameChosen { get; set; }
        public string gameChosenExec { get; set; }


        private void doGame(string chosenGame)
        {

            File.WriteAllText(Core.AppPath + "chosengame.txt", chosenGame);
            
            switch (chosenGame)
            {

                case "Counter-Strike Global Offensive":
                    File.WriteAllText(Core.AppPath + "chosengameexec.txt", "steam://run/730");
                    break;

                case "Dota 2":
                    File.WriteAllText(Core.AppPath + "chosengameexec.txt", "steam://run/570");
                    break;

                case "Shootmania":
                    File.WriteAllText(Core.AppPath + "chosengameexec.txt", "Shootmania");
                    break;

                case "Team Fortress 2":
                    File.WriteAllText(Core.AppPath + "chosengameexec.txt", "steam://run/440");
                    break;

                default: MessageBox.Show("An error occurred.");
                    break;
            }

            try
            {
                gameChosen = File.ReadAllText(Core.AppPath + "chosengame.txt");
                gameChosenExec = File.ReadAllText(Core.AppPath + "chosengameexec.txt");
            }

            catch (FileNotFoundException)
            {
                /* user does not need to know about this error */
            }

            selectedGame = gameChosen;

            noGameSelectedToolStripMenuItem.Text = "Game selected: " + gameChosen;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            doGame("Counter-Strike Global Offensive");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            doGame("Shootmania");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            doGame("Team Fortress 2");
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Counter-Strike: GO");
            toolTip.IsBalloon = true;
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Shootmania");
            toolTip.IsBalloon = true;
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Team Fortress 2");
            toolTip.IsBalloon = true;
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Dota 2");
            toolTip.IsBalloon = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            doGame("Dota 2");
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            Process.Start("http://esagamer.com");
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = ESA_AC.Properties.Resources.ac_button_back_normal;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Image = ESA_AC.Properties.Resources.startscan_active;

            if (!selectedGame.Equals(""))
            {
                startRoutine(DialogResult.OK);
            }

            else
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "No game selected.";
                ef.Show();
                return;
            }
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            button5.Image = ESA_AC.Properties.Resources.startscan_hover;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.Image = ESA_AC.Properties.Resources.startscan_normal;
        }
    }
}
