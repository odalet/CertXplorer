/* 
 * Grabbed from Marco De Sanctis' Actions
 * see http://blogs.ugidotnet.org/crad/articles/38329.aspx
 * Original namespace: Crad.Windows.Forms.Actions
 * License: Common Public License Version 1.0
 * 
 */ 

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Delta.CertXplorer.UI.Actions
{
    public sealed class UIActionTargetDescriptor
    {
        private readonly Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

        public UIActionTargetDescriptor(Type type)
        {
            TargetType = type;
            foreach (var property in TargetType.GetProperties())
                properties.Add(property.Name, property);
        }

        public Type TargetType { get; }

        internal void SetValue(string propertyName, object target, object value)
        {
            if (properties.ContainsKey(propertyName))
                properties[propertyName].SetValue(target, value, null);
        }

        internal object GetValue(string propertyName, object source) => 
            properties.ContainsKey(propertyName) ? properties[propertyName].GetValue(source, null) : null;
    }
}
