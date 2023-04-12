﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersWithBot.Enums;
using CheckersWithBot.FieldModel;
using CheckersWithBot.UserModels;

namespace CheckersWithBot
{
    public class Game
    {
        public Field Field { get; set; }
        public List<User> Users { get; set; }
        private const int _toGetNumber = 65;
        private const int _maxCountStepsWithoutBeating = 15;
        public Game(Field field, List<User> users)
        {
            Field = field;
            Users = users;
        }

        public void PrintAllUsers()
        {
            for (int i = 0; i < Users.Count; i++)
                Console.WriteLine($"Index - {i + 1}. Name - {Users[i].Name}.");
        }
        public bool DoesNameAlreadyExist(string name)
        {
            for (int i = 0; i < Users.Count; i++)
                if (Users[i].Name == name) return true;
            return false;
        }
        private void FillDataForPlayers()
        {
            Users[0].Color = ConsoleColor.White;
            Users[0].TypeDef = CellType.CheckerF;
            Users[0].TypeQ = CellType.QueenF;
            Users[1].Color = ConsoleColor.Black;
            Users[1].TypeDef = CellType.CheckerS;
            Users[1].TypeQ = CellType.QueenS;
        }
        private void InputOfCords(out int cordX, out int cordY)
        {
            do
            {
                Console.Write("Cord X (number): ");
                int.TryParse(Console.ReadLine(), out cordX);
                Console.Write("Cord Y (letter): ");
                char.TryParse(Console.ReadLine().ToUpper(), out char temp);
                cordY = (temp - _toGetNumber);
                cordX--;
            } while (cordX < 0 || cordX >= Field.Map.GetLength(0) ||
                     cordY < 0 || cordY >= Field.Map.GetLength(1));
        }
        public void Start()
        {
            Console.Clear();
            MenuInGame menuInGame = new MenuInGame();
            ExtraMenu extraMenu = new ExtraMenu();
            int cordX = 0;
            int cordY = 0;
            bool isEnd = false;
            bool step = true;
            Point checkerWhichPlayerUsed = new Point(-1, -1);
            int countOfStepsWithoutBeating = 0;
            FillDataForPlayers();

            Field.PrintField(Users[0], false, false);
            do
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    Console.WriteLine($"{i + 1} Player");
                    if (Users[i].GetType() == typeof(Player))
                    {
                        Console.WriteLine($"{Users[i].Name} your turn.");
                        do
                        {
                            step = true;
                            Console.WriteLine("----MENU IN GAME----\n" +
                                "1 - Do Step.\n" +
                                "2 - Surrender.\n" +
                                "3 - Offer Draw.");
                            Console.Write("Enter your choice: ");
                            Enum.TryParse(Console.ReadLine(), out menuInGame);

                            switch (menuInGame)
                            {
                                case MenuInGame.DoStep:
                                    {
                                        Console.WriteLine("Choose your checker: ");
                                        if (Field.DoesCheckerOnFieldCanBeat(Users[i])) // якщо гравецб може побити шашку
                                        {
                                            if (!Users[i].DoesBeatSmbBefore)
                                                Users[i].UserAbleToBit = Field.CollectDictionary(Users[i]);
                                            else
                                                Users[i].UserAbleToBit = Field.CollectDictionaryForOneChecker(Users[i], checkerWhichPlayerUsed);
                                            Users[i].DoesBeatSmbBefore = true;
                                            countOfStepsWithoutBeating = 0;

                                            do
                                            {
                                                Console.WriteLine("U can bit checker: ");
                                                InputOfCords(out cordX, out cordY);
                                            } while ((Field.Map[cordX, cordY].Type != Users[i].TypeDef &&
                                                     Field.Map[cordX, cordY].Type != Users[i].TypeQ) ||
                                                     !Field.DoesPointExistInDict(Users[i], new Point(cordX, cordY)));

                                            // Fill user empty cells
                                            Users[i].CordsOfEmptyCells =
                                                Field.GetEmptyCells(Users[i], new Point(cordX, cordY));

                                            if (Users[i].CordsOfEmptyCells.Count > 0)
                                            {
                                                Console.Clear();
                                                Field.PrintField(Users[i], false, false);

                                                bool didUserDoStep = false;
                                                do
                                                {
                                                    Console.WriteLine("----EXTRA MENU----\n" +
                                                    "1 - Enter cord.\n" +
                                                    "2 - Choose another checker.");
                                                    Console.Write("Enter your choice: ");
                                                    Enum.TryParse(Console.ReadLine(), out extraMenu);
                                                    switch (extraMenu)
                                                    {
                                                        case ExtraMenu.EnterCord:
                                                            {
                                                                Point enemyChecker = new Point(-1, -1);
                                                                if (Users[i].CordsOfEmptyCells.Count > 1)
                                                                {
                                                                    int tempX = 0;
                                                                    int tempY = 0;
                                                                    do
                                                                    {
                                                                        Console.WriteLine("Choose empty(red) cell: ");
                                                                        InputOfCords(out tempX, out tempY);
                                                                    } while (!Field.DoesEmptyCellExistDict(Users[i], new Point(cordX, cordY), new Point(tempX, tempY)));

                                                                    enemyChecker = 
                                                                        Field.GetEnemyPoint(new Point(cordX, cordY),
                                                                            new Point(tempX, tempY));
                                                                    Field.MoveCheck(new Point(cordX, cordY), new Point(tempX, tempY));
                                                                    checkerWhichPlayerUsed = new Point(tempX, tempY);
                                                                }
                                                                else
                                                                {
                                                                    enemyChecker =
                                                                        Field.GetEnemyPoint(new Point(cordX, cordY),
                                                                            Users[i].CordsOfEmptyCells[0]);
                                                                    Field.MoveCheck(new Point(cordX, cordY), Users[i].CordsOfEmptyCells[0]);
                                                                    checkerWhichPlayerUsed = new Point(Users[i].CordsOfEmptyCells[0].CordX, Users[i].CordsOfEmptyCells[0].CordY);
                                                                }
                                                                Field.Map[enemyChecker.CordX, enemyChecker.CordY].Type =
                                                                    CellType.Empty;
                                                                didUserDoStep = true;
                                                            }
                                                            break;
                                                        case ExtraMenu.ChooseAnotherChecker:
                                                            {
                                                                Users[i].CordsOfEmptyCells.Clear();
                                                                step = false;
                                                            }
                                                            break;
                                                        default:
                                                            Console.WriteLine("Error choice...");
                                                            break;
                                                    }
                                                } while (extraMenu != ExtraMenu.ChooseAnotherChecker && !didUserDoStep);
                                            }
                                        }

                                        else 
                                        {
                                            Users[i].DoesBeatSmbBefore = false;
                                            do
                                            {
                                                InputOfCords(out cordX, out cordY);
                                            } while (Field.Map[cordX, cordY].Type != Users[i].TypeDef &&
                                                     Field.Map[cordX, cordY].Type != Users[i].TypeQ);

                                            Field.CollectAllPossibleStepsToMoveCheck(new Point(cordX, cordY), Users[i]); // збір всіх можливих ходів для шашки яку вибрав юзер
                                            Console.Clear();
                                            Field.PrintField(Users[i], false, false);
                                            
                                            bool didUserDoStep = false;

                                            do
                                            {
                                                Console.WriteLine("----EXTRA MENU----\n" +
                                                "1 - Enter cord.\n" +
                                                "2 - Choose another checker.");
                                                Console.Write("Enter your choice: ");
                                                Enum.TryParse(Console.ReadLine(), out extraMenu);
                                                switch (extraMenu)
                                                {
                                                    case ExtraMenu.EnterCord:
                                                        {
                                                            if (Users[i].CordsOfEmptyCells.Count > 1)
                                                            {
                                                                int tempCordX = 0;
                                                                int tempCordY = 0;
                                                                int countOfTries = 0;
                                                                Console.WriteLine("Choose cell: ");
                                                                do
                                                                {
                                                                    InputOfCords(out tempCordX, out tempCordY);
                                                                } while (!Field.DoesCordExistInUserListOfEmptyCells(new Point(tempCordX, tempCordY), Users[i]));

                                                                Field.MoveCheck(new Point(cordX, cordY), new Point(tempCordX, tempCordY));
                                                                Users[i].CordsOfEmptyCells.Clear();

                                                            }
                                                            else
                                                            {
                                                                Field.MoveCheck(new Point(cordX, cordY), Users[i].CordsOfEmptyCells[0]);
                                                                Users[i].CordsOfEmptyCells.Clear();
                                                            }

                                                            didUserDoStep = true;
                                                        }
                                                        break;
                                                    case ExtraMenu.ChooseAnotherChecker:
                                                        {
                                                            Users[i].CordsOfEmptyCells.Clear();
                                                            step = false;
                                                        }
                                                        break;
                                                    default:
                                                        Console.WriteLine("Error choice...");
                                                        break;
                                                }
                                            } while (extraMenu != ExtraMenu.ChooseAnotherChecker && !didUserDoStep);
                                        } //якщо гравець не може побити шашку
                                    }
                                    break;
                                case MenuInGame.Surrender:
                                    {
                                        Users.RemoveAt(i);
                                        isEnd = true;
                                    }
                                    break;
                                case MenuInGame.OfferDraw:
                                    {
                                        if (OfferDraw(i))
                                        {
                                            Console.WriteLine("Player accepted");
                                            isEnd = true;
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Error choice.");
                                    step = false;
                                    break;
                            }
                        } while (!step);
                    } // PLAYER LOGIC

                    else
                    {
                        checkerWhichPlayerUsed = ((Bot)Users[i]).BotStep(this, i, checkerWhichPlayerUsed);
                        if (checkerWhichPlayerUsed.CordX >= 0) countOfStepsWithoutBeating = 0;
                    } //BOT LOGIC

                    //Console.ReadLine();
                    //Console.Clear();

                    if (Users.Count == 2)
                    {
                        Field.PrintField(Users[i], true, Users[i].DoesBeatSmbBefore);
                        Users[i].CordsOfEmptyCells = new List<Point>();

                        IfGotQueen(i); // does player got queen
                        if (Field.CountOfCheckersOnBoard(Users[0]) == 0 && Users.Count == 2)
                        {
                            Users.RemoveAt(0);
                            isEnd = true;
                            break;
                        } // if lost first Player
                        if (Field.CountOfCheckersOnBoard(Users[1]) == 0 && Users.Count == 2)
                        {
                            Users.RemoveAt(1);
                            isEnd = true;
                            break;
                        } // if lost second Player

                        
                        if (checkerWhichPlayerUsed.CordX >= 0 && checkerWhichPlayerUsed.CordY >= 0 &&
                            Field.DoesCurrentCheckerCanBitAnyCheck(Field.Map[checkerWhichPlayerUsed.CordX, checkerWhichPlayerUsed.CordY]) &&
                            Users[i].DoesBeatSmbBefore) i--;
                        else Users[i].DoesBeatSmbBefore = false;
                    }
                    else isEnd = true;
                }

                countOfStepsWithoutBeating++;
                if (countOfStepsWithoutBeating == _maxCountStepsWithoutBeating) isEnd = true;
            } while (!isEnd);

            Field.PrintField(Users[0], false, false);
            Console.ReadLine();

            if (Users.Count == 2)
                Console.WriteLine($"DRAW!");
            else 
                Console.WriteLine($"{Users[0].Name} - WON! HIS COLOR WAS - {Users[0].Color}.");
        }

