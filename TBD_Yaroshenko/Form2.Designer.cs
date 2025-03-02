namespace TBD_Yaroshenko
{
    partial class Form2
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
            buttonFileManager = new Button();
            button2 = new Button();
            button_settings = new Button();
            panelSettings = new Panel();
            panelActions = new Panel();
            btnSaveRole = new Button();
            radioButtonUser = new RadioButton();
            radioButtonDeveloper = new RadioButton();
            radioButtonAdmin = new RadioButton();
            labelSelectedUser = new Label();
            panelFileActions = new Panel();
            btnSaveConfidentiality = new Button();
            radioButtonPublic = new RadioButton();
            radioButtonConfidential = new RadioButton();
            radioButtonSecret = new RadioButton();
            labelSelectedFile = new Label();
            label2 = new Label();
            label1 = new Label();
            comboBoxFiles = new ComboBox();
            comboBoxUsers = new ComboBox();
            panelSettings.SuspendLayout();
            panelActions.SuspendLayout();
            panelFileActions.SuspendLayout();
            SuspendLayout();
            // 
            // buttonFileManager
            // 
            buttonFileManager.Font = new Font("SimSun", 10.8F);
            buttonFileManager.Location = new Point(64, 39);
            buttonFileManager.Name = "buttonFileManager";
            buttonFileManager.Size = new Size(165, 29);
            buttonFileManager.TabIndex = 1;
            buttonFileManager.Text = "File Manager";
            buttonFileManager.UseVisualStyleBackColor = true;
            buttonFileManager.Click += buttonFileManager_Click_1;
            // 
            // button2
            // 
            button2.Font = new Font("SimSun", 10.8F);
            button2.Location = new Point(64, 340);
            button2.Name = "button2";
            button2.Size = new Size(165, 29);
            button2.TabIndex = 2;
            button2.Text = "LogOut";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button_settings
            // 
            button_settings.Font = new Font("SimSun", 10.8F);
            button_settings.Location = new Point(64, 74);
            button_settings.Name = "button_settings";
            button_settings.Size = new Size(165, 29);
            button_settings.TabIndex = 3;
            button_settings.Text = "Settings";
            button_settings.UseVisualStyleBackColor = true;
            button_settings.Click += button_settings_Click;
            // 
            // panelSettings
            // 
            panelSettings.BackColor = Color.FromArgb(224, 224, 224);
            panelSettings.BorderStyle = BorderStyle.FixedSingle;
            panelSettings.Controls.Add(panelActions);
            panelSettings.Controls.Add(panelFileActions);
            panelSettings.Controls.Add(label2);
            panelSettings.Controls.Add(label1);
            panelSettings.Controls.Add(comboBoxFiles);
            panelSettings.Controls.Add(comboBoxUsers);
            panelSettings.Location = new Point(337, 12);
            panelSettings.Name = "panelSettings";
            panelSettings.Size = new Size(489, 357);
            panelSettings.TabIndex = 4;
            panelSettings.Visible = false;
 
            // 
            // panelActions
            // 
            panelActions.BorderStyle = BorderStyle.Fixed3D;
            panelActions.Controls.Add(btnSaveRole);
            panelActions.Controls.Add(radioButtonUser);
            panelActions.Controls.Add(radioButtonDeveloper);
            panelActions.Controls.Add(radioButtonAdmin);
            panelActions.Controls.Add(labelSelectedUser);
            panelActions.Location = new Point(56, 134);
            panelActions.Name = "panelActions";
            panelActions.Size = new Size(387, 202);
            panelActions.TabIndex = 7;
            panelActions.Visible = false;

            // 
            // btnSaveRole
            // 
            btnSaveRole.Font = new Font("SimSun", 10.8F);
            btnSaveRole.Location = new Point(140, 151);
            btnSaveRole.Name = "btnSaveRole";
            btnSaveRole.Size = new Size(94, 29);
            btnSaveRole.TabIndex = 4;
            btnSaveRole.Text = "Save";
            btnSaveRole.UseVisualStyleBackColor = true;
            btnSaveRole.Click += btnSaveRole_Click;
            // 
            // radioButtonUser
            // 
            radioButtonUser.AutoSize = true;
            radioButtonUser.Font = new Font("SimSun", 10.8F);
            radioButtonUser.Location = new Point(13, 121);
            radioButtonUser.Name = "radioButtonUser";
            radioButtonUser.Size = new Size(65, 22);
            radioButtonUser.TabIndex = 3;
            radioButtonUser.TabStop = true;
            radioButtonUser.Text = "User";
            radioButtonUser.UseVisualStyleBackColor = true;
            radioButtonUser.CheckedChanged += radioButtonUser_CheckedChanged;
            // 
            // radioButtonDeveloper
            // 
            radioButtonDeveloper.AutoSize = true;
            radioButtonDeveloper.Font = new Font("SimSun", 10.8F);
            radioButtonDeveloper.Location = new Point(13, 91);
            radioButtonDeveloper.Name = "radioButtonDeveloper";
            radioButtonDeveloper.Size = new Size(110, 22);
            radioButtonDeveloper.TabIndex = 2;
            radioButtonDeveloper.TabStop = true;
            radioButtonDeveloper.Text = "Developer";
            radioButtonDeveloper.UseVisualStyleBackColor = true;
            radioButtonDeveloper.CheckedChanged += radioButtonDeveloper_CheckedChanged;
            // 
            // radioButtonAdmin
            // 
            radioButtonAdmin.AutoSize = true;
            radioButtonAdmin.Font = new Font("SimSun", 10.8F);
            radioButtonAdmin.Location = new Point(14, 61);
            radioButtonAdmin.Name = "radioButtonAdmin";
            radioButtonAdmin.Size = new Size(74, 22);
            radioButtonAdmin.TabIndex = 1;
            radioButtonAdmin.TabStop = true;
            radioButtonAdmin.Text = "Admin";
            radioButtonAdmin.UseVisualStyleBackColor = true;
            radioButtonAdmin.CheckedChanged += radioButtonAdmin_CheckedChanged;
            // 
            // labelSelectedUser
            // 
            labelSelectedUser.AutoSize = true;
            labelSelectedUser.BorderStyle = BorderStyle.Fixed3D;
            labelSelectedUser.Font = new Font("SimSun", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelSelectedUser.Location = new Point(14, 11);
            labelSelectedUser.Name = "labelSelectedUser";
            labelSelectedUser.Size = new Size(37, 15);
            labelSelectedUser.TabIndex = 0;
            labelSelectedUser.Text = "Name";
            // 
            // panelFileActions
            // 
            panelFileActions.BorderStyle = BorderStyle.Fixed3D;
            panelFileActions.Controls.Add(btnSaveConfidentiality);
            panelFileActions.Controls.Add(radioButtonPublic);
            panelFileActions.Controls.Add(radioButtonConfidential);
            panelFileActions.Controls.Add(radioButtonSecret);
            panelFileActions.Controls.Add(labelSelectedFile);
            panelFileActions.Location = new Point(56, 134);
            panelFileActions.Name = "panelFileActions";
            panelFileActions.Size = new Size(387, 202);
            panelFileActions.TabIndex = 6;
            panelFileActions.Visible = false;
            // 
            // btnSaveConfidentiality
            // 
            btnSaveConfidentiality.Font = new Font("SimSun", 10.8F);
            btnSaveConfidentiality.Location = new Point(140, 151);
            btnSaveConfidentiality.Name = "btnSaveConfidentiality";
            btnSaveConfidentiality.Size = new Size(94, 29);
            btnSaveConfidentiality.TabIndex = 4;
            btnSaveConfidentiality.Text = "Save";
            btnSaveConfidentiality.UseVisualStyleBackColor = true;
            btnSaveConfidentiality.Click += btnSaveConfidentiality_Click;
            // 
            // radioButtonPublic
            // 
            radioButtonPublic.AutoSize = true;
            radioButtonPublic.Font = new Font("SimSun", 10.8F);
            radioButtonPublic.Location = new Point(13, 121);
            radioButtonPublic.Name = "radioButtonPublic";
            radioButtonPublic.Size = new Size(83, 22);
            radioButtonPublic.TabIndex = 3;
            radioButtonPublic.TabStop = true;
            radioButtonPublic.Text = "Public";
            radioButtonPublic.UseVisualStyleBackColor = true;
            radioButtonPublic.CheckedChanged += radioButtonPublic_CheckedChanged;
            // 
            // radioButtonConfidential
            // 
            radioButtonConfidential.AutoSize = true;
            radioButtonConfidential.Font = new Font("SimSun", 10.8F);
            radioButtonConfidential.Location = new Point(13, 91);
            radioButtonConfidential.Name = "radioButtonConfidential";
            radioButtonConfidential.Size = new Size(137, 22);
            radioButtonConfidential.TabIndex = 2;
            radioButtonConfidential.TabStop = true;
            radioButtonConfidential.Text = "Confidential";
            radioButtonConfidential.UseVisualStyleBackColor = true;
            radioButtonConfidential.CheckedChanged += radioButtonConfidential_CheckedChanged;
            // 
            // radioButtonSecret
            // 
            radioButtonSecret.AutoSize = true;
            radioButtonSecret.Font = new Font("SimSun", 10.8F);
            radioButtonSecret.Location = new Point(14, 61);
            radioButtonSecret.Name = "radioButtonSecret";
            radioButtonSecret.Size = new Size(83, 22);
            radioButtonSecret.TabIndex = 1;
            radioButtonSecret.TabStop = true;
            radioButtonSecret.Text = "Secret";
            radioButtonSecret.UseVisualStyleBackColor = true;
            radioButtonSecret.CheckedChanged += radioButtonSecret_CheckedChanged;
            // 
            // labelSelectedFile
            // 
            labelSelectedFile.AutoSize = true;
            labelSelectedFile.BorderStyle = BorderStyle.Fixed3D;
            labelSelectedFile.Font = new Font("SimSun", 10.8F);
            labelSelectedFile.Location = new Point(14, 11);
            labelSelectedFile.Name = "labelSelectedFile";
            labelSelectedFile.Size = new Size(46, 20);
            labelSelectedFile.TabIndex = 0;
            labelSelectedFile.Text = "Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.BorderStyle = BorderStyle.Fixed3D;
            label2.Font = new Font("SimSun", 10.8F);
            label2.Location = new Point(22, 41);
            label2.Name = "label2";
            label2.Size = new Size(55, 20);
            label2.TabIndex = 5;
            label2.Text = "Users";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.BorderStyle = BorderStyle.Fixed3D;
            label1.Font = new Font("SimSun", 10.8F);
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(262, 41);
            label1.Name = "label1";
            label1.Size = new Size(55, 20);
            label1.TabIndex = 4;
            label1.Text = "Files";
            // 
            // comboBoxFiles
            // 
            comboBoxFiles.FormattingEnabled = true;
            comboBoxFiles.Location = new Point(262, 80);
            comboBoxFiles.Name = "comboBoxFiles";
            comboBoxFiles.Size = new Size(208, 28);
            comboBoxFiles.TabIndex = 3;
            comboBoxFiles.DropDown += comboBoxFiles_DropDown;
            comboBoxFiles.SelectedIndexChanged += comboBoxFiles_SelectedIndexChanged;
            // 
            // comboBoxUsers
            // 
            comboBoxUsers.FormattingEnabled = true;
            comboBoxUsers.Location = new Point(22, 80);
            comboBoxUsers.Name = "comboBoxUsers";
            comboBoxUsers.Size = new Size(194, 28);
            comboBoxUsers.TabIndex = 2;
            comboBoxUsers.DropDown += comboBoxUsers_DropDown;
            comboBoxUsers.SelectedIndexChanged += comboBoxUsers_SelectedIndexChanged;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(838, 381);
            Controls.Add(panelSettings);
            Controls.Add(button_settings);
            Controls.Add(button2);
            Controls.Add(buttonFileManager);
            Name = "Form2";
            Text = "MainWindow";
            Load += Form2_Load;
            panelSettings.ResumeLayout(false);
            panelSettings.PerformLayout();
            panelActions.ResumeLayout(false);
            panelActions.PerformLayout();
            panelFileActions.ResumeLayout(false);
            panelFileActions.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem вітаємоToolStripMenuItem;
        private Button buttonFileManager;
        private Button button2;
        private Button button_settings;
        private Panel panelSettings;
        private ComboBox comboBoxFiles;
        private ComboBox comboBoxUsers;
        private Label label1;
        private Label label2;
        private Label labelSelectedFile;
        private Panel panelFileActions;
        private Button btnSaveConfidentiality;
        private RadioButton radioButtonPublic;
        private RadioButton radioButtonConfidential;
        private RadioButton radioButtonSecret;
        private Panel panelActions;
        private Button btnSaveRole;
        private RadioButton radioButtonUser;
        private RadioButton radioButtonDeveloper;
        private RadioButton radioButtonAdmin;
        private Label labelSelectedUser;
    }
}