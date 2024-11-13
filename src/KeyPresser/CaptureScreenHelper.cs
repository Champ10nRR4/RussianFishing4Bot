using System.Drawing.Imaging;

namespace KeyPresser
{
    public static class CaptureScreenHelper
    {
        public static byte[] CaptureScreen(int xstart, int ystart, int xwidth, int xheight)
        {
            try
            {
                Bitmap captureBitmap = new Bitmap(xwidth, xheight, PixelFormat.Format32bppArgb);

                Rectangle captureRectangle = new Rectangle(xstart, ystart, xwidth, xheight);// Screen.AllScreens[0].Bounds;
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                //captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                captureGraphics.CopyFromScreen(xstart, ystart, 0, 0, captureRectangle.Size);
                using (var memStream = new MemoryStream()) { 
                    captureBitmap.Save(memStream, ImageFormat.Jpeg);
                    return memStream.ToArray();
                }
            }
            catch (Exception ex)
            {   
                MessageBox.Show(ex.Message);
                return null;
            }

        }
    }
}
