using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using LPO.Global;
using LPO.Utillity;
using ESA_AC;

namespace LPO.GameRuntimeCheck
{
    class FileManagement // Class responsible for managing files and folder creations over HTTP protocol towards a PHP script.
    {

        internal void createFolder(int arg, string username, string game, string matchid)
        {
            WebProxy proxy = WebProxy.GetDefaultProxy();
            proxy.UseDefaultCredentials = true;
            System.Net.WebClient WC = new System.Net.WebClient();
            string requestString = GlobalSettings.Website_URL + "/folders.php?arg=" + arg + "&username=" + username + "&game=" + game + "&matchid=" + matchid;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WC.Proxy = proxy;
            WC.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WC_DownloadStringCompleted);
            WC.DownloadStringAsync(new Uri(requestString));
        }

        void WC_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                switch (TextHandling.Parse(e.Result))
                {
                    case 0:
                        {
                            //continue
                            break;
                        }
                    case 1:
                        {
                            //continue
                            break;
                        }
                    case 2:
                        {
                            //continue
                            break;
                        }
                    default:
                        {
                            ReportForm ef = new ReportForm();
                            ef.label2.Text = e.Result.ToString();
                            ef.Show();
                            break;
                        }
                }
            }
            catch (Exception)
            {
                //we assume the server is down.
                Environment.Exit(0);
            }
        }

        internal void postFile(string username, string getGame, string matchid, string filepath)
        {
            try
            {
                WebProxy proxy = WebProxy.GetDefaultProxy();
                proxy.UseDefaultCredentials = true;
                var myClient = new WebClientEx();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                myClient.Headers.Add("Content-Type", "binary/octet-stream");
                myClient.Proxy = proxy;
                myClient.Timeout = 86400000;
                myClient.filepath = filepath;
                byte[] result = myClient.UploadFile(GlobalSettings.Website_URL + "/upload.php?user=" + username + "&game=" + getGame + "&matchid=" + matchid, "POST", filepath);
                
                string response = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);

                if (response.Contains("Invalid") || response.Contains("Failed!"))
                {
                    ReportForm ef = new ReportForm();
                    ef.label2.Text = response;
                    ef.Show();
                }
            }
            catch (Exception)
            {
                ReportForm ef = new ReportForm();
                ef.label2.Text = "Upload failed";
                ef.Show();
            }
        }
    }

    public class WebClientEx : WebClient
    {

        public int Timeout { get; set; }
        public string filepath { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }
    }
}
