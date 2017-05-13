using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentation
{
    class Program
    {
        private const int NumberOfClass = 3;
        static void Main(string[] args)
        {
            var img = IOimage.ReadImage(@"C:\Users\romashchenko\Desktop\diplom\segmentation\1.jpg");
            var labeledImage = IOimage.ReadImage(@"C:\Users\romashchenko\Desktop\diplom\segmentation\1_labeled.jpg");
            var labelColors = new List<Color> {Color.FromArgb(255, 0, 0), Color.FromArgb(0, 0, 255), Color.FromArgb(255,255,0)};
            var labeledMatrix = GetLabelMatrix(labeledImage, labelColors);
            Webdiff.Debug.PrintMatrix(labeledMatrix, @"C:\Users\romashchenko\Desktop\diplom\segmentation\out");
        }

        public static List<List<int>> GetLabelMatrix(Bitmap img, List<Color> labelColors)
        {
            var labels = new Dictionary<Color, int>();
            var labeledMatrix = new List<List<int>>();
            var i = 1;
            foreach (var labelColor in labelColors)
            {
                labels.Add(labelColor, i);
                i++;
            }
            for (var j=0; j<img.Width; j++)
            {
                labeledMatrix.Add(new List<int>());
                for (var k=0; k< img.Height; k++)
                {
                    var curPixel = img.GetPixel(j, k);
                    labeledMatrix[j].Add(labels.ContainsKey(curPixel) ? labels[curPixel] : 0);
                }
            }
            return labeledMatrix;
        }
    }
}
