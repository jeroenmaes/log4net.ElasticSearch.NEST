﻿using System;
using System.Collections.Generic;
using log4net.ElasticSearch.NEST.Infrastructure;
using log4net.ElasticSearch.NEST.Models;
using Uri = System.Uri;

namespace log4net.ElasticSearch.NEST.Tests.UnitTests.Stubs
{
    public class NestClientStub : INestClient
    {
        readonly Action action;
        readonly IDictionary<Uri, IList<object>> items;

        public NestClientStub(Action action)
        {
            this.action = action;

            items = new Dictionary<Uri, IList<object>>();
        }

        public void Post(List<Uri> uris, string indexName, logEvent item)
        {
            //if (!items.ContainsKey(uri))
            //{
            //    items[uri] = new List<object>();
            //}
            //items[uri].Add(item);

            action();
        }

        public void PostBulk(List<Uri> uris, string indexName, IEnumerable<logEvent> items)
        {

        }

        public IEnumerable<KeyValuePair<Uri, IList<object>>> Items { get { return items; } }
    }
}