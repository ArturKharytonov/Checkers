﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersWithBot.FieldModel;
namespace CheckersWithBot.UserModels
{
    public class Bot : User
    {
        private Random _random = new Random();
        public Bot(string name) : base(name)
        {
            Name = name;
            CordsOfEmptyCells = new List<Point>(); // for one checker(for printing a field)
            UserAbleToBit = new Dictionary<Point, List<Point>>();
            DoesBeatSmbBefore = false;
        }

        public List<Route> AddToListsOfRoutes(List<Route> first, List<Route> second)
        {
            for (int i = 0; i < second.Count; i++)
            {
                first.Add(second[i]);
            }
            return first;
        }
        private User GetEnemy(List<User> users)
        {
            if (this.Name == users[0].Name) return users[1];
            return users[0];
        }
        private int GetCountOfEmptyCells(Field field)
        {
            List<Point> points = new List<Point>();
            int countOfEmptyCells = 0;

            for (int i = 0; i < field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < field.Map.GetLength(1); j++)
                {
                    if (field.Map[i, j].Type == this.TypeDef ||
                        field.Map[i, j].Type == this.TypeQ)
                    {
                        if (this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                        {
                            if (points.Count > 0)
                                countOfEmptyCells += points.Count;
                        }
                    }
                }
            }

            return countOfEmptyCells;
        }
        private void Moving(Point startPoint, Point endPoint, Field field)
        {
            Point enemyChecker =
                field.GetEnemyPoint(startPoint, endPoint);

            field.Map[enemyChecker.CordX, enemyChecker.CordY].Type = CellType.Empty;

            field.MoveCheck(startPoint, endPoint);
        }

        private int GetMaxValueFromListOfRoutes(List<Route> routes)
        {
            int maxValue = Int32.MinValue;
            for (int i = 0; i < routes.Count; i++)
            {
                if(maxValue < routes[i].Value) maxValue = routes[i].Value;
            }
            return maxValue;
        }
        public void BotStep(Game game, int indexOfPlayer)
        {
            this.CordsOfEmptyCells = new List<Point>();
            Field copy = new Field(game.Field);
            if (game.Field.DoesCheckerOnFieldCanBeat(this)) // якщо бот може бити
            {
                DoesBeatSmbBefore = true;
                this.UserAbleToBit = game.Field.CollectDictionary(this);

                if (GetCountOfEmptyCells(game.Field) > 1) // якщо варіантів декілька - знайти найбільш оптимальний
                {
                    try
                    {
                        (Point, Point) bestMove = CheckEveryPossibleStepToBeat(game); // bug here(sometimes in "moving" and "MoveAllCheckerToCheckCorrectField"), have already fixed
                        Moving(bestMove.Item1, bestMove.Item2, game.Field);
                    }
                    catch (NullReferenceException e)
                    {
                        (Point, Point) bestMove = CheckEveryPossibleStepToBeat(game);
                        Moving(bestMove.Item1, bestMove.Item2, game.Field);
                    }
                }
                else // якщо варіант один
                {
                    (Point, List<Point>) fromFunction = GetFromDictionary(game.Field);
                    if (fromFunction.Item1 != null && fromFunction.Item2 != null)
                        Moving(fromFunction.Item1, fromFunction.Item2[0], game.Field);
                }
            }

            else
            {
                DoesBeatSmbBefore = false;
                if (false) // якшо опонент може побити шашку бота CanAnotherPlayerBeatChecker(game)
                {
                    List<Point> allCheckers = new List<Point>();
                    // не включати ті шашки що бот відмовився захищати в else
                }
                else
                {
                    List<(Point, Point)> forCarefulStep = CarefulStep(game);
                    this.CordsOfEmptyCells = new List<Point>();
                    (Point, Point) makeExchange = DoesItPossibleToCreateExchange(game);
                    this.CordsOfEmptyCells = new List<Point>();
                    game.Field = new Field(copy);

                    if (IsPossibleToGetQueen(game.Field, indexOfPlayer)) // чи можна отримати королеву
                        GetQueen(game.Field, indexOfPlayer);

                    else if (IsPossibleToCreateDanger(game)) // чи можна зробити безпечний хід щоб поставити іншу шашку під загрозу
                    {
                        this.CordsOfEmptyCells = new List<Point>();
                        (Point, Point) carefulStepWithDanger = GetPointForDanger(game);
                        this.CordsOfEmptyCells = new List<Point>();
                        if (carefulStepWithDanger.Item1 != null &&
                            carefulStepWithDanger.Item2 != null)
                            game.Field.MoveCheck(carefulStepWithDanger.Item1, carefulStepWithDanger.Item2);
                    }
                    else if (makeExchange.Item1 != null && makeExchange.Item2 != null) // розміни (In process)
                        game.Field.MoveCheck(makeExchange.Item1, makeExchange.Item2);
                    
                    else if (forCarefulStep.Count > 0)
                    {
                        int random = _random.Next(0, forCarefulStep.Count);
                        game.Field.MoveCheck(forCarefulStep[random].Item1, forCarefulStep[random].Item2);
                    }

                    else if (true)
                        DoRandomStep(game);
                }
            }
        }

