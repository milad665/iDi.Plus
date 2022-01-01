namespace iDi.Plus.Utility.Commands;

public interface ICommand
{
    void Execute();
    string Name { get; }
    int Option { get; }
}