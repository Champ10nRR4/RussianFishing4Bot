using Emgu.CV;
using Emgu.CV.CvEnum;

namespace KeyPresser
{
    public class IconFounder
    {
        Mat image = CvInvoke.Imread("path_to_your_image.jpg", ImreadModes.Color);
        Mat template = CvInvoke.Imread("path_to_your_template.png", ImreadModes.Color);

        Mat res = new Mat();

        
    }
}
