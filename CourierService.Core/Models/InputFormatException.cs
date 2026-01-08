namespace CourierService.Core.Models
{
    public sealed class InputFormatException : Exception
    {
        public InputFormatException(string message) : base(message) { }
    }
}
