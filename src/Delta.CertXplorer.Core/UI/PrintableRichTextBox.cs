using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Delta.CertXplorer.Internals;

namespace Delta.CertXplorer.UI
{
    /// <summary>
    /// Inspir� par cet article : http://support.microsoft.com/default.aspx?scid=kb;en-us;812425
    /// </summary>
    public partial class PrintableRichTextBox : RichTextBox
    {
        /// <summary>
        /// Le framework .NET compte en 100�me de pouces, et Win32 en twips.
        /// La valeur <b>ratio</b> contient 1/100 pouces exprim�s en twips, soit 14.4 twips.
        /// </summary>
        private double ratio = double.NaN;
        private int lastPrintedCharIndex = 0;

        public PrintableRichTextBox()
        {
            InitializeComponent();
            InitializeControl();
        }

        private void InitializeControl()
        {
            // The ratio comes from DrawingUnit.Inch.ConvertTo(0.01, DrawingUnit.Twip)
            ratio = 14.4;
            Debug.Assert(ratio == 14.4);

            printDocument.BeginPrint += new PrintEventHandler(OnBeginPrint);
            printDocument.PrintPage += new PrintPageEventHandler(OnPrintPage);
            printDocument.EndPrint += new PrintEventHandler(OnEndPrint);
        }

        public void Print() { Print(false); }
        public void Print(bool showUI) 
        {
            if (showUI)
            {
                using var dlg = new PrintDialog();
                dlg.Document = printDocument;
                if (dlg.ShowDialog() == DialogResult.OK)
                    printDocument.Print();
            }
            else printDocument.Print(); 
        }

        public void PrintPreview()
        {
            using var dlg = new PrintPreviewDialog();
            dlg.Document = printDocument;
            dlg.WindowState = FormWindowState.Maximized;
            dlg.ShowInTaskbar = false;            
            _ = dlg.ShowDialog();
        }

        public void PageSetup()
        {
            using var dlg = new PageSetupDialog();
            dlg.Document = printDocument;
            _ = dlg.ShowDialog();
        }        

        /// <summary>
        /// Render the contents of the RichTextBox for printing
        /// </summary>
        /// <param name="charFrom">caract�re marquant le d�but de l'impression</param>
        /// <param name="charTo">caract�re marquant la fin de l'impression</param>
        /// <param name="e">param�tres de l'�v�nement d'impression</param>
        /// <returns>last character printed + 1 (printing start from this point for next page)</returns>
        private int Print(int charFrom, int charTo, PrintPageEventArgs e)
        {
            //Calculate the area to render and print
            NativeMethods.RECT rectToPrint;
            rectToPrint.Top = (int)(e.MarginBounds.Top * ratio);
            rectToPrint.Bottom = (int)(e.MarginBounds.Bottom * ratio);
            rectToPrint.Left = (int)(e.MarginBounds.Left * ratio);
            rectToPrint.Right = (int)(e.MarginBounds.Right * ratio);

            //Calculate the size of the page
            NativeMethods.RECT rectPage;
            rectPage.Top = (int)(e.PageBounds.Top * ratio);
            rectPage.Bottom = (int)(e.PageBounds.Bottom * ratio);
            rectPage.Left = (int)(e.PageBounds.Left * ratio);
            rectPage.Right = (int)(e.PageBounds.Right * ratio);

            IntPtr hdc = e.Graphics.GetHdc();

            NativeMethods.FORMATRANGE fmtRange;
            fmtRange.chrg.cpMax = charTo;   //Indicate character from to character to 
            fmtRange.chrg.cpMin = charFrom;
            fmtRange.hdc = hdc;             //Use the same DC for measuring and rendering
            fmtRange.hdcTarget = hdc;       //Point at printer hDC
            fmtRange.rc = rectToPrint;      //Indicate the area on page to print
            fmtRange.rcPage = rectPage;     //Indicate size of page
            
            //Get the pointer to the FORMATRANGE structure in memory
            var lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, lparam, false);

            //Send the rendered data for printing 
            var res = NativeMethods.SendMessage(Handle, NativeMethods.EM_FORMATRANGE, (UIntPtr)1, lparam);

            //Free the block of memory allocated
            Marshal.FreeCoTaskMem(lparam);

            //Release the device context handle obtained by a previous call
            e.Graphics.ReleaseHdc(hdc);

            //Return last + 1 character printer
            return res;
        }

        private void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            // Print the content of RichTextBox. Store the last character printed.
            lastPrintedCharIndex = Print(lastPrintedCharIndex, TextLength, e);

            // Check for more pages
            e.HasMorePages = lastPrintedCharIndex < TextLength;
        }

        private void OnBeginPrint(object sender, PrintEventArgs e) => lastPrintedCharIndex = 0;
        private void OnEndPrint(object sender, PrintEventArgs e) {  /* Nothing to do here */ }
    }
}
