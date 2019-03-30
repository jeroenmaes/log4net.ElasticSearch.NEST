using System;
using log4net.ElasticSearch.NEST.Tests.UnitTests.Stubs;

namespace log4net.ElasticSearch.NEST.Tests.UnitTests
{
    public class UnitTestContext : IDisposable
    {
        const string ServerList = "Server=localhost;Index=log_test;Port=9200;rolling=true";
        const int BufferSize = 100;

        public ElasticSearchNestAppender Appender { get; private set; }

        public RepositoryStub Repository { get; private set; }

        public ErrorHandlerStub ErrorHandler { get; private set; }

        public void Dispose()
        {
            if (Appender == null) return;

            try
            {
                Appender.Flush();
            }
            catch{}
        }

        public static UnitTestContext Create(int bufferSize = BufferSize, bool? failSend = null, bool? failClose = null)
        {
            var repository = new RepositoryStub();
            var errorHandler = new ErrorHandlerStub();

            var appender = new TestableAppender(repository)
                {
                    Lossy = false,
                    BufferSize = bufferSize, 
                    ServerList = ServerList, 
                    ErrorHandler = errorHandler, 
                    FailSend = failSend, 
                    FailClose = failClose
                };

            appender.ActivateOptions();

            return new UnitTestContext
                {
                    Repository = repository,
                    ErrorHandler = errorHandler,
                    Appender = appender
                };
        }
    }
}