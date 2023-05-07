using System;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarterKit.Model;

namespace StarterKit
{
    public class CalculateMovesFromNeighbours
    {
        public static Random rnd = new Random();
        public List<Move> CalculateMoves(Map map, List<NeighbourData> neighbourDatas)
        {
            var result = new List<Move>();
            foreach (var data in neighbourDatas)
            {
                for (int d = 0; d < 4; d++)
                {
                    for (int s = 0; s < 2; s++)
                    {
                        var move = new Move(data.xCoor, data.yCoor, (Direction)d, s == 0);
                        if (IsValidMove(map, move))
                        {
                            result.Add(move);
                        }
                    }
                }
            }
            return result;
        }

        static (Map finalMap, List<Move> actualMoves) PlayMovesOnMap(Map m, List<Move> moves)
        {
            //Collect the moves we are actually making this playout.
            var actualMoves = new List<Move>();

            //We don't wanna mess up the original map.. 
            var localMap = CloneMap(m);

            //try to "play" the provided moves, are move 
            foreach (var move in moves)
            {
                Move actualMove = move;

                //make sure this move is valid, otherwise, pick a random valid move..
                if (!IsValidMove(localMap, actualMove))
                {
                    //Oops, the move is not valid. Lets find a random move/
                    //var validMoves = GetValidMoves(localMap);
                    //if (validMoves.Count == 0)
                    //{
                    //    //Surprise. It seems we are done! No more valid moves are found..
                    //    break;
                    //}
                    actualMove = moves[rnd.Next(moves.Count)];
                    break;
                    
                }

                //Execute the move on our map
                localMap.ApplyMove(actualMove);

                //Save a cloned version of the actualMove in our list. 
                actualMoves.Add(actualMove.Clone());
            }
            return (localMap, actualMoves);
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
            return false;
        }

        static Map CloneMap(Map source)
        {
            var result = new Map(source.w, source.h);
            result.data = (int[,])source.data.Clone();
            return result;
        }
    }
}
