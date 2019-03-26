﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Elasticsearch.Net;
using log4net.ElasticSearch.Models;
using Uri = System.Uri;

namespace log4net.ElasticSearch.Infrastructure
{
    public interface INestClient
    {
        void Post(List<Uri> uris, logEvent item);
        void PostBulk(List<Uri> uris, IEnumerable<logEvent> items);
    }

    public class NestClient : INestClient
    {

        private static Nest.ElasticClient _elasticClient;

        private void CreateElasticClient(List<Uri> uris)
        {
            var connectionPool = new SniffingConnectionPool(uris);
            var connectionSettings = new Nest.ConnectionSettings(connectionPool).DefaultIndex($"LOG4TEST".ToLower());
            _elasticClient = new Nest.ElasticClient(connectionSettings);
        }

        public void Post(List<Uri> uris, logEvent item)
        {
            if (_elasticClient == null)
            {
                CreateElasticClient(uris);
            }

            var elasticResponse = _elasticClient.IndexDocument(item);
            if (!elasticResponse.IsValid)
            {
                throw new Exception($"Logging to ElasticSearch failed. Response: {elasticResponse}");
            }
        }

        public void PostBulk(List<Uri> uris, IEnumerable<logEvent> items)
        {
            foreach (var item in items)
            {
                Post(uris, item);
            }
        }
    }
}