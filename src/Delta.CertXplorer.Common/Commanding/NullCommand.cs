namespace Delta.CertXplorer.Commanding
{
    public sealed class NullCommand : ICommand
    {
        public void Run(IVerb verb, params object[] arguments)
        {
            // Method intentionally left empty.
        }
    }
}
