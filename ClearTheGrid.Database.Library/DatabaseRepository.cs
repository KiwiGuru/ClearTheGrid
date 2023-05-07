using Microsoft.EntityFrameworkCore;
using ClearTheGrid.Model;
using ClearTheGrid.DAL;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Identity.Client;

namespace ClearTheGrid.Database.Library
{
    public class DatabaseRepository
    {      
        public LevelSolution? GetLevelSolution(int level)
        {
            using var ctx = new ClearTheGridDBContext();
            var solution = ctx.LevelSolutions
                .FirstOrDefault(x => x.Level == level);            
            return solution;
        }

        public List<LevelSolution> GetAllLevelSolutions()
        {
            using var ctx = new ClearTheGridDBContext();
            var allSolutions = ctx.LevelSolutions.ToList();
            return allSolutions;
        }

        public void SaveLevelSolution(LevelSolution levelSolution)
        {
            using var ctx = new ClearTheGridDBContext();
            var solution = ctx.LevelSolutions
                .Include(x => x.Moves)
                .FirstOrDefault(x => x.Level == levelSolution.Level);
            if (solution != null)
            {
                ctx.LevelSolutions.Remove(solution);
                ctx.SaveChanges();
            }                        
            ctx.LevelSolutions.Add(levelSolution);
            ctx.SaveChanges();            
        }

        public Settings? GetSettings(string name)
        {
            using var ctx = new ClearTheGridDBContext();
            var settings = ctx.Settings
                .FirstOrDefault(x => x.Name == name);
            return settings;
        }

        public List<Settings> GetAllSettings()
        {
            using var ctx = new ClearTheGridDBContext();
            var allSettings = ctx.Settings.ToList();
            return allSettings;
        }

        public void SaveSettings(string name, Settings newSettings)
        {
            var ctx = new ClearTheGridDBContext();
            var settings = ctx.Settings
                .FirstOrDefault(x => x.Name == name);
            if (settings != null)
            {
                settings.CrossoverFactor = newSettings.CrossoverFactor;
                settings.MutationFactor = newSettings.MutationFactor;
                settings.SelectionCount = newSettings.SelectionCount;
                settings.PopulationSize = newSettings.PopulationSize;
                settings.GenerationCount = newSettings.GenerationCount;
                ctx.Update(settings);
                ctx.SaveChanges();
            }
            else
            {                
                ctx.Settings.Attach(newSettings);
                ctx.SaveChanges();
            }            
        }        
    }
}