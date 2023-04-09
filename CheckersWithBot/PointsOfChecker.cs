using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot
{
    public class PointsOfChecker
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public PointsOfChecker(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
}
