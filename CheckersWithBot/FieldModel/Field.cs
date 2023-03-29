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
                        if (j % 2 != 0) Map[i, j] = new Cell(CellType.CheckerF, new Point(i, j));

                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else if (i == 1)
                    {
                        if (j % 2 == 0) Map[i, j] = new Cell(CellType.CheckerF, new Point(i, j));
                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else if (i == 5 || i == 7)
                    {
                        if (j % 2 == 0) Map[i, j] = new Cell(CellType.CheckerS, new Point(i, j));
                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else if (i == 6)
                    {
                        if (j % 2 != 0) Map[i, j] = new Cell(CellType.CheckerS, new Point(i, j));
                        else Map[i, j] = new Cell(new Point(i, j));
                    }
                    else Map[i, j] = new Cell(new Point(i, j));
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

        public void PrintField(User user)
        {
            Console.WriteLine("   A     B     C     D     E     F     G     H");
            for (int i = Map.GetLength(0) - 1; i >= 0; i--)
            {
                Console.Write(" -----------------------------------------------\n");
                Console.Write($"{i}");
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    switch (Map[i, j].Type)
                    {
                        case CellType.CheckerF:
                            {
                                Console.Write($"| ");
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write("C ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("| ");
                            }
                            break;
                        case CellType.CheckerS:
                            {
                                Console.Write($"| ");
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.Write("C ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("| ");
                            }
                            break;
                        case CellType.QueenF:
                            {
                                //Console.Write($"|C* | ");
                                Console.Write($"|");
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write("C* ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("| ");
                            }
                            break;
                        case CellType.QueenS:
                            {
                                Console.Write($"|");
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.Write("C* ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("| ");
                            }
                            break;
                        case CellType.Empty:
                            {
                                if (DoesCordExistInUserListOfEmptyCells(new Point(i, j), user))
                                {
                                    Console.Write("|");
                                    Console.BackgroundColor = user.Color;
                                    Console.Write("   ");
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.Write("| ");
                                }

                                else Console.Write("|   | ");
                            }
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.Write(" -----------------------------------------------\n");
        }

        public bool DoesCheckOnFieldCanBit(User user)
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
                else if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) && (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyQueen)) return true;
                }

                else if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) && (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyQueen)) return true;

                }
                else if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) && (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
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
                        (j + temp + 2 >= 0 && j + temp + 2 < Map.GetLength(1)))
                    {
                        if (Map[i + 2, j + temp + 2].Type == CellType.Empty &&
                            (Map[i + 1, j + temp + 1].Type == enemyChecker ||
                             Map[i + 1, j + temp + 1].Type == enemyQueen)) return true;
                    }
                    else if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                             (j - temp - 2 >= 0 && j - temp - 2 < Map.GetLength(1)))
                    {
                        if (Map[i + 2, j - temp - 2].Type == CellType.Empty &&
                            (Map[i + 1, j - temp - 1].Type == enemyChecker ||
                             Map[i + 1, j - temp - 1].Type == enemyQueen)) return true;
                    }
                }

                temp = 1;
                for (int i = cell.Point.CordX - 1; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                        (j + temp + 2 >= 0 && j + temp + 2 < Map.GetLength(1)))
                    {
                        if (Map[i - 2, j + temp + 2].Type == CellType.Empty &&
                            (Map[i - 1, j + temp + 1].Type == enemyChecker ||
                             Map[i - 1, j + temp + 1].Type == enemyQueen)) return true;
                    }
                    else if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                             (j - temp - 2 >= 0 && j - temp - 2 < Map.GetLength(1)))
                    {
                        if (Map[i - 2, j - temp - 2].Type == CellType.Empty &&
                            (Map[i - 1, j - temp - 1].Type == enemyChecker ||
                             Map[i - 1, j - temp - 1].Type == enemyQueen)) return true;
                    }
                }
            }
            return false;
        }

        public void CollectAllPossibleStepsToMoveCheck(Point point, User user)
        {

        }
    }
}
