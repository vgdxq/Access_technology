namespace TBD_Yaroshenko
{
    partial class MainWind_RoleBased
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
            SettingMenu = new GroupBox();
            button_SaveRoleChange = new Button();
            comboBox_UserRole = new ComboBox();
            label1 = new Label();
            comboBox_Users = new ComboBox();
            button_ApplyFileLevel = new Button();
            comboBox_NewFileLevel = new ComboBox();
            label2 = new Label();
            comboBox_Files = new ComboBox();
            button_LogOut = new Button();
            button_Settings = new Button();
            button_FileManager = new Button();
            button_FileSet = new Button();
            groupBox_Files = new GroupBox();
            button_RefreshFiles = new Button();
            dataGridView_AccessRights = new DataGridView();
            button_SaveAccessRights = new Button();
            SettingMenu.SuspendLayout();
            groupBox_Files.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_AccessRights).BeginInit();
            SuspendLayout();
            // 
            // SettingMenu
            // 
            SettingMenu.Controls.Add(button_SaveRoleChange);
            SettingMenu.Controls.Add(comboBox_UserRole);
            SettingMenu.Controls.Add(label1);
            SettingMenu.Controls.Add(comboBox_Users);
            SettingMenu.Location = new Point(286, 28);
            SettingMenu.Name = "SettingMenu";
            SettingMenu.Size = new Size(222, 199);
            SettingMenu.TabIndex = 11;
            SettingMenu.TabStop = false;
            SettingMenu.Visible = false;
            // 
            // button_SaveRoleChange
            // 
            button_SaveRoleChange.Location = new Point(121, 129);
            button_SaveRoleChange.Name = "button_SaveRoleChange";
            button_SaveRoleChange.Size = new Size(75, 23);
            button_SaveRoleChange.TabIndex = 10;
            button_SaveRoleChange.Text = "Save";
            button_SaveRoleChange.UseVisualStyleBackColor = true;
            button_SaveRoleChange.Click += button_SaveRoleChange_Click_1;
            // 
            // comboBox_UserRole
            // 
            comboBox_UserRole.FormattingEnabled = true;
            comboBox_UserRole.Location = new Point(10, 94);
            comboBox_UserRole.Name = "comboBox_UserRole";
            comboBox_UserRole.Size = new Size(186, 23);
            comboBox_UserRole.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 47);
            label1.Name = "label1";
            label1.Size = new Size(35, 15);
            label1.TabIndex = 6;
            label1.Text = "Users";
            // 
            // comboBox_Users
            // 
            comboBox_Users.FormattingEnabled = true;
            comboBox_Users.Location = new Point(70, 44);
            comboBox_Users.Name = "comboBox_Users";
            comboBox_Users.Size = new Size(126, 23);
            comboBox_Users.TabIndex = 4;
            // 
            // button_ApplyFileLevel
            // 
            button_ApplyFileLevel.Location = new Point(193, 361);
            button_ApplyFileLevel.Name = "button_ApplyFileLevel";
            button_ApplyFileLevel.Size = new Size(75, 23);
            button_ApplyFileLevel.TabIndex = 11;
            button_ApplyFileLevel.Text = "Save";
            button_ApplyFileLevel.UseVisualStyleBackColor = true;
            button_ApplyFileLevel.Click += button_ApplyFileLevel_Click;
            // 
            // comboBox_NewFileLevel
            // 
            comboBox_NewFileLevel.FormattingEnabled = true;
            comboBox_NewFileLevel.Location = new Point(28, 94);
            comboBox_NewFileLevel.Name = "comboBox_NewFileLevel";
            comboBox_NewFileLevel.Size = new Size(171, 23);
            comboBox_NewFileLevel.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(28, 47);
            label2.Name = "label2";
            label2.Size = new Size(30, 15);
            label2.TabIndex = 7;
            label2.Text = "Files";
            // 
            // comboBox_Files
            // 
            comboBox_Files.FormattingEnabled = true;
            comboBox_Files.Location = new Point(73, 44);
            comboBox_Files.Name = "comboBox_Files";
            comboBox_Files.Size = new Size(126, 23);
            comboBox_Files.TabIndex = 5;
            comboBox_Files.SelectedIndexChanged += comboBox_Files_SelectedIndexChanged;
            // 
            // button_LogOut
            // 
            button_LogOut.Location = new Point(51, 394);
            button_LogOut.Name = "button_LogOut";
            button_LogOut.Size = new Size(125, 23);
            button_LogOut.TabIndex = 10;
            button_LogOut.Text = "LogOut";
            button_LogOut.UseVisualStyleBackColor = true;
            button_LogOut.Click += button_LogOut_Click;
            // 
            // button_Settings
            // 
            button_Settings.Location = new Point(30, 72);
            button_Settings.Name = "button_Settings";
            button_Settings.Size = new Size(135, 23);
            button_Settings.TabIndex = 9;
            button_Settings.Text = "User settings";
            button_Settings.UseVisualStyleBackColor = true;
            button_Settings.Click += button_Settings_Click_1;
            // 
            // button_FileManager
            // 
            button_FileManager.Location = new Point(30, 43);
            button_FileManager.Name = "button_FileManager";
            button_FileManager.Size = new Size(137, 23);
            button_FileManager.TabIndex = 8;
            button_FileManager.Text = "FileManager";
            button_FileManager.UseVisualStyleBackColor = true;
            button_FileManager.Click += button_FileManager_Click;
            // 
            // button_FileSet
            // 
            button_FileSet.Location = new Point(30, 101);
            button_FileSet.Name = "button_FileSet";
            button_FileSet.Size = new Size(135, 23);
            button_FileSet.TabIndex = 12;
            button_FileSet.Text = "File settings";
            button_FileSet.UseVisualStyleBackColor = true;
            button_FileSet.Click += button_FileSet_Click_1;
            // 
            // groupBox_Files
            // 
            groupBox_Files.Controls.Add(button_SaveAccessRights);
            groupBox_Files.Controls.Add(button_RefreshFiles);
            groupBox_Files.Controls.Add(dataGridView_AccessRights);
            groupBox_Files.Controls.Add(button_ApplyFileLevel);
            groupBox_Files.Controls.Add(comboBox_NewFileLevel);
            groupBox_Files.Controls.Add(comboBox_Files);
            groupBox_Files.Controls.Add(label2);
            groupBox_Files.Location = new Point(514, 28);
            groupBox_Files.Name = "groupBox_Files";
            groupBox_Files.Size = new Size(274, 418);
            groupBox_Files.TabIndex = 13;
            groupBox_Files.TabStop = false;
            groupBox_Files.Visible = false;
            // 
            // button_RefreshFiles
            // 
            button_RefreshFiles.Location = new Point(6, 390);
            button_RefreshFiles.Name = "button_RefreshFiles";
            button_RefreshFiles.Size = new Size(58, 23);
            button_RefreshFiles.TabIndex = 15;
            button_RefreshFiles.Text = "Refresh";
            button_RefreshFiles.UseVisualStyleBackColor = true;
            button_RefreshFiles.Click += button_RefreshFiles_Click_1;
            // 
            // dataGridView_AccessRights
            // 
            dataGridView_AccessRights.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_AccessRights.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_AccessRights.Location = new Point(6, 138);
            dataGridView_AccessRights.Name = "dataGridView_AccessRights";
            dataGridView_AccessRights.Size = new Size(262, 217);
            dataGridView_AccessRights.TabIndex = 14;
            // 
            // button_SaveAccessRights
            // 
            button_SaveAccessRights.Location = new Point(166, 390);
            button_SaveAccessRights.Name = "button_SaveAccessRights";
            button_SaveAccessRights.Size = new Size(108, 23);
            button_SaveAccessRights.TabIndex = 16;
            button_SaveAccessRights.Text = "Update access";
            button_SaveAccessRights.UseVisualStyleBackColor = true;
            button_SaveAccessRights.Click += button_SaveAccessRights_Click;
            // 
            // MainWind_RoleBased
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox_Files);
            Controls.Add(button_FileSet);
            Controls.Add(SettingMenu);
            Controls.Add(button_LogOut);
            Controls.Add(button_Settings);
            Controls.Add(button_FileManager);
            Name = "MainWind_RoleBased";
            Text = "MainWind_RoleBased";
            SettingMenu.ResumeLayout(false);
            SettingMenu.PerformLayout();
            groupBox_Files.ResumeLayout(false);
            groupBox_Files.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_AccessRights).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox SettingMenu;
        private Button button_ApplyFileLevel;
        private Button button_SaveRoleChange;
        private ComboBox comboBox_NewFileLevel;
        private ComboBox comboBox_UserRole;
        private Label label2;
        private Label label1;
        private ComboBox comboBox_Files;
        private ComboBox comboBox_Users;
        private Button button_LogOut;
        private Button button_Settings;
        private Button button_FileManager;
        private Button button_FileSet;
        private GroupBox groupBox_Files;
        private DataGridView dataGridView_AccessRights;
        private Button button_RefreshFiles;
        private Button button_SaveAccessRights;
    }
}