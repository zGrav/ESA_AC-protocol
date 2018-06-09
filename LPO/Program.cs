using System;
using LPO.Global;
using ESA_AC.Utillity;

namespace LPO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
            AeroControl ac = new AeroControl();
            ac.ControlAero(false);
            Core.Initialize();
        }
    }
}
