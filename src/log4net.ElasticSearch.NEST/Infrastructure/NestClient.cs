﻿using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using log4net.ElasticSearch.NEST.Models;
using Uri = System.Uri;

namespace log4net.ElasticSearch.NEST.Infrastructure
{
    public interface INestClient
    {
        void Post(List<Uri> uris, string indexName, logEvent item);
        void PostBulk(List<Uri> uris, string indexName, IEnumerable<logEvent> items);
    }

    public class NestClient : INestClient
    {

        private static Nest.ElasticClient _elasticClient;

        private void CreateElasticClient(List<Uri> uris, string indexName)
        {
            var connectionPool = new SniffingConnectionPool(uris);
            
            var connectionSettings = new Nest.ConnectionSettings(connectionPool)
                .SniffOnStartup()
                .SniffOnConnectionFault()
                .DisableAutomaticProxyDetection()
                .ThrowExceptions()
                .DefaultIndex(indexName.ToLower());

            _elasticClient = new Nest.ElasticClient(connectionSettings);
        }

        public void Post(List<Uri> uris, string indexName, logEvent item)
        {
            if (_elasticClient == null)
            {
                CreateElasticClient(uris, indexName);
            }

            var elasticResponse = _elasticClient.IndexDocument(item);
            if (!elasticResponse.IsValid)
            {
                throw new Exception($"Logging to ElasticSearch failed. Response: {elasticResponse}");
            }
        }

        public void PostBulk(List<Uri> uris, string indexName, IEnumerable<logEvent> items)
        {
            if (_elasticClient == null)
            {
                CreateElasticClient(uris, indexName);
            }
            
            var elasticResponse = _elasticClient.Bulk(b => b.IndexMany(items, (d, doc) => d.Document(doc)));
            if (!elasticResponse.IsValid)
            {
                throw new Exception($"Logging in Bulk to ElasticSearch failed. Response: {elasticResponse}");
            }
        }
    }
}