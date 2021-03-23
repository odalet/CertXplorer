﻿/*------------------------------------------------------------------------------
 * Debugging Microsoft .NET 2.0 Applications
 * Copyright © 1997-2006 John Robbins -- All rights reserved. 
 -----------------------------------------------------------------------------*/
using System;
using System.Threading;
using System.Windows.Forms;

namespace Delta.CertXplorer.Utility
{
    /// <summary>
    /// Copy and paste data without requiring STA threads!
    /// </summary>
    /// <remarks>
    /// The FCL clipboard class requires the thread calling it to be marked with
    /// <see cref="STAThreadAttribute"/>.  For the most part, that's fine, but 
    /// there are enough instances where this isn't the case; hence this class.
    /// <para>
    /// Credit where credit's due: 
    /// http://www.codinghorror.com/blog/archives/000429.html.  Thanks to Jeff
    /// Atwood showing the basic trick of spawning a new thread with STA set.
    /// (I was ready to do all of this at the API level!  Yeech!)
    /// </para>
    /// <para>
    /// Credits (bis):
    /// Debugging Microsoft .NET 2.0 Applications
    /// Copyright © 1997-2006 John Robbins -- All rights reserved. 
    /// </para>
    /// <para>
    /// Right now this just wraps up the text types.  As future needs dictate, 
    /// I'll add the more interesting data types.
    /// </para>
    /// </remarks>
    public static class SmartClipboard
    {
        private sealed class TextGetData
        {
            public TextGetData(TextDataFormat format)
            {
                Format = format;
                Data = string.Empty;
            }

            public TextDataFormat Format { get; }
            public string Data { get; set; }
        }

        /// <summary>
        /// Adds text data to the Clipboard in UnicodeText format.
        /// </summary>
        /// <param name="text">
        /// The text to add to the Clipboard.
        /// </param>
        public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

        /// <summary>
        /// Adds text data to the Clipboard in the format indicated by the 
        /// specified <see cref="TextDataFormat "/> value. 
        /// </summary>
        /// <param name="text">
        /// The text to add to the Clipboard.
        /// </param>
        /// <param name="format">
        /// One of the <see cref="TextDataFormat"/> values.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="text"/> is null or empty.
        /// Thrown if <paramref name="format"/> is not in the appropriate 
        /// <see cref="TextDataFormat"/> range.
        /// </exception>
        public static void SetText(string text, TextDataFormat format)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException("Invalid parameter", nameof(text));
            if (!IsEnumValid((int)format, 0, 4)) throw new ArgumentException("Invalid parameter", nameof(format));

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                // Everything should be good so package up the data and let the 
                // background thread get it to the clipboard.
                var data = new DataObject(ConvertToDataFormats(format), text);
                StartSetThread(data);
            }
            else Clipboard.SetText(text, format); // We're on an STA thread so I can do the call directly.
        }

        /// <summary>
        /// Retrieves the data from the <see cref="Clipboard"/> in Unicode 
        /// format.
        /// </summary>
        /// <returns>
        /// The <see cref="Clipboard"/> text data or <see cref="String.Empty"/> 
        /// if the <see cref="Clipboard"/> does not contain data in the 
        /// UnicodeText format.
        /// </returns>
        public static string GetText() => GetText(TextDataFormat.UnicodeText);

        /// <summary>
        /// Retrieves text data from the <see cref="Clipboard"/> in the format 
        /// indicated by the specified <see cref="TextDataFormat "/> value. 
        /// </summary>
        /// <param name="format">
        /// One of the <see cref="TextDataFormat "/> values.
        /// </param>
        /// <returns>
        /// The <see cref="Clipboard"/> text data or <see cref="String.Empty"/> 
        /// if the <see cref="Clipboard"/> does not contain data in the 
        /// specified format. 
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="format"/> is not in the appropriate 
        /// <see cref="TextDataFormat"/> range.
        /// </exception>
        public static string GetText(TextDataFormat format)
        {
            // Check the enum value.
            if (!IsEnumValid((int)format, 0, 4)) throw new ArgumentException("Invalid parameter", nameof(format));

            return Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ?
                Clipboard.GetText(format) :
                StartGetTextThread(format);
        }

        private static void StartSetThread(IDataObject data)
        {
            var thread = new Thread(new ParameterizedThreadStart(SetThread));
            // The whole reason for this class is because it's possible to need
            // to put stuff on the clipboard and the thread isn't marked STA.
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(data);
            thread.Join();
        }

        private static void SetThread(object data)
        {
            var realData = data as IDataObject;
            Clipboard.SetDataObject(realData, true);
        }

        private static string StartGetTextThread(TextDataFormat format)
        {
            var data = new TextGetData(format);
            var thread = new Thread(new ParameterizedThreadStart(GetTextThread));
            // The whole reason for this class is because it's possible to need
            // to put stuff on the clipboard and the thread isn't marked STA.
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(data);
            thread.Join();
            return data.Data;
        }

        private static void GetTextThread(object data)
        {
            var realData = data as TextGetData;
            realData.Data = Clipboard.GetText(realData.Format);
        }

        // Stolen from the FCL source code.
        private static string ConvertToDataFormats(TextDataFormat format) => format switch
        {
            TextDataFormat.Text => DataFormats.Text,
            TextDataFormat.UnicodeText => DataFormats.UnicodeText,
            TextDataFormat.Rtf => DataFormats.Rtf,
            TextDataFormat.Html => DataFormats.Html,
            TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue,
            _ => DataFormats.UnicodeText,
        };

        private static bool IsEnumValid(int value, int minValue, int maxValue) => value >= minValue && value <= maxValue;
    }
}
