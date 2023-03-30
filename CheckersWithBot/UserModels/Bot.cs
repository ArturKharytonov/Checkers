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

        public int GetCountOfEmptyCells(Field field)
        {
            List<Point> points = new List<Point>();
            int countOfEmptyCells = 0;

            for (int i = 0; i < field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < field.Map.GetLength(1); j++)
                {
                    if (field.Map[i, j].Type == this.TypeDef ||
                        field.Map[i, j].Type == this.TypeQ &&
                        this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                    {
                        if (points != null)
                            countOfEmptyCells += points.Count;
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
        public bool BotStep(Game game, int indexOfPlayer)
        {
            if (game.Field.DoesCheckerOnFieldCanBit(this)) // якщо бот може бити
            {
                this.UserAbleToBit = game.Field.CollectDictionary(this);

                if (GetCountOfEmptyCells(game.Field) > 1) // якщо варіантів декілька
                {
                    (Point, Point) bestMove = CheckEveryPossibleStepToBit(game.Field, indexOfPlayer);
                    Moving(bestMove.Item1, bestMove.Item2, game.Field);
                }
                else // якщо варіант один
                {
                    (Point, List<Point>) fromFunction = GetFromDictionary(game.Field);
                    if (fromFunction.Item1 != null && fromFunction.Item2 != null)
                        Moving(fromFunction.Item1, fromFunction.Item2[0], game.Field);
                }
                return true;
            }

            else
            {

            }
            return false;
        }

        public (Point, Point) CheckEveryPossibleStepToBit(Field field, int indexOfPlayer) // find best move
        {
            Point bestStartPoint = new Point(-1, -1);
            Point bestEndPoint = new Point(-1, -1);
            List<Point> points = new List<Point>();

            int bestCountOfBeatenCheckers = 0;
            bool wasBeaten = true;


            for (int i = 0; i < field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < field.Map.GetLength(1); j++)
                {
                    if (field.Map[i, j].Type == this.TypeDef ||
                        field.Map[i, j].Type == this.TypeQ &&
                        this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                    {
                        if (points != null)
                        {
                            for (int k = 0; k < points.Count; k++)
                            {
                                
                            }
                        }
                    }
                }
            }
            return (bestStartPoint, bestEndPoint);
        }
        
        public (Point, List<Point>) GetFromDictionary(Field field)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < field.Map.GetLength(0); i++)
            {
                for (int j = 0; j < field.Map.GetLength(1); j++)
                {
                    if (field.Map[i, j].Type == this.TypeDef ||
                        field.Map[i, j].Type == this.TypeQ &&
                        this.UserAbleToBit.TryGetValue(new Point(i, j), out points))
                    {
                        return (new Point(i, j), points);
                    }
                }
            }
            return (null, null);
        } // used just for situation when bot have 1 possible attack
    }
}
