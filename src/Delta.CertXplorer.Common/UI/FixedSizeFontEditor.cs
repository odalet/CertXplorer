using System;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;

namespace Delta.CertXplorer.UI
{
    internal sealed class FixedSizeFontEditor : FontEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editedValue = value;
            using (var fontDialog = new FontDialog())
            {
                fontDialog.ShowApply = false;
                fontDialog.ShowColor = false;
                fontDialog.AllowVerticalFonts = false;
                fontDialog.AllowScriptChange = false;
                fontDialog.FixedPitchOnly = true;
                fontDialog.ShowEffects = false;
                fontDialog.ShowHelp = false;

                if (value is Font font) fontDialog.Font = font;

                if (fontDialog.ShowDialog() == DialogResult.OK)
                    editedValue = fontDialog.Font;
            }

            value = editedValue;
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;
    }
}
