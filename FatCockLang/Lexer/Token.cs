class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
    public object Line { get; internal set; }
}