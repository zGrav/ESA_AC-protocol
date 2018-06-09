using Microsoft.Win32;
using System;
using System.Diagnostics;
using ESA_AC;

namespace LPO.Utillity
{
    class ProcessManagement // Class responsible to check for a specified process
    {
        internal static unsafe bool ProcessIsRunning(string processName)
        {
            fixed (char* namePtr = processName)
            {
                return (NativeProcessHelper.Helper.ContainsProcess(namePtr) == 1);
            }
        }

        internal static void TriggerTaskmanager(bool enable)
        {
            try
            {
                RegistryKey systemRegistry = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");

                systemRegistry.SetValue("DisableTaskMgr", enable ? 0 : 1);
                systemRegistry.Close();
            }
            catch (UnauthorizedAccessException)
            {
                ErrorForm ef = new ErrorForm();
                ef.label2.Text = "This program needs to be run as Administrator." + Environment.NewLine + "Program terminating.";
                ef.Show();
                Environment.Exit(0);
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
    }
}
