using System.Collections.Generic;
using log4net.ElasticSearch.NEST.Infrastructure;
using log4net.ElasticSearch.NEST.Models;
using Uri = System.Uri;

namespace log4net.ElasticSearch.NEST
{
    public interface IRepository
    {
        void Add(IEnumerable<logEvent> logEvents, int bufferSize);
    }

    public class Repository : IRepository
    {
        private readonly List<Uri> _uri;
        private readonly INestClient _client;
        private readonly string _rollingFormat;
        private readonly bool _rolling;
        private readonly string _indexName;

        public Repository(List<Uri> uri, string indexName, INestClient client, bool rolling, string rollingFormat)
        {
            this._uri = uri;
            this._client = client;
            this._indexName = indexName;
            this._rolling = rolling;
            this._rollingFormat = rollingFormat;
        }

        /// <summary>
        /// Post the event(s) to the Elasticsearch API. If the bufferSize in the connection
        /// string is set to more than 1, assume we use the _bulk API for better speed and
        /// efficiency
        /// </summary>
        /// <param name="logEvents">A collection of logEvents</param>
        /// <param name="bufferSize">The BufferSize as set in the connection string details</param>
        public void Add(IEnumerable<logEvent> logEvents, int bufferSize)
        {
            try
            {
                var indexName = _indexName.ToLower();
                if (_rolling)
                {
                    indexName = "{0}-{1}".With(indexName, Clock.Date.ToString(_rollingFormat));
                }

                if (bufferSize <= 1)
                {
                    // Post the logEvents one at a time throught the ES insert API
                    logEvents.Do(logEvent => _client.Post(_uri, indexName, logEvent));
                }
                else
                {
                    // Post the logEvents all at once using the ES _bulk API
                    _client.PostBulk(_uri, indexName, logEvents);
                }   
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }

        public static IRepository Create(string connectionString, string indexName, bool rolling, string rollingFormat)
        {
            return Create(connectionString, indexName, new NestClient(), rolling, rollingFormat);
        }

        public static IRepository Create(string connectionString, string indexName, INestClient client, bool rolling, string rollingFormat)
        {
            var nodeArray = connectionString.Split(',');
            var uris = new List<Uri>();
            foreach (var nodeName in nodeArray)
            {
                if (!string.IsNullOrEmpty(nodeName))
                {
                    uris.Add(new Uri(nodeName));
                }
            }

            return new Repository(uris, indexName, client, rolling, rollingFormat);
        }
    }
}
