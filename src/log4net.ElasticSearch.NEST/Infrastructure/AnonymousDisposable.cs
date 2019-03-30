using System;

namespace log4net.ElasticSearch.NEST.Infrastructure
{
    public class AnonymousDisposable : IDisposable
    {
        readonly Action action;

        public AnonymousDisposable(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}