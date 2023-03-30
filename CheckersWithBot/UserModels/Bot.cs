﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersWithBot.UserModels
{
    public class Bot : User
    {
        public Bot(string name) : base(name)
        {
            Name = name;
            CordsOfEmptyCells = new List<Point>();
            //CordsOfCheckersThatCanBit = new List<Point>();
            //HisChecksThatAbleToBit = new List<Point>();
            UserAbleToBit = new Dictionary<Point, List<Point>>();
            DoesBitSmbBefore = false;
        }

        public bool BotStep(Game game, int indexOfPlayer)
        {
            return false;
        }
    }
}