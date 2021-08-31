using System;
using iDi.Blockchain.Core.Commands;

namespace iDi.Plus.Application.Services.Commands.Handlers
{
    public abstract class PrivilegedCommandHandlerBase<TCommand> : ICommandHandler<TCommand> where TCommand : CommandBase
    {
        public void Handle(TCommand command)
        {
            Validate(command);
            DoHandle(command);
        }

        private void Validate(TCommand command)
        {
            //Check if node is verified and is coming from a verified ip
            throw new NotImplementedException();
        }

        public abstract void DoHandle(TCommand command);
    }
}