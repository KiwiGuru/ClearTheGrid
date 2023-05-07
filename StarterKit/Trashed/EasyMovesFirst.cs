using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit
{
    public class EasyMovesFirst
    {
        //This is an attempt to get the easiest moves as a human would do e.g., 2 of the same numbers
        //Empty move
        static Move emptyMove = new Move(0, 0, Direction.U, false);
        //Try to get the easiest moves first > if 2 of the same numbers are found can result in 0
        public void FindEasyMove(Map currentMap)
        {
            //Maximum number of moves on a map are at most the number of cells.
            var moves = new List<Move>();
            for (int i = 0; i < currentMap.w * currentMap.h; i++)
            {
                moves.Add(emptyMove);
            }

        }

    }
}
