using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Delta.CertXplorer.Logging;

namespace Delta.CertXplorer.UI.ToolWindows
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms convention")]
    public partial class LogWindowControl : UserControl
    {
        private readonly List<ToolStripItem> items = new();
        private LogLevel selectedLevel = LogLevel.All;
        private ITextBoxAppender appender = null;

        public LogWindowControl()
        {
            InitializeComponent();

            cm.Renderer = VS2015ThemeProvider.Renderer;
            ts.Renderer = VS2015ThemeProvider.Renderer;
        }

        public event EventHandler WordWrapChanged;
        public event LinkClickedEventHandler LinkClicked;

        public LogLevel SelectedLevel
        {
            get => selectedLevel;
            set
            {
                if (value != selectedLevel)
                {
                    selectedLevel = value;
                    logLevelList.SelectedItem = selectedLevel;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Initialize();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (appender != null)
                appender.LogThreshold = Enabled ? selectedLevel : LogLevel.Off;
        }

        private void Initialize()
        {
            var wrapper = tb is RichTextBox rtb ? new ThreadSafeRichTextBoxWrapper(rtb) : new ThreadSafeTextBoxWrapper(tb);
            var logService = This.Logger;
            if (logService is ITextBoxAppendable appendable)
            {
                var log = appendable;
                appender = log.AddLogBox(wrapper, "%date [%thread] (%logger) %-8level- %message%newline");
                if (appender != null) appender.LogThreshold = SelectedLevel;
            }

            var levels = new List<LogLevel>();
            levels.AddRange(Enums<LogLevel>.Values);
            levels.Sort((l1, l2) => (int)l1 - (int)l2);
            levels.ForEach(level => logLevelList.Items.Add(level));

            // TODO: select the log level depending on a settings file.
            logLevelList.SelectedItem = selectedLevel;

            foreach (ToolStripItem item in cm.Items) items.Add(item);
            foreach (ToolStripItem item in ts.Items)
            {
                if (item != logLeveltoolStripLabel &&
                    item != logLevelList &&
                    item != toggleWordWrapToolStripButton)
                    items.Add(item);
            }

            UpdateToolStripItemsEnabledState();

            tb.TextChanged += (s, e) => UpdateToolStripItemsEnabledState();
            tb.LinkClicked += (s, e) => LinkClicked?.Invoke(this, e);
        }

        private void UpdateToolStripItemsEnabledState()
        {
            foreach (var item in items) item.Enabled = tb.TextLength > 0;
        }

        private void CopyLog()
        {
            if (tb.SelectionLength != 0)
            {
                tb.Copy();
                return;
            }

            tb.SelectAll();
            tb.Copy();
            tb.Select(tb.Text.Length - 1, 0);
            tb.ScrollToCaret();
        }

        private void ClearLog() => tb.Clear();
        private void SelectLog() => tb.SelectAll();

        private void ToggleWordWrapLog()
        {
            tb.WordWrap = !tb.WordWrap;
            WordWrapChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SaveLog()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Log Files (*.log, *.txt)|*.log;*.txt|Rich Text Files (*.rtf)|*.rtf|All Files|*.*",
                FilterIndex = 0,
                DefaultExt = "*.log",
                OverwritePrompt = true,
                SupportMultiDottedExtensions = true,
                CheckPathExists = true,
                ValidateNames = true
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                var filename = string.Empty;
                try
                {
                    filename = dialog.FileName;

                    var streamType = filename.EndsWith(".rtf") ?
                        RichTextBoxStreamType.RichText :
                        RichTextBoxStreamType.UnicodePlainText;
                    tb.SaveFile(filename, streamType);
                }
                catch (Exception ex)
                {
                    This.Logger.Error($"Error while saving data into file file://{filename}: {ex.Message}", ex);
                    _ = ErrorBox.Show(this, $"An error occurred while saving a file: {ex.Message}");
                }
            }
        }

        private void Print() => tb.Print();
        private void PrintWithUI() => tb.Print(true);
        private void PrintPreview() => tb.PrintPreview();
        private void PageSetup() => tb.PageSetup();

        private void DisposeAppender()
        {
            if (appender == null) return;
            appender.Dispose();
            appender = null;
        }

        private void saveToolStripButton_Click(object sender, EventArgs e) => SaveLog();
        private void copyToolStripButton_Click(object sender, EventArgs e) => CopyLog();
        private void clearAllToolStripButton_Click(object sender, EventArgs e) => ClearLog();
        private void toggleWordWrapToolStripButton_Click(object sender, EventArgs e) => ToggleWordWrapLog();
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) => SaveLog();
        private void printToolStripMenuItem_Click(object sender, EventArgs e) => Print();
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) => CopyLog();
        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e) => ClearLog();
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) => SelectLog();
        private void printToolStripSplitButton_ButtonClick(object sender, EventArgs e) => Print();
        private void quickPrintToolStripMenuItem_Click(object sender, EventArgs e) => Print();
        private void printToolStripMenuItem1_Click(object sender, EventArgs e) => PrintWithUI();
        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e) => PrintPreview();
        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e) => PageSetup();
        private void logLevelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLevel = (LogLevel)logLevelList.SelectedItem;
            if (appender != null) appender.LogThreshold = selectedLevel;
        }
    }
}
