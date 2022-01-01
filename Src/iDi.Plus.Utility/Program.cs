// See https://aka.ms/new-console-template for more information

using iDi.Plus.Tool.Commands;

var commands = new List<ICommand>
{
    new CreateNodeIdCommand()
};

Console.Clear();
Console.WriteLine("Please choose an option below (type option number and press enter):");
foreach (var command in commands.OrderBy(c => c.Option))
    Console.WriteLine($"{command.Option}.\t{command.Name}");

var selectedOption = 0;
while (commands.All(c => c.Option != selectedOption))
{
    var option = Console.ReadLine();
    int.TryParse(option, out selectedOption);
}

commands.FirstOrDefault(c => c.Option == selectedOption)?.Execute();
