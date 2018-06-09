using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using LPO.Global;
using LPO.Utillity;
using ESA_AC;
using ESA_AC.Utillity;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Management;
using System.IO.Compression;

namespace LPO.GameRuntimeCheck
{
    class RuntimeChecker // Class responsible for running and maintaining our routine
    {
        private ProcessWatchDog watcher;
        private Timer timer;
        private Timer gamerunningtimer;
        private FileStream _fs;
        private string username;
        private string getGameExec = File.ReadAllText(Core.AppPath + "chosengameexec.txt");
        private string getGame = File.ReadAllText(Core.AppPath + "chosengame.txt");
        private string getMatchID = File.ReadAllText(Core.AppPath + "matchid.txt");
        
        //timestamps
        private int getTimeProcesses;
        private int getTimepre;
        public static int getTimeZip;

        private ServerType type;
        internal event CheckerStoppedEvent OnStopped;

        public static string GetFilePath(int processId)
        {
           string result = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId);
                foreach (ManagementObject fp in searcher.Get())
                {
                    result = fp["ExecutablePath"].ToString();
                    break;
                }
                return result;
            }
            catch (Exception)
            {
                result = "";
                return result;
            }
        }

        public static string GetOSFriendlyName()
   {
        string result = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
        foreach (ManagementObject os in searcher.Get())
        {
            result = os["Caption"].ToString();
            break;
        }
        return result;
    }

        private string GetMacAddress()
{
    string macAddresses = string.Empty;

    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
    {
        if (nic.OperationalStatus == OperationalStatus.Up)
        {
            macAddresses += nic.GetPhysicalAddress().ToString();
            break;
        }
    }

    return macAddresses;
}

        public RuntimeChecker(ServerType type, string username)
        {
            this.type = type;
            this.username = username;

            try
            {
                FileManagement fm2 = new FileManagement();
                fm2.createFolder(1, username, getGame, null);
                FileManagement fm3 = new FileManagement();
                fm3.createFolder(2, username, getGame, getMatchID);

                getTimeZip = TextHandling.GetUnixTimestamp();
            }
            catch (Exception)
            {
                    // we assume that the server is down.
                    Environment.Exit(0);
            }

            if (!Directory.Exists(Core.AppPath + "acscreens"))
            {
                Directory.CreateDirectory(Core.AppPath + "acscreens");
            }

            ZipStorer zip;
            zip = ZipStorer.Create(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", "ESA AntiCheat log for match " + getMatchID.ToString());
            zip.Close();

            _fs = new FileStream(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                
            TaskManagerIsRunning();

            ProcessManagement.TriggerTaskmanager(false);

            writePreGameProcesses();

            FileManagement fm4 = new FileManagement();
            fm4.postFile(username, getGame, getMatchID, Core.AppPath + "ac_logbeforegame_" + getTimepre + "_matchid" + getMatchID + ".txt");

            File.Delete(Core.AppPath + "ac_logbeforegame_" + getTimepre + "_matchid" + getMatchID + ".txt");

	    using (ScreenshotDump screenpre = new ScreenshotDump())
            {
                try
                {
                    AeroControl ac = new AeroControl();
                    ac.ControlAero(false);

                    screenpre.SaveToFile(Core.AppPath + "acscreens\\" + "screen_beforegame_" + getTimepre + "_matchid" + getMatchID + ".jpeg");

                    fm4.postFile(username, getGame, getMatchID, Core.AppPath + "acscreens\\" + "screen_beforegame_" + getTimepre + "_matchid" + getMatchID + ".jpeg");

                    File.Delete(Core.AppPath + "acscreens\\" + "screen_beforegame_" + getTimepre + "_matchid" + getMatchID + ".jpeg");
                }

                catch (Exception)
                {
                    File.WriteAllText(Core.AppPath + "acscreens\\captureerror_" + getTimepre + "_matchid" + getMatchID + ".txt", "Failed to grab screenshot! at " + DateTime.Now.TimeOfDay);

                    FileManagement fm = new FileManagement();

                    fm.postFile(username, getGame, getMatchID, Core.AppPath + "acscreens\\captureerror_" + getTimepre + "_matchid" + getMatchID + ".txt");

                    File.Delete(Core.AppPath + "acscreens\\captureerror_" + getTimepre + "_matchid" + getMatchID + ".txt");

                    ReportForm ef = new ReportForm();
                    ef.label2.Text = "Screenshot capture failed!";
                    ef.Show();
                }
            }

            //---timer---
            this.timer = new Timer()
            {
                AutoReset = true,
                Interval = 60000,
                Enabled = true
            };

            timer.Elapsed += tick;
            timer.Start();

            this.watcher = new ProcessWatchDog(1000);
            this.watcher.OnNewProcess += new NewProcessStartedEvent(watcher_OnNewProcess);

            this.gamerunningtimer = new Timer()
            {
                AutoReset = true,
                Interval = 5000,
                Enabled = true
            };

            gamerunningtimer.Elapsed += gametick;
            gamerunningtimer.Start();

          LaunchGame();
        }

        internal void writePreGameProcesses()
        {
            string osname = GetOSFriendlyName();
            string macaddr = GetMacAddress();

            Process[] processlist = System.Diagnostics.Process.GetProcesses();

            GameReport report = new GameReport();
            report.WriteLine("Anti Cheat Version " + Core.AC_Ver + Environment.NewLine);
            report.WriteLine("Anti Cheat Report: " + DateTime.Now.ToString().Remove(11) + "  " + DateTime.Now.TimeOfDay + Environment.NewLine);
            report.WriteLine("From user: " + username + " - playing: " + getGame + Environment.NewLine);
            report.WriteLine("User HWID: " + Core.ComputerHWID + " - Computer Name: " + System.Environment.MachineName + " - Windows Version: " + osname + " - MAC Address: " + macaddr + Environment.NewLine);
            report.WriteLine("Match ID: " + getMatchID + Environment.NewLine);

            getTimepre = TextHandling.GetUnixTimestamp();

            foreach (Process proc_loopVariable in processlist)
            {
                try
                {
                    report.WriteLine(proc_loopVariable.ProcessName + " " + proc_loopVariable.MainWindowTitle + GetFilePath(proc_loopVariable.Id) + Environment.NewLine);
                }
                catch (Exception) { /* no need for user to know about this error */ }
            }

            File.WriteAllText(Core.AppPath + "ac_logbeforegame_" + getTimepre + "_matchid" + getMatchID + ".txt", report.toFile());
        }

        private void LaunchGame()
        {
                    bool notSteam = true;
                    string osname = GetOSFriendlyName();
                    if (type == ServerType.PrimaryServer || type == ServerType.SecondaryServer)
                    {

                        if (getGameExec.Equals("steam://run/730"))
                        {
                            AeroControl ac = new AeroControl();
                            ac.ControlAero(false);
                            Process.Start("steam://run/730");
                            getGameExec = "csgo.exe";
                            File.WriteAllText(Core.AppPath + "chosengameexec.txt", "csgo.exe");
                            notSteam = false;
                        }

                        else  if (getGameExec.Equals("steam://run/440"))
                        {
                            AeroControl ac = new AeroControl();
                            ac.ControlAero(false);

                            Process.Start("steam://run/440");
                            getGameExec = "hl2.exe";
                            File.WriteAllText(Core.AppPath + "chosengameexec.txt", "hl2.exe");
                            notSteam = false;
                        }

                        else if (getGameExec.Equals("steam://run/570"))
                        {
                            AeroControl ac = new AeroControl();
                            ac.ControlAero(false);

                            Process.Start("steam://run/570");
                            getGameExec = "dota.exe";
                            File.WriteAllText(Core.AppPath + "chosengameexec.txt", "dota.exe");
                            notSteam = false;
                        }

                        else  if (notSteam == true)
                        {
                            AeroControl ac = new AeroControl();
                            ac.ControlAero(false);
                            try
                            {
                                Process.Start(File.ReadAllText(Core.AppPath + "chosengamefullexec.txt"));
                            }
                            catch (Exception)
                            {
                                    ErrorForm ef = new ErrorForm();
                                    ef.label2.Text = "Game was not detected on " + Environment.NewLine + "this game path.";
                                    ef.Show();

                                    OnStopped();

                                    CheckIfGameIsRunning();

                                    return;
                            }
                        }
                    }
                }

        internal static bool TaskManagerIsRunning()
        {
            Process[] processes = Process.GetProcessesByName("taskmgr");
            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    processes[i].Kill();
                }

                return true;
            }

            return false;
        }

        internal static bool IsGameRunning(string process)
        {
            Process[] processes = Process.GetProcessesByName(process);
            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    processes[i].Kill();
                }

                return true;
            }

            return false;
        }

        internal void writeProcesses()
        {
            string osname = GetOSFriendlyName();
            string macaddr = GetMacAddress();

            Process[] processlist = System.Diagnostics.Process.GetProcesses();

            GameReport report = new GameReport();
            report.WriteLine("Anti Cheat Version " + Core.AC_Ver + Environment.NewLine);
            report.WriteLine("Anti Cheat Report: " + DateTime.Now.ToString().Remove(11) + "  " + DateTime.Now.TimeOfDay + Environment.NewLine);
            report.WriteLine("From user: " + username + " - playing: " + getGame + Environment.NewLine);
            report.WriteLine("User HWID: " + Core.ComputerHWID + " - Computer Name: " + System.Environment.MachineName + " - Windows Version: " + osname + " - MAC Address: " + macaddr + Environment.NewLine);
            report.WriteLine("Match ID: " + getMatchID + Environment.NewLine);

            foreach (Process process in processlist)
            {
                try
                {
                    report.WriteLine(process.ProcessName + " " + process.MainWindowTitle + " " + GetFilePath(process.Id) + Environment.NewLine);
                }
                catch (Exception) { /* the user does not need to know about this error. */ }
            }

            getTimeProcesses = TextHandling.GetUnixTimestamp();

            File.WriteAllText(Core.AppPath + "ac_log_" + getTimeProcesses + "_matchid" + getMatchID + ".txt", report.toFile());
        }

        private void Execute()
        {
            try
            {
                _fs.Close();

                try
                {
                    FileManagement fm1 = new FileManagement();
                    fm1.createFolder(0, username, null, null);
                    FileManagement fm2 = new FileManagement();
                    fm2.createFolder(1, username, getGame, null);
                    FileManagement fm3 = new FileManagement();
                    fm3.createFolder(2, username, getGame, getMatchID);
                }
                catch (Exception)
                {
                    // we assume that the server is down.
                    Environment.Exit(0);
                }

                FileStream fs = new FileStream(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
               
                writeProcesses();

                FileManagement fm = new FileManagement();

                fs.Close();

                ZipStorer zip;

                zip = ZipStorer.Open(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileAccess.Write);
                zip.AddFile(ZipStorer.Compression.Store, Core.AppPath + "ac_log_" + getTimeProcesses + "_matchid" + getMatchID + ".txt", "ac_log_" + getTimeProcesses + "_matchid" + getMatchID + ".txt", "Anticheat process log during game for match ID " + getMatchID.ToString() + "and at " + DateTime.Now.ToString());
                zip.Close();

                File.Delete(Core.AppPath + "ac_log_" + getTimeProcesses + "_matchid" + getMatchID + ".txt");

                FileStream fs2 = new FileStream(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                int getTime2 = TextHandling.GetUnixTimestamp();

                using (ScreenshotDump screen = new ScreenshotDump())
                {

                    try
                    {
                        AeroControl ac = new AeroControl();
                        ac.ControlAero(false);

                        screen.SaveToFile(Core.AppPath + "acscreens\\" + "screen_" + getTime2 + "_matchid" + getMatchID + ".jpeg");
                        
                        fs2.Close();

                        zip = ZipStorer.Open(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileAccess.Write);
                        zip.AddFile(ZipStorer.Compression.Store, Core.AppPath + "acscreens\\" + "screen_" + getTime2 + "_matchid" + getMatchID + ".jpeg", "screen_" + getTime2 + "_matchid" + getMatchID + ".jpeg", "Screenshot during game for match ID " + getMatchID.ToString() + "and at " + DateTime.Now.ToString());
                        zip.Close();

                        File.Delete(Core.AppPath + "acscreens\\" + "screen_" + getTime2 + "_matchid" + getMatchID + ".jpeg");
                        
                    }

                    catch (Exception)
                    {
                        File.WriteAllText(Core.AppPath + "acscreens\\captureerror_" + getTime2 + "_matchid" + getMatchID + ".txt", "Failed to grab screenshot! at " + DateTime.Now.TimeOfDay);

                        fm.postFile(username, getGame, getMatchID, Core.AppPath + "acscreens\\captureerror_" + getTime2 + "_matchid" + getMatchID + ".txt");

                        File.Delete(Core.AppPath + "acscreens\\captureerror_" + getTime2 + "_matchid" + getMatchID + ".txt");

                        ReportForm ef = new ReportForm();
                        ef.label2.Text = "Screenshot capture failed!";
                        ef.Show();
                    }
                }

                FileStream fs3 = new FileStream(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                using (ScreenshotBitBltDump screenbitblthandle = new ScreenshotBitBltDump())
                {
                    try
                    {
                        string grabProc;

                        grabProc = File.ReadAllText(Core.AppPath + "chosengameexec.txt");
                        grabProc = grabProc.Substring(0, grabProc.Length - 4);

                        Process[] getHandle = Process.GetProcessesByName(grabProc);

                        foreach (Process p in getHandle)
                        {

                            IntPtr windowHandle = p.MainWindowHandle;

                            AeroControl ac = new AeroControl();
                            ac.ControlAero(false);

                            screenbitblthandle.CaptureWindowToFile(windowHandle, Core.AppPath + "acscreens\\" + "screen_bitblt_handle_" + getTime2 + "_matchid" + getMatchID + ".jpeg", ImageFormat.Jpeg);

                            fs3.Close();

                            zip = ZipStorer.Open(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileAccess.Write);
                            zip.AddFile(ZipStorer.Compression.Store, Core.AppPath + "acscreens\\" + "screen_bitblt_handle_" + getTime2 + "_matchid" + getMatchID + ".jpeg", "screen_bitblt_handle_" + getTime2 + "_matchid" + getMatchID + ".jpeg", "Screenshot aggro mode during game for match ID " + getMatchID.ToString() + "and at " + DateTime.Now.ToString());
                            zip.Close();

                            File.Delete(Core.AppPath + "acscreens\\" + "screen_bitblt_handle_" + getTime2 + "_matchid" + getMatchID + ".jpeg");

                        }
                    }
                    catch (Exception)
                    {
                        File.WriteAllText(Core.AppPath + "acscreens\\captureerror_" + getTime2 + "_matchid" + getMatchID + ".txt", "Failed to grab screenshot! at " + DateTime.Now.TimeOfDay);

                        fm.postFile(username, getGame, getMatchID, Core.AppPath + "acscreens\\captureerror_" + getTime2 + "_matchid" + getMatchID + ".txt");

                        File.Delete(Core.AppPath + "acscreens\\captureerror_" + getTime2 + "_matchid" + getMatchID + ".txt");

                        ReportForm ef = new ReportForm();
                        ef.label2.Text = "Screenshot capture failed!";
                        ef.Show();
                    }
                }

                FileStream fs4 = new FileStream(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                if (ProcessManagement.ProcessIsRunning("taskmgr.exe"))
                {
                    int tskmgrtimestamp = TextHandling.GetUnixTimestamp();

                    GameReport taskmgrReport = new GameReport();

                    taskmgrReport.WriteLine("Taskmgr opened while anticheat running for user " + username + " at " + DateTime.Now.TimeOfDay + " timestamp: " + tskmgrtimestamp + " on match: " + getMatchID);

                    File.WriteAllText(Core.AppPath + "acscreens\\taskmgr.txt", taskmgrReport.toFile());

                    fm.postFile(username, getGame, getMatchID, Core.AppPath + "acscreens\\taskmgr.txt");

                    File.Delete(Core.AppPath + "acscreens\\taskmgr.txt");

                    TaskManagerIsRunning();
                }

                fs4.Close();
            }
            catch (Exception ex)
            {
                ReportForm ef = new ReportForm();
                ef.label2.Text = ex.ToString();
                ef.Show();
            }
        }

        internal void tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Execute();

            _fs = new FileStream(Core.AppPath + "acscreens\\log_" + getMatchID + "_" + getGame + "_" + getTimeZip + ".zip", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        internal void gametick(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckIfGameIsRunning();
        }

        internal void CheckIfGameIsRunning()
        {
            if (!ProcessManagement.ProcessIsRunning(File.ReadAllText(Core.AppPath + "chosengameexec.txt")))
            {
                ProcessManagement.TriggerTaskmanager(true);
                timer.Enabled = false;
                timer.Stop();

                gamerunningtimer.Enabled = false;
                gamerunningtimer.Stop();

                this.watcher.OnNewProcess -= new NewProcessStartedEvent(watcher_OnNewProcess);

                _fs.Close();

                if (OnStopped != null)
                    OnStopped();
            }
        }

        private void watcher_OnNewProcess(int PID)
        {
            try {
            Process process = Process.GetProcessById(PID);

            GameReport report = new GameReport();
            report.WriteLine("New process started => " + process.ProcessName + " " + process.MainWindowTitle + " " + GetFilePath(PID) + " " + DateTime.Now.ToString() + Environment.NewLine + "On matchID: " + getMatchID + Environment.NewLine);

            File.AppendAllText(Core.AppPath + "newproc.txt", report.toFile());

            FileManagement fm = new FileManagement();
            fm.postFile(username, getGame, getMatchID, Core.AppPath + "newproc.txt");

            File.Delete(Core.AppPath + "newproc.txt");

            }
            catch (IOException)
            {
                // we continue, no need to warn user about this error.
            }
            catch (ArgumentException)
            {
                // we continue, no need to warn user about this error.
            }
        }
    }

    enum ServerType //no longer used
    {
        PrimaryServer = 1, // mainserver
        SecondaryServer = 2 //backupserver
    }

    delegate void CheckerStoppedEvent();
}
