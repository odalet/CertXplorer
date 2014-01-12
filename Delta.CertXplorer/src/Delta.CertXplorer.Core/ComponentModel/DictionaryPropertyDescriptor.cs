﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Delta.CertXplorer.ComponentModel
{
    /// <summary>
    /// This property descriptor allows a dictionary to be displayed in a property grid.
    /// </summary>
    internal class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>Binding flags to use when retrieveing instance constructors.</summary>
        private const BindingFlags instanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private IDictionary dictionary = null;
        private object key = null;
        private bool isReadOnly = false;

        public DictionaryPropertyDescriptor(IDictionary d, object k) : this(d, k, true) { }

        public DictionaryPropertyDescriptor(IDictionary d, object k, bool readOnly)
            : base(k.ToString(), null)
        {
            dictionary = d;
            key = k;
            isReadOnly = readOnly;
        }

        public override bool CanResetValue(object component)
        {
            var t = GetValueType();
            return (t.IsClass || t.IsInterface || t.IsValueType);
        }

        public override Type ComponentType { get { return null; } }

        public override object GetValue(object component) { return dictionary[key]; }

        public override bool IsReadOnly { get { return isReadOnly; } }

        public override Type PropertyType
        {
            get
            {
                if (dictionary[key] == null) return GetValueType();
                else return dictionary[key].GetType();
            }
        }

        public override void ResetValue(object component)
        {
            // TODO: manage nullable types.

            var t = GetValueType();
            if (t.IsClass || t.IsInterface) dictionary[key] = null;
            else if (t.IsValueType) dictionary[key] = Activator.CreateInstance(t);
        }

        public override void SetValue(object component, object value)
        {
            if (isReadOnly) throw new InvalidOperationException("This object is read-only");
            else dictionary[key] = value;
        }

        public override bool ShouldSerializeValue(object component) { return false; }

        private Type GetValueType()
        {
            Type t = dictionary.GetType();
            if (t.IsGenericType)
                return t.GetGenericArguments()[1];
            else return typeof(object);
        }
    }
}
