using System;
using System.Security;
using System.Reflection;
using System.Windows.Forms;

using Delta.CertXplorer.Internals;

namespace Delta.CertXplorer.UI
{
    /// <summary>
    /// Default implementation of <see cref="ISimpleUIService"/>.
    /// </summary>
    internal class SimpleUIService : ISimpleUIService
    {
        public IWin32Window Owner => NativeMethods.ActiveWindow;

        public void ShowErrorBox(string message) => ShowErrorBox(null, message);
        public void ShowErrorBox(Exception exception) => ShowErrorBox(exception, string.Empty);
        public void ShowErrorBox(Exception exception, string message)
        {
            if (exception == null)
            {
                _ = ErrorBox.Show(Owner, string.IsNullOrEmpty(message) ? SR.Error : message);
                return;
            }

            var text = string.IsNullOrEmpty(message) ?
                exception.ToFormattedString() :
                $"{message}\r\n\r\n{exception.ToFormattedString()}";

            _ = ErrorBox.Show(Owner, text);
        }

        public void ShowWarningBox(string message) => WarningBox.Show(Owner, message);
        public void ShowInformationBox(string message) => InformationBox.Show(Owner, message);
        public DialogResult ShowQuestionBox(string message) => QuestionBox.Show(Owner, message);

        public DialogResult ShowMessageBox(string message) => ShowMessageBox(
            message, SR.Message, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);

        public DialogResult ShowMessageBox(string message, string caption) => ShowMessageBox(
            message, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons) => ShowMessageBox(
            message, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) => ShowMessageBox(
            message, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0);

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) => MessageBox.Show(
            Owner, message, caption, buttons, icon, defaultButton, 0);

        public DialogResult ShowMessageBox(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options) => MessageBox.Show(
            Owner, message, caption, buttons, icon, defaultButton, options);


        public string ShowInputBox(string message) => ShowInputBox(
            message, GetTitleFromAssembly(Assembly.GetCallingAssembly()), string.Empty);

        public string ShowInputBox(string message, string caption) => ShowInputBox(
            message, caption, string.Empty);

        public string ShowInputBox(string message, string caption, string defaultValue) => InputBox.Show(
            Owner, string.IsNullOrEmpty(message) ? SR.NoMessage : message, caption, defaultValue);

        public DialogResult ShowDialog(CommonDialog commonDialog) => commonDialog == null ? 
            throw new ArgumentNullException("commonDialog") : 
            commonDialog.ShowDialog(Owner);

        private static string GetTitleFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetName().Name;
            }
            catch (SecurityException)
            {
                var fullName = assembly.FullName;
                var index = fullName.IndexOf(',');
                return index >= 0 ? fullName.Substring(0, index) : string.Empty;
            }
        }
    }
}
