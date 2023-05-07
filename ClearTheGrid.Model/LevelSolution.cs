using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTheGrid.Model
{
    public class LevelSolution
    {
        public LevelSolution()
        {
            Id = Guid.NewGuid();
            Moves = new List<ResultMove>();
        }
        public Guid Id { get; set; }
        public int Level { get; set; }
        public List<ResultMove> Moves { get; set; }
    }
}
