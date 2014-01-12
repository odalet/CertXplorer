namespace Delta.CertXplorer.ComponentModel
{

    /// <summary>
    /// Custom type converter allowing to display a dictionary in a property grid.
    /// The dictionary is displayed as a set of read-only properties.
    /// </summary>
    public class ReadOnlyDictionaryConverter : DictionaryConverter
    {
        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsReadOnly
        {
            get { return true; }
        }
    }
}
