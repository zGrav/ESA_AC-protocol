using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ESA_AC.Utillity
{
    class AeroControl // Class responsible for disabling Aero
    {
        public readonly uint DWM_EC_DISABLECOMPOSITION = 0;
        public readonly uint DWM_EC_ENABLECOMPOSITION = 1;
        [DllImport("dwmapi.dll", EntryPoint = "DwmEnableComposition")]
        protected extern static uint Win32DwmEnableComposition(uint uCompositionAction);
        public bool ControlAero(bool enable)
        {
            try
            {
                if (enable)
                    Win32DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
                if (!enable)
                    Win32DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);

                return true;
            }
            catch { return false; }
        }
    }
}
