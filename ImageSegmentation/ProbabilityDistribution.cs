using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentation
{
    class ProbabilityDistribution
    {
        public double FirstClass;
        public double SecondClass;

        public ProbabilityDistribution(double first, double second)
        {
            FirstClass = first;
            SecondClass = second;
        }
    }
}
