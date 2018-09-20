using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace Lykke.Frontend.WampHost.Contracts.Commands
{
    /// <summary>
    /// Request 2fa confirmation command
    /// </summary>
    [ProtoContract]
    public class RequestConfirmationCommand
    {
        [ProtoMember(1)]
        public Guid ClientId { get; set; }

        [ProtoMember(2)]
        public Guid OperationId { get; set; }

        [ProtoMember(3)]
        public string ConfirmationType { get; set; }
    }
}
