namespace CalculatorApp.EntityService
{
    public interface ICustomizer
    {
        string GlobalAlternateDelimiter { get; }
        bool DenyNegativeNumbers { get; }
        int NumberUpperBound { get; }
        string Config(ITokenizer target, string text);
        void ReadArguments(string[] args);
    }
}