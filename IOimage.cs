using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentation
{
    class IOimage
    {
        public static Bitmap ReadImage(string path)
        {
            return new Bitmap(Image.FromFile(path));
        }

        public static void SaveImage(Bitmap image, string path)
        {
            var filename = path + "out.png";
            image.Save(filename);
        }

    }
}
