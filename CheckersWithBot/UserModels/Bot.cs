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
        public Bot(string name) : base(name)
        {
            Name = name;
            CordsOfEmptyCells = new List<Point>(); // for one checker(for printing a field)
            UserAbleToBit = new Dictionary<Point, List<Point>>();
            DoesBitSmbBefore = false;
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
                field.GetEnemyPoint(startPoint, endPoint); // bug will be here if bot will beat by queen

            field.Map[enemyChecker.CordX, enemyChecker.CordY].Type = CellType.Empty;

            field.MoveCheck(startPoint, endPoint);
        }

        public void BotStep(Game game, int indexOfPlayer)
        {
            if (game.Field.DoesCheckerOnFieldCanBit(this)) // якщо бот може бити
            {
                this.UserAbleToBit = game.Field.CollectDictionary(this);

                if (GetCountOfEmptyCells(game.Field) > 1) // якщо варіантів декілька - знайти найбільш оптимальний
                {
                    int enemyIndex = 0;
                    if (indexOfPlayer == 0) enemyIndex = 1;
                    (Point, Point) bestMove = CheckEveryPossibleStepToBit(game, enemyIndex);
                    Moving(bestMove.Item1, bestMove.Item2, game.Field);
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
                if (CanAnotherPlayerBeatChecker(game)) // якшо опонент може побити шашку бота
                {

                }
                else
                {
                    (Point, Point) forCarefulStep = CarefulStep(game);

                    if (IsPossibleToGetQueen(game.Field, indexOfPlayer)) // чи можна отримати королеву
                        GetQueen(game.Field, indexOfPlayer);

                    else if (IsPossibleToCreateDanger(game)) // чи можна зробити безпечний хід щоб поставити іншу шашку під загрозу
                    {
                        (Point, Point) carefulStepWithDanger = GetPointForDanger(game);
                        if (carefulStepWithDanger.Item1 != null &&
                            carefulStepWithDanger.Item2 != null)
                            game.Field.MoveCheck(carefulStepWithDanger.Item1, carefulStepWithDanger.Item2);
                        
                    }
                    else if (false) // розміни
                    {

                    }
                    else if (forCarefulStep.Item1 != null && // пошук найбільш безпечного ходу який не зробить ніякої шкоди
                             forCarefulStep.Item2 != null) 
                        game.Field.MoveCheck(forCarefulStep.Item1, forCarefulStep.Item2);
                    else if (true)
                        DoRandomStep(game);
                    
                }
            }
        }

        // beating
        private (Point, Point) CheckEveryPossibleStepToBit(Game game, int enemyIndex) // find best move
        {
            //need to collect route for another player, and then compare routes and find best move

            List<Route> routes = new List<Route>();
            Point bestStartPoint = new Point(-1, -1);
            Point bestEndPoint = new Point(-1, -1);
            List<Point> points = new List<Point>();

            Field copyField = new Field(game.Field);

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
                                CollectAllPossibleMoves(routes, startPoint, game, copyField); // collected routes for bot
                            }
                        }
                    }
                }
            }
            CalculateValuesOfRoutes(routes, game.Field);

            return (bestStartPoint, bestEndPoint);
        }
        private void CollectAllPossibleMoves(List<Route> routes, Point startPoint, Game game, Field copyField)
        {
            for (int k = 0; k < routes.Count; k++)
            {
                do
                {
                    if (!DoesValueExistInRout(routes[k].Path, startPoint))
                        routes[k].Path.Add(startPoint);
                    
                    List<Point> emptyCells = game.Field.CollectEmptyCells(game.Field.Map[startPoint.CordX,
                        startPoint.CordY]);

                    if (emptyCells.Count > 1)
                    {
                        Moving(startPoint, emptyCells[0], game.Field);
                        startPoint = emptyCells[0];

                        for (int l = 1; l < emptyCells.Count; l++)
                        {
                            routes.Add(new Route(routes[k]));
                            routes[routes.Count - 1].Path.Add(emptyCells[l]);
                        }
                    }
                    else if (emptyCells.Count == 1)
                    {
                        Moving(startPoint, emptyCells[0], game.Field);
                        startPoint = emptyCells[0];
                    }
                } while (game.Field.DoesCurrentCheckerCanBitAnyCheck(game.Field.Map[startPoint.CordX, startPoint.CordY]));
                game.Field = new Field(copyField);
            }
        }
        private bool DoesValueExistInRout(List<Point> values, Point checkValue)
        {
            foreach (Point value in values)
            {
                if (value.CordX == checkValue.CordX &&
                    value.CordY == checkValue.CordY) return true;
            }

            return false;
        }
        private void CalculateValuesOfRoutes(List<Route> routes, Field field)
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
                        if (field.Map[enemyChecker.CordX, enemyChecker.CordY].Type == CellType.CheckerF ||
                            field.Map[enemyChecker.CordX, enemyChecker.CordY].Type == CellType.CheckerS)
                            routes[i].Count++;
                        else routes[i].Count += 3;
                    }
                }
            }
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

        
        // beating of bots' checkers!!! (IN PROCESS)
        // is bots' checker under danger
        private bool CanAnotherPlayerBeatChecker(Game game)
        {
            User enemy = GetEnemy(game.Users);
            
            if (game.Field.DoesCheckerOnFieldCanBit(enemy)) return true;

            return false;
        }
        
        // create logic to protect checkers!
        // is bots' checker under danger

        // is it possible to get a queen
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
                            Moving(new Point(0, i),
                                new Point(0, i + 1),
                                field);

                        else if (i - 1 >= 0 &&
                                 field.Map[0, i - 1].Type == CellType.Empty)
                            Moving(new Point(0, i),
                                new Point(0, i - 1),
                                field);
                    }
                }
            }
        }
        // is it possible to get a queen


        // careful step with danger for enemy
        private bool IsPossible(Point startPoint, Point endPoint, Field field, Field copy, User enemy)
        {
            field.MoveCheck(startPoint, endPoint);
            if (!field.DoesCheckerOnFieldCanBit(enemy) && field.DoesCheckerOnFieldCanBit(this))
            // bug if bot came from block when enemy can bit his checker this "if" won't work, ->
            // bug: maybe i need ref here few bool variables or create method can enemy beat any checker besides these which bot declined protecting
            {
                field = new Field(copy);
                return true;
            }
            field = new Field(copy);
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
                            if (IsPossible(startPoint, endPoint, game.Field, copyField, enemy)) return true;
                        
                    }
                }
            }
            return false;
        }
        private (Point, Point) GetPointForDanger(Game game)
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
                            if (IsPossible(startPoint, endPoint, game.Field, copyField, enemy)) return (startPoint, endPoint);
                        }
                    }
                }
            }
            return (null, null);
        }
        // careful step with danger for enemy

        // exchanges (IN PROCESS)
        public bool DoesItPossibleToCreateExchange(Game game)
        {
            User enemy = GetEnemy(game.Users);

            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        Point startPoint = new Point(i, j);
                        List<Point> collectEmptyCells = game.Field.CollectEmptyCells(game.Field.Map[i, j]);
                        if (collectEmptyCells.Count > 0)
                        {
                            foreach (Point endPoint in collectEmptyCells)
                            {
                                Moving(startPoint, endPoint, game.Field);
                                // here i need logic:
                                // 1. collect count of checkers that enemy can bit
                                // 2. and after that collect count of checkers that bot can beat
                                // find best checkers to move
                                // maybe i can use function "CheckEveryPossibleStepToBit"

                            }
                        }
                    }
                }
            }
            return false;
        }
        // exchanges

        // just careful step
        private (Point, Point) CarefulStep(Game game)
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
                        Point starPoint = new Point(i, j);
                        Field copyField = new Field(game.Field);
                        foreach (Point endPoint in this.CordsOfEmptyCells)
                        {
                            game.Field.MoveCheck(starPoint, endPoint);
                            if (!game.Field.DoesCheckerOnFieldCanBit(enemy)) //bug: as well as in careful step with danger for enemy
                            {
                                game.Field = new Field(copyField);
                                return (starPoint, endPoint);
                            }
                            game.Field = new Field(copyField);
                        }
                    }
                }
            }
            return (null, null);
        }
        // just careful step

        // random step
        public void DoRandomStep(Game game)
        {
            for (int i = 0; i < game.Field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < game.Field.Map.GetLength(1); j++)
                {
                    if (game.Field.Map[i, j].Type == this.TypeDef ||
                        game.Field.Map[i, j].Type == this.TypeQ)
                    {
                        List<Point> collectEmptyCells = game.Field.CollectEmptyCells(game.Field.Map[i, j]);
                        if (collectEmptyCells.Count > 0)
                            Moving(new Point(i, j), collectEmptyCells[0], game.Field);
                        
                    }
                }
            }

        }
        // random step

        //Point startPoint = new Point(i, j);

        //Point copyStartPoint = new Point(startPoint.CordX, startPoint.CordY);

        //foreach (Point endPoint in points)
        //{
        //    Point copyEndPoint = new Point(endPoint.CordX, endPoint.CordY);

        //    PassAllPossibleMoves(startPoint, endPoint, game, ref tempCount, ref tempWasBeaten, enemyIndex);
        //    if (tempCount > bestCountOfBeatenCheckers &&  
        //        !tempWasBeaten) // додати ще чи отримав гравець дамку після цього
        //    {
        //        bestStartPoint = copyStartPoint;
        //        bestEndPoint = copyEndPoint;
        //        bestCountOfBeatenCheckers = tempCount;
        //        wasBeaten = tempWasBeaten;
        //    }
        //    else if (tempCount > bestCountOfBeatenCheckers &&
        //             tempWasBeaten && wasBeaten)
        //    {
        //        bestStartPoint = copyStartPoint;
        //        bestEndPoint = copyEndPoint;
        //        bestCountOfBeatenCheckers = tempCount;
        //    }
        //    else if (tempCount >= bestCountOfBeatenCheckers - 1 &&
        //             !tempWasBeaten && wasBeaten)
        //    {
        //        bestStartPoint = copyStartPoint;
        //        bestEndPoint = copyEndPoint;
        //        wasBeaten = tempWasBeaten;
        //        bestCountOfBeatenCheckers = tempCount;
        //    }

        //    tempCount = 0;
        //    tempWasBeaten = false;
        //    game.Field = new Field(copyField);
        //}

        //private void PassAllPossibleMoves(Point startPoint, Point endPoint, Game game, ref int tempCount, ref bool tempWasBeaten, int enemyIndex)
        //{
        //    Point enemyChecker = game.Field.GetEnemyPoint(startPoint, endPoint);
        //    if (game.Field.Map[enemyChecker.CordX, enemyChecker.CordY].Type == CellType.QueenF ||
        //        game.Field.Map[enemyChecker.CordX, enemyChecker.CordY].Type == CellType.QueenS) tempCount += 3;
        //    else tempCount++;

        //    Moving(startPoint, endPoint, game.Field);
        //    startPoint = endPoint;
        //    if (game.Field.CountOfCheckersOnBoard(game.Users[enemyIndex]) == 0)
        //    {
        //        tempCount = int.MaxValue;
        //        tempWasBeaten = false;
        //        return;
        //    }
        //    if (game.Field.DoesCurrentCheckerCanBitAnyCheck(game.Field.Map[startPoint.CordX, startPoint.CordY]))
        //    {
        //        List<Point> allPossibleMoves = game.Field.CollectEmptyCells(game.Field.Map[startPoint.CordX, startPoint.CordY]); // bug here!!!
        //        if (allPossibleMoves.Count > 0)
        //        {
        //            for (int i = 0; i < allPossibleMoves.Count; i++)
        //                PassAllPossibleMoves(startPoint, allPossibleMoves[i], game, ref tempCount, ref tempWasBeaten, enemyIndex);
        //        }
        //    }
        //    else if(game.Field.CanAnotherPlayerBeatOurChecker(endPoint, this)) // check if checker was beaten after all
        //        tempWasBeaten = true;

        //    return;
        //}
    }
}
