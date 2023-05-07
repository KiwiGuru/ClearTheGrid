using System;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace StarterKit
{
    public class NearestMoves
    {
        //TODO Does not work
        public bool SeekNearestMove(Map map)
        {
            bool isDone = false;
            int foundDirections = 0;
            int[,] tempMatrix = map.data;
            int i = 0;
            int j = 0;
            var numbersLeft = 0;
            var maxIterations = map.h*map.w;
            var Iterations = 0;
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    if (tempMatrix[x, y] != 0)
                    {
                        numbersLeft++;
                    }
                }
            }
            List<Move> newMoves = new();
            while (!isDone)
            {
                i = new Random().Next(0, map.w);
                j = new Random().Next(0, map.h);
                foundDirections = 0;
                while (tempMatrix[i, j] == 0)
                {
                    i = new Random().Next(0, map.w);
                    j = new Random().Next(0, map.h);
                }
                var value = tempMatrix[i, j];
                //Old fashoned Cases?
                if ((i + value) < map.w)
                {
                    if (tempMatrix[i + value, j]!=0)
                    {
                        Console.WriteLine($"Move Right, + or - {value} from {i},{j} to {i + value},{j} with value {tempMatrix[i + value, j]}");
                        newMoves.Add(new Move(i, j, (Direction)1, false));
                        foundDirections += 1;
                    }                    
                }
                else if(i-value >= 0)
                {
                    if (tempMatrix[i - value, j] != 0)
                    {
                        Console.WriteLine($"Move Left, + or - {value} from {i},{j} to {i-value},{j} with value {tempMatrix[i - value, j]}");
                        newMoves.Add(new Move(i, j, (Direction)3, false));
                        foundDirections += 1;
                    }                    
                }
                if ((j + value) < map.h)
                {
                    if (tempMatrix[i, j + value] != 0)
                    {
                        Console.WriteLine($"Move Down, + or - {value} from {i},{j} to {i},{j + value} with value {tempMatrix[i, j+value]}");
                        newMoves.Add(new Move(i, j, (Direction)0, false));
                        foundDirections += 1;
                    }
                }
                else if(j-value >=0)
                {
                    if (tempMatrix[i, j - value] != 0)
                    {
                        Console.WriteLine($"Move Up, + or - {value} from {i},{j + value} with value to {tempMatrix[i, j - value]}");
                        newMoves.Add(new Move(i, j, (Direction)2, false));
                        foundDirections += 1;
                    }
                }
                if (foundDirections>0)
                {
                    foreach (var move in newMoves)
                    {
                        if (IsValidMove(map, move))
                        {
                            map.ApplyMove(move);
                            Console.WriteLine(move);
                            numbersLeft--;
                            tempMatrix = map.data;
                        }
                    }
                    newMoves.Clear();
                }
                
                Iterations++;
                if (Iterations >= maxIterations)
                {
                    return false;                  
                }
                if (numbersLeft == 0)
                {
                    return false;
                }
            }
            return true;
        }

        static bool IsValidMove(Map map, Move move)
        {
            int sourcevalue = map.data[move.x, move.y];
            if (sourcevalue == 0) return false;

            var dircoords = move.dir.AsCoords();
            int dx = move.x + dircoords.xstep * sourcevalue;
            int dy = move.y + dircoords.ystep * sourcevalue;

            if (dx >= 0 && dy >= 0 && dx < map.w && dy < map.h)
            {
                int destvalue = map.data[dx, dy];
                if (destvalue == 0) return false;
                return true;
            }
            //Recursion for try
            if (move.add == false)
            {
                move.add = true;
                IsValidMove(map, move);
            }            
            return false;
        }

    }
}
