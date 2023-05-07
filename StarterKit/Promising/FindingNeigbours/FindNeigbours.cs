using Shared;
using StarterKit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit
{
    public class FindNeigbours
    {       
        //For sorting purposes
        private List<NeighbourData> neighbourDatas;
        private int[] direction = new int[2];

        public FindNeigbours()
        {
            neighbourDatas = new List<NeighbourData>();
            direction[0] = 1;
            direction[1] = -1;
        }

        public List<NeighbourData> CountAndSortNeighbours(Map map)
        {
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    if (map.data[x,y] !=0)
                    {
                        neighbourDatas.Add(new NeighbourData() { value = map.data[x, y], xCoor = x, yCoor = y, HorizontalNeighbours = 0 });
                    }
                }
            }
            var maxValue = neighbourDatas.MaxBy(x => x.value).value;
            for (int i = 0; i < neighbourDatas.Count(); i++)
            {
                //The step length across the board
                int step = 0;
                //Horizontal
                for (int j = 0; j < direction.Length; j++)
                {                    
                    step = neighbourDatas[i].value* direction[j];                    
                    if (neighbourDatas[i].xCoor + step < map.w && neighbourDatas[i].xCoor + step >= 0 && (neighbourDatas[i].value + step) < maxValue)
                    {
                        if (map.data[neighbourDatas[i].xCoor + step, neighbourDatas[i].yCoor] !=0)
                        {
                            neighbourDatas[i].HorizontalNeighbours++;
                        }                                       
                    }
                }
                //Vertical
                for (int j = 0; j < direction.Length; j++)
                {
                    step = neighbourDatas[i].value * direction[j];
                    if (neighbourDatas[i].yCoor + step < map.h && neighbourDatas[i].yCoor + step >= 0 && (neighbourDatas[i].value + step) < maxValue)
                    {
                        if (map.data[neighbourDatas[i].xCoor, neighbourDatas[i].yCoor + step] != 0)
                        {
                            neighbourDatas[i].VerticalNeighbours++;
                        }
                    }
                }
            }

            //Test with sorting for later prioritisation
            //Lowest total on top?
            if (neighbourDatas.MaxBy(x=>x.VerticalNeighbours)?.VerticalNeighbours > neighbourDatas.MaxBy(x => x.HorizontalNeighbours)?.HorizontalNeighbours)
            {
                neighbourDatas.Sort((x, y) => x.VerticalNeighbours.CompareTo(y.VerticalNeighbours));
            }
            else
            {
                neighbourDatas.Sort((x, y) => x.HorizontalNeighbours.CompareTo(y.HorizontalNeighbours));
            } 
            
            for (int i = 0; i < neighbourDatas.Count; i++)
            {
                Console.WriteLine($"Number {neighbourDatas[i].value} has {neighbourDatas[i].HorizontalNeighbours} horizontal and {neighbourDatas[i].VerticalNeighbours} vertical neighbours, x:{neighbourDatas[i].xCoor} y:{neighbourDatas[i].yCoor}");
            }
            //Console.WriteLine($"{stopwatch.ElapsedTicks} Ticks {stopwatch.ElapsedMilliseconds} milliseconds");
            return neighbourDatas;
        }
    }
}
