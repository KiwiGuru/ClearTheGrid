using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.Model
{
    public class NeighbourData
    {
        public int value { get; set; }
        public int xCoor { get; set; }
        public int yCoor { get; set; }
        public int HorizontalNeighbours { get; set; }
        public int VerticalNeighbours { get; set; }
    }
}
