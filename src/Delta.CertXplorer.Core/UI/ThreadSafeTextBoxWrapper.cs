using System;
using System.Drawing;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public class ThreadSafeTextBoxWrapper
    {
        private readonly TextBoxBase control;

        private delegate void SetTextDelegate(string text);
        private delegate string GetTextDelegate();
        private delegate void SetFontDelegate(Font font);
        private delegate Font GetFontDelegate();
        private delegate void AppendTextDelegate(string text);
        private delegate void ScrollToCaretDelegate();

        public ThreadSafeTextBoxWrapper(TextBoxBase textbox) => control = textbox ?? throw new ArgumentNullException("textbox");

        public string Text 
        {
            get => GetText(); 
            set => SetText(value); 
        }

        public Font Font
        {
            get => GetFont();
            set => SetFont(value);
        }

        public void AppendText(string text)
        {
            if (control.IsDisposed) return;
            if (control.InvokeRequired)
            {
                var del = new AppendTextDelegate(AppendText);
                _ = control.Invoke(del, new object[] { text });
            }
            else control.AppendText(text);
        }

        public void ScrollToCaret()
        {
            if (control.IsDisposed) return;
            if (control.InvokeRequired)
            {
                var del = new ScrollToCaretDelegate(ScrollToCaret);
                _ = control.Invoke(del);
            }
            else control.ScrollToCaret();
        }

        private void SetText(string text)
        {
            if (control.IsDisposed) return;
            if (control.InvokeRequired)
            {
                var del = new SetTextDelegate(SetText);
                _ = control.Invoke(del, new object[] { text });
            }
            else control.Text = text;
        }

        private string GetText()
        {
            if (control.IsDisposed) return string.Empty;
            if (control.InvokeRequired)
            {
                var del = new GetTextDelegate(GetText);
                return (string)control.Invoke(del);
            }
            else return control.Text;
        }

        private void SetFont(Font font)
        {
            if (control.IsDisposed) return;
            if (control.InvokeRequired)
            {
                var del = new SetFontDelegate(SetFont);
                _ = control.Invoke(del, new object[] { font });
            }
            else control.Font = font;
        }

        private Font GetFont()
        {
            if (control.IsDisposed) return null;
            if (control.InvokeRequired)
            {
                var del = new GetFontDelegate(GetFont);
                return (Font)control.Invoke(del);
            }
            else return control.Font;
        }
    }
}
