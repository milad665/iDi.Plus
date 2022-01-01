using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Utility.Commands;

public class CreateNodeIdCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine($"Enter target directory path (default = {Directory.GetCurrentDirectory()}):");
        var path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
            path = Directory.GetCurrentDirectory();
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Path '{path}' does not exist");
            Console.WriteLine($"Creating path '{path}'");
            Directory.CreateDirectory(path);
        }

        var keys = DigitalSignatureKeys.Generate();

        var l1Password = string.Empty;
        var l1PasswordConf = string.Empty;
        var l2Password = string.Empty;
        var l2PasswordConf = string.Empty;

        while (string.IsNullOrWhiteSpace(l1Password) || l1Password != l1PasswordConf)
        {
            Console.WriteLine("Enter Level-1 Password: ");
            l1Password = Console.ReadLine();
            Console.WriteLine("Confirm Level-1 Password: ");
            l1PasswordConf = Console.ReadLine();
        }

        while (string.IsNullOrWhiteSpace(l2Password) || l2Password != l2PasswordConf)
        {
            Console.WriteLine("Enter Level-2 Password: ");
            l2Password = Console.ReadLine();
            Console.WriteLine("Confirm Level-2 Password: ");
            l2PasswordConf = Console.ReadLine();
        }

        var pkcs12Data = keys.ToPkcs12(l1Password, l2Password);
        var fileName = Path.Combine(path, $"{keys.PublicKey.ToHexString().Substring(0,10)}.p12");
        using var file = File.OpenWrite(fileName);
        file.Write(pkcs12Data);
        Console.WriteLine($"Successfully created file '{fileName}'.");
        Console.WriteLine();
        Console.WriteLine("Public key:");
        Console.WriteLine(keys.PublicKey.ToHexString());
        Console.WriteLine(new string('-',30));
    }

    public string Name => "Create node id";
    public int Option => 1;
}