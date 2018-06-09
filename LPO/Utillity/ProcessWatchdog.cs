using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LPO.Utillity
{
    class ProcessWatchDog // Class responsible for monitoring newly opened processes
    {
        /// <summary>
        /// Event that gets called when the process is started
        /// </summary>
        internal event NewProcessStartedEvent OnNewProcess;

        /// <summary>
        /// Holds a list containing loaded processes
        /// </summary>
        private List<int> loadedProcesses;

        /// <summary>
        /// The timer that calls back when we are gonna check if the process is running
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializes the process scout class
        /// </summary>
        /// <param name="processName">The name of the process</param>
        public ProcessWatchDog(int checkInterval)
        {
            this.timer = new Timer()
            {
                Enabled = true,
                Interval = checkInterval,
            };

            this.timer.Elapsed += new ElapsedEventHandler(onProcessCheck);

            RegisterProcesses();
        }

        /// <summary>
        /// Adds all the procesess PID to the loadedProcesses list
        /// </summary>
        private void RegisterProcesses()
        {
            this.loadedProcesses = NativeProcessHelper.Helper.GetPIDList();
        }

        /// <summary>
        /// Callback from the timer class
        /// </summary>
        /// <param name="sender">sener object</param>
        /// <param name="e">Event arg object</param>
        private void onProcessCheck(object sender, ElapsedEventArgs e)
        {
            List<int> processes = NativeProcessHelper.Helper.GetPIDList();

            int PID;
            for (int i = 0; i < processes.Count; i++)
            {
                PID = processes[i];
                if (!loadedProcesses.Contains(PID))
                {
                    if (OnNewProcess != null)
                        OnNewProcess(PID);

                    loadedProcesses.Add(PID);
                }
            }
        }
    }

    delegate void NewProcessStartedEvent(int pid);
}
