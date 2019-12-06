using System;
using System.Linq;
using FluentAssertions;
using log4net.ElasticSearch.NEST.Infrastructure;
using log4net.ElasticSearch.NEST.Models;
using log4net.ElasticSearch.NEST.Tests.UnitTests.Stubs;
using Xunit;

namespace log4net.ElasticSearch.NEST.Tests.UnitTests
{
    public class RepositoryTests
    {
        [Fact]
        public void Index_rolls_over_when_date_changes_during_single_call_to_add_multiple_log_entries()
        {
            var logEvents = new[]
                    {
                        new logEvent(), new logEvent(), new logEvent(), new logEvent()
                    };

            using (Clock.Freeze(new DateTime(2015, 01, 01, 23, 59, 58)))
            {
                var clientStub = new NestClientStub(() => Clock.Freeze(Clock.Now.AddSeconds(1)));

                var repository = NEST.Repository.Create("http://localhost:9200", "IndexName", true, "yyyy.MM.dd");

                repository.Add(logEvents, 0);

                clientStub.Items.Count().Should().Be(2);
                clientStub.Items.First().Value.Count.Should().Be(2);
                clientStub.Items.Second().Value.Count.Should().Be(2);
            }
        }
    }
}