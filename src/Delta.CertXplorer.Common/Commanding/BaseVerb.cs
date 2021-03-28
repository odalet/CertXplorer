namespace Delta.CertXplorer.Commanding
{
    public abstract class BaseVerb : IVerb
    {
        protected BaseVerb(string verbName) => Name = verbName;
        public string Name { get; }
    }
}
