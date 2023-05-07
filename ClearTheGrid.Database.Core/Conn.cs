//using DNACruiserIII.Database.Library;
//using DNACruiserIII.Model;
//using DNACruiserIII.Model.XMLModel;
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using XML.Library;

//namespace DNACruiserIII.Database.Core
//{
//    /// <summary>
//    /// Module for updating database to update HMI plate tracking data and tracking liquids.
//    /// </summary>
//    [ComVisible(true),
//    Guid("5bccc9e8-50fe-4d2c-86f3-ae50c9448f56"),
//    ClassInterface(ClassInterfaceType.None)]
//    public class Conn : IDatabaseRepository
//    {
//        private static readonly DatabaseRepository _databaseRepository = new DatabaseRepository();
//        private static readonly XMLFacade _xmlFacade = new XMLFacade();        

//        public async Task<List<PlateSet>> GetPlateSets()
//        {
//            return await _databaseRepository.GetPlateSets();
//        }

//        public async Task<int> SetPlateSetsAsync(PlateSet plateSet)
//        {
//            return await _databaseRepository.SetPlateSetsAsync(plateSet);
//        }

//        public async Task<ConsumableUseCount> GetConsumableCount(string name)
//        {
//            return await _databaseRepository.GetConsumableCount(name);
//        }

//        public async Task<int> SetConsumableCount(string name, int count)
//        {
//            return await _databaseRepository.SetConsumableCount(name, count);
//        }

//        public async Task<TipStatus> GetTipStatus(string moduleName)
//        {
//            return await _databaseRepository.GetTipStatus(moduleName);
//        }

//        public async Task<int> UpdateTipStatus(string tipModule, int status, int rackNr)
//        {
//            return await _databaseRepository.UpdateTipStatus(tipModule, status, rackNr);
//        }

//        public async Task<string> GetPlateTrackingBarcode(string name)
//        {
//            var plateTrackingData = await _databaseRepository.GetPlateTrackingDataByName(name);
//            return plateTrackingData.Barcode;
//        }

//        public async Task<string> GetPlateTrackingArriveTime(string name)
//        {
//            var plateTrackingData = await _databaseRepository.GetPlateTrackingDataByName(name);
//            return plateTrackingData.ArriveTime;
//        }

//        public async Task<TrackingLiquid> GetTrackingLiquid(string name)
//        {
//            return await _databaseRepository.GetTrackingLiquid(name);
//        }

//        public async Task<int> UpdatePlateTrackingObject(PlateTrackingData dataToUpdate, int status)
//        {                
//            return await _databaseRepository.UpdatePlateTrackingObject(dataToUpdate, status);            
//        }

//        public async Task<int> UpdatePlateTracking(string name, string barcode, int status, string action, string currentTime, bool sourcePlate)
//        {
//            return await _databaseRepository.UpdatePlateTracking(name, barcode, status, action, currentTime, sourcePlate);
//        }

//        public async Task<PlateTrackingData> GetPlateTrackingDataByName(string name)
//        {            
//            return await _databaseRepository.GetPlateTrackingDataByName(name);
//        }

//        public async Task<int> UpdateTrackingLiquid(TrackingLiquid trackingLiquid)
//        {
//            return await _databaseRepository.UpdateTrackingLiquid(trackingLiquid);
//        }

//        public async Task<SystemConfiguration> GetSystemConfiguration(string name)
//        {
//            return await _databaseRepository.GetSystemConfiguration(name);
//        }

//        public async Task<SystemSettings> GetSystemSettings(string name)
//        {
//            return await _databaseRepository.GetSettings(name);
//        }

//        public async Task<SourcePlate> GetPlateInfoSource(string plateId)
//        {
//            return await _databaseRepository.GetPlateInfoSource(plateId);
//        }

//        public async Task<Analysis> GetAnalysisIdFromSource(SourcePlate plateInfo)
//        {
//            return await _databaseRepository.GetAnalysisIdFromSource(plateInfo);
//        }

//        public async Task<AnalysisGroup> GetAnalysisGroupFromAnalysis(Analysis analysis)
//        {
//            return await _databaseRepository.GetAnalysisGroupFromAnalysis(analysis);
//        }

//        public async Task<LabOrder> GetLabOrderFromAnalysisGroup(AnalysisGroup analysisGroup)
//        {
//            return await _databaseRepository.GetLabOrderFromAnalysisGroup(analysisGroup);
//        }

//        public async Task<string> GetRecipeNameFromLabOrder(LabOrder labOrder)
//        {
//            return await _databaseRepository.GetRecipeNameFromLabOrder(labOrder);
//        }

//        public async Task<DestinationPlate> GetPlateInfoDestinationFromAnalysisId(Analysis analysis)
//        {
//            return await _databaseRepository.GetPlateInfoDestinationFromAnalysisId(analysis);
//        }

//        public async Task<List<SourcePlate>> GetAllSourcePlatesFromDestinationPlateId(string plateId)
//        {
//            return await _databaseRepository.GetAllSourcePlatesFromDestinationPlateId(plateId);
//        }

//        public async Task<SourcePlate> GetSourceFromAnalysisId(Guid analysis)
//        {
//            return await _databaseRepository.GetSourceFromAnalysisId(analysis);
//        }

//        public async void ImportBatch(string fileFullName)
//        {
//            if (string.IsNullOrWhiteSpace(fileFullName) || !File.Exists(fileFullName))
//            {
//                return;
//            }

//            var batch = _xmlFacade.GetDataFromFile<Batch>(fileFullName);
//            if(GetBatch(batch.BatchName) == null)
//            {
//                await _databaseRepository.ImportBatch(batch);
//            }
//        }

//        /// <summary>
//        /// Will try to lookup the PlateId (Barcode) in the XML in the import directory.
//        /// If exists returns the batchname to import into the database. Xperimate will handle the rest
//        /// </summary>
//        public string CheckPlateIdExistInAnyBatchXML(string targetDirectory, string plateId)
//        {            
//            string batchName = "";
//            string fileContents;
//            List<string> fileList = new List<string>();
//            string[] fileEntries = Directory.GetFiles(targetDirectory);
//            // Process the list of files found in the directory.
//            try
//            {         
//                foreach (string item in fileEntries)
//                {
//                    using (var sr = new StreamReader(item))
//                    {
//                        fileContents = sr.ReadToEnd();
//                    }
//                    if (fileContents.Contains(plateId))
//                    {
//                        batchName = item;
//                        break;
//                    }
//                    else
//                    {
//                        batchName = null;
//                    }
//                }
//                return batchName;
//            }
//            catch (System.Exception)
//            {
//                return null;
//            }       
//        }

//        public Batch GetBatch(string name)
//        {
//            return _databaseRepository.GetBatch(name);
//        }

//        public List<Batch> GetAllBatches()
//        {
//            return _databaseRepository.GetAllBatches();
//        }
//        public async Task<List<Alarm>> GetAlarms()
//        {
//            return await _databaseRepository.GetAlarms();
//        }
//        public void ClearAllBatches()
//        {
//            _databaseRepository.ClearAllBatches();
//        }
//        public async Task<object> GetDryPlate()
//        {
//            return await _databaseRepository.GetDryPlate();
//        }
//        public async Task<object> GetEluatedPlate()
//        {
//            return await _databaseRepository.GetEluatedPlate();
//        }
//        public async Task<object> GetIncubatedPlate()
//        {
//            return await _databaseRepository.GetIncubatedPlate();
//        }
//    }
//}
