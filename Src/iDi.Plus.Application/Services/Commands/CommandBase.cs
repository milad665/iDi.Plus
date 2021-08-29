using iDi.Blockchain.Core.Commands;
using System;
using System.Net;

namespace iDi.Plus.Application.Services.Commands
{
    public abstract class CommandBase : ICommand
    {
        public Guid NodeId { get; set; }
        public IPAddress NodeIP { get; set; }
        
    }
}
