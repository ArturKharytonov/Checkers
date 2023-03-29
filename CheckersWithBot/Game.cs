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
            Users[0].Color = ConsoleColor.DarkCyan;
            Users[0].TypeDef = CellType.CheckerF;
            Users[0].TypeQ = CellType.QueenF;
            Users[1].Color = ConsoleColor.DarkMagenta;
            Users[1].TypeDef = CellType.CheckerS;
            Users[1].TypeQ = CellType.QueenS;
        }

        private void InputOfCords(ref int cordX, ref int cordY)
        {
            do
            {
                Console.Write("Cord X: ");
                int.TryParse(Console.ReadLine(), out cordX);
                Console.Write("Cord Y: ");
                char.TryParse(Console.ReadLine().ToUpper(), out char temp);
                cordY = (temp - _toGetNumber);

            } while (cordX < 0 || cordX >= Field.Map.GetLength(0) ||
                     cordY < 0 || cordY >= Field.Map.GetLength(1));
        }

        public void Start()
        {
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
                    Console.Clear();
                    Field.PrintField(Users[i]);
                    if (Users[i].GetType() == typeof(Player))
                    {
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
                                        Console.WriteLine($"{Users[i].Name} enter cords of your check.");
                                        //Users[i].CordsOfEmptyCells = Field.DoesPlayerCanBitAFigure(Users[i]);
                                        if (Field.DoesCheckOnFieldCanBit(Users[i])) // якщо гравецб може побити шашку
                                        {
                                            //collect able to bit. point and list of points that could be killed by that check
                                            Field.PrintField(Users[i]);

                                            //Users[i].DoesBitSmbBefore = true;
                                        }
                                        else 
                                        {
                                            do
                                            {
                                                InputOfCords(ref cordX, ref cordY);
                                            } while (Field.Map[cordX, cordY].Type != Users[i].TypeDef &&
                                                     Field.Map[cordX, cordY].Type != Users[i].TypeQ);

                                            Field.CollectAllPossibleStepsToMoveCheck(new Point(cordX, cordY), Users[i]); // збір всіх можливих ходів для шашки яку вибрав юзер
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
                                                            do
                                                            {
                                                                InputOfCords(ref tempCordX, ref tempCordY);
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
                                        } //якщо гравецб не може побити шашку
                                    }
                                    break;
                                case MenuInGame.Surrender:
                                    {
                                        Users.RemoveAt(i);
                                        isEnd = true;
                                    }
                                    break;
                                case MenuInGame.OfferDraw: // IN proccess
                                    {

                                    }
                                    break;
                                default:
                                    Console.WriteLine("Error choice.");
                                    step = false;
                                    break;
                            }
                        } while (!step);
                    }
                    else if (((Bot)Users[i]).BotStep(this, i)) Users[i].DoesBitSmbBefore = true;

                    
                    //clear all list of cords after step
                }
            } while (!isEnd);
            Console.WriteLine($"{Users[0].Name} - WON! HIS COLOR WAS - {Users[0].Color}.");
        }
    }
}
