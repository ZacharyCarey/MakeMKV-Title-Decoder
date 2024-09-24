namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator {
    partial class DelayEditorForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            ApplyBtn = new Button();
            ClipSyncRadioBtn = new RadioButton();
            ManualRadioBtn = new RadioButton();
            ClipList = new ComboBox();
            DelayMSBox = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)DelayMSBox).BeginInit();
            SuspendLayout();
            // 
            // ApplyBtn
            // 
            ApplyBtn.Location = new Point(191, 195);
            ApplyBtn.Name = "ApplyBtn";
            ApplyBtn.Size = new Size(75, 23);
            ApplyBtn.TabIndex = 0;
            ApplyBtn.Text = "Apply";
            ApplyBtn.UseVisualStyleBackColor = true;
            ApplyBtn.Click += ApplyBtn_Click;
            // 
            // ClipSyncRadioBtn
            // 
            ClipSyncRadioBtn.AutoSize = true;
            ClipSyncRadioBtn.Location = new Point(12, 12);
            ClipSyncRadioBtn.Name = "ClipSyncRadioBtn";
            ClipSyncRadioBtn.Size = new Size(123, 19);
            ClipSyncRadioBtn.TabIndex = 1;
            ClipSyncRadioBtn.TabStop = true;
            ClipSyncRadioBtn.Text = "Sync to clip length";
            ClipSyncRadioBtn.UseVisualStyleBackColor = true;
            ClipSyncRadioBtn.CheckedChanged += ClipSyncRadioBtn_CheckedChanged;
            // 
            // ManualRadioBtn
            // 
            ManualRadioBtn.AutoSize = true;
            ManualRadioBtn.Location = new Point(12, 100);
            ManualRadioBtn.Name = "ManualRadioBtn";
            ManualRadioBtn.Size = new Size(131, 19);
            ManualRadioBtn.TabIndex = 2;
            ManualRadioBtn.TabStop = true;
            ManualRadioBtn.Text = "Constant Value (ms)";
            ManualRadioBtn.UseVisualStyleBackColor = true;
            ManualRadioBtn.CheckedChanged += ManualRadioBtn_CheckedChanged;
            // 
            // ClipList
            // 
            ClipList.DrawMode = DrawMode.OwnerDrawFixed;
            ClipList.DropDownStyle = ComboBoxStyle.DropDownList;
            ClipList.FormattingEnabled = true;
            ClipList.Location = new Point(35, 37);
            ClipList.Name = "ClipList";
            ClipList.Size = new Size(231, 24);
            ClipList.TabIndex = 3;
            ClipList.DrawItem += ClipList_DrawItem;
            // 
            // DelayMSBox
            // 
            DelayMSBox.Location = new Point(35, 125);
            DelayMSBox.Maximum = new decimal(new int[] { 36000000, 0, 0, 0 });
            DelayMSBox.Name = "DelayMSBox";
            DelayMSBox.Size = new Size(231, 23);
            DelayMSBox.TabIndex = 4;
            // 
            // DelayEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 226);
            Controls.Add(DelayMSBox);
            Controls.Add(ClipList);
            Controls.Add(ManualRadioBtn);
            Controls.Add(ClipSyncRadioBtn);
            Controls.Add(ApplyBtn);
            Name = "DelayEditorForm";
            Text = "DelayEditorForm";
            FormClosing += DelayEditorForm_FormClosing;
            Load += DelayEditorForm_Load;
            ((System.ComponentModel.ISupportInitialize)DelayMSBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ApplyBtn;
        private RadioButton ClipSyncRadioBtn;
        private RadioButton ManualRadioBtn;
        private ComboBox ClipList;
        private NumericUpDown DelayMSBox;
    }
}