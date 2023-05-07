using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTheGrid.Model
{
    public class Settings
    {
        public Settings()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int GenerationCount { get; set; }
        public int PopulationSize { get; set; }
        public int SelectionCount { get; set; }
        public double MutationFactor { get; set; }
        public double CrossoverFactor { get; set; }
    }
}
