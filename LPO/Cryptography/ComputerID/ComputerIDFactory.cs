using System.Management;
using System.Text;

namespace LPO.Cryptography.ComputerID
{
    class ComputerIDFactory
    {
        internal static string GenerateComputerID()
        {
            //Creates the HWID from the ProcessorID, Video Controller RAM and the size of the disk drive

            StringBuilder computerID = new StringBuilder();
            ManagementObjectSearcher searcher;

            searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                computerID.Append(queryObj["ProcessorId"]);
            }

            searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                computerID.Append(queryObj["AdapterRAM"]);
            }

            searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                computerID.Append(queryObj["Size"]);
            }

            return computerID.ToString();
        }
    }
}
