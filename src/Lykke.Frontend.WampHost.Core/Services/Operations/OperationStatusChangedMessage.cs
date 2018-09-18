using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Core.Services.Operations
{
    public enum OperationStatus
    {
        ConfirmationRequested,
        Confirmed,
        Failed,
    }

    public class OperationStatusChangedMessage
    {
        public Guid OperationId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OperationStatus Status { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}

