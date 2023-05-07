using ClearTheGrid.GUI.Model;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearTheGrid.GUI.GeneticPattern
{
    /// <summary>
    /// This class is meant to select the best results, crossbreed those, and send them back for further processing
    /// The amount of good candidates to breed with
    /// </summary>
    public class SelectionAndBreeding
    {
        /// <summary>
        /// An empty and invalid move, we use it as template to force a 'random' move.
        /// This is from the starterkit and will be used to increase variety in the population
        /// </summary>
        static Move emptyMove = new Move(0, 0, Direction.U, false);
        private static readonly Random rnd = new Random();

        /// <summary>
        /// The selection process
        /// </summary>
        /// <param name="population">The population containing the individuals and their moveset</param>
        /// <param name="breedCount">The amount of the best selected individuals based on score</param>
        /// <returns></returns>
        public List<Individual> SelectionCrossoverAndMutatingProcedure(List<Individual> population, int breedCount, double crossoverFactor, double mutationFactor)
        {
            //TODO TEST THIS
            //First, remove unfit moves. Maybe all of them, maybe some
            //population = RemoveUnfitMoves(population);
            //List of selected individuals
            List<Individual> mutatedIndividuals = new List<Individual>();
            //Select the best for breeding.
            var selectedIndividuals = population.Where(x => x.MovesPlayed.Count > 0).OrderBy(x => x.Score).Take(breedCount).ToList();
            //Shift things up a bit, take some from the middle of the sorted list
            var selectedOriginalIndividuals = population.OrderBy(x => x.Score).Skip(population.Count/2).Take(breedCount-breedCount/2).ToList();

            //Determine score occurences for plateau detection           
            var scoreOccurenceCount = selectedIndividuals.Where(x => x.Score == selectedIndividuals.First().Score).Count();

            //Soft reset when plateau is reaced
            if (scoreOccurenceCount >= breedCount)
            {                
                selectedIndividuals = population.Take(breedCount).ToList();
            }

            mutatedIndividuals = CrossAndMutateIndividuals(selectedIndividuals, selectedOriginalIndividuals, crossoverFactor, mutationFactor);
            return mutatedIndividuals;
        }

        /// <summary>
        /// Crosses over the selected best individuals based on chance
        /// </summary>
        /// <param name="selectedIndividuals">The selected best individuals</param>
        /// <returns>Returns list of crossed and posibly mutated individuals based on the best individuals</returns>
        private List<Individual> CrossAndMutateIndividuals(List<Individual> selectedIndividuals, List<Individual> selectedOriginalIndividuals, double crossoverFactor, double mutationFactor)
        {
            //Crossover Time!
            var crossedIndividuals = CrossIndividuals(selectedIndividuals, selectedOriginalIndividuals, crossoverFactor);
            //Mutating Time!
            var crossedAndMutatedIndividuals = MutateIndividuals(crossedIndividuals, mutationFactor);            
            return crossedAndMutatedIndividuals;
        }

        /// <summary>
        /// Crosses over moves from the individuals
        /// </summary>
        /// <param name="selectedIndividuals">A list of best individuals</param>
        /// <param name="selectedOriginalIndividuals">A list of random individuals</param>
        /// <returns></returns>
        private List<Individual> CrossIndividuals(List<Individual> selectedIndividuals,List<Individual> selectedOriginalIndividuals, double crossoverFactor)
        {
            //Probability of getting a change in the moves from another random selected individual from the Selectedforbreeding Pool
            foreach (var individual in selectedIndividuals)
            {                
                double randomDoubleCrossover = rnd.NextDouble();
                if (randomDoubleCrossover <= crossoverFactor)
                {
                    if (individual.MovesPlayed.Count() > 0)
                    {
                        var originalIndividual = selectedOriginalIndividuals[rnd.Next(selectedOriginalIndividuals.Count)];
                        var originalMoveNumber = rnd.Next(originalIndividual.MovesPlayed.Count);
                        var moveNumberToReplace = rnd.Next(individual.MovesPlayed.Count);
                        if (originalIndividual.MovesPlayed.Count > 0)
                        {
                            individual.MovesPlayed[moveNumberToReplace] = originalIndividual.MovesPlayed[originalMoveNumber];
                        }                        
                    }                   
                }
            }            
            return selectedIndividuals;
        }

        private List<Individual> MutateIndividuals(List<Individual> individuals, double mutationFactor)
        {
            //Each individual has chance of mutation
            foreach (var individual in individuals)
            {
                double randomDoubleMutation = rnd.NextDouble();
                if (randomDoubleMutation <= mutationFactor)
                {                    
                    if (individual.MovesPlayed.Count() > 0)
                    {
                        //Replace with an emtpy move stated on top
                        individual.MovesPlayed[rnd.Next(individual.MovesPlayed.Count)] = emptyMove;
                    }
                }
            }            
            return individuals;
        }

        private List<Individual> RemoveUnfitMoves(List<Individual> individuals)
        {
            //TODO Calculate a dynamic fitness threshold for moves to be removed. Not enough are being removed I think
            foreach (var individual in individuals)
            {
                if (individual.MovesPlayed.Count != 0)
                {
                    var unfitMovesToRemove = individual.MoveFitness.Select((number, index) => new { number, index })
                    .Where(pair => pair.number == 0)
                    .Select(pair => pair.index)
                    .ToList();
                    unfitMovesToRemove.Sort();  
                    for (int i = unfitMovesToRemove.Count-1; i >= 0; i--)
                    {
                        individual.MovesPlayed.RemoveAt(unfitMovesToRemove[i]);
                    }
                    var fitMovesToKeep = individual.MoveFitness.Select((number, index) => new { number, index })
                    .Where(pair => pair.number > 0)
                    .Select(pair => pair.index)
                    .ToList();
                    double[] newMoveFitness = new double[fitMovesToKeep.Count];
                    for (int i = 0; i < newMoveFitness.Length; i++)
                    {
                        newMoveFitness[i] = individual.MoveFitness[fitMovesToKeep[i]];
                    }
                    individual.MoveFitness = newMoveFitness;
                    int[] order = new int[individual.MovesPlayed.Count];
                    for (int i = 0; i < order.Length; i++)
                    {
                        order[i] = i;
                    }
                    individual.MoveOrder = order;
                }  
            }
            return individuals;
        }
    }
}