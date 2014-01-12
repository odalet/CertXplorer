using System;
using System.Collections;
using System.ComponentModel;

namespace Delta.CertXplorer.ComponentModel
{
    /// <summary>
    /// Custom type converter allowing to display a dictionary in a property grid.
    /// The dictionary is displayed as a set of properties.
    /// </summary>
    public class DictionaryConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this object supports properties, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)" /> should be called to find the properties of this object; otherwise, false.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

        /// <summary>
        /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object" /> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> with the properties that are exposed for this data type, or null if there are no properties.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var dictionary = value as IDictionary;
            if (dictionary == null) return null;

            var properties = new System.Collections.Generic.List<PropertyDescriptor>();
            foreach (DictionaryEntry entry in dictionary)
                properties.Add(new DictionaryPropertyDescriptor(dictionary, entry.Key, IsReadOnly));

            var props = properties.ToArray();
            return new PropertyDescriptorCollection(props);
        }

        /// <summary>
        /// Gets a value indicating whether the wrapped dictionary should be considered read only.
        /// </summary>
        protected virtual bool IsReadOnly
        {
            get { return false; }
        }
    }
}
