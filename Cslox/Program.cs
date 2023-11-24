using Cslox.Lang;

var hadError = false;

if (args.Length > 1)
{
    Console.WriteLine("Usage: cslox [script]");
    Environment.Exit(64);
}
else if (args.Length == 2)
{
    RunFile(args[0]);
}
else
{
    RunPrompt();
}

void RunFile(string path)
{
    var fileContents = File.ReadAllText(path);

    Run(fileContents);
    
    if (hadError)
        Environment.Exit(65);
}

void RunPrompt()
{
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            break;
        
        Run(input);

        hadError = false;
    }
}

static void Run(string contents)
{
    Scanner scanner = new(contents);

    var tokens = scanner.ScanTokens();

    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
}