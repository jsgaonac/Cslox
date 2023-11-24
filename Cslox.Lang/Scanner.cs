namespace Cslox.Lang;

public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens;
    private int _current = 0;
    private int _start = 0;
    private int _line = 1;

    private bool IsAtEnd => _current >= _source.Length;
    private char Peek => IsAtEnd ? '\0' : _source[_current];
    private char PeekNext => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];

    private static readonly Dictionary<string, TokenType> Keywords;

    static Scanner()
    {
        Keywords = new Dictionary<string, TokenType>()
        {
            { "and", TokenType.And },
            { "class", TokenType.Class },
            { "else", TokenType.Else },
            { "false", TokenType.False },
            { "for", TokenType.For },
            { "fun", TokenType.Fun },
            { "if", TokenType.If },
            { "nil", TokenType.Nil },
            { "or", TokenType.Or },
            { "print", TokenType.Print },
            { "return", TokenType.Return },
            { "super", TokenType.Super },
            { "this", TokenType.This },
            { "true", TokenType.True },
            { "var", TokenType.Var },
            { "while", TokenType.While },
        };
    }
    
    public Scanner(string source)
    {
        _source = source;
        _tokens = new List<Token>();
    }
    
    public IReadOnlyList<Token> ScanTokens()
    {
        while (!IsAtEnd)
        {
            _start = _current;

            ScanToken();
        }
        
        _tokens.Add(new Token(TokenType.Eof, string.Empty, null, _line));

        return _tokens;
    }

    private void ScanToken()
    {
        var @char = Advance();

        var tokenType = @char switch
        {
            '(' => TokenType.LeftParen,
            ')' => TokenType.RightParen,
            '{' => TokenType.LeftBrace,
            '}' => TokenType.RightBrace,
            ',' => TokenType.Comma,
            '.' => TokenType.Dot,
            '-' => TokenType.Minus,
            '+' => TokenType.Plus,
            ';' => TokenType.Semicolon,
            '*' => TokenType.Star,
            '!' => Match('=') ? TokenType.BangEqual : TokenType.Bang,
            '=' => Match('=') ? TokenType.EqualEqual : TokenType.Equal,
            '<' => Match('=') ? TokenType.LessEqual : TokenType.Less,
            '>' => Match('=') ? TokenType.GreaterEqual : TokenType.Greater,
            '/' => Match('/') ? HandleComment() : TokenType.Slash,
            ' ' or '\r' or '\t' => TokenType._Ignore,
            '\n' => HandleNewLine(),
            '"' => HandleString(),
            _ when IsDigit(@char) => HandleNumber(),
            _ when IsAlpha(@char) => HandleIdentifier(),
            _ => TokenType._Invalid
        };

        if (tokenType == TokenType._Ignore)
            return;
        
        if (tokenType == TokenType._Invalid)
            Log.Error(_line, $"Unexpected character: {@char}");

        AddToken(tokenType);
    }

    private static bool IsDigit(char c) => c >= '0' && c <= '9';

    private static bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

    private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    private TokenType HandleNumber()
    {
        while (IsDigit(Peek)) Advance();

        if (Peek == '.' && IsDigit(PeekNext))
        {
            Advance();

            while (IsDigit(Peek)) Advance();
        }

        var numberStr = GetSubstring(_start, _current);
        
        AddToken(TokenType.Number, Double.Parse(numberStr));

        return TokenType._Ignore;
    }

    private TokenType HandleString()
    {
        while (Peek != '"' && !IsAtEnd)
        {
            if (Peek == '\n') _line++;

            Advance();
        }

        if (IsAtEnd)
        {
            Log.Error(_line, "Unterminated string");

            return TokenType._Invalid;
        }

        Advance();

        var text = GetSubstring(_start + 1, _current - 1);

        AddToken(TokenType.String, text);
        
        return TokenType._Ignore;
    }

    private TokenType HandleNewLine()
    {
        _line++;
        
        return TokenType._Ignore;
    }

    private TokenType HandleIdentifier()
    {
        while (IsAlphaNumeric(Peek)) Advance();

        var identifier = GetSubstring(_start, _current);

        if (!Keywords.TryGetValue(identifier, out var tokenType))
            tokenType = TokenType.Identifier;
        
        AddToken(tokenType);
        
        return TokenType._Ignore;
    }

    private char Advance() => _source[_current++];

    private bool Match(char expected)
    {
        if (IsAtEnd || _source[_current] != expected) return false;

        _current++;

        return true;
    }

    private TokenType HandleComment()
    {
        while (Peek != '\n' && !IsAtEnd)
            Advance();

        return TokenType._Ignore;
    }

    private void AddToken(TokenType tokenType) => AddToken(tokenType, null);

    private void AddToken(TokenType tokenType, object? literal)
    {
        var lexeme = GetSubstring(_start, _current);
        
        _tokens.Add(new Token(tokenType, lexeme, literal, _line));
    }

    private string GetSubstring(int startIndex, int endIndex) => _source[startIndex..endIndex];
}
