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
        private const string pathToSource = @"C:\Users\Lera\Desktop\магистрская\segmentation\";

        static void Main(string[] args)
        {
            var img = IOimage.ReadImage(pathToSource + "1.png");
            var labeledImage = IOimage.ReadImage(pathToSource + "1_labeled.png");
            var labelColors = new List<Color> { Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255) };
            var imageMatrix = new List<List<Color>>();
            var labeledMatrix = GetLabelMatrix(labeledImage, img, labelColors, imageMatrix);
            GetResultImage(labeledMatrix, pathToSource + "1_.png", 0);
            var parity = 1;
            var i = 0;
            while (i < 100)
            {
                //if (i % 10 == 0)
                //    IterationRMult(imageMatrix, labeledMatrix, parity);
                //else
                    IterationMult(imageMatrix, labeledMatrix, parity);
                
                i++;
                parity = (parity + 1) % 2;

                GetResultImage(labeledMatrix, pathToSource + "1_.png", i);
            }
        }

        public static void Iteration(List<List<Color>> img, List<List<ProbabilityDistribution>> labeled, int parity)
        {
            for (int i = 0; i < img.Count; i++)
            {
                for (int j = 0; j < img[0].Count; j++)
                {
                    if ((i + j) % 2 == parity)
                    {
                        var upperNeighbor = (i - 1 >= 0) ? labeled[i - 1][j] : new ProbabilityDistribution(0, 0);
                        var leftNeighbor = (j - 1 >= 0) ? labeled[i][j - 1] : new ProbabilityDistribution(0, 0);
                        var rightNeighbor = (j + 1 < img[0].Count) ? labeled[i][j + 1] : new ProbabilityDistribution(0, 0);
                        var lowNeighbor = (i + 1 < img.Count) ? labeled[i + 1][j] : new ProbabilityDistribution(0, 0);
                        var currentPixel = img[i][j];
                        var iMax = img.Count;
                        var jMax = img[0].Count;
                        var p1 =  (upperNeighbor.FirstClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.FirstClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.FirstClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.FirstClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));
                        var p2 = (upperNeighbor.SecondClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.SecondClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.SecondClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.SecondClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));

                        labeled[i][j] = new ProbabilityDistribution((double)p1 / (p1 + p2), (double)p2 / (p1 + p2));
                    }
                }
            }
        }

        public static void IterationMult(List<List<Color>> img, List<List<ProbabilityDistribution>> labeled, int parity)
        {
            for (int i = 0; i < img.Count; i++)
            {
                for (int j = 0; j < img[0].Count; j++)
                {
                    if ((i + j) % 2 == parity)
                    {
                        var upperNeighbor = (i - 1 >= 0) ? labeled[i - 1][j] : new ProbabilityDistribution(0, 0);
                        var leftNeighbor = (j - 1 >= 0) ? labeled[i][j - 1] : new ProbabilityDistribution(0, 0);
                        var rightNeighbor = (j + 1 < img[0].Count) ? labeled[i][j + 1] : new ProbabilityDistribution(0, 0);
                        var lowNeighbor = (i + 1 < img.Count) ? labeled[i + 1][j] : new ProbabilityDistribution(0, 0);
                        var multFirstClass = upperNeighbor.FirstClass * leftNeighbor.FirstClass * rightNeighbor.FirstClass * lowNeighbor.FirstClass;
                        var multSecClass = upperNeighbor.SecondClass * leftNeighbor.SecondClass * rightNeighbor.SecondClass * lowNeighbor.SecondClass;
                        var currentPixel = img[i][j];
                        var iMax = img.Count;
                        var jMax = img[0].Count;
                        var p1 = multFirstClass*3 + (upperNeighbor.FirstClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.FirstClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.FirstClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.FirstClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));
                        var p2 = multSecClass*3 + (upperNeighbor.SecondClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.SecondClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.SecondClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.SecondClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));

                        labeled[i][j] = new ProbabilityDistribution((double)p1 / (p1 + p2), (double)p2 / (p1 + p2));
                    }
                }
            }
        }

        public static void IterationR(List<List<Color>> img, List<List<ProbabilityDistribution>> labeled, int parity)
        {
            var iMax = img.Count;
            var jMax = img[0].Count;
            var rnd = new Random();
            for (int i = 0; i < img.Count; i++)
            {
                for (int j = 0; j < img[0].Count; j++)
                {
                    if ((i + j) % 2 == parity)
                    {
                        var rnd1 = (double)rnd.Next(0, 1000)/1000;
                        var upperNeighbor = (i - 1 >= 0) ? (rnd1 < labeled[i - 1][j].FirstClass ? new ProbabilityDistribution(1,0)
                            : new ProbabilityDistribution(0,1)) : new ProbabilityDistribution(0, 0);
                        var rnd2 = (double)rnd.Next(0, 1000) / 1000;
                        var leftNeighbor = (j - 1 >= 0) ? (rnd2 < labeled[i][j - 1].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var rnd3 = (double)rnd.Next(0, 1000) / 1000;
                        var rightNeighbor = (j + 1 < jMax) ? (rnd3 < labeled[i][j + 1].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var rnd4 = (double)rnd.Next(0, 1000) / 1000;
                        var lowNeighbor = (i + 1 < iMax) ? (rnd4 < labeled[i + 1][j].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var currentPixel = img[i][j];
                        var p1 = (upperNeighbor.FirstClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.FirstClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.FirstClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.FirstClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));
                        var p2 = (upperNeighbor.SecondClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.SecondClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.SecondClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.SecondClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));

                        labeled[i][j] = new ProbabilityDistribution((double)p1 / (p1 + p2), (double)p2 / (p1 + p2));
                    }
                }
            }
        }

        public static void IterationRMult(List<List<Color>> img, List<List<ProbabilityDistribution>> labeled, int parity)
        {
            var iMax = img.Count;
            var jMax = img[0].Count;
            var rnd = new Random();
            for (int i = 0; i < img.Count; i++)
            {
                for (int j = 0; j < img[0].Count; j++)
                {
                    if ((i + j) % 2 == parity)
                    {
                        var rnd1 = (double)rnd.Next(0, 1000) / 1000;
                        var upperNeighbor = (i - 1 >= 0) ? (rnd1 < labeled[i - 1][j].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var rnd2 = (double)rnd.Next(0, 1000) / 1000;
                        var leftNeighbor = (j - 1 >= 0) ? (rnd2 < labeled[i][j - 1].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var rnd3 = (double)rnd.Next(0, 1000) / 1000;
                        var rightNeighbor = (j + 1 < jMax) ? (rnd3 < labeled[i][j + 1].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var rnd4 = (double)rnd.Next(0, 1000) / 1000;
                        var lowNeighbor = (i + 1 < iMax) ? (rnd4 < labeled[i + 1][j].FirstClass ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1)) : new ProbabilityDistribution(0, 0);
                        var multFirstClass = upperNeighbor.FirstClass * leftNeighbor.FirstClass * rightNeighbor.FirstClass * lowNeighbor.FirstClass;
                        var multSecClass = upperNeighbor.SecondClass * leftNeighbor.SecondClass * rightNeighbor.SecondClass * lowNeighbor.SecondClass;

                        var currentPixel = img[i][j];
                        var p1 = multFirstClass+(upperNeighbor.FirstClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.FirstClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.FirstClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.FirstClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));
                        var p2 = multSecClass+(upperNeighbor.SecondClass *
                                  ((i - 1 >= 0) ? ColorMetric(currentPixel, img[i - 1][j]) : 0)
                                  +
                                  leftNeighbor.SecondClass *
                                  ((j - 1 >= 0) ? ColorMetric(currentPixel, img[i][j - 1]) : 0)
                                  +
                                  lowNeighbor.SecondClass *
                                  ((i + 1 < iMax) ? ColorMetric(currentPixel, img[i + 1][j]) : 0)
                                  +
                                  rightNeighbor.SecondClass *
                                  ((j + 1 < jMax) ? ColorMetric(currentPixel, img[i][j + 1]) : 0));

                        labeled[i][j] = new ProbabilityDistribution((double)p1 / (p1 + p2), (double)p2 / (p1 + p2));
                    }
                }
            }
        }


        public static double ColorMetric(Color color1, Color color2)
        {
            var distance = Math.Abs(color1.R - color2.R) +
                           Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
            if (765 - distance == 0)
            {
                var a = distance;
            }
            return (Math.Abs(765 - distance) * 100)
                ;
        }

        public static double ColorMetric2(Color color1, Color color2)
        {
            var distance = Math.Sqrt(Math.Pow(color1.R - color2.R, 2) +
                           Math.Pow(color1.G - color2.G, 2) + Math.Pow(color1.B - color2.B, 2));
            //if (distance == 0)
            //    return 10;
            return (1/distance)
                ;
        }

        public static List<List<ProbabilityDistribution>> GetLabelMatrix(Bitmap img, Bitmap img2, List<Color> labelColors, List<List<Color>> imageMatrix)
        {
            var labels = new Dictionary<string, int>();
            var labeledMatrix = new List<List<ProbabilityDistribution>>();
            var rnd = new Random();
            var k = 0;
            foreach (var labelColor in labelColors)
                labels.Add(labelColor.R + " " + labelColor.G + " " + labelColor.B, ++k);
            for (var i = 0; i < img.Width; i++)
            {
                labeledMatrix.Add(new List<ProbabilityDistribution>());
                imageMatrix.Add(new List<Color>());
                for (var j = 0; j < img.Height; j++)
                {
                    var curPixel = img.GetPixel(i, j);
                    var curPixel2 = img2.GetPixel(i, j);
                    imageMatrix[i].Add(curPixel2);
                    if (labels.ContainsKey(curPixel.R + " " + curPixel.G + " " + curPixel.B))
                    {
                        labeledMatrix[i].Add((labels[curPixel.R + " " + curPixel.G
                                                     + " " + curPixel.B] == 1)
                            ? new ProbabilityDistribution(1, 0)
                            : new ProbabilityDistribution(0, 1));
                    }
                    else
                    {
                        var p = (double)rnd.Next(0, 1000) / 1000;
                        labeledMatrix[i].Add(new ProbabilityDistribution(p, 1 - p));
                    }
                }
            }
            return labeledMatrix;
        }
        public static List<List<int>> GetResultImage(List<List<ProbabilityDistribution>> matrix, string path, int k)
        {
            Bitmap img = IOimage.ReadImage(path);
            var resultMatrix = new List<List<int>>();
            for (var i = 0; i < matrix.Count; i++)
            {
                resultMatrix.Add(new List<int>());
                for (var j = 0; j < matrix[0].Count; j++)
                {
                    resultMatrix[i].Add((matrix[i][j].FirstClass > matrix[i][j].SecondClass) ? 1 : 2);
                    img.SetPixel(i, j, (resultMatrix[i][j] == 1) ? Color.Red : Color.Blue);
                }
            }
            IOimage.SaveImage(img, pathToSource, k);
            return resultMatrix;
        }

    }
}

