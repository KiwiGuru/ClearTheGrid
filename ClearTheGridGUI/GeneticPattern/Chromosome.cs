using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit
{
    //Part of the genetic algoritm, to keep track of evolutions voor the more difficult levels
    class Chromosome
    {
        public int Fitness { get; set; }

        public Chromosome()
        {
            Random random = new Random();
            Fitness = random.Next(100);
        }

        public Chromosome Breed(Chromosome otherParent)
        {
            Chromosome child = new Chromosome();
            child.Fitness = (this.Fitness + otherParent.Fitness) / 2;
            return child;
        }

        public void Mutate()
        {
            Random random = new Random();
            if (random.Next(100) < 5)
            {
                Fitness += random.Next(10) - 5;
                if (Fitness < 0)
                {
                    Fitness = 0;
                }
                else if (Fitness > 99)
                {
                    Fitness = 99;
                }
            }
        }
    }
}
