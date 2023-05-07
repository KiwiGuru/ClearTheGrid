using ClearTheGrid.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClearTheGrid.DAL
{
    public class ClearTheGridDBContext : DbContext
    {   
        public ClearTheGridDBContext() : base()
        {
            
        }

        public DbSet<Settings> Settings { get; set; }
        public DbSet<LevelSolution> LevelSolutions { get; set; }
        public DbSet<ResultMove> ResultMoves { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //OBVIOUSELY the automatic trust of a certificate is not best practise!            
            optionsBuilder.UseSqlServer(@"Data Source=localhost\SqlExpress01;Trusted_Connection=True;TrustServerCertificate=True;Initial Catalog=ClearTheGridDB;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            

            //Some base settings when building the database anew
            builder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = new Guid("a829abb3-b012-4597-a2a2-15374a36cd8c"),
                    Name = "BaseSettings",
                    GenerationCount = 100000,
                    SelectionCount = 12,
                    PopulationSize = 120,
                    CrossoverFactor = 0.08,
                    MutationFactor = 0.09
                });
            builder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = new Guid("50ce0b7b-3e00-4991-b2d6-e5a7905ed8e1"),
                    Name = "AgressiveSettings",
                    GenerationCount = 100000,
                    SelectionCount = 12,
                    PopulationSize = 120,
                    CrossoverFactor = 0.12,
                    MutationFactor = 0.16
                });
            builder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = new Guid("222525bf-ba39-404f-acaf-2047a43f831d"),
                    Name = "ExtraAgressiveSettings",
                    GenerationCount = 100000,
                    SelectionCount = 12,
                    PopulationSize = 120,
                    CrossoverFactor = 0.2,
                    MutationFactor = 0.24
                });
            builder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = new Guid("edae7cf4-e5b6-4dbc-a404-be8c80a7e80c"),
                    Name = "MediumSettings",
                    GenerationCount = 100000,
                    SelectionCount = 12,
                    PopulationSize = 120,
                    CrossoverFactor = 0.1,
                    MutationFactor = 0.12
                });
            builder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = new Guid("f7b994bd-8d07-4eb3-904e-d2fe63558968"),
                    Name = "ExtremeSettings",
                    GenerationCount = 100000,
                    SelectionCount = 12,
                    PopulationSize = 120,
                    CrossoverFactor = 0.35,
                    MutationFactor = 0.30
                });            
        }
    }
}
