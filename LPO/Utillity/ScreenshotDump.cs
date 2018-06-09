using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace LPO.Utillity
{
    class ScreenshotDump : IDisposable // Class responsible for capturing a Screenshot using CopyFromScreen
    {
        //Code from http://www.crowsprogramming.com/archives/78

        private Bitmap bitmap;

        public ScreenshotDump()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            this.bitmap = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                gr.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
        }

        internal void SaveToFile(string path)
        {
                bitmap.Save(path, ImageFormat.Jpeg);
        }

        public void Dispose()
        {
            bitmap.Dispose();
            bitmap = null;
        }
    }
}