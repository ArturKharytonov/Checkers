using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot.UserModels
{
    public abstract class User
    {
        public string Name { get; set; }
        public CellType TypeDef { get; set; }
        public CellType TypeQ { get; set; }
        public ConsoleColor Color { get; set; }
        public List<Point> CordsOfEmptyCells { get; set; }
        public Dictionary<Point, List<Point>> UserAbleToBit { get; set; }
        public bool DoesBeatSmbBefore { get; set; }
        public int IndexOfPlayer { get; set; }

        public User(string name)
        {
            Name = name;
            CordsOfEmptyCells = new List<Point>();
            UserAbleToBit = new Dictionary<Point, List<Point>>();
            DoesBeatSmbBefore = false;
        }
    }
}
