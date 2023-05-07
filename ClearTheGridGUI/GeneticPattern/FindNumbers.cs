using Shared;
using ClearTheGrid.GUI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearTheGrid.GUI.GeneticPattern
{
    /// <summary>
    /// This class is for finding numbers
    /// </summary>
    public class FindNumbers
    {       
        //For storing purposes
        private List<NumberData> numberDatas;

        public FindNumbers()
        {
            numberDatas = new List<NumberData>();
        }
        
        public List<NumberData> CountNumbers(Map map)
        {
            numberDatas.Clear();
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    if (map.data[x,y] !=0)
                    {
                        numberDatas.Add(new NumberData() { value = map.data[x, y], xCoor = x, yCoor = y});
                    }
                }
            }   
            return numberDatas;
        }
    }
}
