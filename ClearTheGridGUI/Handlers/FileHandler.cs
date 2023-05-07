using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ClearTheGrid.GUI.Model;
using ClearTheGrid.Model;

namespace ClearTheGrid.GUI.Handlers
{
    public class FileHandler
    {        
        private static string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static string _levels = "Solutions/";
        private static string _extension = ".sol";

        public static void WriteSolution(string fileName, LevelSolution levelSolution)
        {
            string path = $@"{_basePath}\{_levels}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var resultMoves = levelSolution.Moves.OrderBy(x => x.Rank);
            string fullPath = $"{path}{fileName}{_extension}";
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            foreach (var item in resultMoves)
            {
                File.AppendAllText(fullPath, $"{item.Move}\n");
            }
        }
    }
}
