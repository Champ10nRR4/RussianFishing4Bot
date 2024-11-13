using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace KeyPresser
{
    public class TemplateMatchDetector
    {

        public bool DetectValid(byte[] imageBuffer, string iamgeName)
        {
            var result = false;
            Mat image = new Mat();
            CvInvoke.Imdecode(imageBuffer, ImreadModes.Color, image);
            var fileName = iamgeName;// ? "template_blue.png" : "template.png";

            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            Mat template = CvInvoke.Imread(filePath, ImreadModes.Color);
            Mat templateBlue = CvInvoke.Imread(filePath, ImreadModes.Color);


            // Create the result matrix
            Mat resultMatrix = new Mat();

            //// Perform template matching
            CvInvoke.MatchTemplate(image, template, resultMatrix, TemplateMatchingType.CcoeffNormed);

            //// Find the location of the best match
            double minVal = 0.0, maxVal = 0.0;
            Point minLoc = new Point(), maxLoc = new Point();
            CvInvoke.MinMaxLoc(resultMatrix, ref minVal, ref maxVal, ref minLoc, ref maxLoc);


            CvInvoke.Threshold(resultMatrix, resultMatrix, 0.85, 1, Emgu.CV.CvEnum.ThresholdType.ToZero);

            var matches = resultMatrix.ToImage<Gray, byte>();

            for (int i = 0; i < matches.Rows; i++)
            {
                for (int j = 0; j < matches.Cols; j++)
                {
                    if (matches[i, j].Intensity > 0.8)
                    {
                        result = true;
                        //System.Drawing.Point loc = new System.Drawing.Point(j, i);
                        //System.Drawing.Rectangle box = new System.Drawing.Rectangle(loc, template.Size);

                        //CvInvoke.Rectangle(image, box, new Emgu.CV.Structure.MCvScalar(0, 255, 0), 2);
                    }
                }
            }

            //CvInvoke.Imshow("templates detected", image);
            //CvInvoke.Imshow("templateOutput", resultMatrix);



            // Draw a rectangle around the matched region
            //Rectangle match = new Rectangle(maxLoc, template.Size);
            //CvInvoke.Rectangle(image, match, new MCvScalar(0, 255, 0), 2);

            ////// Save the output image
            //CvInvoke.Imwrite("C:\\Users\\vgpri\\OneDrive\\Documents\\Russian Fishing 4\\Screenshots\\output.jpg", image);

            //Console.WriteLine("Template matching done. Check output.jpg for the result.");

            return result;
        }
    }
}
