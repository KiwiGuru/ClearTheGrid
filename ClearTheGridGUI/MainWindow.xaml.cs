using System;
using ScottPlot;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using ClearTheGrid.GUI.GeneticPattern;
using ClearTheGrid.GUI.Model;
using Microsoft.VisualBasic;
using StarterKit.Promising.GeneticPattern;
using System.Text.RegularExpressions;
using ScottPlot.Drawing.Colormaps;
using System.Collections.Immutable;
using System.Diagnostics;
using ScottPlot.Renderable;
using ClearTheGrid.GUI.Handlers;

namespace ClearTheGrid.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private static GeneticProgram geneticProgram = new();
        private static SelectionAndBreeding breeding = new();
        private static SystemControlHandler systemControlHandler = new();
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32", SetLastError = true)]
        public static extern void FreeConsole();

        static string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        static string _levels = "Levels/";

        //For the input fields
        static Regex regexintegers = new Regex("^[0-9]+");
        static Regex regexdoubles = new Regex("^[.0-9]+");

        private bool isCanceled = false;
        private bool isPaused = false;
        private bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
            PopulateSettingsInterface();
        }
        
        /// <summary>
        /// The main loop for the solver
        /// </summary>
        /// <param name="levelRange"></param>
        /// <param name="populationSize"></param>
        /// <param name="breedCount"></param>
        /// <param name="generationCount"></param>
        /// <param name="crossoverFactor"></param>
        /// <param name="mutationFactor"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        private Task StartSolving(int levelRange = 1, 
            int populationSize = 250, 
            int breedCount = 5, 
            int generationCount = 10, 
            double crossoverFactor = 0.05, 
            double mutationFactor = 0.05,
            int maxLevel = 99
            )
        {
            isRunning = true;
            string LevelPath = $"{_basePath}{_levels}";
            string levelSubFolder = String.Empty;
            double worstScore = 0;
            double bestScore;

            double[] scoreList = new double[populationSize];
            Stopwatch watch = new Stopwatch();
            for (int level = levelRange; level < maxLevel + 1; level++)
            {
                bestScore = 9999;
                watch.Restart();
                ClearGraph();
                Console.WriteLine($"Loading level {level}");

                //Interpret level subfolder, TODO this looks dumb
                if (level.ToString().Length == 3)
                {
                    levelSubFolder = $"{level.ToString()[0]}{"XX"}";
                }
                else
                {
                    levelSubFolder = "0XX";
                }

                //load a level map
                var currentMap = LoadLevel(System.IO.Path.Join(LevelPath, levelSubFolder, $"{level}.txt"));

                List<Individual> result = new List<Individual>();
                //Start timer
                watch.Start();
                //Generate first Genration
                var generatedPopulationData = geneticProgram.GeneratePopulation(currentMap, populationSize);
                //Run the initial first generation
                result = geneticProgram.PlayInitialPopulation(currentMap, generatedPopulationData.Population, generatedPopulationData.numberDatas);

                //Check the Fitness of the movement!
                //Loop here for X amount of generations
                for (int i = 0; i < generationCount; i++)
                {
                    Dispatcher.Invoke(() =>
                    {
                        lbGenerationCounter.Content = (i + 1);
                        lbSimulationTime.Content = watch.Elapsed;
                    });

                    if (result.Exists(x => x.Score == 0))
                    {
                        watch.Stop();
                        Individual finalIndividual = result.Where(x => x.Score == 0).FirstOrDefault();
                        //All done! solution found
                        //Save to Database!
                        systemControlHandler.SaveLevelSolution(level, finalIndividual.MovesPlayed);

                        string messageString = String.Empty;
                        for (int k = 0; k < finalIndividual.MovesPlayed.Count(); k++)
                        {
                            messageString += '\n';
                            messageString += finalIndividual.MovesPlayed[k];
                        }
                        Dispatcher.Invoke(() =>
                        {
                            lbGenerationCounter.Content = (i);
                            lbSimulationTime.Content = watch.Elapsed;
                        });
                        MessageBox.Show($"Solution Found in Generation {i}, With these moves {messageString}");
                        break;
                    }

                    //Prepare and run the next generation and breed them!                
                    result = breeding.SelectionCrossoverAndMutatingProcedure(result, breedCount, crossoverFactor, mutationFactor);
                    var nextGeneration = geneticProgram.GenerateNextGeneration(currentMap, result, populationSize);
                    result = geneticProgram.PlayNextGeneration(currentMap, nextGeneration);
                    //Check the Fitness of the movement!                    
                    //result = geneticProgram.GetMoveFitness(result, generatedPopulationData.numberDatas, currentMap);

                    for (int j = 0; j < result.Count; j++)
                    {
                        scoreList[j] = result[j].Score;
                    }                    

                    worstScore = scoreList.Max();
                    populationSize = scoreList.Count();                    
                    scoreList = scoreList.OrderByDescending(x=>x).ToArray();
                    if (scoreList.Min()< bestScore)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpdateTotalGraph(scoreList, worstScore + 2, populationSize);
                        });
                        bestScore = scoreList.Min();
                    }
                    if (isPaused)
                    {
                        while (isPaused)
                        {
                            Task.Delay(100);
                        }
                    }
                    if (isCanceled)
                    {
                        break;
                    }
                }
                watch.Stop();
                scoreList = scoreList.OrderByDescending(x => x).ToArray();
                worstScore = scoreList.Max();
                Dispatcher.Invoke(() =>
                {
                    UpdateTotalGraph(scoreList, worstScore + 2, populationSize);
                });
                populationSize = scoreList.Count();                
            }
            watch.Reset();
            MessageBox.Show("Run Completed!");
            Dispatcher.Invoke(() =>
            {
                btnStartRun.IsEnabled = true;
            });
            isRunning = false;
            return Task.CompletedTask;
        }        

        static Map LoadLevel(string filename)
        {
            var levelLines = System.IO.File.ReadAllLines(filename);
            Map m = Map.Parse(levelLines);
            return m;
        }

        public void UpdateTotalGraph(double[] dataY, double yAxisLimit, double xAxisLimit)
        {
            double LowerBoundry = 0.5;
            GraphTotal.Plot.Clear();
            GraphTotal.Background = Brushes.Black;
            GraphTotal.Plot.AddBar(dataY, System.Drawing.Color.DarkSlateGray);            
            GraphTotal.Plot.AddHorizontalLine(dataY.Min(), System.Drawing.Color.YellowGreen,6);
            GraphTotal.Plot.SetAxisLimitsY(LowerBoundry, yAxisLimit);
            GraphTotal.Plot.SetAxisLimitsX(0, xAxisLimit);
            GraphTotal.Plot.XLabel("Individual Number");
            GraphTotal.Plot.YLabel("Score");            
            GraphTotal.Plot.Style(ScottPlot.Style.Blue2);
            GraphTotal.Refresh();
        }

        public void ClearGraph()
        {            
            GraphTotal.Plot.Clear();
        }

        /// <summary>
        /// To populate the interface with available settings from the database.
        /// </summary>
        /// <returns></returns>
        public Task PopulateSettingsInterface()
        {
            var settings = systemControlHandler.GetAllSettings();
            foreach (var item in settings)
            {
                cbbSettingsSelection.Items.Add(item.Name);
            }
            cbbSettingsSelection.SelectedIndex = 0;
            var selectedSettings = systemControlHandler.GetSettings(cbbSettingsSelection.SelectedItem.ToString());
            tbCrossoverEntry.Text = selectedSettings.CrossoverFactor.ToString();
            tbGenerationCountEntry.Text = selectedSettings.GenerationCount.ToString();
            tbBreedSelectionCountEntry.Text = selectedSettings.SelectionCount.ToString();
            tbMutationEntry.Text = selectedSettings.MutationFactor.ToString();
            tbPopulationCountEntry.Text = selectedSettings.PopulationSize.ToString();
            return Task.CompletedTask;
        }

        private void btnStartRun_Click(object sender, RoutedEventArgs e)
        {
            btnStartRun.IsEnabled = false;
            isCanceled = false;
            if (ValidateLevelEntry() && ValidateSettings())
            {
                var level = int.Parse(tbLevelEntry.Text.ToString());
                var maxLevel = int.Parse(tbLevelEntryMax.Text.ToString());
                var settings = systemControlHandler.GetSettings(cbbSettingsSelection.SelectedItem.ToString());
                ClearGraph();
                Dispatcher.Invoke(() =>
                {
                    Task.Run(() => StartSolving(level, settings.PopulationSize, settings.SelectionCount, settings.GenerationCount, settings.CrossoverFactor, settings.MutationFactor, maxLevel));
                });
            }     
        }

        private void btnStopRun_Click(object sender, RoutedEventArgs e)
        {
            isCanceled = true;
        }

        private void tbLevelEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regexintegers.IsMatch(e.Text);
        }

        private void tbLevelEntryMax_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regexintegers.IsMatch(e.Text);
        }

        private void tbPopulationCountEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regexintegers.IsMatch(e.Text);
        }

        private void tbBreedSelectionCountEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regexintegers.IsMatch(e.Text);
        }

        private void tbGenerationCountEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regexintegers.IsMatch(e.Text);
        }

        private void tbMutationEntry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regexdoubles.IsMatch(e.Text);
        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateSettings())
            {
                ClearTheGrid.Model.Settings settings = new ClearTheGrid.Model.Settings()
                {
                    Name = cbbSettingsSelection.SelectedItem.ToString(),
                    CrossoverFactor = double.Parse(tbCrossoverEntry.Text.ToString()),
                    GenerationCount = int.Parse(tbGenerationCountEntry.Text.ToString()),
                    SelectionCount = int.Parse(tbBreedSelectionCountEntry.Text.ToString()),
                    MutationFactor = double.Parse(tbMutationEntry.Text.ToString()),
                    PopulationSize = int.Parse(tbPopulationCountEntry.Text.ToString())
                };
                systemControlHandler.SaveSettings(settings);
            }            
        }

        private void cbbSettingsSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSettings = systemControlHandler.GetSettings(cbbSettingsSelection.SelectedItem.ToString());
            tbCrossoverEntry.Text = selectedSettings.CrossoverFactor.ToString();
            tbGenerationCountEntry.Text = selectedSettings.GenerationCount.ToString();
            tbBreedSelectionCountEntry.Text = selectedSettings.SelectionCount.ToString();
            tbMutationEntry.Text = selectedSettings.MutationFactor.ToString();
            tbPopulationCountEntry.Text = selectedSettings.PopulationSize.ToString();
        }

        private void btnPauseRun_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                if (isPaused)
                {
                    isPaused = false;
                    btnStopRun.IsEnabled = true;
                }
                else
                {
                    isPaused = true;
                    btnStopRun.IsEnabled = false;
                }
            }            
        }

        /// <summary>
        /// Quickly check if the settings entered are valid
        /// </summary>
        /// <returns></returns>
        private bool ValidateSettings()
        {
            if (!string.IsNullOrEmpty(tbGenerationCountEntry.Text) || !string.IsNullOrEmpty(tbPopulationCountEntry.Text) || !string.IsNullOrEmpty(tbBreedSelectionCountEntry.Text) || !string.IsNullOrEmpty(tbMutationEntry.Text) || !string.IsNullOrEmpty(tbCrossoverEntry.Text))
            {
                try
                {
                    int generationCount = int.Parse(tbGenerationCountEntry.Text);
                    int populationCount = int.Parse(tbPopulationCountEntry.Text);
                    int breedSelection = int.Parse(tbBreedSelectionCountEntry.Text);
                    double mutationFactor = double.Parse(tbMutationEntry.Text);
                    double crossoverFactor = double.Parse(tbCrossoverEntry.Text);

                    if (breedSelection > populationCount || generationCount < 1 || mutationFactor >= 1 || crossoverFactor >= 1)
                    {
                        MessageBox.Show("Input parameters incorrect");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Input parameters incorrect");
                    return false;
                }  
            }
            else
            {
                MessageBox.Show("Input parameters incorrect");
                return false;
            }
        }

        /// <summary>
        /// Check if levelrange entered is valid
        /// </summary>
        /// <returns></returns>
        private bool ValidateLevelEntry()
        {
            try
            {
                int levelEntry = int.Parse(tbLevelEntry.Text);
                int levelMax = int.Parse(tbLevelEntryMax.Text);
                int diff = levelEntry - levelMax;
                if (diff < 0 || levelMax < levelEntry)
                {
                    MessageBox.Show("Input parameters incorrect");
                    return false;
                }
                else
                {                    
                    return true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Input parameters incorrect");
                return false;
            }
            
        }
    }
}