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

        //public Field()
        //{
        //    Map = new Cell[8, 8];
        //    for (int i = 0; i < Map.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < Map.GetLength(1); j++)
        //        {
        //            Map[i, j] = new Cell(new Point(i, j));
        //        }
        //    }

        //    Map[0, 4] = new Cell(CellType.CheckerF, new Point(0, 4));
        //    Map[1, 1] = new Cell(CellType.CheckerF, new Point(1, 1));
        //    Map[1, 5] = new Cell(CellType.CheckerF, new Point(1, 5));
        //    Map[2, 6] = new Cell(CellType.CheckerF, new Point(2, 6));
        //    Map[3, 1] = new Cell(CellType.CheckerF, new Point(3, 1));
        //    Map[4, 2] = new Cell(CellType.CheckerF, new Point(4, 2));

        //    Map[5, 1] = new Cell(CellType.CheckerS, new Point(5, 1));
        //    Map[6, 0] = new Cell(CellType.CheckerS, new Point(6, 0));
        //    Map[6, 4] = new Cell(CellType.CheckerS, new Point(6, 4));
        //    Map[6, 6] = new Cell(CellType.CheckerS, new Point(6, 6));
        //    Map[7, 7] = new Cell(CellType.CheckerS, new Point(7, 7));
        //}

        public Field(Field field)
        {
            Map = new Cell[8, 8];
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    Map[i, j] = new Cell(field.Map[i, j].Type, new Point(i, j));
                }
            }
        }

        public int CountOfCheckersOnBoard(User user)
        {
            int count = 0;
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j].Type == user.TypeDef ||
                        Map[i, j].Type == user.TypeQ) count++;
                }
            }
            return count;
        }
        // FIELD
        public void PrintRaw(int i, User user, bool afterStep, bool beating)
        { 
            Console.Write($"  \u2502");
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (i % 2 == 0 && j % 2 != 0) Console.BackgroundColor = ConsoleColor.White;
                else if (i % 2 != 0 && j % 2 == 0) Console.BackgroundColor = ConsoleColor.White;

                if (DoesCordExistInUserListOfEmptyCells(new Point(i, j), user))
                    Console.BackgroundColor = ConsoleColor.Green;
                
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
                                if (afterStep && beating)
                                {
                                    Console.BackgroundColor = ConsoleColor.Blue;
                                    Console.Write("       ");
                                    Console.BackgroundColor = ConsoleColor.Black;
                                }
                                else if (afterStep)
                                {
                                    Console.BackgroundColor = ConsoleColor.Green;
                                    Console.Write("       ");
                                    Console.BackgroundColor = ConsoleColor.Black;
                                }
                                else
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    Console.Write("       ");
                                    Console.BackgroundColor = ConsoleColor.Black;
                                }
                                
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
        public void PrintField(User user, bool afterStep, bool beating)
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
                PrintRaw(i, user, afterStep, beating);
                Console.Write($"{i+1} \u2502");
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (i % 2 == 0 && j % 2 != 0) Console.BackgroundColor = ConsoleColor.White;
                    else if(i % 2 != 0 && j % 2 == 0) Console.BackgroundColor = ConsoleColor.White;

                    if (DoesCordExistInUserListOfEmptyCells(new Point(i, j), user))
                        Console.BackgroundColor = ConsoleColor.Green;
                    
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
                                Console.Write("   @   ");
                            }
                            break;
                        case CellType.QueenS:
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("   @   ");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        case CellType.Empty:
                            {
                                if (DoesCordExistInUserListOfEmptyCells(new Point(i, j), user))
                                {
                                    if (afterStep && beating)
                                    {
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        Console.Write("       ");
                                        Console.BackgroundColor = ConsoleColor.Black;
                                    }
                                    else if (afterStep)
                                    {
                                        Console.BackgroundColor = ConsoleColor.Green;
                                        Console.Write("       ");
                                        Console.BackgroundColor = ConsoleColor.Black;
                                    }
                                    else
                                    {
                                        Console.BackgroundColor = ConsoleColor.DarkRed;
                                        Console.Write("       ");
                                        Console.BackgroundColor = ConsoleColor.Black;
                                    }
                                }

                                else Console.Write("       ");
                            }
                            break;
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.Write($"\u2502");
                Console.WriteLine();
                PrintRaw(i, user,afterStep, beating);
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
        public bool DoesCheckerOnFieldCanBeat(User user, List<Point> underAttack)
        {
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j].Type == user.TypeDef || Map[i, j].Type == user.TypeQ)
                        if (DoesCurrentCheckerCanBitAnyCheck(Map[i, j], underAttack)) return true;
                }
            }
            return false;
        }
        public bool DoesCurrentCheckerCanBitAnyCheck(Cell cell, List<Point> underAttack)
        {
            CellType enemyChecker;
            CellType enemyQueen;
            CellType currentPlayerChecker;
            CellType currentPlayerQueen;
            if (cell.Type == CellType.CheckerF || cell.Type == CellType.QueenF)
            {
                enemyChecker = CellType.CheckerS;
                enemyQueen = CellType.QueenS;
                currentPlayerChecker = CellType.CheckerF;
                currentPlayerQueen = CellType.QueenF;
            }
            else
            {
                enemyChecker = CellType.CheckerF;
                enemyQueen = CellType.QueenF;
                currentPlayerChecker = CellType.CheckerS;
                currentPlayerQueen = CellType.QueenS;
            }

            if (cell.Type == CellType.CheckerF || cell.Type == CellType.CheckerS) // перевірка чи проста шашка може побити
            {
                if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) &&
                    (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY + 1].Type == enemyQueen) &&
                        !underAttack.Contains(new Point(cell.Point.CordX + 1, cell.Point.CordY + 1))) return true;
                }
                if ((cell.Point.CordX + 2 >= 0 && cell.Point.CordX + 2 < Map.GetLength(0)) &&
                         (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX + 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX + 1, cell.Point.CordY - 1].Type == enemyQueen) &&
                        !underAttack.Contains(new Point(cell.Point.CordX + 1, cell.Point.CordY - 1))) return true;
                }

                if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) &&
                         (cell.Point.CordY + 2 >= 0 && cell.Point.CordY + 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY + 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY + 1].Type == enemyQueen) &&
                        !underAttack.Contains(new Point(cell.Point.CordX - 1, cell.Point.CordY + 1))) return true;

                }
                if ((cell.Point.CordX - 2 >= 0 && cell.Point.CordX - 2 < Map.GetLength(0)) &&
                         (cell.Point.CordY - 2 >= 0 && cell.Point.CordY - 2 < Map.GetLength(1)))
                {
                    if (Map[cell.Point.CordX - 2, cell.Point.CordY - 2].Type == CellType.Empty &&
                        (Map[cell.Point.CordX - 1, cell.Point.CordY - 1].Type == enemyChecker ||
                         Map[cell.Point.CordX - 1, cell.Point.CordY - 1].Type == enemyQueen) &&
                        !underAttack.Contains(new Point(cell.Point.CordX - 1, cell.Point.CordY - 1))) return true;
                }
            }

            else // перевірка чи дамка може побити
            {
                int temp = 1;
                int j = cell.Point.CordY;
                bool blockMoveRight = false;
                bool blockMoveLeft = false;

                for (int i = cell.Point.CordX; i < Map.GetLength(0); i++, temp++) // перевірка вниз //bug with cordX +- 1 mb i will replace it for time everywhere
                {
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) && !blockMoveRight)
                    {
                        if (Map[i + 1, j + temp].Type == currentPlayerChecker ||
                            Map[i + 1, j + temp].Type == currentPlayerQueen) blockMoveRight = true;

                        else if ((Map[i + 1, j + temp].Type == enemyChecker ||
                                  Map[i + 1, j + temp].Type == enemyQueen) && 
                                  !underAttack.Contains(new Point(i + 1, j + temp)))
                        {
                            if (Map[i + 2, j + temp + 1].Type == CellType.Empty) return true;

                            else blockMoveRight = true;
                        }
                    }
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) && !blockMoveLeft)
                    {
                        if (Map[i + 1, j - temp].Type == currentPlayerChecker ||
                            Map[i + 1, j - temp].Type == currentPlayerQueen) blockMoveLeft = true;

                        else if ((Map[i + 1, j - temp].Type == enemyChecker ||
                                  Map[i + 1, j - temp].Type == enemyQueen) &&
                                 !underAttack.Contains(new Point(i + 1, j - temp)))
                        {
                            if (Map[i + 2, j - temp - 1].Type == CellType.Empty)
                                return true;
                            else blockMoveLeft = true;
                        }
                    }
                }

                blockMoveLeft = false;
                blockMoveRight = false;
                temp = 1;
                j = cell.Point.CordY;
                for (int i = cell.Point.CordX; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) && !blockMoveRight)
                    {

                        if (Map[i - 1, j + temp].Type == currentPlayerChecker ||
                            Map[i - 1, j + temp].Type == currentPlayerQueen) blockMoveRight = true;

                        else if ((Map[i - 1, j + temp].Type == enemyChecker ||
                                  Map[i - 1, j + temp].Type == enemyQueen) &&
                                 !underAttack.Contains(new Point(i - 1, j + temp)))
                        {
                            if (Map[i - 2, j + temp + 1].Type == CellType.Empty)
                                return true;
                            else
                                blockMoveRight = true;

                        }
                    }
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) && !blockMoveLeft)
                    {
                        if (Map[i - 1, j - temp].Type == currentPlayerChecker ||
                            Map[i - 1, j - temp].Type == currentPlayerQueen) blockMoveLeft = true;

                        else if ((Map[i - 1, j - temp].Type == enemyChecker ||
                                  Map[i - 1, j - temp].Type == enemyQueen) &&
                                  !underAttack.Contains(new Point(i - 1, j - temp)))
                        {
                            if (Map[i - 2, j - temp - 1].Type == CellType.Empty)
                                return true;
                            else blockMoveLeft = true;
                        }
                    }
                }
            }
            return false;
        } // checking if checker can bit

        public bool DoesCheckerOnFieldCanBeat(User user)
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
            CellType currentPlayerChecker;
            CellType currentPlayerQueen;
            if (cell.Type == CellType.CheckerF || cell.Type == CellType.QueenF)
            {
                enemyChecker = CellType.CheckerS;
                enemyQueen = CellType.QueenS;
                currentPlayerChecker = CellType.CheckerF;
                currentPlayerQueen = CellType.QueenF;
            }
            else
            {
                enemyChecker = CellType.CheckerF;
                enemyQueen = CellType.QueenF;
                currentPlayerChecker = CellType.CheckerS;
                currentPlayerQueen = CellType.QueenS;
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
                bool blockMoveRight = false;
                bool blockMoveLeft = false;

                for (int i = cell.Point.CordX; i < Map.GetLength(0); i++, temp++) // перевірка вниз //bug with cordX +- 1 mb i will replace it for time everywhere
                {
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) && !blockMoveRight)
                    {
                        if (Map[i + 1, j + temp].Type == currentPlayerChecker ||
                            Map[i + 1, j + temp].Type == currentPlayerQueen) blockMoveRight = true;

                        else if (Map[i + 1, j + temp].Type == enemyChecker ||
                                  Map[i + 1, j + temp].Type == enemyQueen)
                        {
                            if (Map[i + 2, j + temp + 1].Type == CellType.Empty) return true;

                            else blockMoveRight = true;
                        }
                    }
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) && !blockMoveLeft)
                    {
                        if (Map[i + 1, j - temp].Type == currentPlayerChecker ||
                            Map[i + 1, j - temp].Type == currentPlayerQueen) blockMoveLeft = true;

                        else if (Map[i + 1, j - temp].Type == enemyChecker ||
                                  Map[i + 1, j - temp].Type == enemyQueen)
                        {
                            if (Map[i + 2, j - temp - 1].Type == CellType.Empty)
                                return true;
                            else blockMoveLeft = true;
                        }
                    }
                }

                blockMoveLeft = false;
                blockMoveRight = false;
                temp = 1; 
                j = cell.Point.CordY;
                for (int i = cell.Point.CordX; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) && !blockMoveRight)
                    {

                        if (Map[i - 1, j + temp].Type == currentPlayerChecker ||
                            Map[i - 1, j + temp].Type == currentPlayerQueen) blockMoveRight = true;

                        else if (Map[i - 1, j + temp].Type == enemyChecker ||
                                  Map[i - 1, j + temp].Type == enemyQueen)
                        {
                            if(Map[i - 2, j + temp + 1].Type == CellType.Empty)
                                return true;
                            else
                                 blockMoveRight = true;
                            
                        }
                    }
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) && !blockMoveLeft)
                    {
                        if (Map[i - 1, j - temp].Type == currentPlayerChecker ||
                            Map[i - 1, j - temp].Type == currentPlayerQueen) blockMoveLeft = true;
                        
                        else if (Map[i - 1, j - temp].Type == enemyChecker ||
                                  Map[i - 1, j - temp].Type == enemyQueen)
                        {
                            if (Map[i - 2, j - temp - 1].Type == CellType.Empty)
                                return true;
                            else blockMoveLeft = true;
                        }
                    }
                }
            }
            return false;
        } // checking if checker can bit
        public List<Point> CollectEmptyCells(Cell cell)
        {
            List<Point> emptyCells = new List<Point>();
            CellType enemyChecker;
            CellType enemyQueen;
            CellType currentPlayerChecker;
            CellType currentPlayerQueen;
            if (cell.Type == CellType.CheckerF || cell.Type == CellType.QueenF)
            {
                enemyChecker = CellType.CheckerS;
                enemyQueen = CellType.QueenS;
                currentPlayerChecker = CellType.CheckerF;
                currentPlayerQueen = CellType.QueenF;
            }
            else
            {
                enemyChecker = CellType.CheckerF;
                enemyQueen = CellType.QueenF;
                currentPlayerChecker = CellType.CheckerS;
                currentPlayerQueen = CellType.QueenS;
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
                bool blockMoveRight = false;
                bool blockMoveLeft = false;
                for (int i = cell.Point.CordX; i < Map.GetLength(0); i++, temp++) // перевірка вниз
                {
                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) && !blockMoveRight)
                    {
                        if (Map[i + 1, j + temp].Type == currentPlayerChecker ||
                            Map[i + 1, j + temp].Type == currentPlayerQueen) blockMoveRight = true;

                        else if (Map[i + 1, j + temp].Type == enemyChecker ||
                             Map[i + 1, j + temp].Type == enemyQueen)
                        {
                            if (Map[i + 2, j + temp + 1].Type == CellType.Empty)
                            {
                                int valueY = j + temp + 1;
                                for (int valueX = i + 2;
                                     valueX < Map.GetLength(0) && valueY < Map.GetLength(1);
                                     valueX++, valueY++)
                                {
                                    if (Map[valueX, valueY].Type == CellType.Empty)
                                        emptyCells.Add(new Point(valueX, valueY));
                                    else break;
                                }
                            }
                            else blockMoveRight = true;
                        }
                    }

                    if ((i + 2 >= 0 && i + 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) && !blockMoveLeft)
                    {
                        if (Map[i + 1, j - temp].Type == currentPlayerChecker ||
                            Map[i + 1, j - temp].Type == currentPlayerQueen) blockMoveLeft = true;

                        else if (Map[i + 1, j - temp].Type == enemyChecker ||
                             Map[i + 1, j - temp].Type == enemyQueen)
                        {
                            if (Map[i + 2, j - temp - 1].Type == CellType.Empty)
                            {
                                int valueY = j - temp - 1;
                                for (int valueX = i + 2; valueX < Map.GetLength(0) && valueY >= 0; valueX++, valueY--)
                                {
                                    if (Map[valueX, valueY].Type == CellType.Empty)
                                        emptyCells.Add(new Point(valueX, valueY));
                                    else break;
                                }
                            }
                            else blockMoveLeft = true;
                        }
                    }
                }

                blockMoveRight = false;
                blockMoveLeft = false;
                temp = 1;
                j = cell.Point.CordY;
                for (int i = cell.Point.CordX; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                        (j + temp + 1 >= 0 && j + temp + 1 < Map.GetLength(1)) && !blockMoveRight)
                    {
                        if (Map[i - 1, j + temp].Type == currentPlayerChecker ||
                            Map[i - 1, j + temp].Type == currentPlayerQueen) blockMoveRight = true;

                        if (Map[i - 1, j + temp].Type == enemyChecker ||
                             Map[i - 1, j + temp].Type == enemyQueen)
                        {
                            if (Map[i - 2, j + temp + 1].Type == CellType.Empty)
                            {
                                int valueY = j + temp + 1;
                                for (int valueX = i - 2; valueX >= 0 && valueY < Map.GetLength(1); valueX--, valueY++)
                                {
                                    if (Map[valueX, valueY].Type == CellType.Empty)
                                        emptyCells.Add(new Point(valueX, valueY));
                                    else break;
                                }
                            }
                            else blockMoveRight = true;
                        }
                    }
                    if ((i - 2 >= 0 && i - 2 < Map.GetLength(0)) &&
                             (j - temp - 1 >= 0 && j - temp - 1 < Map.GetLength(1)) && !blockMoveLeft)
                    {
                        if (Map[i - 1, j - temp].Type == currentPlayerChecker ||
                            Map[i - 1, j - temp].Type == currentPlayerQueen) blockMoveLeft = true;

                        else if (Map[i - 1, j - temp].Type == enemyChecker ||
                             Map[i - 1, j - temp].Type == enemyQueen)
                        {
                            if (Map[i - 2, j - temp - 1].Type == CellType.Empty)
                            {
                                int valueY = j - temp - 1;
                                for (int valueX = i - 2; valueX >= 0 && valueY >= 0; valueX--, valueY--)
                                {
                                    if (Map[valueX, valueY].Type == CellType.Empty)
                                        emptyCells.Add(new Point(valueX, valueY));
                                    else break;
                                }
                            }
                            else blockMoveLeft = true;
                        }
                    }
                }
            }
            return emptyCells;
        } // collecting if checker can bit
        public void CollectAllPossibleStepsToMoveCheck(Point points, User user)
        {
            if (Map[points.CordX, points.CordY].Type == CellType.CheckerF)
            {
                if ((points.CordX + 1 >= 0 && points.CordX + 1 < Map.GetLength(0)) &&
                    (points.CordY + 1 >= 0 && points.CordY + 1 < Map.GetLength(1)) &&
                    Map[points.CordX + 1, points.CordY + 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX + 1, points.CordY + 1));

                if ((points.CordX + 1 >= 0 && points.CordX + 1 < Map.GetLength(0)) &&
                    (points.CordY - 1 >= 0 && points.CordY - 1 < Map.GetLength(1)) &&
                    Map[points.CordX + 1, points.CordY - 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX + 1, points.CordY - 1));
            }
            else if (Map[points.CordX, points.CordY].Type == CellType.CheckerS)
            {
                if ((points.CordX - 1 >= 0 && points.CordX - 1 < Map.GetLength(0)) &&
                    (points.CordY + 1 >= 0 && points.CordY + 1 < Map.GetLength(1)) &&
                    Map[points.CordX - 1, points.CordY + 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX - 1, points.CordY + 1));

                if ((points.CordX - 1 >= 0 && points.CordX - 1 < Map.GetLength(0)) &&
                    (points.CordY - 1 >= 0 && points.CordY - 1 < Map.GetLength(1)) &&
                    Map[points.CordX - 1, points.CordY - 1].Type == CellType.Empty)
                    user.CordsOfEmptyCells.Add(new Point(points.CordX - 1, points.CordY - 1));
            }

            else
            {
                int temp = 1;
                int j = points.CordY;
                bool blockMoveRight = false;
                bool blockMoveLeft = false;
                if (Map[points.CordX, points.CordY].Type == CellType.QueenF)
                {

                }
                for (int i = points.CordX; i < Map.GetLength(0); i++, temp++) // перевірка вниз
                {
                    if ((i + 1 >= 0 && i + 1 < Map.GetLength(0)) &&
                        (j + temp >= 0 && j + temp < Map.GetLength(1)) &&
                        Map[i + 1, j + temp].Type == CellType.Empty && !blockMoveRight)
                    {
                        user.CordsOfEmptyCells.Add(new Point(i + 1, j + temp));
                    }
                    else blockMoveRight = true;

                    if ((i + 1 >= 0 && i + 1 < Map.GetLength(0)) &&
                        (j - temp >= 0 && j - temp < Map.GetLength(1)) &&
                        Map[i + 1, j - temp].Type == CellType.Empty && !blockMoveLeft)
                    {
                        user.CordsOfEmptyCells.Add(new Point(i + 1, j - temp));
                    }
                    else blockMoveLeft = true;
                }

                temp = 1;
                blockMoveRight = false;
                blockMoveLeft = false;
                j = points.CordY;

                for (int i = points.CordX; i >= 0; i--, temp++) // перевірка вверх
                {
                    if ((i - 1 >= 0 && i - 1 < Map.GetLength(0)) &&
                        (j + temp  >= 0 && j + temp < Map.GetLength(1)) &&
                        Map[i - 1, j + temp].Type == CellType.Empty && !blockMoveRight)
                    {
                        user.CordsOfEmptyCells.Add(new Point(i - 1, j + temp));
                    }
                    else blockMoveRight = true;

                    if ((i - 1 >= 0 && i - 1 < Map.GetLength(0)) &&
                        (j - temp >= 0 && j - temp < Map.GetLength(1)) &&
                        Map[i - 1, j - temp].Type == CellType.Empty && !blockMoveLeft)
                    {
                        user.CordsOfEmptyCells.Add(new Point(i - 1, j - temp));
                    }
                    else blockMoveLeft = true;
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
        public Dictionary<Point, List<Point>> CollectDictionaryForOneChecker(User user, Point point)
        {
            Dictionary<Point, List<Point>> dict = new Dictionary<Point, List<Point>>();
            List<Point> cells = CollectEmptyCells(Map[point.CordX, point.CordY]);
            if(cells.Count > 0)
                dict.Add(point, cells);
            return dict;
        }
        public bool DoesPointExistInDict(User user, Point point)
        {
            return user.UserAbleToBit.ContainsKey(point);
        }
        public List<Point> GetEmptyCells(User user, Point point)
        {
            List<Point> points = new List<Point>();
            if (user.UserAbleToBit.TryGetValue(point, out points))
            {
                if (points.Count > 0) return points;
            }
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
            if (Map[point.CordX, point.CordY].Type == CellType.CheckerF ||
                Map[point.CordX, point.CordY].Type == CellType.CheckerS)
            {
                if (point.CordX + 2 == emptyCell.CordX && point.CordY + 2 == emptyCell.CordY)
                    return new Point(point.CordX + 1, point.CordY + 1);
                if (point.CordX + 2 == emptyCell.CordX && point.CordY - 2 == emptyCell.CordY)
                    return new Point(point.CordX + 1, point.CordY - 1);
                if (point.CordX - 2 == emptyCell.CordX && point.CordY + 2 == emptyCell.CordY)
                    return new Point(point.CordX - 1, point.CordY + 1);
                if (point.CordX - 2 == emptyCell.CordX && point.CordY - 2 == emptyCell.CordY)
                    return new Point(point.CordX - 1, point.CordY - 1);
            }
            else
            {
                if (point.CordX < emptyCell.CordX) // будем рухатись вверх
                {
                    if (point.CordY < emptyCell.CordY)
                    {
                        int j = point.CordY + 1;
                        for (int i = point.CordX + 1; i < emptyCell.CordX && j < emptyCell.CordY; i++, j++)
                        {
                            if (Map[i, j].Type != CellType.Empty &&
                                Map[i, j].Type != Map[point.CordX, point.CordY].Type)
                                return new Point(i, j);
                        }
                    } // і праворуч
                    else
                    {
                        int j = point.CordY - 1;
                        for (int i = point.CordX + 1; i < emptyCell.CordX && j >= 0; i++, j--)
                        {
                            if (Map[i, j].Type != CellType.Empty &&
                                Map[i, j].Type != Map[point.CordX, point.CordY].Type)
                                return new Point(i, j);
                        }
                    } // і ліворуч
                }

                else // будем рухатись вниз
                {
                    if (point.CordY < emptyCell.CordY)
                    {
                        int j = point.CordY + 1;
                        for (int i = point.CordX - 1; i > emptyCell.CordX && j < emptyCell.CordY; i--, j++)
                        {
                            if (Map[i, j].Type != CellType.Empty &&
                                Map[i, j].Type != Map[point.CordX, point.CordY].Type)
                                return new Point(i, j);
                        }
                    } // і праворуч
                    else
                    {
                        int j = point.CordY - 1;
                        for (int i = point.CordX - 1; i > emptyCell.CordX && j >= 0; i--, j--)
                        {
                            if (Map[i, j].Type != CellType.Empty &&
                                Map[i, j].Type != Map[point.CordX, point.CordY].Type)
                                return new Point(i, j);
                        }
                    } // і ліворуч
                }
            }
            return null;
        }
    }
}
