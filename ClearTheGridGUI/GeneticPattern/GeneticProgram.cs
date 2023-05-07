using Shared;
using ClearTheGrid.GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using ScottPlot.Statistics;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Net.WebSockets;

namespace ClearTheGrid.GUI.GeneticPattern
{
    public class GeneticProgram
    {
        private FindNumbers _findNumbers = new FindNumbers();
        private static readonly Random rnd = new Random();        

        /// <summary>
        /// Generate the initial population based on random startpoints on the map and randomized valid moves
        /// </summary>
        /// <param name="map"></param>
        /// <param name="PopulationSize"></param>
        /// <returns></returns>
        public (List<int[]> Population, List<NumberData>numberDatas) GeneratePopulation(Map map, int PopulationSize = 100)
        {
            //Extract the numbers from the map including coordinates. Needed to generate moves
            var numberDatas = _findNumbers.CountNumbers(map);

            //The order of numbers with arraysize of amount of found numbers in the map.
            //For shuffling later on
            int[] order = new int[numberDatas.Count];
            int[] numbers = new int[numberDatas.Count];

            //Initialize the populations' generation
            List<int[]> population = new List<int[]>();

            //Add simple array of indexes for use in ordering
            for (int i = 0; i < numberDatas.Count; i++)
            {
                order[i] = i;
            }

            //place randomized orders in list
            for (int i = 0; i < PopulationSize; i++)
            {
                //Shuffle each order
                order = order.OrderBy(x => rnd.Next()).ToArray();
                population.Add(order);
            }
            return (population, numberDatas);
        }

        //WE need to append a NEW moveset to the existing ones from the selection procedure. But we need an updated map as well inbetween to resume
        //from a certain point. Sort of backtracking. But in the end, the population is played on the ORIGINAL map. If we would not do that, we would lose variety!
        public List<Individual> GenerateNextGeneration(Map map, List<Individual> PreviousIndividuals,int PopulationSize = 100)
        {
            foreach (var individual in PreviousIndividuals)
            {
                var temp = PlayNewMovesOnMap(map, individual.MovesPlayed);
                var numberDatas = _findNumbers.CountNumbers(map);
                int[] order = new int[numberDatas.Count];
                for (int i = 0; i < order.Length; i++)
                {
                    order[i] = i;
                }
                var validMoves = GetAllValidMoves(map, order.OrderBy(x => rnd.Next()).ToArray(), numberDatas);
                order = new int[validMoves.Count];
                for (int i = 0; i < order.Length; i++)
                {
                    order[i] = i;
                }
                individual.MoveOrder = order.OrderBy(x => rnd.Next()).ToArray();
            }
            var countStart = PreviousIndividuals.Count;
            for (int i = countStart; i < PopulationSize; i++)
            {
                var individualRandomSelector = rnd.Next(PreviousIndividuals.Count);
                PreviousIndividuals.Add(new Individual()
                {
                    MoveOrder = PreviousIndividuals[individualRandomSelector].MoveOrder.OrderBy(x => rnd.Next()).ToArray(),
                    MovesPlayed = PreviousIndividuals[individualRandomSelector].MovesPlayed,
                    Score = 9999
                });
            }
            return PreviousIndividuals;
        }
        
        /// <summary>
        /// Play the next generation. This time, the individuals are already created
        /// </summary>
        /// <param name="map"></param>
        /// <param name="individuals"></param>
        /// <returns></returns>
        public List<Individual> PlayNextGeneration(Map map,List<Individual> individuals)
        {
            foreach (var individual in individuals)
            {                   
                var result = PlayNewMovesOnMap(map, individual.MovesPlayed);
                individual.Score = result.Score;
                individual.MovesPlayed = result.actualMoves;
                int[] order = new int[individual.MovesPlayed.Count];
                for (int i = 0; i < order.Length; i++)
                {
                    order[i] = i;
                }
                individual.MoveOrder = order;
            }
            return individuals;
        }

