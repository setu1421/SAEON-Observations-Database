using SAEON.Azure.CosmosDB;
using SAEON.Azure.Storage;
using SAEON.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.Azure
{
    public class ObservationsAzure : IDisposable
    {
        private bool disposedValue;
        private const string BlobStorageContainer = "saeon-observations";
        //private const string ObservationsStorageTable = "Observations";
        private const string CosmosDBDatabase = "saeon-observations";
        private const string CosmosDBContainer = "Observations";
        //private const string CosmosDBPartitionKey = "/importBatch/id";
        private const string CosmosDBPartitionKey = "/importBatchId";

        public static bool Enabled { get; private set; }
        public static bool StorageEnabled { get; private set; }
        public static bool CosmosDBEnabled { get; private set; }
        public static bool CosmosDBBulkEnabled { get; private set; }
        public int BatchSize => int.Parse(ConfigurationManager.AppSettings["AzureCosmosDBBatchSize"] ?? AzureCosmosDB<ObservationItem>.DefaultBatchSize.ToString());

        private AzureStorage Storage;
        private AzureCosmosDB<ObservationItem> CosmosDB;

        static ObservationsAzure()
        {
            using (SAEONLogs.MethodCall(typeof(ObservationsAzure)))
            {
                try
                {
                    Enabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureEnabled"] ?? "false");
                    if (Enabled)
                    {
                        StorageEnabled = bool.Parse(ConfigurationManager.AppSettings["AzureStorageEnabled"] ?? "false");
                        CosmosDBEnabled = bool.Parse(ConfigurationManager.AppSettings["AzureCosmosDBEnabled"] ?? "false");
                        CosmosDBBulkEnabled = bool.Parse(ConfigurationManager.AppSettings["AzureCosmosDBBulkEnabled"] ?? "false");
                    }
                    SAEONLogs.Information("Azure: {Enabled} Storage: {StorageEnabled} CosmosDB: {CosmosDBEnabled} CosmosDBBulk: {CosmosDBBulkEnabled}", Enabled, StorageEnabled, CosmosDBEnabled, CosmosDBBulkEnabled);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public ObservationsAzure()
        {
            using (SAEONLogs.MethodCall(GetType()))
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
                            CosmosDB = new AzureCosmosDB<ObservationItem>(CosmosDBDatabase, CosmosDBContainer, CosmosDBPartitionKey, CosmosDBBulkEnabled);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ObservationsAzure()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Storage = null;
                    CosmosDB.Dispose();
                    CosmosDB = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task InitializeAsync()
        {
            if (!ObservationsAzure.Enabled) return;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    if (StorageEnabled)
                    {
                        SAEONLogs.Verbose("Ensuring Storage Container exists");
                        await Storage.EnsureContainerAsync(BlobStorageContainer).ConfigureAwait(false);
                    }
                    if (CosmosDBEnabled)
                    {
                        SAEONLogs.Verbose("Ensuring CosmosDB Container exists");
                        await CosmosDB.EnsureContainerAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }


        public void Initialize()
        {
            if (!ObservationsAzure.Enabled) return;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    InitializeAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        #region Storage
        public async Task UploadAsync(string folder, string fileName, string fileContents)
        {
            if (!Enabled || !StorageEnabled) return;
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Folder", folder }, { "FileName", fileName } }))
            {
                try
                {
                    var date = DateTime.Now;
                    var blobContainerClient = Storage.GetBlobContainerClient(BlobStorageContainer);
                    await blobContainerClient.UploadBlobAsync($"{folder}/{fileName}", fileContents).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public void Upload(string folder, string fileName, string fileContents)
        {
            if (!Enabled || !StorageEnabled) return;
            using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "Folder", folder }, { "FileName", fileName } }))
            {
                try
                {
                    UploadAsync(folder, fileName, fileContents).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
        #endregion

        #region CosmosDB
        public async Task<ObservationItem> GetObservationAsync(ObservationItem item)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.GetItemAsync(item, i => i.ImportBatchId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public ObservationItem GetObservation(ObservationItem item)
        {
            return GetObservationAsync(item).GetAwaiter().GetResult();
        }

        public async Task<(ObservationItem item, CosmosDBCost<ObservationItem> cost)> GetObservationWithCostAsync(ObservationItem item)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.GetItemWithCostAsync(item, i => i.ImportBatchId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }


        public (ObservationItem item, CosmosDBCost<ObservationItem> cost) GetObservationWithCost(ObservationItem item)
        {
            return GetObservationWithCostAsync(item).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<ObservationItem>> GetObservationsAsync(Expression<Func<ObservationItem, bool>> predicate)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.GetItemsAsync(predicate).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public IEnumerable<ObservationItem> GetObservations(Expression<Func<ObservationItem, bool>> predicate)
        {
            return GetObservationsAsync(predicate).GetAwaiter().GetResult();
        }

        public async Task<(IEnumerable<ObservationItem> items, CosmosDBCost<ObservationItem> cost)> GetObservationsWithCostAsync(Expression<Func<ObservationItem, bool>> predicate)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.GetItemsWithCostAsync(predicate).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public (IEnumerable<ObservationItem> items, CosmosDBCost<ObservationItem> cost) GetObservationsWithCost(Expression<Func<ObservationItem, bool>> predicate)
        {
            return GetObservationsWithCostAsync(predicate).GetAwaiter().GetResult();
        }

        public async Task<CosmosDBCost<ObservationItem>> AddObservationAsync(ObservationItem item)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return (await CosmosDB.CreateItemWithCostAsync(item, i => i.ImportBatchId).ConfigureAwait(false)).cost;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public CosmosDBCost<ObservationItem> AddObservation(ObservationItem item)
        {
            return AddObservationAsync(item).GetAwaiter().GetResult();
        }

        public async Task<CosmosDBCost<ObservationItem>> ReplaceObservationAsync(ObservationItem item)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return (await CosmosDB.ReplaceItemWithCostAsync(item, i => i.ImportBatchId).ConfigureAwait(false)).cost;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public CosmosDBCost<ObservationItem> ReplaceObservation(ObservationItem item)
        {
            return ReplaceObservationAsync(item).GetAwaiter().GetResult();
        }

        public async Task<CosmosDBCost<ObservationItem>> UpsertObservationAsync(ObservationItem item)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return (await CosmosDB.UpsertItemWithCostAsync(item, i => i.ImportBatchId).ConfigureAwait(false)).cost;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public CosmosDBCost<ObservationItem> UpsertObservation(ObservationItem item)
        {
            return UpsertObservationAsync(item).GetAwaiter().GetResult();
        }

        public async Task<CosmosDBCost<ObservationItem>> UpsertObservationsAsync(List<ObservationItem> items)
        {
            if (!Enabled || !CosmosDBEnabled) return new CosmosDBCost<ObservationItem>();
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.UpsertItemsAsync(items, i => i.ImportBatchId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public CosmosDBCost<ObservationItem> UpsertObservations(List<ObservationItem> items)
        {
            return UpsertObservationsAsync(items).GetAwaiter().GetResult();
        }

        public async Task<CosmosDBCost<ObservationItem>> DeleteObservationAsync(ObservationItem item)
        {
            if (!Enabled || !CosmosDBEnabled) return new CosmosDBCost<ObservationItem>();
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return (await CosmosDB.DeleteItemWithCostAsync(item, i => i.ImportBatchId).ConfigureAwait(false)).cost;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public CosmosDBCost<ObservationItem> DeleteObservation(ObservationItem item)
        {
            return DeleteObservationAsync(item).GetAwaiter().GetResult();
        }

        public async Task<(CosmosDBCost<ObservationItem> totalCost, CosmosDBCost<ObservationItem> readCost, CosmosDBCost<ObservationItem> deleteCost)> DeleteImportBatchAsync(Guid importBatchId)
        {
            if (!Enabled || !CosmosDBEnabled) return default;
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    return await CosmosDB.DeleteItemsAsync(i => i.ImportBatch.Id == importBatchId, i => i.ImportBatchId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public (CosmosDBCost<ObservationItem> totalCost, CosmosDBCost<ObservationItem> readCost, CosmosDBCost<ObservationItem> deleteCost) DeleteImportBatch(Guid importBatchId)
        {
            return DeleteImportBatchAsync(importBatchId).GetAwaiter().GetResult();
        }
        #endregion
    }
}