        // beating "DONE"
        private void MoveAllCheckerToCheckCorrectField(Route route, Field field)
        {
            for (int i = 0; i < route.Path.Count; i++)
            {
                if (i + 1 < route.Path.Count)
                    Moving(route.Path[i], route.Path[i + 1], field);
            }
        } // BUG HERE
        private (Point, Point) CheckEveryPossibleStepToBeat(Game game) // find best move
        {
            List<Route> routes = new List<Route>();
            List<Point> points = new List<Point>();
            int stopped = 0;
            Field copyField = new Field(game.Field);
            int countOfBotCheckers = game.Field.CountOfCheckersOnBoard(this);
            int countOfEnemyCheckers = game.Field.CountOfCheckersOnBoard(GetEnemy(game.Users));

            for (int i = 0; i < copyField.Map.GetLength(0); i++)
            {
                for (int j = 0; j < copyField.Map.GetLength(1); j++)
                {
                    if (copyField.Map[i, j].Type == this.TypeDef ||
                        copyField.Map[i, j].Type == this.TypeQ)
                    {
                        if (this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                        {
                            if (points.Count > 0)
                            {
                                Point startPoint = new Point(i, j);
                                routes.Add(new Route(new List<Point>() { startPoint }));
                                CollectAllPossibleMoves(routes, startPoint, game, copyField, ref stopped); // collected routes for bot
                            }
                        }
                    }
                }
            }
            CalculateValuesOfRoutes(routes, game.Field, copyField); // for bot // maybe add checking if bot won if yes step all and return Point to get final step
            game.Field = new Field(copyField);
            Dictionary<Point, List<Route>> consequences = CollectDictionaryOfRoutes(routes, game, copyField); // bug in collect dictionary

            (Point, Point) bestPoints = CompareAndFindBestPoints(routes, consequences, countOfBotCheckers, countOfEnemyCheckers);
            return (bestPoints.Item1, bestPoints.Item2);
        }
        private Dictionary<Point, List<Route>> CollectDictionaryOfRoutes(List<Route> botRoutes, Game game, Field copy)
        {
            Dictionary<Point, List<Route>> consequences = new Dictionary<Point, List<Route>>();
            User enemy = GetEnemy(game.Users);

            for (int i = 0; i < botRoutes.Count; i++)
            {
                List<Route> enemyRoutes = new List<Route>();
                for (int j = 0; j < botRoutes[i].Path.Count; j++)
                {
                    if (j + 1 < botRoutes[i].Path.Count)
                    {
                        Moving(botRoutes[i].Path[j], botRoutes[i].Path[j + 1], game.Field);
                    }
                }
                Field secondCopy = new Field(game.Field);
                // collect all cells which can bit bot checkers
                enemy.UserAbleToBit = game.Field.CollectDictionary(enemy);
                if (enemy.UserAbleToBit.Count > 0)
                {
                    enemyRoutes = CollectRoutesForEnemy(enemy, game, secondCopy);
                    CalculateValuesOfRoutes(enemyRoutes, game.Field, secondCopy);
                    if (consequences.ContainsKey(botRoutes[i].Path[botRoutes[i].Path.Count - 1]))
                    {
                        List<Route> routes = new List<Route>();
                        consequences.TryGetValue(botRoutes[i].Path[botRoutes[i].Path.Count - 1], out routes);
                        routes = AddToListsOfRoutes(routes, enemyRoutes);
                        consequences.Remove(botRoutes[i].Path[botRoutes[i].Path.Count - 1]);

                        consequences.Add(botRoutes[i].Path[botRoutes[i].Path.Count - 1], routes);
                    }
                    else consequences.Add(botRoutes[i].Path[botRoutes[i].Path.Count - 1], enemyRoutes);
                    //game.Field = new Field(copy);
                }
                game.Field = new Field(copy);
            }
            return consequences;
        }

        private void CollectAllPossibleMoves(List<Route> routes, Point startPoint, Game game, Field copyField, ref int whereStopped, bool needICorrectFormField = false)
        {
            for (int k = whereStopped; k < routes.Count; k++)
            {
                startPoint = routes[k].Path[routes[k].Path.Count - 1];
                do
                {
                    //if (!DoesValueExistInRout(routes[k].Path, startPoint))
                    //    routes[k].Path.Add(startPoint);

                    if (needICorrectFormField)
                    {
                        MoveAllCheckerToCheckCorrectField(routes[k], game.Field);
                        needICorrectFormField = false;
                    }
                    
                    List<Point> emptyCells = game.Field.CollectEmptyCells(game.Field.Map[routes[k].Path[routes[k].Path.Count - 1].CordX,
                        routes[k].Path[routes[k].Path.Count - 1].CordY]);

                    if (emptyCells.Count > 1)
                    {
                        Moving(startPoint, emptyCells[0], game.Field);
                        startPoint = emptyCells[0];

                        for (int l = 1; l < emptyCells.Count; l++)
                        {
                            routes.Add(new Route(routes[k]));
                            routes[routes.Count - 1].Path.Add(emptyCells[l]);
                        }
                        routes[k].Path.Add(startPoint);
                    }
                    else if (emptyCells.Count == 1)
                    {
                        Moving(startPoint, emptyCells[0], game.Field);
                        startPoint = emptyCells[0];
                        routes[k].Path.Add(startPoint);
                    }
                    else break;
                    
                } while (game.Field.DoesCurrentCheckerCanBitAnyCheck(game.Field.Map[startPoint.CordX, startPoint.CordY]));
                needICorrectFormField = true;
                game.Field = new Field(copyField);
            }
            whereStopped = routes.Count;
        }
        private void CalculateValuesOfRoutes(List<Route> routes, Field field, Field copy)
        {
            for (int i = 0; i < routes.Count; i++)
            {
                for (int j = 0; j < routes[i].Path.Count; j++)
                {
                    if (j + 1 < routes[i].Path.Count)
                    {
                        Point startPoint = routes[i].Path[j];
                        Point endPoint = routes[i].Path[j + 1];
                        Point enemyChecker = field.GetEnemyPoint(startPoint, endPoint);
                        if (enemyChecker != null)
                        {
                            if (field.Map[enemyChecker.CordX, enemyChecker.CordY].Type == CellType.CheckerF ||
                                field.Map[enemyChecker.CordX, enemyChecker.CordY].Type == CellType.CheckerS)
                                routes[i].Value++;
                            else if (field.Map[enemyChecker.CordX, enemyChecker.CordY].Type != CellType.Empty)
                                routes[i].Value += 3;
                            Moving(startPoint, endPoint, field);
                        }
                    }
                        
                }
                field = new Field(copy);
            }
        }
        private List<Route> CollectRoutesForEnemy(User enemy, Game game, Field copyField)
        {
            List<Route> enemyRoutes = new List<Route>();
            List<Point> points = new List<Point>();
            int stopped = 0;
            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < copyField.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == enemy.TypeDef ||
                        game.Field.Map[i, j].Type == enemy.TypeQ)
                    {
                        if (enemy.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                        {
                            if (points.Count > 0)
                            {
                                Point startPoint = new Point(i, j);
                                enemyRoutes.Add(new Route(new List<Point>() { startPoint }));
                                CollectAllPossibleMoves(enemyRoutes, startPoint, game, copyField, ref stopped); // collected routes for bot
                            }
                        }
                    }
                }
            }

