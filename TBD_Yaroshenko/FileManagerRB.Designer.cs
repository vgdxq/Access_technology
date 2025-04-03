namespace TBD_Yaroshenko
{
    partial class FileManagerRB
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            labelAccessTime = new Label();
            textBoxFileContent = new RichTextBox();
            btnSave = new Button();
            buttonRotate = new Button();
            pictureBox = new PictureBox();
            listBoxFiles = new ListBox();
            accessTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // labelAccessTime
            // 
            labelAccessTime.AutoSize = true;
            labelAccessTime.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelAccessTime.Location = new Point(29, 359);
            labelAccessTime.Name = "labelAccessTime";
            labelAccessTime.Size = new Size(0, 15);
            labelAccessTime.TabIndex = 11;
            // 
            // textBoxFileContent
            // 
            textBoxFileContent.Location = new Point(321, 28);
            textBoxFileContent.Name = "textBoxFileContent";
            textBoxFileContent.ReadOnly = true;
            textBoxFileContent.ScrollBars = RichTextBoxScrollBars.Vertical;
            textBoxFileContent.Size = new Size(428, 274);
            textBoxFileContent.TabIndex = 10;
            textBoxFileContent.Text = "";
            textBoxFileContent.Visible = false;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            btnSave.Location = new Point(554, 308);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(106, 26);
            btnSave.TabIndex = 9;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // buttonRotate
            // 
            buttonRotate.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            buttonRotate.Location = new Point(398, 308);
            buttonRotate.Name = "buttonRotate";
            buttonRotate.Size = new Size(106, 26);
            buttonRotate.TabIndex = 8;
            buttonRotate.Text = "Rotate";
            buttonRotate.UseVisualStyleBackColor = true;
            buttonRotate.Click += buttonRotate_Click;
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Location = new Point(321, 28);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(428, 274);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 7;
            pictureBox.TabStop = false;
            pictureBox.Visible = false;
            // 
            // listBoxFiles
            // 
            listBoxFiles.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.ItemHeight = 15;
            listBoxFiles.Location = new Point(20, 28);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.Size = new Size(295, 259);
            listBoxFiles.TabIndex = 6;
            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;
            // 
            // accessTimer
            // 
            accessTimer.Tick += accessTimer_Tick;
            // 
            // FileManagerRB
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 407);
            Controls.Add(labelAccessTime);
            Controls.Add(textBoxFileContent);
            Controls.Add(btnSave);
            Controls.Add(buttonRotate);
            Controls.Add(pictureBox);
            Controls.Add(listBoxFiles);
            Name = "FileManagerRB";
            Text = "FileManager";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelAccessTime;
        private RichTextBox textBoxFileContent;
        private Button btnSave;
        private Button buttonRotate;
        private PictureBox pictureBox;
        private ListBox listBoxFiles;
        private System.Windows.Forms.Timer accessTimer;
    }
}