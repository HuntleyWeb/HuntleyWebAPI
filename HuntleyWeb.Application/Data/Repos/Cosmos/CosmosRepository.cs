using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HuntleyWeb.Application.Data.Repos.Cosmos
{
    public abstract class CosmosRepository<T>
    {
        protected readonly Database Database;
        protected readonly Container Container;
        protected readonly CosmosClient Client;
        protected readonly int TimeoutInMilliseconds;

        public CosmosRepository(
            CosmosClient client,
            string databaseName,
            string containerName,
            string partitionKeyPath = "/PartitionKey",
            int timeoutInMillisecs = 3000,
            bool ensureCreated = true,
            int? defaultTtl = null)
        {
            Client = client;
            TimeoutInMilliseconds = timeoutInMillisecs;

            if (ensureCreated)
            {
                try
                {
                    var database = Client.CreateDatabaseIfNotExistsAsync(databaseName).GetAwaiter().GetResult();

                    Container = database.Database
                        .CreateContainerIfNotExistsAsync(new ContainerProperties
                        {
                            Id = containerName,
                            PartitionKeyPath = partitionKeyPath,
                            DefaultTimeToLive = defaultTtl
                        }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                Container = Client.GetContainer(databaseName, containerName);
            }
        }

        public async Task<T> CreateAsync(T item, ItemRequestOptions options = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var response = await Container.CreateItemAsync(item, null, options, GetCancellationTokenWithTimeout());

            return response;
        }

        public async Task<T> UpsertAsync(T item, string partitionKey, ItemRequestOptions options = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var response = await Container.UpsertItemAsync(item, new PartitionKey(partitionKey), options, GetCancellationTokenWithTimeout());

            return response.Resource;
        }


        public async Task<bool> DeleteDocumentAsync(string documentId, string partitionKeyValue, ItemRequestOptions options = null)
        {
            var response = await Container.DeleteItemAsync<T>(documentId, new PartitionKey(partitionKeyValue), options);

            return IsSuccessStatusCode(response.StatusCode);
        }

        public async Task<T> QuerySingleAsync(string query, QueryRequestOptions options = null)
        {
            T result = default(T);

            var resultSetIterator = Container.GetItemQueryIterator<T>(query, null, options);

            while(resultSetIterator.HasMoreResults)
            {
                var resonse = await resultSetIterator.ReadNextAsync(GetCancellationTokenWithTimeout());

                if (resonse.Count() > 0)
                    result = resonse.FirstOrDefault();
            }

            return result;
        }

        public async Task<List<T>> RunQuery(string query, ItemRequestOptions options = null)
        {
            var items = new List<T>();

            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            using FeedIterator<T> feed = Container.GetItemQueryIterator<T>(
                query
            );

            while (feed.HasMoreResults)
            {
                FeedResponse<T> results = await feed.ReadNextAsync();
                foreach (T item in results)
                {
                    items.Add(item);                    
                }
            }            

            return items;
        }

        private CancellationToken GetCancellationTokenWithTimeout()
        {
            if (TimeoutInMilliseconds > 0)
            {
                var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(TimeoutInMilliseconds));

                return cts.Token;
            }

            return default;
        }

        private bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            return ((int)statusCode >= 200) && ((int)statusCode <= 299);
        }
    }
}
