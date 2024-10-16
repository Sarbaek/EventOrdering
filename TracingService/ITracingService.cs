namespace EventOrdering
{
    public interface ITracingService
    {
        public void LogTrace(string message, string origin);
        public void LogError(string message, string origin);
    }
}
