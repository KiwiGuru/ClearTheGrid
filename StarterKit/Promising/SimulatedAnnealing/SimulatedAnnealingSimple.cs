using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit
{
    public class SimulatedAnnealingSimple
    {
        private readonly Random _random = new Random();
        private readonly double _endTemperature;
        private readonly double _startTemperature;
        private readonly int _maxIterations;
        private readonly double _alpha;

        public SimulatedAnnealingSimple(double startTemperature, double endTemperature, int maxIterations, double alpha)
        {
            _startTemperature = startTemperature;
            _endTemperature = endTemperature;
            _maxIterations = maxIterations;
            _alpha = alpha;
        }

        public void BestMoves(Map map)
        {
            int[,] distances = map.data;
            int[,] tmp = new int[map.w, map.w];
            int[] order = new int[map.w];
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    tmp[x,y] = distances[x,y];
                }
                order[x] = x;
            }

            
            double temperature = _startTemperature;
            double coolingRate = _alpha;

            while (temperature > _endTemperature)
            {
                int i = new Random().Next(0, order.Length);
                int j = new Random().Next(0, order.Length);

                int h = new Random().Next(0, order.Length);
                int k = new Random().Next(0, order.Length);
                int numberCount = 0;
                bool isDone = false;               
                               
                while (!isDone)
                {                    
                    while (tmp[i, j] == 0)
                    {
                        i = new Random().Next(0, order.Length);
                        j = new Random().Next(0, order.Length);
                        
                    }
                    while (tmp[h, k] == 0)
                    {
                        h = new Random().Next(0, order.Length);
                        k = new Random().Next(0, order.Length);
                    }
                    if ((i!=h && j==k) || (j != k && i == h))
                    {
                        isDone = true;
                    }
                    else
                    {
                        //Reset?
                        i = new Random().Next(0, order.Length);
                        j = new Random().Next(0, order.Length);
                        h = new Random().Next(0, order.Length);
                        k = new Random().Next(0, order.Length);
                    }
                }

                //Console.WriteLine($"{tmp[order[i], order[j]]}{" "}{tmp[order[j], order[i]]}");
                //int deltaDistance = tmp[order[i], order[j]] - tmp[order[j], order[i]];
                Console.WriteLine($"{tmp[order[i], order[j]]}{" "}{tmp[order[h], order[k]]}");
                int deltaDistance = tmp[i, j] - tmp[h,k];
                if (deltaDistance < 0)
                {
                    int temp = order[i];
                    order[i] = order[j];
                    order[j] = temp;
                }
                else
                {
                    double probability = Math.Exp(-deltaDistance / temperature);
                    if (new Random().NextDouble() < probability)
                    {
                        int temp = order[i];
                        order[i] = order[j];
                        order[j] = temp;
                    }
                    tmp[i, j] = tmp[i, j] - tmp[h, k];
                    tmp[h, k] = 0;

                    for (int a = 0; a < order.Length; a++)
                    {
                        for (int b = 0; b < order.Length; b++)
                        {
                            if (tmp[a, b] != 0) numberCount++;
                        }
                    }
                    if (numberCount < 2)
                    {
                        Console.WriteLine("That's it");
                        break;
                    }
                }
                temperature *= (1 - coolingRate);
                
            }

            Console.WriteLine("Best order:");
            foreach (int city in order)
            {
                Console.Write(city + " ");
            }
        }
    }
}
