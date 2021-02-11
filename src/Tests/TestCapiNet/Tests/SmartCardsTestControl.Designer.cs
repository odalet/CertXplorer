namespace TestCapiNet.Tests
{
    partial class SmartCardsTestControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.testsPanel = new System.Windows.Forms.Panel();
            this.getChallengeButton = new System.Windows.Forms.Button();
            this.smartCardReaderChooser = new TestCapiNet.UI.SmartCardReaderChooser();
            this.getReaderInformationButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.testsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.testsPanel);
            this.groupBox1.Location = new System.Drawing.Point(3, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(655, 341);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tests";
            // 
            // testsPanel
            // 
            this.testsPanel.AutoScroll = true;
            this.testsPanel.Controls.Add(this.getReaderInformationButton);
            this.testsPanel.Controls.Add(this.getChallengeButton);
            this.testsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testsPanel.Location = new System.Drawing.Point(3, 16);
            this.testsPanel.Name = "testsPanel";
            this.testsPanel.Size = new System.Drawing.Size(649, 322);
            this.testsPanel.TabIndex = 0;
            // 
            // getChallengeButton
            // 
            this.getChallengeButton.Location = new System.Drawing.Point(3, 32);
            this.getChallengeButton.Name = "getChallengeButton";
            this.getChallengeButton.Size = new System.Drawing.Size(155, 23);
            this.getChallengeButton.TabIndex = 0;
            this.getChallengeButton.Text = "GET CHALLENGE";
            this.getChallengeButton.UseVisualStyleBackColor = true;
            this.getChallengeButton.Click += new System.EventHandler(this.getChallengeButton_Click);
            // 
            // smartCardReaderChooser
            // 
            this.smartCardReaderChooser.Dock = System.Windows.Forms.DockStyle.Top;
            this.smartCardReaderChooser.Location = new System.Drawing.Point(0, 0);
            this.smartCardReaderChooser.Name = "smartCardReaderChooser";
            this.smartCardReaderChooser.Size = new System.Drawing.Size(661, 62);
            this.smartCardReaderChooser.TabIndex = 0;
            // 
            // getReaderInformationButton
            // 
            this.getReaderInformationButton.Location = new System.Drawing.Point(3, 3);
            this.getReaderInformationButton.Name = "getReaderInformationButton";
            this.getReaderInformationButton.Size = new System.Drawing.Size(155, 23);
            this.getReaderInformationButton.TabIndex = 1;
            this.getReaderInformationButton.Text = "Get Reader Information";
            this.getReaderInformationButton.UseVisualStyleBackColor = true;
            this.getReaderInformationButton.Click += new System.EventHandler(this.getReaderInformationButton_Click);
            // 
            // SmartCardsTestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.smartCardReaderChooser);
            this.Name = "SmartCardsTestControl";
            this.Size = new System.Drawing.Size(661, 412);
            this.groupBox1.ResumeLayout(false);
            this.testsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.SmartCardReaderChooser smartCardReaderChooser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel testsPanel;
        private System.Windows.Forms.Button getChallengeButton;
        private System.Windows.Forms.Button getReaderInformationButton;
    }
}
