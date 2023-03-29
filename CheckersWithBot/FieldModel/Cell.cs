using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot.FieldModel
{
    public class Cell
    {
        public CellType Type { get; set; }
        public Point Point { get; set; }

        public Cell(CellType cellType, Point point)
        {
            Type = cellType;
            Point = point;
        }
        public Cell(Point point)
        {
            Type = CellType.Empty;
            Point = point;
        }
    }
}
