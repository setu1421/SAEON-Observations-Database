using SAEON.Azure.CosmosDB;
using SAEON.Azure.Storage;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Azure
{
    public class Azure
    {
        private const string BlobStorageContainer = "saeon-observations";
        //private const string ObservationsStorageTable = "Observations";
        private const string CosmosDBDatabase = "saeon-observations";
        private const string CosmosDBCollection = "Observations";
        private const string CosmosDBPartitionKey = "/ImportBatch/id";

        public static bool Enabled { get; private set; } = false;
        public static bool StorageEnabled { get; private set; } = false;
        public static bool CosmosDBEnabled { get; private set; } = false;
        public int BatchSize { get { return int.Parse(ConfigurationManager.AppSettings["AzureBatchSize"] ?? AzureCosmosDB<ObservationDocument>.DefaultBatchSize.ToString()); } }

        private AzureStorage Storage = null;
        private AzureCosmosDB<ObservationDocument> CosmosDB = null;

        static Azure()
        {
            using (Logging.MethodCall(typeof(Azure)))
            {
                try
                {
                    Enabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureEnabled"] ?? "false");
                    if (Enabled)
                    {
                        StorageEnabled = bool.Parse(ConfigurationManager.AppSettings["AzureStorageEnabled"] ?? "false");
                        CosmosDBEnabled = bool.Parse(ConfigurationManager.AppSettings["AzureCosmosDBEnabled"] ?? "false");
                    }
                    Logging.Information("Azure: {Enabled} Storage: {StorageEnabled} CosmosDB: {CosmosDBEnabled}", Enabled, StorageEnabled, CosmosDBEnabled);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public Azure()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    if (Enabled)
                    {
                        if (StorageEnabled)
                        {
                            var connectionStringName = "AzureStorage";
                            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString;
                            if (string.IsNullOrEmpty(connectionString))
                            {
                                connectionString = ConfigurationManager.AppSettings[connectionStringName];
                            }
                            Storage = new AzureStorage(connectionString);
                        }
                        if (CosmosDBEnabled)
                        {
                            CosmosDB = new AzureCosmosDB<ObservationDocument>(CosmosDBDatabase, CosmosDBCollection, CosmosDBPartitionKey);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        ~Azure()
        {
            Storage = null;
            CosmosDB = null;
        }

        public async Task InitializeAsync()
        {
            if (!Azure.Enabled) return;
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    if (StorageEnabled)
                    {
                        Logging.Verbose("Ensuring Storage Container exists");
                        await Storage.EnsureContainerAsync(Azure.BlobStorageContainer);
                    }
                    if (CosmosDBEnabled)
                    {
                        Logging.Verbose("Ensuring CosmosDB Collection exists");
                        await CosmosDB.EnsureCollectionAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }


        public void Initialize()
        {
            if (!Azure.Enabled) return;
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    InitializeAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #region Storage
        public async Task UploadAsync(string folder, string fileName, string fileContents)
        {
            if (!Enabled || !StorageEnabled) return;
            using (Logging.MethodCall(GetType(), new ParameterList { { "Folder", folder }, { "FileName", fileName } }))
            {
                try
                {
                    var date = DateTime.Now;
                    var container = Storage.GetContainer(BlobStorageContainer);
                    var bytes = Encoding.UTF8.GetBytes(fileContents);
                    await container.UploadBlobAsync($"{folder}/{fileName}", bytes);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public void Upload(string folder, string fileName, string fileContents)
        {
            if (!Enabled || !StorageEnabled) return;
            using (Logging.MethodCall(GetType(), new ParameterList { { "Folder", folder }, { "FileName", fileName } }))
            {
                try
                {
                    UploadAsync(folder, fileName, fileContents).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion

        #region CosmosDB
        public async Task<AzureCost> AddObservationAsync(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var (item, cost) = await CosmosDB.CreateItemAsync(document);
                    return cost;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public AzureCost AddObservation(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost(); 
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return AddObservationAsync(document).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<AzureCost> UpdateObservationAsync(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var (item, cost) = await CosmosDB.UpdateItemAsync(document);
                    return cost;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public AzureCost UpdateObservation(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return UpdateObservationAsync(document).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<AzureCost> UpsertObservationAsync(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var (item, cost) = await CosmosDB.UpsertItemAsync(document);
                    return cost;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public AzureCost UpsertObservation(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return UpsertObservationAsync(document).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<AzureCost> UpsertObservationsAsync(List<ObservationDocument> documents)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.UpsertItemsAsync(documents);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public AzureCost UpsertObservations(List<ObservationDocument> documents)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return UpsertObservationsAsync(documents).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<AzureCost> DeleteObservationAsync(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    var (item, cost) = await CosmosDB.DeleteItemAsync(document, i => i.ImportBatch.Id);
                    return cost;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public AzureCost DeleteObservation(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return DeleteObservationAsync(document).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public async Task<AzureCost> DeleteImportBatchAsync(Guid importBatchId)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    //var resp = await CosmosDB.DeleteItemsAsync(i => i.Id, importBatchId.ToString(), i => i.ImportBatch.Id == importBatchId);
                    //return resp;
                    var resp = await CosmosDB.DeleteItemsAsync("ImportBatch.id","ImportBatch.id",importBatchId);
                    return resp;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public AzureCost DeleteImportBatch(Guid importBatchId)
        {
            if (!Enabled || !CosmosDBEnabled) return new AzureCost();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    return DeleteImportBatchAsync(importBatchId).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion
    }
}