using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentation
{
    class SubgradientUpsurge
    {
        public static List<List<int>> Initialization(int height, int weight)
        {
            var lambdaMatrix = new List<List<int>>();
            var rnd = new Random();
            for (var i = 0; i < height; i++)
            {
                lambdaMatrix.Add(new List<int>());
                for (var j = 0; j < weight; j++)
                {
                    lambdaMatrix[i].Add(rnd.Next(0,2)); 
                }
            }
            return lambdaMatrix;
        }

        public static void LambdaToL()
        {

        }
    }
}
