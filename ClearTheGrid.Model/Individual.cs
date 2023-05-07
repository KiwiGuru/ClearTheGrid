using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTheGrid.Model
{
    public class Individual
    {        
        public List<Move> MovesPlayed { get; set; }
        public int[] MoveOrder { get; set; }
        public int Score { get; set; }
    }
}
