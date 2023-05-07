using ClearTheGrid.GUI.GeneticPattern;
using ClearTheGrid.GUI.Model;
using ClearTheGrid.Database.Library;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ClearTheGrid.Model;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Threading;

namespace ClearTheGrid.GUI.Handlers
{
    public class SystemControlHandler
    {
        private static DatabaseRepository DatabaseRepository = new();
        private static FileHandler FileHandler = new();

        public Settings? GetSettings(string name = "BaseSettings")
        {
            return DatabaseRepository.GetSettings(name);
        }

        public List<Settings> GetAllSettings()
        {
            return DatabaseRepository.GetAllSettings();
        }

        public void SaveSettings(Settings settings)
        {
            DatabaseRepository.SaveSettings(settings.Name, settings);
        }

        /// <summary>
        /// Saves the level solution to database. Please note that the 'Move' object is from the original starterkit
        /// </summary>
        /// <param name="level"></param>
        /// <param name="levelSolution"></param>
        public void SaveLevelSolution(int level, List<Move> moves)
        {
            //This is not the nicest, but I wanted to keep the original Move object intact
            LevelSolution levelSolution = new LevelSolution() { Level = level };
            for (int i = 0; i < moves.Count; i++)
            {
                var oldMove = moves[i].ToString();
                levelSolution.Moves.Add(new ResultMove() { Move = oldMove, Rank = i });
            }
            DatabaseRepository.SaveLevelSolution(levelSolution);
            //And export right away
            ExportSolution(levelSolution);
        }

        public LevelSolution GetLevelSolution(int level)
        {
            return DatabaseRepository.GetLevelSolution(level);
        }

        public void ExportSolution(LevelSolution levelSolution)
        {
            FileHandler.WriteSolution(levelSolution.Level.ToString(), levelSolution);
        }
    }
}