        public bool OfferDraw(int index)
        {
            int tempIndex = 0;

            if (index == 0) tempIndex = 1;
            else tempIndex = 0;

            Console.WriteLine($"{Users[index].Name} offers draw: ");
            if (Users[tempIndex].GetType() == typeof(Player))
            {
                
                char choice = ' ';
                do
                {
                    Console.Write("Would u like to accept draw[y/n]: ");
                    char.TryParse(Console.ReadLine().ToLower(), out choice);
                } while (choice != 'y' && choice != 'n');

                return choice == 'y';
            }
            else // write logic when bot need to accept draw
            {
                (int, int) values = GetValueOfCheckers();
                if (tempIndex == 0 && values.Item1 <= values.Item2 && CountOfCheckers() <= 4)
                    return true;
                if (tempIndex == 1 && values.Item1 >= values.Item2 && CountOfCheckers() <= 4)
                    return true;
            }
            return false;
        }

        public (int, int) GetValueOfCheckers()
        {
            int valueOfWhiteCheckers = 0;
            int valueOfOfBlackCheckers = 0;
            for (int i = 0; i < Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < Field.Map.GetLength(1); j++)
                {
                    if (Field.Map[i, j].Type == CellType.CheckerF) valueOfWhiteCheckers++;
                    else if(Field.Map[i, j].Type == CellType.QueenF) valueOfWhiteCheckers+=3;
                    else if (Field.Map[i, j].Type == CellType.CheckerS) valueOfOfBlackCheckers++;
                    else if (Field.Map[i, j].Type == CellType.QueenS) valueOfOfBlackCheckers += 3;
                }
            }

            return (valueOfWhiteCheckers, valueOfOfBlackCheckers);
        }

        public int CountOfCheckers()
        {
            int count = 0;
            for (int i = 0; i < Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < Field.Map.GetLength(1); j++)
                {
                    if (Field.Map[i, j].Type != CellType.Empty) count++;
                }
            }

            return count;
        }
        public void IfGotQueen(int indexOfPlayer)
        {
            if (indexOfPlayer == 0)
            {
                for (int i = 0; i < Field.Map.GetLength(1); i++)
                {
                    if (Field.Map[Field.Map.GetLength(0) - 1, i].Type == CellType.CheckerF)
                        Field.Map[Field.Map.GetLength(0) - 1, i].Type = CellType.QueenF;
                }
            }
            else
            {
                for (int i = 0; i < Field.Map.GetLength(1); i++)
                {
                    if (Field.Map[0, i].Type == CellType.CheckerS)
                        Field.Map[0, i].Type = CellType.QueenS;
                }
            }
        }
    }
}
