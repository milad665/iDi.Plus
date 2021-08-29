namespace iDi.Blockchain.Core.Commands
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        public void Handle(TCommand command);
    }
}