using Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit
{
    public class FindIntersects
    {
        //An attempt to solve using vectors with length (90degree only) and the intersects. Intersect points may be used to order. Let's try
        //A vector has a direction and length. In this case a Number, E.g. 3, has length 3. If that vector hits another number at tip(maxlength)
        //, Moves are possible in that direction. 


        private List<Move> _moves = new();
        private List<int> Hdirections = new List<int>();
        private List<int> Vdirections = new List<int>();

        public FindIntersects()
        {
            Hdirections.Add(1);//Right
            Hdirections.Add(-1);//Left
            Vdirections.Add(1);//Down
            Vdirections.Add(-1);//Up
        }
        //TODO For leftover, y=ax+b = 0
        //>> if 2 was found with neighbour 3 to the left
        //>> [3][1][2] Solve = 0
        //>> [2] = [3] +S*[1] =  0, Where S = Sign(+1 or -1)
        //>> S*[1] = [2] - [3]
        //>> S = -1
        //>> Update board

        //>> Store those leftover numbers somewhere. Determine size during calculation
        int leftOverNumberCount = 0;
        List<int> leftOverNumbers = new List<int>();

        //Lets define a starting point and 'look' at the next number
        public List<Move> FindEasyMoves(Map currentMap)
        {
            //Find the first number and second matching number. If found, great, if not, not so great.
            //But add some easy moves to cross off, but only if there is ONE exact match. Keep the rest for something else
            
            int firstNumber = 0;
            int hIntersect = 0;
            int vIntersect = 0;

            int[] coordinateFirstNumber = new int[2];
            
            int direction = 0;
            Map updatedMap = currentMap;
            bool isDone = false;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            //Lets find some exact matches..
            while (!isDone)
            {
                currentMap = updatedMap;
                //Both Start at ZERO. After a loop, no longer ZERO. This is to accumulate some easy moves
                //Start condition for every cycle
                bool numberFound = false;
                if (coordinateFirstNumber[0] + hIntersect > updatedMap.w|| coordinateFirstNumber[1] + vIntersect > updatedMap.h)
                {
                    isDone = true;
                }
                for (int x = coordinateFirstNumber[0]+hIntersect; x < currentMap.w; x++)
                {
                    for (int y = coordinateFirstNumber[1]+vIntersect; y < currentMap.h; y++)
                    {
                        if (currentMap.data[x, y] !=0 && firstNumber != currentMap.data[x, y])
                        {
                            coordinateFirstNumber[0] = x;
                            coordinateFirstNumber[1] = y;
                            firstNumber = currentMap.data[x, y];
                            numberFound = true;
                            break;
                        }                    
                    }
                    if (numberFound)
                    {
                        break;
                    }
                    //When end reached, no more intersects
                    isDone = true;
                }
                //First number found. Lets determine direction possibilities
                //Horizontal first
                //TODO Fix mistake
                foreach (var item in Hdirections)
                {                    
                    direction = item;
                    var coorValue = (coordinateFirstNumber[0] + (direction * firstNumber));
                    if (coorValue >= 0 && coordinateFirstNumber[0] + coorValue < currentMap.w)
                    {                        
                        hIntersect = coorValue;
                        //See if matching number exists, Then remove it from this map
                        if (currentMap.data[coordinateFirstNumber[0] + hIntersect, coordinateFirstNumber[1]] == firstNumber)
                        {
                            updatedMap.data[coordinateFirstNumber[0], coordinateFirstNumber[1]] = 0;
                            updatedMap.data[coordinateFirstNumber[0] + hIntersect, coordinateFirstNumber[1]] = 0;
                            _moves.Add(new Move(coordinateFirstNumber[0], coordinateFirstNumber[1], (Direction)1, false));
                            Console.WriteLine($"{coordinateFirstNumber[0]},{coordinateFirstNumber[1]} To {coordinateFirstNumber[0] + hIntersect},{coordinateFirstNumber[1]}");
                            break;
                        }                        
                    }                                     
                }
                
                //Vertical Second
                foreach (var item in Vdirections)
                {
                    direction = item;
                    var coorValue = (coordinateFirstNumber[1] + (direction * firstNumber));
                    if (coorValue>=0 && coordinateFirstNumber[1]+ coorValue < currentMap.h)
                    {                        
                        vIntersect = coorValue;
                        if (currentMap.data[coordinateFirstNumber[0], coordinateFirstNumber[1] + vIntersect] == firstNumber)
                        {
                            updatedMap.data[coordinateFirstNumber[0], coordinateFirstNumber[1]] = 0;
                            updatedMap.data[coordinateFirstNumber[0] + hIntersect, coordinateFirstNumber[1] + vIntersect] = 0;
                            _moves.Add(new Move(coordinateFirstNumber[0], coordinateFirstNumber[1], (Direction)0, false));
                            Console.WriteLine($"{coordinateFirstNumber[0]},{coordinateFirstNumber[1]} To {coordinateFirstNumber[0]},{coordinateFirstNumber[1] + vIntersect}");
                            break;
                        }                        
                    }                                       
                }
            }
            //Calculate the leftovers and try to throw them back in somehow TODO         
            updatedMap = CalculateRemainder(updatedMap);
            foreach (var item in _moves)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine($"{stopwatch.ElapsedTicks} Ticks. {stopwatch.ElapsedMilliseconds} MilliSeconds.");
            stopwatch.Stop();
            return _moves;
        }

        public Map CalculateRemainder(Map remainingMap)
        {
            List<int> xCoordinates = new List<int>();
            List<int> yCoordinates = new List<int>();

            //int maximum = 0;
            //int maximumIndex = 0;
            //int minimum = 0;
            //int minimumIndex = 0;
            int tmpCounter = 0;
            for (int x = 0; x < remainingMap.w; x++)
            {                
                for (int y = 0; y < remainingMap.h; y++)
                {                    
                    if (remainingMap.data[x, y] != 0)
                    {
                        xCoordinates.Add(x);
                        yCoordinates.Add(y);
                        leftOverNumberCount++;
                        leftOverNumbers.Add(remainingMap.data[x, y]);
                        Console.WriteLine($"{xCoordinates[tmpCounter]},{yCoordinates[tmpCounter]} Value {remainingMap.data[x, y]}");
                        tmpCounter++;
                    }
                }                
            }
            int calculatedValue = 0;
            int carryoverIndex = 0;
            //maximum = leftOverNumbers.Max();
            //maximumIndex = leftOverNumbers.IndexOf(maximum);
            //minimum = leftOverNumbers.Min();
            //minimumIndex = leftOverNumbers.IndexOf(minimum);

            //Look for things horizontal
            //TODO AND vertical
            bool stepFound = false;
            //Used to switch sign
            int sign = 1;
            //Used to give direction for move
            Direction hDirection = Direction.R;

            //Coordinates
            int step = 0;
            int xCoor = 0;
            int yCoor = 0;
            //TODO check for somekind of recursion/Brute force for the time being
            for (int i = 0; i < leftOverNumbers.Count(); i++)
            {
                step = sign * leftOverNumbers[i];
                xCoor = xCoordinates[i] + step;
                yCoor = yCoordinates[i];
                if (xCoor < 0 || xCoor > remainingMap.w-1)
                {
                    i++;
                }
                else
                {                    
                    if (remainingMap.data[xCoor, yCoor] != 0)
                    {
                        calculatedValue = Math.Abs(remainingMap.data[xCoor, yCoor] + sign * leftOverNumbers[i]);
                        stepFound = leftOverNumbers.Contains(calculatedValue);
                    }
                    if (stepFound)
                    {
                        remainingMap.data[xCoor, yCoor] = calculatedValue;
                        remainingMap.data[xCoordinates[i], yCoordinates[i]] = 0;                        
                        leftOverNumbers[Math.Abs(leftOverNumbers[i])] = calculatedValue;
                        leftOverNumbers[i] = 0;                        
                        _moves.Add(new Move(xCoordinates[i], yCoordinates[i], hDirection, sign == 1));
                        i++;
                    }
                    else
                    {
                        //Switch go Left or Right
                        if (sign == 1)
                        {
                            sign = -1;
                            hDirection = Direction.L;
                            i--;
                        }
                        else
                        {
                            sign = 1;
                            hDirection = Direction.R;
                            i--;
                        }
                    }                
                    carryoverIndex = leftOverNumbers.Count()-i;
                }
            }
            //TODO Fix adding the last one
            ////Switch go Left or Right            
            if (remainingMap.data[xCoordinates[carryoverIndex] + sign * leftOverNumbers[carryoverIndex], yCoordinates[carryoverIndex]] != 0)
            {
                calculatedValue = remainingMap.data[xCoordinates[carryoverIndex] + sign * leftOverNumbers[carryoverIndex], yCoordinates[carryoverIndex]] + sign * leftOverNumbers[carryoverIndex];
                stepFound = leftOverNumbers.Contains(calculatedValue) && remainingMap.data[xCoordinates[carryoverIndex] + sign * leftOverNumbers[carryoverIndex], yCoordinates[carryoverIndex]] != 0;
            }
            if (stepFound)
            {
                remainingMap.data[xCoordinates[carryoverIndex] + sign * leftOverNumbers[carryoverIndex], yCoordinates[carryoverIndex]] = calculatedValue;
                remainingMap.data[xCoordinates[carryoverIndex], yCoordinates[carryoverIndex]] = 0;                
                leftOverNumbers[Math.Abs(leftOverNumbers[carryoverIndex])] = calculatedValue;
                leftOverNumbers.RemoveAt(carryoverIndex);
                _moves.Add(new Move(xCoordinates[carryoverIndex], yCoordinates[carryoverIndex], hDirection, sign == 1));                                  
            }
            //Check where additions or substractions can take place on the map
            return remainingMap;
        }
    }
}
