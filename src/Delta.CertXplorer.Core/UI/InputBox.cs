using System;
using System.Drawing;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public partial class InputBox : Form
    {
        private string output = string.Empty;

        public static string Show(IWin32Window owner, string prompt) => Show(owner, prompt, SR.Input);
        public static string Show(IWin32Window owner, string prompt, string title) => Show(owner, prompt, title, string.Empty);
        public static string Show(IWin32Window owner, string prompt, string title, string defaultResponse) => Show(owner, prompt, title, defaultResponse, -1, -1);
        public static string Show(IWin32Window owner, string prompt, string title, string defaultResponse, int xpos, int ypos)
        {
            using var box = new InputBox(prompt, title, defaultResponse, xpos, ypos);
            _ = box.ShowDialog(owner);
            return box.output;
        }

        protected string boxPrompt = string.Empty;
        protected string boxTitle = string.Empty;
        protected string boxDefaultResponse = string.Empty;
        protected int xloc = -1;
        protected int yloc = -1;

        protected InputBox() { InitializeComponent(); }

        protected InputBox(string prompt, string title, string defaultResponse, int xpos, int ypos)
        {
            InitializeComponent();

            boxPrompt = string.IsNullOrEmpty(prompt) ? SR.NoMessage : prompt;
            boxTitle = string.IsNullOrEmpty(title) ? SR.Input : title;
            boxDefaultResponse = defaultResponse;
            xloc = xpos;
            yloc = ypos;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeInputBox();
            textBox.Select();
        }

        protected TextBox TextBox { get { return textBox; } }
        
        protected virtual void InitializeInputBox()
        {
            Text = boxTitle;
            label.Text = boxPrompt;
            textBox.Text = boxDefaultResponse;

            okButton.Click += (s, e) =>
            {
                output = textBox.Text;
                Close();
            };

            cancelButton.Click += (s, e) => Close();

            SizeF ef;
            using (var graphics = label.CreateGraphics())
                ef = graphics.MeasureString(boxPrompt, label.Font, label.Width);
            
            if (ef.Height > label.Height)
            {
                int heightGap = (int)Math.Round((double)ef.Height) - label.Height;                
                label.Height += heightGap;
                textBox.Top += heightGap;
                Height += heightGap;
            }

            MinimumSize = Size;

            SuspendLayout();
            if (xloc == -1 && yloc == -1)
                StartPosition = Owner != null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
            else
            {
                if (xloc == -1) xloc = 600;
                if (yloc == -1) yloc = 350;

                StartPosition = FormStartPosition.Manual;
                DesktopLocation = new Point(xloc, yloc);
            }

            ResumeLayout(true);
        }
    }
}
