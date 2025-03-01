namespace TBD_Yaroshenko
{
    partial class FileManager
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
            listBoxFiles = new ListBox();
            pictureBox = new PictureBox();
            buttonRotate = new Button();
            btnSave = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // listBoxFiles
            // 
            listBoxFiles.Font = new Font("SimSun", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.ItemHeight = 18;
            listBoxFiles.Location = new Point(24, 31);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.Size = new Size(295, 274);
            listBoxFiles.TabIndex = 0;
            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.Transparent;
            pictureBox.Location = new Point(331, 31);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(422, 274);
            pictureBox.TabIndex = 1;
            pictureBox.TabStop = false;
            // 
            // buttonRotate
            // 
            buttonRotate.Font = new Font("SimSun", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonRotate.Location = new Point(402, 311);
            buttonRotate.Name = "buttonRotate";
            buttonRotate.Size = new Size(106, 26);
            buttonRotate.TabIndex = 2;
            buttonRotate.Text = "Rotate";
            buttonRotate.UseVisualStyleBackColor = true;
            buttonRotate.Click += buttonRotate_Click;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("SimSun", 10.8F);
            btnSave.Location = new Point(558, 311);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(106, 26);
            btnSave.TabIndex = 3;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // FileManager
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(777, 405);
            Controls.Add(btnSave);
            Controls.Add(buttonRotate);
            Controls.Add(pictureBox);
            Controls.Add(listBoxFiles);
            Font = new Font("SimSun", 10.8F);
            Name = "FileManager";
            Text = "FileManager";
            Load += FileManager_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBoxFiles;
        private PictureBox pictureBox;
        private Button buttonRotate;
        private Button btnSave;
    }
}