using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot
{
    public class Point
    {
        public int CordX { get; set; }
        public int CordY { get; set; }

        public Point(int cordX, int cordY)
        {
            CordX = cordX;
            CordY = cordY;
        }


        public override int GetHashCode()
        {
            return CordX.GetHashCode() ^ CordY.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Point other = (Point)obj;
            return CordX == other.CordX && CordY == other.CordY;
        }
    }
}
