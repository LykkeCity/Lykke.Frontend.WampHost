﻿using System.Collections.Generic;

namespace Lykke.Frontend.WampHost.Models
{
    public class IsAliveResponse
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Env { get; set; }
        public bool IsDebug { get; set; }
        public IEnumerable<IssueIndicator> IssueIndicators { get; set; }
        public int SessionCount { get; set; }

        public class IssueIndicator
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
