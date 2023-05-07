using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit
{
    //An attempt to use SA, Simulated Annealing
    public class SimulatedAnnealing
    {
        private readonly Random _random = new Random();
        private readonly double _startTemperature;
        private readonly double _endTemperature;
        private readonly int _maxIterations;
        private readonly double _alpha;

        public SimulatedAnnealing(double startTemperature, double endTemperature, int maxIterations, double alpha)
        {
            _startTemperature = startTemperature;
            _endTemperature = endTemperature;
            _maxIterations = maxIterations;
            _alpha = alpha;
        }

        public double Anneal(Func<double[], double> objectiveFunction, double[] initialSolution)
        {
            var currentSolution = initialSolution;
            var currentEnergy = objectiveFunction(currentSolution);
            var bestSolution = currentSolution;
            var bestEnergy = currentEnergy;

            for (var i = 0; i < _maxIterations; i++)
            {
                var temperature = Temperature(i);
                var newSolution = Perturb(currentSolution);
                var newEnergy = objectiveFunction(newSolution);
                var deltaEnergy = newEnergy - currentEnergy;

                if (deltaEnergy < 0 || Math.Exp(-deltaEnergy / temperature) > _random.NextDouble())
                {
                    currentSolution = newSolution;
                    currentEnergy = newEnergy;

                    if (newEnergy < bestEnergy)
                    {
                        bestSolution = newSolution;
                        bestEnergy = newEnergy;
                    }
                }
            }

            return bestEnergy;
        }

        private double Temperature(int iteration)
        {
            return _startTemperature * Math.Pow(_alpha, iteration);
        }

        private double[] Perturb(double[] solution)
        {
            var result = new double[solution.Length];
            Array.Copy(solution, result, solution.Length);

            var index = _random.Next(solution.Length);
            result[index] += (_random.NextDouble() * 2 - 1);

            return result;
        }
    }
}
