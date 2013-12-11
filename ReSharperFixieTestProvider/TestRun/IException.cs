namespace FixiePlugin.TestRun
{
    public interface IException
    {
        string Type { get; }
        string Message { get; }
        string StackTrace { get; }
    }
}