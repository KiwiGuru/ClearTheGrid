using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTheGrid.GUI.Model
{
    public class Individual
    {        
        public List<Move> MovesPlayed { get; set; } = new List<Move>();
        public int[] MoveOrder { get; set; }
        public double[] MoveFitness { get; set; }
        public int Score { get; set; }
    }
}
