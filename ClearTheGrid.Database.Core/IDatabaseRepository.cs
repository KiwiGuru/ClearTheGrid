//using DNACruiserIII.Model;
//using DNACruiserIII.Model.XMLModel;
//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;

//namespace DNACruiserIII.Database.Core
//{
//    [ComVisible(true),
//       Guid("26566e5f-9aee-46d5-b1a3-926933035227")]
//    public interface IDatabaseRepository
//    {
//        Task<ConsumableUseCount> GetConsumableCount(string name);
//        Task<int> SetConsumableCount(string name, int count);
//        Task<string> GetPlateTrackingBarcode(string name);
//        Task<TrackingLiquid> GetTrackingLiquid(string name);
//        Task<int> UpdatePlateTracking(string name, string barcode, int status, string action, string currentTime, bool sourcePlate);
//        Task<int> UpdatePlateTrackingObject(PlateTrackingData dataToUpdate, int status);
//        Task<TipStatus> GetTipStatus(string moduleName);
//        Task<int> UpdateTipStatus(string tipModule, int status, int rackNr);
//        Task<int> UpdateTrackingLiquid(TrackingLiquid trackingLiquid);
//        void ImportBatch(string fileFullName);
//        void ClearAllBatches();
//        Batch GetBatch(string name);
//        Task<SourcePlate> GetPlateInfoSource(string plateId);
//        Task<Analysis> GetAnalysisIdFromSource(SourcePlate plateInfo);
//        Task<AnalysisGroup> GetAnalysisGroupFromAnalysis(Analysis analysis);
//        Task<LabOrder> GetLabOrderFromAnalysisGroup(AnalysisGroup analysisGroup);
//        Task<string> GetRecipeNameFromLabOrder(LabOrder labOrder);
//        Task<DestinationPlate> GetPlateInfoDestinationFromAnalysisId(Analysis analysis);
//        Task<SystemConfiguration> GetSystemConfiguration(string name);
//        List<Batch> GetAllBatches();
//        Task<string> GetPlateTrackingArriveTime(string name);
//        Task<PlateTrackingData> GetPlateTrackingDataByName(string name);
//        Task<object> GetDryPlate();
//        Task<object> GetEluatedPlate();
//        Task<object> GetIncubatedPlate();
//        string CheckPlateIdExistInAnyBatchXML(string plateId, string targetDirectory);
//        Task<List<SourcePlate>> GetAllSourcePlatesFromDestinationPlateId(string plateId);
//        Task<SourcePlate> GetSourceFromAnalysisId(Guid analysis);
//        Task<List<Alarm>> GetAlarms();
//        Task<List<PlateSet>> GetPlateSets();
//        Task<int> SetPlateSetsAsync(PlateSet plateSet);
//    }
//}
