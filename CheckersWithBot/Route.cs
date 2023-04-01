using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot
{
    public class Route
    {
        public List<Point> Path { get; set; }
        public int Count { get; set; }

        public Route(Route route)
        {
            Path = new List<Point>();
            for (int i = 0; i < route.Path.Count; i++)
            {

                Point point = new Point(route.Path[i].CordX, route.Path[i].CordY);
                Path.Add(point);
            }
        }
        public Route(List<Point> path)
        {
            Path = path;
            Count = Path.Count;
        }
        public Route()
        {
            Path = new List<Point>();
        }
    }
}