        public List<Individual> PlayInitialPopulation(Map map, List<int[]> population, List<NumberData> numberDatas)
        {
            //Initialize the valid move list in which valid moves per cycle are stored
            List<List<Move>> validMoves = new List<List<Move>>();
            List<Individual> individuals = new List<Individual>();
                      
            double[] scoreList = new double[population.Count()];  

            //Reset the best score
            int bestScore = 9999;

            //Get valid moves and play the population
            for (int i = 0; i < population.Count(); i++)
            {                
                validMoves.Add(GetAllValidMoves(map, population[i], numberDatas));
                var result = PlayMovesOnMap(map, validMoves[i]);
                int[] newOrder = new int[result.actualMoves.Count];
                for (int j = 0; j < newOrder.Length; j++)
                {
                    newOrder[j] = j;
                }
                individuals.Add(new Individual
                {
                    Score = result.Score,
                    MovesPlayed = result.actualMoves,
                    MoveOrder = newOrder
                });

                if (result.Score < bestScore)
                {
                    bestScore = result.Score;
                    scoreList[i] = bestScore;
                }
                else
                {
                    scoreList[i] = result.Score;
                }
                if (bestScore == 0)
                {
                    scoreList[i] = 0;
                    break;
                }
            }
            return (individuals);
        }

        //Based on the starter kit, Moves are generated and checked, and played
        //given, shuffled coordinates.
        static List<Move> GetAllValidMoves(Map map, int[] order, List<NumberData> numberDatas)
        {
            var result = new List<Move>();
            //Lets no longer try every cell but cycle the given populationentries instead!
            //This cycles through all populations containing random order of numbers        
            for (int i = 0; i < order.Count(); i++)
            {
                //For readability for now
                int x = numberDatas[order[i]].xCoor;
                int y = numberDatas[order[i]].yCoor;

                for (int d = 0; d < 4; d++)
                {
                    int s = rnd.Next(2);
                    var move = new Move(x, y, (Direction)d, s == 0);
                    if (IsValidMove(map, move))
                    {                        
                        result.Add(move);
                    }
                }            
            }            
            return result;
        }

        //Given a moveset based on population, Return the score!
        static (int Score, List<Move> actualMoves, Map alteredMap) PlayMovesOnMap(Map m, List<Move> moves)
        {
            //Collect the moves we are actually making this playout.
            var actualMoves = new List<Move>();

            //We don't wanna mess up the original map.. 
            //Indeed.....
            var localMap = CloneMap(m);

            //Initial score BEFORE moving new population
            int score;
            //try to "play" the provided moves
            //Added to return scores after applying moves for population cycling when best score has been reached
            foreach (var move in moves)
            {
                Move actualMove = move;

                //make sure this move is valid, otherwise, pick a random valid move..
                if (!IsValidMove(localMap, actualMove))
                {
                    //This time, the next set of moves comes from the randomized moves from 
                    //the randomized numbers. That is the reason for the break in this location.
                    score = GetMapScore(localMap);
                }
                else
                {
                    //Execute the move on our map
                    localMap.ApplyMove(actualMove);

                    //Save a cloned version of the actualMove in our list. 
                    //They are also required for output for solution files 
                    actualMoves.Add(actualMove.Clone());
                }
            }
            score = GetMapScore(localMap);
            return (score, actualMoves, localMap);
        }
        //This also gives a moveset based on population but also checks for new possibilities
        private (int Score, List<Move> actualMoves, Map alteredMap) PlayNewMovesOnMap(Map m, List<Move> moves)
        {
            //Collect the moves we are actually making this playout.
            var actualMoves = new List<Move>();

            //We don't wanna mess up the original map.. 
            //Indeed.....
            var localMap = CloneMap(m);

            //Initial score BEFORE moving new population
            int score;
            var numberDatas = _findNumbers.CountNumbers(localMap);
            //try to "play" the provided moves
            //Added to return scores after applying moves for population cycling when best score has been reached
            foreach (var move in moves)
            {
                Move actualMove = move;
                numberDatas = _findNumbers.CountNumbers(localMap);

                //make sure this move is valid, otherwise, pick a random valid move..
                if (IsValidMove(localMap, actualMove))
                {
                    //Execute the move on our map
                    localMap.ApplyMove(actualMove);                    
                    actualMoves.Add(actualMove.Clone());
                    score = GetMapScore(localMap);
                }
            }
            int[] newOrder = new int[numberDatas.Count];
            for (int i = 0; i < newOrder.Length; i++)
            {
                newOrder[i] = i;
            }
            var newMoves = GetAllValidMoves(localMap, newOrder, numberDatas);
            score = GetMapScore(localMap);
            foreach (var move in newMoves)
            {
                actualMoves.Add(move);
            }
            return (score, actualMoves, localMap);
        }

