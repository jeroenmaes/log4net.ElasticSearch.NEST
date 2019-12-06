namespace log4net.ElasticSearch.NEST.Tests.UnitTests
{
    public class TestableAppender : ElasticSearchNestAppender
    {
        readonly IRepository repository;

        public TestableAppender(IRepository repository)
        {
            this.repository = repository;
        }

        public bool? FailSend { get; set; }

        public bool? FailClose { get; set; }

        protected override IRepository CreateRepository(string connectionString, string indexName, bool rolling, string rollingFormat)
        {
            return repository;
        }

        protected override bool TryAsyncSend(System.Collections.Generic.IEnumerable<Core.LoggingEvent> events)
        {
            return FailSend.HasValue ? !FailSend.Value : base.TryAsyncSend(events);
        }

        protected override bool TryWaitAsyncSendFinish()
        {
            return FailClose.HasValue ? !FailClose.Value : base.TryWaitAsyncSendFinish();
        }
    }
}