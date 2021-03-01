using System;
using System.Collections.Generic;
using System.Linq;
using Delta.CertXplorer.DocumentModel;

namespace Delta.CertXplorer.Commanding
{
    internal static partial class Commands
    {
        private sealed class CommandBindingDescriptor
        {
            private CommandBindingDescriptor(IVerb verb, Type commandType, Type targetType, bool isDefaultBinding)
            {
                Verb = verb;
                CommandType = commandType;
                TargetType = targetType;
                IsDefaultBinding = isDefaultBinding;
            }

            public Type CommandType { get; }
            public Type TargetType { get; }
            public IVerb Verb { get; }
            public bool IsDefaultBinding { get; }

            public static CommandBindingDescriptor Create<V, C>(Type targetType = null, bool isDefaultBinding = false)
                where V : IVerb
                where C : ICommand
            {
                if (targetType == null && isDefaultBinding) throw new ArgumentException(
                    "Can't have both a null target type and default binding set.", nameof(targetType));

                IVerb verb;
                try
                {
                    verb = (IVerb)Activator.CreateInstance(typeof(V));
                }
                catch (Exception ex)
                {
                    This.Logger.Error($"Could not create an instance of verb type {typeof(V)}: {ex.Message}", ex);
                    throw;
                }

                if (verb == null)
                {
                    var message = $"Could not convert the verb type {typeof(V)} to IVerb";
                    This.Logger.Error(message);
                    throw new InvalidCastException(message);
                }

                return new CommandBindingDescriptor(verb, typeof(C), targetType, isDefaultBinding);
            }
        }

        private static readonly Dictionary<Type, List<CommandBindingDescriptor>> commandBindings = new Dictionary<Type, List<CommandBindingDescriptor>>();

        static Commands()
        {
            AddCommandBinding<OpenFileVerb, OpenFileCommand>(typeof(string));
            AddCommandBinding<OpenCertificateVerb, OpenCertificateCommand>(typeof(X509Object));
            AddCommandBinding<OpenExistingDocumentVerb, OpenExistingDocumentCommand>(typeof(IDocument));
            AddCommandBinding<CloseDocumentVerb, CloseDocumentCommand>(typeof(IDocument));
        }

        public static void RunVerb(IVerb verb, params object[] arguments) => RunVerb<object>(verb, arguments);

        public static void RunVerb<T>(IVerb verb, params T[] arguments) =>
            RunVerbImpl(verb, arguments == null ? new object[0] : arguments.Cast<object>().ToArray(), typeof(T));

        private static void RunVerbImpl(IVerb verb, object[] arguments, Type firstArgumentType = null)
        {
            if (verb == null) verb = NullVerb.Instance;
            if (firstArgumentType == null && arguments != null && arguments.Length > 0)
                firstArgumentType = arguments[0].GetType();

            var descriptor = FindDescriptor(verb, firstArgumentType);
            if (descriptor?.CommandType == null)
            {
                var firstArg = firstArgumentType?.ToString() ?? "<null>";
                This.Logger.Error(
                    $"Could not find a command associated to Objects of type {firstArg} with Verb {verb.Name}");
                return;
            }

            ICommand command;
            try
            {
                command = (ICommand)Activator.CreateInstance(descriptor.CommandType);
            }
            catch (Exception ex)
            {
                This.Logger.Error($"The command type {descriptor.CommandType} is not convertible to ICommand: {ex.Message}", ex);
                return;
            }

            // We have a command object. Invoke it
            command.Run(verb, arguments);
        }

        private static CommandBindingDescriptor FindDescriptor(IVerb verb, Type targetType)
        {
            if (verb == null) verb = NullVerb.Instance;
            var verbType = verb.GetType();

            if (!commandBindings.ContainsKey(verbType)) return null;

            var list = commandBindings[verbType];

            CommandBindingDescriptor result = null;
            if (targetType != null)
            {
                result = list.FirstOrDefault(b => b.TargetType != null && b.TargetType == targetType); // exact matching
                if (result != null) return result;

                result = list.FirstOrDefault(b => b.TargetType != null && b.TargetType.IsA(targetType)); // inheritance matching
                if (result != null) return result;
            }

            return list.FirstOrDefault(b => b.TargetType == null); // non-typed commands
        }

        private static void AddCommandBinding<V, C>(Type acceptedTargetType = null, bool isDefaultBinding = false)
            where V : IVerb
            where C : ICommand
        {
            List<CommandBindingDescriptor> list;
            if (commandBindings.ContainsKey(typeof(V))) list = commandBindings[typeof(V)];
            else
            {
                list = new List<CommandBindingDescriptor>();
                commandBindings.Add(typeof(V), list);
            }

            var descriptor = CommandBindingDescriptor.Create<V, C>(acceptedTargetType, isDefaultBinding);
            list.Add(descriptor);
        }
    }
}
