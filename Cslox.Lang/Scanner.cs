namespace Cslox.Lang;

public class Scanner
{
    private readonly string _contents;
    
    public Scanner(string contents)
    {
        _contents = contents;
    }
    
    public IReadOnlyList<Token> ScanTokens()
    {
        return Array.Empty<Token>();
    }
}
