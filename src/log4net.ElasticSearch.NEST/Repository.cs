using System.Collections.Generic;
using log4net.ElasticSearch.Infrastructure;
using log4net.ElasticSearch.Models;
using Nest;
using Uri = System.Uri;

namespace log4net.ElasticSearch
{
    public interface IRepository
    {
        void Add(IEnumerable<logEvent> logEvents, int bufferSize);
    }

    public class Repository : IRepository
    {
        readonly List<Uri> uri;
        readonly INestClient client;
        readonly string indexName;

        Repository(List<Uri> uri, string indexName, INestClient client)
        {
            this.uri = uri;
            this.client = client;
            this.indexName = indexName;
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
                if (bufferSize <= 1)
                {
                    // Post the logEvents one at a time throught the ES insert API
                    logEvents.Do(logEvent => client.Post(uri, indexName, logEvent));
                }
                else
                {
                    // Post the logEvents all at once using the ES _bulk API
                    client.PostBulk(uri, indexName, logEvents);
                }   
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }

        public static IRepository Create(string connectionString, string indexName)
        {
            return Create(connectionString, indexName, new NestClient());
        }

        public static IRepository Create(string connectionString, string indexName, INestClient client)
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

            return new Repository(uris, indexName, client);
        }
    }
}
