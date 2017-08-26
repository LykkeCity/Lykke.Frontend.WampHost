using System;
using System.Collections.Generic;
using Lykke.Frontend.WampHost.Core.Domain.Health;

namespace Lykke.Frontend.WampHost.Core.Services
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    public interface IHealthService
    {
        int SessionCount { get; }

        string GetHealthViolationMessage();
        IEnumerable<HealthIssue> GetHealthIssues();

        void TraceWampSessionCreated(object sender, EventArgs e);
        void TraceWampSessionClosed(object sender, EventArgs e);    
    }
}