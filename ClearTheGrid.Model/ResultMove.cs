using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTheGrid.Model
{
    public class ResultMove
    {
        public ResultMove()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid LevelSolutionId { get; set; }
        public int Rank { get; set; }
        public string Move { get; set; }
        public LevelSolution LevelSolution { get; set; }
    }
}
