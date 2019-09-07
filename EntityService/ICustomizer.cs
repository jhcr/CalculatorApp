namespace CalculatorApp.EntityService
{
    public interface ICustomizer
    {
        void Config(ILexer target, ref string text);
    }
}