            return enemyRoutes;
        }
        private (Point, Point) CompareAndFindBestPoints(List<Route> botRoutes, Dictionary<Point, List<Route>> enemyRoutes, int countOfBotCheckers, int countOfEnemyCheckers)
        {
            List<Route> tempList = new List<Route>();
            Point bestStartPoint = botRoutes[0].Path[0];
            Point bestEndPoint = botRoutes[0].Path[1];
            int bestvalueOfBotRout = botRoutes[0].Value;

            for (int i = 1; i < botRoutes.Count; i++)
            {
                int valueOfBotRoute = botRoutes[i].Value;
                if (enemyRoutes.TryGetValue(botRoutes[i].Path[botRoutes[i].Path.Count - 1], out tempList))
                {
                    int maxValue = 0;
                    if (tempList.Count > 0)
                    {
                        maxValue = tempList[0].Value;
                        for (int j = 1; j < tempList.Count; j++)
                        {
                            if (maxValue < tempList[j].Value)
                                maxValue = tempList[j].Value;
                        }
                    }
                    if (valueOfBotRoute - maxValue > bestvalueOfBotRout)
                    {
                        bestvalueOfBotRout = valueOfBotRoute - maxValue;
                        bestStartPoint = botRoutes[i].Path[0];
                        bestEndPoint = botRoutes[i].Path[1];
                    }
                    else if (valueOfBotRoute - maxValue >= bestvalueOfBotRout &&
                             countOfBotCheckers >= countOfEnemyCheckers)
                    {
                        bestvalueOfBotRout = valueOfBotRoute - maxValue;
                        bestStartPoint = botRoutes[i].Path[0];
                        bestEndPoint = botRoutes[i].Path[1];
                    }
                }
                else if (valueOfBotRoute > bestvalueOfBotRout)
                {
                    bestvalueOfBotRout = valueOfBotRoute;
                    bestStartPoint = botRoutes[i].Path[0];
                    bestEndPoint = botRoutes[i].Path[1];
                }
            }
            return (bestStartPoint, bestEndPoint);
        }
        private (Point, List<Point>) GetFromDictionary(Field field)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < field.Map.GetLength(1); j++)
                {
                    if (field.Map[i, j].Type == this.TypeDef ||
                        field.Map[i, j].Type == this.TypeQ)
                    {
                        if (this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                            return (new Point(i, j), points);
                    }
                }
            }
            return (null, null);
        } // used just for situation when bot have 1 possible attack
        // beating

        // protecting of bots' checkers!!! (IN PROCESS)
        // is bots' checker under danger
        private bool CanAnotherPlayerBeatChecker(Game game)
        {
            User enemy = GetEnemy(game.Users);
            
            if (game.Field.DoesCheckerOnFieldCanBeat(enemy)) return true;

            return false;
        }
        // create logic to protect checkers!
        // is bots' checker under danger


        // is it possible to get a queen "DONE"
        private bool IsPossibleToGetQueen(Field field, int indexOfPlayer)
        {
            if (indexOfPlayer == 0)
            {
                for (int i = 0; i < field.Map.GetLength(1); i++)
                {
                    if (field.Map[field.Map.GetLength(0) - 2, i].Type == CellType.CheckerF &&
                        ((i + 1 < field.Map.GetLength(1) &&
                         field.Map[field.Map.GetLength(0) - 1, i + 1].Type == CellType.Empty) ||
                        (i - 1 >= 0 &&
                         field.Map[field.Map.GetLength(0) - 1, i - 1].Type == CellType.Empty))) return true;
                }
            }
            else
            {
                for (int i = 0; i < field.Map.GetLength(1); i++)
                {
                    if (field.Map[1, i].Type == CellType.CheckerS &&
                        ((i + 1 < field.Map.GetLength(1) &&
                          field.Map[0, i + 1].Type == CellType.Empty) ||
                         (i - 1 >= 0 &&
                          field.Map[0, i - 1].Type == CellType.Empty))) return true;
                }
            }
            return false;
        }
        private void GetQueen(Field field, int indexOfPlayer)
        {
            if (indexOfPlayer == 0)
            {
                for (int i = 0; i < field.Map.GetLength(1); i++)
                {
                    if (field.Map[field.Map.GetLength(0) - 2, i].Type == CellType.CheckerF)
                    {
                        if (i + 1 < field.Map.GetLength(1) &&
                            field.Map[field.Map.GetLength(0) - 1, i + 1].Type == CellType.Empty)
                            field.MoveCheck(new Point(field.Map.GetLength(0) - 2, i),
                                new Point(field.Map.GetLength(0) - 1, i + 1));
                        
                        else if (i - 1 >= 0 && 
                                 field.Map[field.Map.GetLength(0) - 1, i - 1].Type == CellType.Empty)
                            field.MoveCheck(new Point(field.Map.GetLength(0) - 2, i),
                                new Point(field.Map.GetLength(0) - 1, i - 1));
                    }
                }
            }
            else
            {
                for (int i = 0; i < field.Map.GetLength(1); i++)
                {
                    if (field.Map[1, i].Type == CellType.CheckerS)
                    {
                        if (i + 1 < field.Map.GetLength(1) &&
                            field.Map[0, i + 1].Type == CellType.Empty)
                            field.MoveCheck(new Point(1, i),
                                new Point(0, i + 1));

                        else if (i - 1 >= 0 &&
                                 field.Map[0, i - 1].Type == CellType.Empty)
                            field.MoveCheck(new Point(1, i),
                                new Point(0, i - 1));
                    }
                }
            }
        }
        // is it possible to get a queen


        // careful step with danger for enemy "A little fix"
        private bool IsPossible(Point startPoint, Point endPoint, Field field, User enemy)
        {
            field.MoveCheck(startPoint, endPoint);
            if (!field.DoesCheckerOnFieldCanBeat(enemy) && field.DoesCheckerOnFieldCanBeat(this)) return true;
            // bug if bot came from block when enemy can bit his checker this "if" won't work, ->
            // bug: maybe i need ref here few bool variables or create method can enemy beat any checker besides these which bot declined protecting

            return false;
        }
        private bool IsPossibleToCreateDanger(Game game)
        {
            User enemy = GetEnemy(game.Users);

            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        game.Field.CollectAllPossibleStepsToMoveCheck(new Point(i, j), this);
                        Point startPoint = new Point(i, j);
                        Field copyField = new Field(game.Field);
                        foreach (Point endPoint in this.CordsOfEmptyCells)
                        {
                            if (IsPossible(startPoint, endPoint, game.Field, enemy))
                            {
                                game.Field = new Field(copyField);
                                return true;
                            }
                            game.Field = new Field(copyField);
                            this.CordsOfEmptyCells = new List<Point>();
                        }
                    }
                }
            }
            return false;
        }
        private (Point, Point) GetPointForDanger(Game game)
        {
            User enemy = GetEnemy(game.Users);
            Field copyField = new Field(game.Field);

            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        game.Field.CollectAllPossibleStepsToMoveCheck(new Point(i, j), this);
                        Point startPoint = new Point(i, j);
                        foreach (Point endPoint in this.CordsOfEmptyCells)
                        {
                            if (IsPossible(startPoint, endPoint, game.Field, enemy))
                            {
                                game.Field = new Field(copyField);
                                return (startPoint, endPoint);
                            }
                            game.Field = new Field(copyField);
                        }
                        this.CordsOfEmptyCells = new List<Point>();
                    }
                }
            }
            
            return (null, null);
        }
        // careful step with danger for enemy


        // exchanges (IN PROCESS)
        private (Point, Point) DoesItPossibleToCreateExchange(Game game)
        {
            User enemy = GetEnemy(game.Users);
            Dictionary<Point, List<Point>> points = new Dictionary<Point, List<Point>>();
            List<List<Route>> routesOfEnemy = new List<List<Route>>();
            List<Dictionary<int, List<Route>>> botRoutes = new List<Dictionary<int,List<Route>>>();
            
            int countOfBotCheckers = game.Field.CountOfCheckersOnBoard(this);
            int countOfEnemyCheckers = game.Field.CountOfCheckersOnBoard(enemy);

            Field copy = new Field(game.Field);
            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        Point startPoint = new Point(i, j);
                        game.Field.CollectAllPossibleStepsToMoveCheck(new Point(i, j), this);
                        if (this.CordsOfEmptyCells.Count > 0)
                        {
                            List<Point> tempEndPoints = new List<Point>();
                            foreach (Point endPoint in this.CordsOfEmptyCells)
                            {
                                game.Field.MoveCheck(startPoint, endPoint);
                                Field secondCopy = new Field(game.Field);

                                if (game.Field.DoesCheckerOnFieldCanBeat(enemy))
                                {
                                    enemy.UserAbleToBit = game.Field.CollectDictionary(enemy);
                                    List<Route> tempList = CollectRoutes(enemy, game, secondCopy);
                                    if (tempList.Count > 0)
                                    {
                                        tempEndPoints.Add(endPoint);
                                        CalculateValuesOfRoutes(tempList, game.Field, secondCopy);
                                        routesOfEnemy.Add(tempList);
                                    }
                                }
                                // then to every route of enemy create dictionary with(int index, List<Route> routes) of bots' routes
                                game.Field = new Field(copy);
                            }
                            if(tempEndPoints.Count > 0) points.Add(startPoint, tempEndPoints);
                        }
                        this.CordsOfEmptyCells = new List<Point>();
                    }
                }
            }

            if (routesOfEnemy.Count > 0)
            {
                botRoutes = GetAllPossibleVariationsOfRoutesForBot(enemy, game, copy, routesOfEnemy, points);
                (Point, Point) fromComparing = Compare(points, routesOfEnemy, botRoutes, game);
                if (fromComparing.Item1.CordX != -1 && fromComparing.Item2.CordX != -1)
                    return fromComparing;
                
                //compare botRoutes and routesOfEnemy variables
            }
            return (null, null);
        }
        private List<Route> CollectRoutes(User user, Game game, Field copy)
        {
            List<Route> routes = CollectRoutesForEnemy(user, game, copy);

            game.Field = new Field(copy);
            return routes;
        }
        private List<Dictionary<int, List<Route>>> GetAllPossibleVariationsOfRoutesForBot(User enemy, Game game, Field copy, List<List<Route>> routesOfEnemy, Dictionary<Point, List<Point>> points)
        {
            List<Dictionary<int, List<Route>>> botList = new List<Dictionary<int,List<Route>>>();
            List<Point> tempListOfEndPoints = new List<Point>();
            int indexOfEnemyRoutes = 0;

            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if(points.TryGetValue(new Point(i, j), out tempListOfEndPoints))
                    {
                        if (tempListOfEndPoints.Count > 0)
                        {
                            foreach (var point in tempListOfEndPoints)
                            {
                                game.Field.MoveCheck(new Point(i, j), point);
                                Field secondCopyField = new Field(game.Field);
                                botList.Add(GetDictionaryForBot(routesOfEnemy[indexOfEnemyRoutes], game, secondCopyField)); //logic of collecting dictionary
                                indexOfEnemyRoutes++;
                                game.Field = new Field(copy);
                            }
                        }
                    }
                }
            }
            return botList;
        }
        private Dictionary<int, List<Route>> GetDictionaryForBot(List<Route> enemyRoutes, Game game, Field field)
        {
            Dictionary<int, List<Route>> toReturn = new Dictionary<int, List<Route>>();

            for (int i = 0; i < enemyRoutes.Count; i++)
            {
                for (int j = 0; j < enemyRoutes[i].Path.Count; j++)
                {
                    if (j + 1 < enemyRoutes[i].Path.Count)
                    {
                        Moving(new Point(enemyRoutes[i].Path[j].CordX, enemyRoutes[i].Path[j].CordY),
                            new Point(enemyRoutes[i].Path[j + 1].CordX, enemyRoutes[i].Path[j + 1].CordY),
                            game.Field);
                    }
                }
                Field secondCopy = new Field(game.Field);
                this.UserAbleToBit = game.Field.CollectDictionary(this);
                List<Route> collect = CollectRoutesForEnemy(this, game, secondCopy);
                CalculateValuesOfRoutes(collect, secondCopy, new Field(secondCopy));
                toReturn.Add(i, collect);
                game.Field = new Field(field);
            }
            return toReturn;
        }
        private (Point, Point) Compare(Dictionary<Point, List<Point>> points, List<List<Route>> routesOfEnemy, List<Dictionary<int, List<Route>>> botRoutes, Game game)
        {
            List<Point> tempListOfEndPoints = new List<Point>();
            List<Route> tempBotRoutes = new List<Route>();
            Point bestStartPoint = new Point(-1, -1);
            Point bestEndPoint = new Point(-1, -1);
            int bestDifference = Int32.MinValue;
            int index = 0;
            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (points.TryGetValue(new Point(i, j), out tempListOfEndPoints))
                    {
                        foreach (var endPoint in tempListOfEndPoints)
                        {
                            for (int k = 0; k < routesOfEnemy[index].Count; k++)
                            {
                                

                                if (botRoutes[index].TryGetValue(k, out tempBotRoutes) &&
                                    tempBotRoutes.Count > 0)
                                {
                                    if (routesOfEnemy[index][k].Value > GetMaxValueFromListOfRoutes(tempBotRoutes))
                                    {
                                        if (bestStartPoint.CordX == i && bestStartPoint.CordY == j)
                                        {
                                            bestDifference = Int32.MinValue;
                                            bestStartPoint = new Point(-1, -1);
                                            bestEndPoint = new Point(-1, -1);
                                        }
                                    }
                                    else if (bestDifference < GetMaxValueFromListOfRoutes(tempBotRoutes) -
                                        routesOfEnemy[index][k].Value)
                                    {
                                        bestStartPoint = new Point(i, j);
                                        bestEndPoint = new Point(endPoint.CordX, endPoint.CordY);
                                        bestDifference = GetMaxValueFromListOfRoutes(tempBotRoutes) -
                                                         routesOfEnemy[index][k].Value;
                                    }
                                }
                                else if (tempBotRoutes.Count == 0) return (new Point(-1, -1), new Point(-1, -1));
                            }
                            index++;
                        }
                    }
                }
            }

            return (bestStartPoint, bestEndPoint);
        } // bug: i have to compare better moves
        // exchanges

        // just careful step "A little fix"
        private List<(Point, Point)> CarefulStep(Game game) //bug: as well as in "isPossible"
        {
            List<(Point, Point)> list = new List<(Point, Point)> ();
            User enemy = GetEnemy(game.Users);
            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        game.Field.CollectAllPossibleStepsToMoveCheck(new Point(i, j), this);
                        Point starPoint = new Point(i, j);
                        Field copyField = new Field(game.Field);
                        foreach (Point endPoint in this.CordsOfEmptyCells)
                        {
                            game.Field.MoveCheck(starPoint, endPoint);
                            if (!game.Field.DoesCheckerOnFieldCanBeat(enemy)) 
                            {
                                game.Field = new Field(copyField);
                                list.Add((starPoint, endPoint));
                                
                            }
                            game.Field = new Field(copyField);
                        }
                        this.CordsOfEmptyCells = new List<Point>();
                    }
                }
            }
            return list;
        }
        // just careful step

        // random step "DONE"
        public void DoRandomStep(Game game)
        {
            List<Point> allPoints = new List<Point>();
            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        game.Field.CollectAllPossibleStepsToMoveCheck(new Point(i, j), this);
                        if (this.CordsOfEmptyCells.Count > 0)
                            allPoints.Add(new Point(i, j));
                        this.CordsOfEmptyCells = new List<Point>();

                    }
                }
            }

            if (allPoints.Count > 0)
            {
                int whatCord = _random.Next(0, allPoints.Count);
                game.Field.CollectAllPossibleStepsToMoveCheck(new Point(allPoints[whatCord].CordX, allPoints[whatCord].CordY), this);
                int emptyCell = _random.Next(0, this.CordsOfEmptyCells.Count);
                game.Field.MoveCheck(allPoints[whatCord], this.CordsOfEmptyCells[emptyCell]);
            }
            else
                game.Users.Remove(this);
            
        }
        // random step
    }
}

//private bool DoesValueExistInRout(List<Point> values, Point checkValue)
//{
//    foreach (Point value in values)
//    {
//        if (value.CordX == checkValue.CordX &&
//            value.CordY == checkValue.CordY) return true;
//    }

//    return false;
//}