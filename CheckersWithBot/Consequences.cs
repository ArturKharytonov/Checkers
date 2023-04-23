using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot
{
    public class Consequences
    {
        public Point Point { get; set; }
        public List<Route> Routes { get; set; }

        public Consequences(Point point, List<Route> routes)
        {
            Point = point;
            Routes = routes;
        }
    }
}
