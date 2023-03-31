using System;
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

        private int GetEnemyIndex(int index)
        {
            if (index == 0) return 1;
            return 0;
        }
        public int GetCountOfEmptyCells(Field field)
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
        public void Moving(Point startPoint, Point endPoint, Field field)
        {
            Point enemyChecker =
                field.GetEnemyPoint(startPoint, endPoint);

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
                    (Point, Point) bestMove = CheckEveryPossibleStepToBit(game, indexOfPlayer);
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
                //Console.ReadLine();
            }
        }


        // beating
        /// Maybe i need replace all steps not step by step.
        public (Point, Point) CheckEveryPossibleStepToBit(Game game, int enemyIndex) // find best move
        {
            Point bestStartPoint = new Point(-1, -1);
            Point bestEndPoint = new Point(-1, -1);
            List<Point> points = new List<Point>();
            
            int bestCountOfBeatenCheckers = 0;
            bool wasBeaten = true;

            int tempCount = 1;
            bool tempWasBeaten = false;

            Field copyField = new Field(game.Field);

            for (int i = 0; i < copyField.Map.GetLength(0); i++)
            {
                for (int j = 0; j < copyField.Map.GetLength(1); j++)
                {
                    if (copyField.Map[i, j].Type == this.TypeDef ||
                        copyField.Map[i, j].Type == this.TypeQ &&
                        this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                    {
                        if (this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                        {
                            if (points.Count > 0)
                            {
                                Point startPoint = new Point(i, j);
                                Point copyStartPoint = new Point(startPoint.CordX, startPoint.CordY);
                                foreach (Point endPoint in points)
                                {
                                    Point copyEndPoint = new Point(endPoint.CordX, endPoint.CordY);
                                    //Moving(startPoint, endPoint, game.Field);
                                    PassAllPossibleMoves(startPoint, endPoint, game, ref tempCount, ref tempWasBeaten, enemyIndex);

                                    if (tempCount > bestCountOfBeatenCheckers &&  
                                        !tempWasBeaten) // додати ще чи отримав гравець дамку після цього
                                    {
                                        bestStartPoint = copyStartPoint;
                                        bestEndPoint = copyEndPoint;
                                        bestCountOfBeatenCheckers = tempCount;
                                        wasBeaten = tempWasBeaten;
                                    }
                                    else if (tempCount >= bestCountOfBeatenCheckers - 1 &&
                                             !tempWasBeaten && wasBeaten)
                                    {
                                        bestStartPoint = copyStartPoint;
                                        bestEndPoint = copyEndPoint;
                                        wasBeaten = false;
                                        bestCountOfBeatenCheckers = tempCount;
                                    }
                                    tempCount = 1;
                                }
                            }
                        }
                    }
                }
            }

            game.Field = new Field(copyField);
            return (bestStartPoint, bestEndPoint);
        }
        public void PassAllPossibleMoves(Point startPoint, Point endPoint, Game game, ref int tempCount, ref bool tempWasBeaten, int enemyIndex)
        {
            Moving(startPoint, endPoint, game.Field);
            startPoint = endPoint;
            if (game.Field.CountOfCheckersOnBoard(game.Users[enemyIndex]) == 0)
            {
                tempCount = int.MaxValue;
                return;
            }
            if (game.Field.DoesCurrentCheckerCanBitAnyCheck(game.Field.Map[startPoint.CordX, startPoint.CordY]))
            {
                tempCount++;
                
                List<Point> allPossibleMoves = game.Field.CollectEmptyCells(game.Field.Map[startPoint.CordX, startPoint.CordY]); // bug here!!!

                for (int i = 0; i < allPossibleMoves.Count; i++)
                    PassAllPossibleMoves(startPoint, allPossibleMoves[i], game, ref tempCount, ref tempWasBeaten, enemyIndex);
                
            }
            else if(game.Field.CanAnotherPlayerBeatOurChecker(endPoint, this)) // check if checker was beaten after all
                tempWasBeaten = true;

            return;
        }
        public (Point, List<Point>) GetFromDictionary(Field field)
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


        // beating of bots' checkers!!!
        
    }
}
