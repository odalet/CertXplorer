using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Delta.CertXplorer.Extensibility
{
    [Serializable]
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException() : base() { }
        public ServiceNotFoundException(string message) : base(message) { }
        public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        public ServiceNotFoundException(Type type) : base(BuildMessage(type)) => ServiceType = type;
        public ServiceNotFoundException(Type type, Exception innerException) : base(BuildMessage(type), innerException) => ServiceType = type;
        public ServiceNotFoundException(Type type, string message) : base(BuildMessage(type, message)) => ServiceType = type;
        public ServiceNotFoundException(Type type, string message, Exception innerException) : base(BuildMessage(type, message), innerException) => ServiceType = type;
        protected ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public Type ServiceType { get; }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            info.AddValue("ServiceType", ServiceType);
            base.GetObjectData(info, context);
        }

        private static string BuildMessage(Type type) => BuildMessage(type, string.Empty);

        private static string BuildMessage(Type type, string message) => string.IsNullOrEmpty(message) ?
            $"Service of type {type} couldn't be found" :
            $"Service of type {type} couldn't be found: {message}";
    }

    [Serializable]
    public sealed class ServiceNotFoundException<T> : ServiceNotFoundException
    {
        public ServiceNotFoundException() : base(typeof(T)) { }
        public ServiceNotFoundException(Exception innerException) : base(typeof(T), innerException) { }
        public ServiceNotFoundException(string message) : base(typeof(T), message) { }
        public ServiceNotFoundException(string message, Exception innerException) : base(typeof(T), message, innerException) { }
        private ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}