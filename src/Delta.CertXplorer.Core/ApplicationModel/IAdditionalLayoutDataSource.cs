﻿using System;
using System.Xml;

namespace Delta.CertXplorer.ApplicationModel
{
    /// <summary>
    /// When implemented by a <see cref="System.Windows.Forms.Form"/>, allows a <see cref="ILayoutService"/>
    /// to register additional layout-related data from the Form.
    /// </summary>
    public interface IAdditionalLayoutDataSource
    {
        /// <summary>
        /// This method is called by a <see cref="ILayoutService"/> when the form is created. Its 
        /// previously serialized layout data is provided in the <paramref name="data"/> parameter.
        /// </summary>
        /// <param name="data">The previously serialized layout data.</param>
        void SetAdditionalLayoutData(string data);

        /// <summary>
        /// This method is called by a <see cref="ILayoutService"/> when the form is closed. It must 
        /// return the layout data that will be serialized.
        /// </summary>
        /// <returns>The layout data to serialize.</returns>
        string GetAdditionalLayoutData();
    }
}
