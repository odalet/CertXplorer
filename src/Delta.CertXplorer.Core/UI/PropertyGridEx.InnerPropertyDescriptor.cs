using System;
using System.ComponentModel;
using Delta.CertXplorer.ComponentModel;

namespace Delta.CertXplorer.UI
{
    partial class PropertyGridEx
    {
        private sealed class InnerPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor property = null;
            
            private string name = string.Empty;
            private string description = string.Empty;
            private string category = string.Empty;

            public InnerPropertyDescriptor(object ownerObject, PropertyDescriptor baseProperty) : base(baseProperty) 
            {
                if (ownerObject == null) throw new ArgumentNullException(nameof(ownerObject));
                property = baseProperty ?? throw new ArgumentNullException(nameof(baseProperty));
            }

            public override string Description
            {
                get
                {
                    if (string.IsNullOrEmpty(description)) description = BuildDescription();
                    return description;
                }
            }

            public override string Category
            {
                get
                {
                    if (string.IsNullOrEmpty(category)) category = BuildCategory();
                    return category;
                }
            }

            public override string DisplayName
            {
                get
                {
                    if (string.IsNullOrEmpty(name)) name = BuildName();
                    return name;
                }
            }

            public override bool IsReadOnly => property.IsReadOnly;
            public override Type ComponentType => property.ComponentType;
            public override Type PropertyType => property.PropertyType;

            public override bool CanResetValue(object component) => property.CanResetValue(component);
            public override object GetValue(object component) => property.GetValue(component);
            public override void ResetValue(object component) => property.ResetValue(component);
            public override void SetValue(object component, object value) => property.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => property.ShouldSerializeValue(component);

            private string BuildName()
            {
                var propertyName = property.DisplayName;
                foreach (Attribute attribute in property.Attributes)
                {
                    if (attribute is NameAttribute na)
                        propertyName = na.Name;
                }

                return propertyName;
            }
            
            private string BuildDescription()
            {
                var propertyDescription = property.Description;
                foreach (Attribute attribute in property.Attributes)
                {
                    if (attribute is DescriptionAttribute da)
                        propertyDescription = da.Description;
                }

                return propertyDescription; 
            }

            private string BuildCategory()
            {
                var propertyCategory = property.Category;
                foreach (Attribute attribute in property.Attributes)
                {
                    if (attribute is CategoryAttribute ca)
                        propertyCategory = ca.Category;
                }

                return propertyCategory;
            }
        }
    }
}
