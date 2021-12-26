namespace iDi.Blockchain.Framework.Commands
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        public void Handle(TCommand command);
    }
}