        /// <summary>
        /// Check if the Move is a valid move on the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        static bool IsValidMove(Map map, Move move)
        {
            //TODO Better Idea! First check if the number operation LOWERS the sum of all numbers (of the original board in TOTAL). The goal is to reach Sum = ZERO
            //In the ideal world, each move lowers the sum of all numbers. Worst case, it does not
            int sourcevalue = map.data[move.x, move.y];
            if (sourcevalue == 0) return false;

            var dircoords = move.dir.AsCoords();
            int dx = move.x + dircoords.xstep * sourcevalue;
            int dy = move.y + dircoords.ystep * sourcevalue;

            if (dx >= 0 && dy >= 0 && dx < map.w && dy < map.h)
            {                
                int destvalue = map.data[dx, dy];
                if (destvalue == 0) return false;
                
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Score the map based on numbers left
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        static int GetMapScore(Map map)
        {
            int result = 0;
            //we count the number of cells left on the map..
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    if (map.data[x, y] > 0) result++;
                }
            }
            return result;
        }

        //Determine the normalized fitness of the INDIVIDUAL. Total must be 100% (or 1)
        //E.g., does the move lower the total, and by how much. Which are the best possible moves?
        //TODO discard the worst?!
        public List<Individual> GetMoveFitness(List<Individual> individuals, List<NumberData> numberDatas, Map map)
        {
            int[] numbers = new int[numberDatas.Count];

            foreach (var individual in individuals)
            {             
                for (int i = 0; i < numberDatas.Count; i++)
                {
                    numbers[i] = numberDatas[i].value;
                }
                //calculate sum of numbers on this map
                int sum = numbers.Aggregate((a, b) => a + b);

                //The value to compare it to??????
                int sourceValue = 0;
                int direction = 0;
                int destinationValue = 0;
                int sign = 0;
                int resultBoardSum = 0;
                double[] fitness = new double[individual.MovesPlayed.Count];

                for (int x = 0; x < individual.MovesPlayed.Count; x++)
                {
                    if (IsValidMove(map, individual.MovesPlayed[x]))
                    {
                        //Calculate for direction. Negative for left, and up on the map and vice versa                    
                        if (individual.MovesPlayed[x].add)
                        {
                            sign = 1;
                        }
                        else
                        {
                            sign = -1;
                        }

                        if (individual.MovesPlayed[x].dir == Direction.L || individual.MovesPlayed[x].dir == Direction.U)
                        {
                            direction = -1;
                        }
                        else if (individual.MovesPlayed[x].dir == Direction.R || individual.MovesPlayed[x].dir == Direction.D)
                        {
                            direction = 1;
                        }

                        if (individual.MovesPlayed[x].dir == Direction.L || individual.MovesPlayed[x].dir == Direction.R)
                        {
                            sourceValue = map.data[individual.MovesPlayed[x].x, individual.MovesPlayed[x].y];
                            destinationValue = map.data[individual.MovesPlayed[x].x + (direction * sourceValue), individual.MovesPlayed[x].y];
                        }
                        else if (individual.MovesPlayed[x].dir == Direction.U || individual.MovesPlayed[x].dir == Direction.D)
                        {
                            sourceValue = map.data[individual.MovesPlayed[x].x, individual.MovesPlayed[x].y];
                            destinationValue = map.data[individual.MovesPlayed[x].x, individual.MovesPlayed[x].y + (direction * sourceValue)];
                        }
                        // if a square is eliminated, this is positive for fitness
                        bool elimination = destinationValue + (sign * sourceValue) == 0;
                        //if the total of all numbers decreased or stays the same. If not, probably less fit move
                        resultBoardSum = sum + (sign * sourceValue) - sourceValue;
                        if (elimination)
                        {
                            fitness[x] = (double)(sum - resultBoardSum) / sum;
                        }
                        else
                        {
                            fitness[x] = (double)(sum - resultBoardSum) / sum;
                        }
                        individual.MoveFitness = fitness;
                    }                    
                }
            }
            return individuals;
        }

        static Map CloneMap(Map source)
        {
            var result = new Map(source.w, source.h);
            result.data = (int[,])source.data.Clone();
            return result;
        }
    }
}
