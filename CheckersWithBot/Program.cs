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
    internal class Program
    {
        static void Main(string[] args)
        {
            Field field = new Field();
            Game game = new Game(field, new List<User>());

            bool isEnd = false;
            MainMenu mainMenu = new MainMenu();

            do
            {
                Console.WriteLine("----MENU----\n" +
                   "1 - Add player.\n" +
                   "2 - Add bot.\n" +
                   "3 - Delete player.\n" +
                   "4 - START GAME.\n" +
                   "5 - Show Players.\n" +
                   "6 - EXIT.");
                Console.Write("Enter your choice: ");
                Enum.TryParse(Console.ReadLine(), out mainMenu);
                switch (mainMenu)
                {
                    case MainMenu.AddPlayer:
                        {
                            if (game.Users.Count < 2)
                            {
                                Console.Write("Enter player name: ");
                                string name = Console.ReadLine();
                                if (name != "" &&
                                    !game.DoesNameAlreadyExist(name))
                                {
                                    Player player = new Player(name);
                                    game.Users.Add(player);
                                    Console.WriteLine("Player was successfully added");
                                }
                                else Console.WriteLine("Incorrect name or name already exist...");
                            }
                            else Console.WriteLine("Max count of players...");
                        }
                        break;
                    case MainMenu.AddBot:
                        {
                            if (game.Users.Count < 2)
                            {
                                Console.Write("Enter bot name: ");
                                string name = Console.ReadLine();
                                if (name != "" &&
                                    !game.DoesNameAlreadyExist(name))
                                {
                                    Bot bot = new Bot(name);
                                    game.Users.Add(bot);
                                    Console.WriteLine("Bot was successfully added");
                                }
                                else Console.WriteLine("Incorrect name or name already exist...");
                            }
                            else Console.WriteLine("Max count of players...");
                        }
                        break;
                    case MainMenu.DeletePlayer:
                        {
                            if (game.Users.Count > 0)
                            {
                                int index = -1;
                                do
                                {
                                    Console.Write("Enter index of user who u wanna delete:");
                                    int.TryParse(Console.ReadLine(), out index);
                                } while (index <= 0 || index > game.Users.Count);
                                game.Users.RemoveAt(index - 1);
                                Console.WriteLine("User was successfully deleted");
                            }
                            else Console.WriteLine("List of players is empty...");
                        }
                        break;
                    case MainMenu.StartGame:
                        {
                            if (game.Users.Count == 2)
                            {
                                game.Start();

                                field = new Field();
                                game = new Game(field, new List<User>());
                            }
                        }
                        break;
                    case MainMenu.ShowPlayers:
                        {
                            if (game.Users.Count != 0) game.PrintAllUsers();

                            else Console.WriteLine("List of users is empty.");
                        }
                        break;
                    case MainMenu.Exit:
                        isEnd = true;
                        break;
                    default:
                        Console.WriteLine("Error choice");
                        break;
                }
                Console.ReadLine();
                Console.Clear();
            } while (!isEnd);
        }
    }
}
