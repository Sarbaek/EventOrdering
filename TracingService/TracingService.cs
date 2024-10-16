namespace EventOrdering
{
    public class TracingService : ITracingService
    {
        public void LogError(string message, string origin)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error logged from {origin}: {message}");
            Console.ResetColor();
        }
        public void LogTrace(string message, string origin)
        {
            Console.WriteLine($"Trace logged from {origin}: {message}");
        }
    }
}