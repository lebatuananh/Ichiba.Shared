using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;
using Shared.Common;

namespace Shared.Elasticsearch
{
    public class ElasticIndexer<TElasticConnectionSetting>
        where TElasticConnectionSetting : ElasticConnectionSetting, new()
    {
        public ElasticIndexer(ElasticClientProvider<TElasticConnectionSetting> clientProvider,
            IOptions<TElasticConnectionSetting> setting)
        {
            Client = clientProvider.Client;
            Index = setting.Value.Index;
        }

        public ElasticClient Client { get; }
        public string Index { get; }

        public async Task<IndexResult> IndexDocument<TEntity>(TEntity model) where TEntity : class
        {
            var response = await Client.IndexDocumentAsync(model);

            if (!response.IsValid)
                return new IndexResult
                {
                    IsValid = false,
                    ErrorReason = response.ServerError?.Error?.Reason,
                    Exception = response.OriginalException
                };

            Debug.WriteLine("Successfully indexed");

            return new IndexResult
            {
                IsValid = true
            };
        }

        public async Task<IndexResult> IndexDocuments<TEntity>(params TEntity[] models) where TEntity : class
        {
            return await IndexDocuments(models, Index);
        }

        private void SanitizeIndexName(ref string index)
        {
            // The index must be lowercase, this is a requirement from Elastic
            if (index == null)
                index = Index;
            else
                index = index.ToLower();
        }

        private async Task<IndexResult> IndexDocuments<TEntity>(TEntity[] models, string index) where TEntity : class
        {
            var batchSize = 10000; // magic
            var totalBatches = (int) Math.Ceiling((double) models.Length / batchSize);

            for (var i = 0; i < totalBatches; i++)
            {
                var response = await Client.IndexManyAsync(models.Skip(i * batchSize).Take(batchSize), index);

                if (!response.IsValid)
                    return new IndexResult
                    {
                        IsValid = false,
                        ErrorReason = response.ServerError?.Error?.Reason,
                        Exception = response.OriginalException
                    };
                Debug.WriteLine($"Successfully indexed batch {i + 1}");
            }

            return new IndexResult
            {
                IsValid = true
            };
        }

        //private async Task ConfigurePagination(string index)
        //{
        //    // Max out the result window so you can have pagination for >100 pages
        //    await this.Client.UpdateIndexSettingsAsync(index, ixs => ixs
        //         .IndexSettings(s => s
        //             .Setting("max_result_window", int.MaxValue)));
        //}

        //public async Task CreateIndexIfItDoesntExist(string index)
        //{
        //    if (!this.Client.IndexExists(index).Exists)
        //    {
        //        var indexDescriptor = new CreateIndexDescriptor(index);
        //        //.Mappings(mappings => mappings
        //        //    .Map<Product>(m => m.AutoMap()));

        //        await this.Client.CreateIndexAsync(index, i => indexDescriptor);
        //    }
        //}

        //public async Task DeleteIndexIfExists(string index, bool shouldDeleteIndex)
        //{
        //    if (this.Client.IndexExists(index).Exists && shouldDeleteIndex)
        //    {
        //        await this.Client.DeleteIndexAsync(index);
        //    }
        //}

        public SearchResult<T> MapResponseToSearchResult<T>(ISearchResponse<T> response, Paging paging) where T : class
        {
            paging.Total = (int) response.Total;

            return new SearchResult<T>
            {
                IsValid = response.IsValid,
                ErrorMessage = response.ApiCall.OriginalException?.Message,
                Total = response.Total,
                ElapsedMilliseconds = response.Took,
                Page = paging.PageIndex,
                PageSize = paging.PageSize,
                Results = response?.Documents.ToList()
            };
        }
    }
}