using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace Delta.CertXplorer.UI
{
    partial class PropertyGridEx
    {
        /// <summary>
        /// Wraps a real object so that we add custom behavior (mainly redefining properties)
        /// before the object is displayed in the property grid.
        /// </summary>
        private sealed class ObjectWrapper : ICustomTypeDescriptor, IComponent
        {
            private readonly object wrappedObject;
            private PropertyDescriptorCollection properties = null;

            public ObjectWrapper(object objectToWrap) =>
                wrappedObject = objectToWrap ?? throw new ArgumentNullException(nameof(objectToWrap));

            public event EventHandler Disposed;

            public ISite Site { get; set; }

            // nothing to do here apart from invoking the event
            public void Dispose() => Disposed?.Invoke(this, EventArgs.Empty);

            public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetPropertyDescriptors(attributes);

            public PropertyDescriptorCollection GetProperties() => GetPropertyDescriptors(null);

            public TypeConverter GetConverter() => TypeDescriptor.GetConverter(wrappedObject, true);

            public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(wrappedObject, attributes, true);

            public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(wrappedObject, true);

            public string GetComponentName() => TypeDescriptor.GetComponentName(wrappedObject, true);

            public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(wrappedObject, true);

            public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(wrappedObject, true);

            public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(wrappedObject, true);

            public string GetClassName() => TypeDescriptor.GetClassName(wrappedObject, true);

            public object GetEditor(Type editorBaseType) => wrappedObject == null ? null : TypeDescriptor.GetEditor(wrappedObject, editorBaseType, true);

            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                var trace = new StackTrace();
                var found = trace.GetFrames().FirstOrDefault(f => f.GetMethod().Name == "DisplayHotCommands");
                if (found == null) return wrappedObject; // normal call, we return the wrapped object
                else
                {
                    Console.WriteLine("Call emmitted from DisplayHotCommands");
                    return this;
                }
            }

            private PropertyDescriptorCollection GetPropertyDescriptors(Attribute[] attributes)
            {
                if (properties == null)
                {
                    var baseProperties = attributes == null
                        ? TypeDescriptor.GetProperties(wrappedObject, true)
                        : TypeDescriptor.GetProperties(wrappedObject, attributes, true);

                    properties = new PropertyDescriptorCollection(baseProperties
                        .Cast<PropertyDescriptor>()
                        .Select(p => new InnerPropertyDescriptor(wrappedObject, p))
                        .ToArray())
                        ;
                }

                return properties;
            }
        }
    }
}
