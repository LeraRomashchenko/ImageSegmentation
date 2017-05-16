using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageSegmentation;

namespace Webdiff
{
    class Debug
    {
        public static void PrintMatrix(List<List<double>> matrix, string filename)
        {
            File.WriteAllText(filename, "");
            for (int i = 0; i < matrix.Count; i++)
            {
                string s = "";
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    s += matrix[i][j] + " ";
                }
                File.AppendAllLines(filename, new[] { s + "\n" });
            }
        }


        public static void PrintMatrix(List<List<int>> matrix, string filename)
        {
            File.WriteAllText(filename, "");
            for (int i = 0; i < matrix.Count; i++)
            {
                string s = "";
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    s += matrix[i][j] + " ";
                }
                File.AppendAllLines(filename, new[] { s + "\n" });
            }
        }

        public static void PrintMatrix(List<List<ProbabilityDistribution>> matrix, string filename)
        {
            File.WriteAllText(filename, "");
            for (int i = 0; i < matrix.Count; i++)
            {
                string s = "";
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    s += "(" + matrix[i][j].FirstClass + " , " + matrix[i][j].SecondClass + ") ";
                }
                File.AppendAllLines(filename, new[] { s + "\n" });
            }
        }

        public static void PrintMatrix(List<List<Color>> matrix, string filename)
        {
            File.WriteAllText(filename, "");
            for (int i = 0; i < matrix.Count; i++)
            {
                string s = "";
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    s += "(" + matrix[i][j].R  + " , " + matrix[i][j].G + " , " + matrix[i][j].B + ") ";
                }
                File.AppendAllLines(filename, new[] { s + "\n" });
            }
        }
    }
}
