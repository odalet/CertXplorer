using System;
using System.ComponentModel;
using System.Globalization;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal interface IDisplayTypeWrapper
    {
        string DisplayType { get; }
    }

    internal sealed class CustomExpandableObjectConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is IDisplayTypeWrapper w)
                    return w.DisplayType;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
