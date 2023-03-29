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
        private int _toGetNumber = 17;
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
                int.TryParse(Console.ReadLine(), out cordY);
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
                                        else //якщо гравецб не може побити шашку
                                        {
                                            //Users[i].DoesBitSmbBefore = false;
                                            do
                                            {
                                                InputOfCords(ref cordX, ref cordY);
                                            } while (Field.Map[cordX, cordY].Type != Users[i].TypeDef &&
                                                     Field.Map[cordX, cordY].Type != Users[i].TypeQ);
                                        }
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
