namespace Games.Microservice.shared
{
    public static class CorrelationContext
    {
        private static readonly AsyncLocal<string> _current = new();

        public static string Current
        {
            get => _current.Value ??= Guid.NewGuid().ToString();
            set => _current.Value = value;
        }
    }
}
