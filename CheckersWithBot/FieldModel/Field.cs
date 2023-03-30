using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersWithBot.UserModels;

namespace CheckersWithBot.FieldModel
{
    public class Field
    {
        public Cell[,] Map { get; set; }
        public Field()
        {
            Map = new Cell[8, 8];
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (i == 0 || i == 2)
                    {
                        if (j % 2 == 0) Map[i, j] = new Cell(CellType.CheckerF, new Point(i, j));

                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else if (i == 1)
                    {
                        if (j % 2 != 0) Map[i, j] = new Cell(CellType.CheckerF, new Point(i, j));
                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else if (i == 5 || i == 7)
                    {
                        if (j % 2 != 0) Map[i, j] = new Cell(CellType.CheckerS, new Point(i, j));
                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else if (i == 6)
                    {
                        if (j % 2 == 0) Map[i, j] = new Cell(CellType.CheckerS, new Point(i, j));
                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else Map[i, j] = new Cell(new Point(i, j));
                }
            }
        }

        // FIELD
        public void PrintRaw(int i, User user)
        { 
            Console.Write($"  \u2502");
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (i % 2 == 0 && j % 2 != 0) Console.BackgroundColor = ConsoleColor.White;
                else if (i % 2 != 0 && j % 2 == 0) Console.BackgroundColor = ConsoleColor.White;

                switch (Map[i, j].Type)
                {
                    case CellType.CheckerF:
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("       ");
                        }
                        break;
                    case CellType.CheckerS:
                        { 
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("       ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        break;
                    case CellType.QueenF:
                        {

                            Console.Write("       ");
                        }
                        break;
                    case CellType.QueenS:
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("       ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        break;
                    case CellType.Empty:
                        {
                            if (DoesCordExistInUserListOfEmptyCells(new Point(i, j), user))
                            {

                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                Console.Write("       ");
                                Console.BackgroundColor = ConsoleColor.Black;

                            }

                            else Console.Write("       ");
                        }
                        break;
                }

                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.Write($"\u2502");
            Console.WriteLine();
        }
        public void PrintField(User user)
        {
            Console.WriteLine("      A      B      C      D      E      F      G      H");
            Console.Write("  \u250c");
            for (int i = 0; i < 18; i++)
            {
                Console.Write("\u2500\u2500\u2500");
            }
            Console.Write("\u2500\u2500\u2510");
            Console.WriteLine();
            for (int i = Map.GetLength(0) - 1; i >= 0; i--)
            {
                PrintRaw(i, user);
                Console.Write($"{i+1} \u2502");
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (i % 2 == 0 && j % 2 != 0) Console.BackgroundColor = ConsoleColor.White;
                    else if(i % 2 != 0 && j % 2 == 0) Console.BackgroundColor = ConsoleColor.White;

                    switch (Map[i, j].Type)
                    {
                        case CellType.CheckerF: 
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("   \u25a0   ");
                                
                            }
                            break;
                        case CellType.CheckerS:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("   \u25a0   ");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        case CellType.QueenF:
                            {
                                Console.Write("   O   ");
                            }
                            break;
                        case CellType.QueenS:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("   O   ");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        case CellType.Empty:
                            {
                                if (DoesCordExistInUserListOfEmptyCells(new Point(i, j), user))
                                {
                                    
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    Console.Write("       ");
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    
                                }

                                else Console.Write("       ");
                            }
                            break;
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.Write($"\u2502");
                Console.WriteLine();
                PrintRaw(i, user);
            }

            Console.Write("  \u2514");
            for (int i = 0; i < 18; i++)
            {
                Console.Write("\u2500\u2500\u2500");
            }

            Console.Write("\u2500\u2500\u2518");
            Console.WriteLine();
        }
        // FIELD

        public bool DoesCheckerOnFieldCanBit(User user)
        {
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if(Map[i, j].Type == user.TypeDef || Map[i, j].Type == user.TypeQ)
                        if (DoesCurrentCheckerCanBitAnyCheck(Map[i, j])) return true;
                }
            }
            return false;
        }
        public bool DoesCurrentCheckerCanBitAnyCheck(Cell cell)
        {
            CellType enemyChecker;
            CellType enemyQueen;
            if (cell.Type == CellType.CheckerF || cell.Type == CellType.QueenF)
            {
                enemyChecker = CellType.CheckerS;
                enemyQueen = CellType.QueenS;
            }
            else
            {
                enemyChecker = CellType.CheckerF;
                enemyQueen = CellType.QueenF;
            }

            if (cell.Type == CellType.CheckerF || cell.Type == CellType.CheckerS) // перевірка чи проста шашка може побити
            {
                if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) &&
                    (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY + 1].Type == enemyQueen)) return true;
                }
                if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) &&
                         (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyQueen)) return true;
                }

                if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) &&
                         (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyQueen)) return true;

                }
                if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) &&
                         (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY - 1].Type == enemyQueen)) return true;
                }
            }

            else // перевірка чи дамка може побити
            {
                int temp = 1;
                int j = cell.Point.CordY;

                for (int i = cell.Point.CordX + 1; i < Map.GetLength(0); i++, temp++) // перевірка вниз
                {
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)))
                    {
                        if (Map[i + 2, j + temp + 1].Type == CellType.Empty &&
                            (Map[i + 1, j + temp].Type == enemyChecker ||
                             Map[i + 1, j + temp].Type == enemyQueen)) return true;
                    }
                    else if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)))
                    {
                        if (Map[i + 2, j - temp - 1].Type == CellType.Empty &&
                            (Map[i + 1, j - temp].Type == enemyChecker ||
                             Map[i + 1, j - temp].Type == enemyQueen)) return true;
                    }
                }

                temp = 1;
                for (int i = cell.Point.CordX - 1; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)))
                    {
                        if (Map[i - 2, j + temp + 1].Type == CellType.Empty &&
                            (Map[i - 1, j + temp].Type == enemyChecker ||
                             Map[i - 1, j + temp].Type == enemyQueen)) return true;
                    }
                    else if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)))
                    {
                        if (Map[i - 2, j - temp - 1].Type == CellType.Empty &&
                            (Map[i - 1, j - temp].Type == enemyChecker ||
                             Map[i - 1, j - temp].Type == enemyQueen)) return true;
                    }
                }
            }
            return false;
        }
        public List<Point> CollectEmptyCells(Cell cell)
        {
            List<Point> emptyCells = new List<Point>();
            List<Point> enemyCheckers = new List<Point>();
            CellType enemyChecker;
            CellType enemyQueen;
            if (cell.Type == CellType.CheckerF || cell.Type == CellType.QueenF)
            {
                enemyChecker = CellType.CheckerS;
                enemyQueen = CellType.QueenS;
            }
            else
            {
                enemyChecker = CellType.CheckerF;
                enemyQueen = CellType.QueenF;
            }

            if (cell.Type == CellType.CheckerF || cell.Type == CellType.CheckerS) // перевірка чи проста шашка може побити
            {
                if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) &&
                    (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY + 1].Type == enemyQueen)) emptyCells.Add(new Point(cell.Point.CordX + 2, cell.Point.CordY + 2));
                }
                if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) && (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyQueen)) emptyCells.Add(new Point(cell.Point.CordX + 2, cell.Point.CordY - 2));
                }

                if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) && (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyQueen)) emptyCells.Add(new Point(cell.Point.CordX - 2, cell.Point.CordY + 2));

                }
                if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) && (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY - 1].Type == enemyQueen)) emptyCells.Add(new Point(cell.Point.CordX - 2, cell.Point.CordY - 2));
                }
            }

            else // перевірка чи дамка може побити
            {
                int temp = 1;
                int j = cell.Point.CordY;

                for (int i = cell.Point.CordX + 1; i < Map.GetLength(0); i++, temp++) // перевірка вниз
                {
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)))
                    {
                        if (Map[i + 2, j + temp + 1].Type == CellType.Empty &&
                            (Map[i + 1, j + temp].Type == enemyChecker ||
                             Map[i + 1, j + temp].Type == enemyQueen)) emptyCells.Add(new Point(i + 2, j + temp + 1));
                    }
                    else if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)))
                    {
                        if (Map[i + 2, j - temp - 1].Type == CellType.Empty &&
                            (Map[i + 1, j - temp].Type == enemyChecker ||
                             Map[i + 1, j - temp].Type == enemyQueen)) emptyCells.Add(new Point(i + 2, j - temp - 1));
                    }
                }

                temp = 1;
                for (int i = cell.Point.CordX - 1; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)))
                    {
                        if (Map[i - 2, j + temp + 1].Type == CellType.Empty &&
                            (Map[i - 1, j + temp].Type == enemyChecker ||
                             Map[i - 1, j + temp].Type == enemyQueen)) emptyCells.Add(new Point(i - 2, j + temp + 1));
                    }
                    else if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)))
                    {
                        if (Map[i - 2, j - temp - 1].Type == CellType.Empty &&
                            (Map[i - 1, j - temp].Type == enemyChecker ||
                             Map[i - 1, j - temp].Type == enemyQueen)) emptyCells.Add(new Point(i - 2, j - temp - 1));
                    }
                }
            }
            return emptyCells;
        }

        public void CollectAllPossibleStepsToMoveCheck(Point points, User user)
        {
            if (Map[points.CordX, points.CordY].Type == CellType.CheckerF)
            {
                if ((points.CordX + 1 >= 0 && points.CordX + 1 < Map.GetLength(0)) && (points.CordY + 1 >= 0 && points.CordY + 1 < Map.GetLength(1)) && Map[points.CordX + 1, points.CordY + 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX + 1, points.CordY + 1));

                if ((points.CordX + 1 >= 0 && points.CordX + 1 < Map.GetLength(0)) && (points.CordY - 1 >= 0 && points.CordY - 1 < Map.GetLength(1)) && Map[points.CordX + 1, points.CordY - 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX + 1, points.CordY - 1));
            }
            else if (Map[points.CordX, points.CordY].Type == CellType.CheckerS)
            {
                if ((points.CordX - 1 >= 0 && points.CordX - 1 < Map.GetLength(0)) && (points.CordY + 1 >= 0 && points.CordY + 1 < Map.GetLength(1)) && Map[points.CordX - 1, points.CordY + 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX - 1, points.CordY + 1));

                if ((points.CordX - 1 >= 0 && points.CordX - 1 < Map.GetLength(0)) && (points.CordY - 1 >= 0 && points.CordY - 1 < Map.GetLength(1)) && Map[points.CordX - 1, points.CordY - 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX - 1, points.CordY - 1));
            }

            else
            {
                int temp = 1;
                int j = points.CordY;

                for (int i = points.CordX + 1; i < Map.GetLength(0); i++, temp++) // перевірка вниз
                {
                    if ((i + 1 >= 0 && i + 1 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) &&
                        Map[i + 1, j + temp + 1].Type == CellType.Empty) user.CordsOfEmptyCells.Add(new Point(i + 1, j + temp + 1));

                    else if ((i + 1 >= 0 && i + 1 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) &&
                             Map[i + 1, j - temp - 1].Type == CellType.Empty) user.CordsOfEmptyCells.Add(new Point(i + 1, j - temp - 1));

                }

                temp = 1;
                for (int i = points.CordX - 1; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 1 >= 0 && i - 1 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) &&
                        Map[i - 1, j + temp + 1].Type == CellType.Empty) user.CordsOfEmptyCells.Add(new Point(i - 1, j + temp + 1));


                    else if ((i - 1 >= 0 && i - 1 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) &&
                             Map[i - 1, j - temp - 1].Type == CellType.Empty) user.CordsOfEmptyCells.Add(new Point(i - 1, j - temp - 1));
                }
            }
        }
        public bool DoesCordExistInUserListOfEmptyCells(Point point, User user) //CordsOfEmptyCells
        {
            if (user.CordsOfEmptyCells.Count <= 0) return false;

            for (int i = 0; i < user.CordsOfEmptyCells.Count; i++)
            {
                if (user.CordsOfEmptyCells[i].CordX == point.CordX && user.CordsOfEmptyCells[i].CordY == point.CordY) return true;
            }
            return false;
        }
        public void MoveCheck(Point startPoint, Point endPoint)
        {
            CellType temp = Map[startPoint.CordX, startPoint.CordY].Type;

            Map[startPoint.CordX, startPoint.CordY].Type = CellType.Empty;

            Map[endPoint.CordX, endPoint.CordY].Type = temp;
        }// Move in default step


        //collect all checkers that could be killed and current player checkers that can bit these checkers.
        public Dictionary<Point, List<Point>> CollectDictionary(User user)
        {
            Dictionary<Point, List<Point>> dict = new Dictionary<Point,List<Point>>();

            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j].Type == user.TypeDef || Map[i, j].Type == user.TypeQ)
                    {
                        List<Point> cells = CollectEmptyCells(Map[i, j]);
                        if (cells.Count > 0)
                            dict.Add(new Point(i, j), cells);
                        
                        
                    }
                }
            }
            return dict;
        }
        public bool DoesPointExistInDict(User user, Point point)
        {
            return user.UserAbleToBit.ContainsKey(point);
        }
        public List<Point> GetEmptyCells(User user, Point point)
        {
            List<Point> points = new List<Point>();
            user.UserAbleToBit.TryGetValue(point, out points);
            if (points.Count > 0) return points;


            return new List<Point>();
        }
        public bool DoesEmptyCellExistDict(User user, Point point, Point emptyCell)
        {
            List<Point> points = new List<Point>();
            user.UserAbleToBit.TryGetValue(point, out points);
            if (points.Count > 0)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].CordX == emptyCell.CordX && points[i].CordY == emptyCell.CordY) return true;
                }
            }

            return false;
        }
        public Point GetEnemyPoint(Point point, Point emptyCell)
        {
            if (point.CordX + 2 == emptyCell.CordX && point.CordY + 2 == emptyCell.CordY)
                return new Point(point.CordX + 1, point.CordY + 1);
            else if(point.CordX + 2 == emptyCell.CordX && point.CordY - 2 == emptyCell.CordY) 
                return new Point(point.CordX + 1, point.CordY - 1);
            else if(point.CordX - 2 == emptyCell.CordX && point.CordY + 2 == emptyCell.CordY)
                return new Point(point.CordX - 1, point.CordY + 1);
            else if(point.CordX - 2 == emptyCell.CordX && point.CordY - 2 == emptyCell.CordY) 
                return new Point(point.CordX - 1, point.CordY - 1);

            return null;
        }
    }
}
