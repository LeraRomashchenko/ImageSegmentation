using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentation
{
    class Program
    {
        private const int NumberOfClass = 3;
        private const string pathToSource = @"C:\Users\Lera\Desktop\магистрская\segmentation\";
        static void Main(string[] args)
        {
            var img = IOimage.ReadImage(pathToSource + "1.jpg");
            var labeledImage = IOimage.ReadImage(pathToSource + "1_labeled.png");
            File.WriteAllText(pathToSource + "log", "");
            var labelColors = new List<Color> {Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255)};
            var labeledMatrix = GetLabelMatrix(labeledImage, labelColors);
            Webdiff.Debug.PrintMatrix(labeledMatrix, pathToSource + "out");

            var parity = 0;
            var i = 0;
            var _res = GetResultImage(labeledMatrix, pathToSource + "2.jpg", pathToSource);
           // Webdiff.Debug.PrintMatrix(_res, pathToSource + "out2");
            while (i < 20)
            {
                Iteration(labeledMatrix, img, parity);
                i++;
                parity = (parity + 1) % 2;

                var _res1 = GetResultImage(labeledMatrix, pathToSource + "2.jpg", pathToSource);
               // Webdiff.Debug.PrintMatrix(labeledMatrix, pathToSource + "out");
                //Webdiff.Debug.PrintMatrix(_res1, pathToSource + "out2");
            }
        }

        public static List<List<ProbabilityDistribution>> GetLabelMatrix(Bitmap img, List<Color> labelColors)
        {
            var labels = new Dictionary<string, int>();
            var labeledMatrix = new List<List<ProbabilityDistribution>>();
            var rnd = new Random();
            var i = 0;
            foreach (var labelColor in labelColors)
                labels.Add(labelColor.R + " " + labelColor.G + " " + labelColor.B, ++i);
            for (var j = 0; j < img.Width; j++)
            {
                labeledMatrix.Add(new List<ProbabilityDistribution>());
                for (var k = 0; k < img.Height; k++)
                {
                    var curPixel = img.GetPixel(j, k);
                    if (labels.ContainsKey(curPixel.R + " " + curPixel.G + " " + curPixel.B))
                    {
                        labeledMatrix[j].Add((labels[curPixel.R + " " + curPixel.G
                       + " " + curPixel.B] == 1) ? new ProbabilityDistribution(1, 0) : new ProbabilityDistribution(0, 1));
                    }

                    else
                    {
                        var p = (double) rnd.Next(0, 1000) / 1000;
                        labeledMatrix[j].Add(new ProbabilityDistribution(p,1 - p));
                    }
                }
            }
            return labeledMatrix;
        }

        public static void Iteration(List<List<ProbabilityDistribution>> labelMatrix, Bitmap img, int parity)
        {
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    if ((i + j)%2==parity)
                    {
                        if (i == 81 && j == 141)
                        {
                            var upperNeighbor = (i - 1 >= 0) ? labelMatrix[i - 1][j] : new ProbabilityDistribution(1, 1);
                            var leftNeighbor = (j - 1 >= 0) ? labelMatrix[i][j - 1] : new ProbabilityDistribution(1, 1);
                            var rightNeighbor = (j + 1 < img.Height)
                                ? labelMatrix[i][j + 1]
                                : new ProbabilityDistribution(1, 1);
                            var lowNeighbor = (i + 1 < img.Width)
                                ? labelMatrix[i + 1][j]
                                : new ProbabilityDistribution(1, 1);
                            var currentPixel = img.GetPixel(i, j);
                            var p1 = (labelMatrix[i][j].FirstClass + upperNeighbor.FirstClass + leftNeighbor.FirstClass
                                    + rightNeighbor.FirstClass + lowNeighbor.FirstClass) *
                                    ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1) *
                                    ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1) *
                                    ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1) *
                                    ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1);
                            ;
                            var p2 = (labelMatrix[i][j].SecondClass + upperNeighbor.SecondClass + leftNeighbor.SecondClass
                                     + rightNeighbor.SecondClass + lowNeighbor.SecondClass) *
                                     ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1) *
                                     ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1) *
                                     ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1) *
                                     ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1);

                            labelMatrix[i][j] = new ProbabilityDistribution((double) p1/(p1 + p2), (double) p2/(p1 + p2));
                        }
                        else
                        {
                            var upperNeighbor = (i - 1 >= 0) ? labelMatrix[i - 1][j] : new ProbabilityDistribution(1, 1);
                            var leftNeighbor = (j - 1 >= 0) ? labelMatrix[i][j - 1] : new ProbabilityDistribution(1, 1);
                            var rightNeighbor = (j + 1 < img.Height)
                                ? labelMatrix[i][j + 1]
                                : new ProbabilityDistribution(1, 1);
                            var lowNeighbor = (i + 1 < img.Width)
                                ? labelMatrix[i + 1][j]
                                : new ProbabilityDistribution(1, 1);
                            var currentPixel = img.GetPixel(i, j);
                            var p1 = labelMatrix[i][j].FirstClass * (upperNeighbor.FirstClass * 
                                ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1) 
                                + leftNeighbor.FirstClass *
                                ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1) 
                                + rightNeighbor.FirstClass * 
                                ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1)
                                + lowNeighbor.FirstClass* 
                                ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1))
                                      
                            ;
                            var p2 = labelMatrix[i][j].SecondClass * ( upperNeighbor.SecondClass *
                                ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1)
                                + leftNeighbor.SecondClass *
                                ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1)
                                + rightNeighbor.SecondClass *
                                ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1)
                                + lowNeighbor.SecondClass *
                                ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1));
                            var _a = ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1);
                            var _b = ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1);
                            var _c = ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1);
                            var _d = ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1);

                            //var p2 = (labelMatrix[i][j].SecondClass *( upperNeighbor.FirstClass * leftNeighbor.FirstClass
                            //         * rightNeighbor.FirstClass * lowNeighbor.FirstClass) *
                            //         ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1) *
                            //         ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1) *
                            //         ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1) *
                            //         ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1));
                            //;
                            //var p1 = labelMatrix[i][j].FirstClass *( upperNeighbor.SecondClass * leftNeighbor.SecondClass
                            //         * rightNeighbor.SecondClass * lowNeighbor.SecondClass)*
                            //         ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1) *
                            //         ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1) *
                            //         ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1) *
                            //         ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1);
                            //                            if (Double.IsNaN((double) p1/(p1 + p2)))
                            //                            {
                            //var _a = ((i - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i - 1, j)) : 1);
                            //var _b = ((j - 1 >= 0) ? ColorMetric(currentPixel, img.GetPixel(i, j - 1)) : 1);
                            //var _c = ((i + 1 < img.Width) ? ColorMetric(currentPixel, img.GetPixel(i + 1, j)) : 1);
                            //var _d = ((j + 1 < img.Height) ? ColorMetric(currentPixel, img.GetPixel(i, j + 1)) : 1);
                            //File.AppendAllText(pathToSource + "log", p1 + "\t" + p2 + "\n");
                            //                            }
                            labelMatrix[i][j] = new ProbabilityDistribution((double)p1 / (p1 + p2), (double)p2 / (p1 + p2));
                        }
                    }
                }
            }
        }

        public static List<List<int>> GetResultImage(List<List<ProbabilityDistribution>> matrix, string path, string pathOut)
        {
            Bitmap img = IOimage.ReadImage(path);
            var resultMatrix = new List<List<int>>();
            for (var i = 0; i < matrix.Count; i++)
            {
                resultMatrix.Add(new List<int>());
                for (var j = 0; j < matrix[0].Count; j++)
                {
                    resultMatrix[i].Add((matrix[i][j].FirstClass < matrix[i][j].SecondClass) ? 1 : 2);
                    img.SetPixel(i, j, (resultMatrix[i][j] == 2) ? Color.Red : Color.Blue);
                }
            }
            IOimage.SaveImage(img, pathOut);
            return resultMatrix;
        }

        public static double ColorMetric1(Color color1, Color color2)
        {
            var distance = (double)(Math.Pow(color1.R - color2.R, 2) +
                                      Math.Pow(color1.G - color2.G, 2) + Math.Pow(color1.B - color2.B, 2)) / 195075
;
            if (distance == 0)
                return 0;
            return (distance);
        }
        public static double ColorMetric(Color color1, Color color2)
        {
            var distance = (double)(Math.Pow(color1.R - color2.R, 2) +
                                      Math.Pow(color1.G - color2.G, 2) + Math.Pow(color1.B - color2.B, 2))
;
            if (distance == 0)
                return 1;
            return ((double)1/distance);
        }
        public static double ColorMetric2(Color color1, Color color2)
        {
            var distance = (double)(Math.Abs(color1.R - color2.R) +
                                      Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B))
;
            return (765 - distance);
        }
    }
}
