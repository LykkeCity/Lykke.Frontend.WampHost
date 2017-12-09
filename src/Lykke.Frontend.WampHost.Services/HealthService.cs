using System;
using System.Collections.Generic;
using Lykke.Frontend.WampHost.Core.Domain.Health;
using Lykke.Frontend.WampHost.Core.Services;
using System.Threading;

namespace Lykke.Frontend.WampHost.Services
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    public class HealthService : IHealthService
    {
        public int SessionCount => _sessionsCount;

        private int _sessionsCount;

        public string GetHealthViolationMessage()
        {
            // TODO: Check gathered health statistics, and return appropriate health violation message, or NULL if service hasn't critical errors
            return null;
        }

        public IEnumerable<HealthIssue> GetHealthIssues()
        {
            var issues = new HealthIssuesCollection();

            // TODO: Check gathered health statistics, and add appropriate health issues message to issues
            return issues;
        }

        public void TraceWampSessionCreated(object sender, EventArgs e)
        {
            Interlocked.Increment(ref _sessionsCount);
        }

        public void TraceWampSessionClosed(object sender, EventArgs e)
        {
            Interlocked.Decrement(ref _sessionsCount);
        }
    }
}
