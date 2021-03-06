﻿using FluentAssertions;
using log4net.ElasticSearch.NEST.Models;
using log4net.ElasticSearch.NEST.Tests.Infrastructure;
using log4net.ElasticSearch.NEST.Tests.Infrastructure.Builders;
using Nest;
using Xunit;
using Xunit.Sdk;

namespace log4net.ElasticSearch.NEST.Tests.IntegrationTests
{
    [Collection("IndexCollection")]
    public class ElasticSearchTests
    {
        private ElasticClient elasticClient;
        private IntegrationTestFixture testFixture;

        public ElasticSearchTests(IntegrationTestFixture testFixture)
        {
            this.testFixture = testFixture;
            elasticClient = testFixture.Client;
        }

        [Fact]
        public void Can_insert_record()
        {
            var indexResponse = elasticClient.IndexDocument(LogEventBuilder.Default.LogEvent);

            indexResponse.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_indexed_document()
        {
            var logEvent = LogEventBuilder.Default.LogEvent;

            elasticClient.IndexDocument(logEvent);    

            Retry.Ignoring<XunitException>(() =>
                {
                    var logEntries =
                        elasticClient.Search<logEvent>(
                            sd => sd.Query(qd => qd.Term(le => le.className, logEvent.className)));

                    logEntries.Total.Should().Be(1);                    
                });
        }

    }
}