using System;
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
        private int _toGetNumber = 65;
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

            FillDataForPlayers();

            do
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    Field.PrintField(Users[i]);
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
                                        if (Field.DoesCheckerOnFieldCanBit(Users[i])) // якщо гравецб може побити шашку
                                        {
                                            
                                            Users[i].UserAbleToBit = Field.CollectDictionary(Users[i]);
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
                                                Field.PrintField(Users[i]);

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
                                                                int tempX = 0;
                                                                int tempY = 0;
                                                                do
                                                                {
                                                                    Console.WriteLine("Choose empty(red) cell: ");
                                                                    InputOfCords(out tempX, out tempY);
                                                                } while (!Field.DoesEmptyCellExistDict(Users[i], new Point(cordX, cordY), new Point(tempX, tempY)));

                                                                Point enemyChecker =
                                                                    Field.GetEnemyPoint(new Point(cordX, cordY),
                                                                        new Point(tempX, tempY));

                                                                Field.Map[enemyChecker.CordX, enemyChecker.CordY].Type =
                                                                    CellType.Empty;

                                                                Field.MoveCheck(new Point(cordX, cordY), new Point(tempX, tempY));

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
                                            do
                                            {
                                                InputOfCords(out cordX, out cordY);
                                            } while (Field.Map[cordX, cordY].Type != Users[i].TypeDef &&
                                                     Field.Map[cordX, cordY].Type != Users[i].TypeQ);

                                            Field.CollectAllPossibleStepsToMoveCheck(new Point(cordX, cordY), Users[i]); // збір всіх можливих ходів для шашки яку вибрав юзер
                                            Console.Clear();
                                            Field.PrintField(Users[i]);

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
                        ((Bot)Users[i]).BotStep(this, i); 
                    } //BOT LOGIC

                    Console.ReadLine();
                    Console.Clear();

                    if (Users.Count == 2)
                    {
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

                        if (Field.DoesCheckerOnFieldCanBit(Users[i])) i--;
                    }
                }
            } while (!isEnd);

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

            }
            return false;
        } // bug WRITE TILL END
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
