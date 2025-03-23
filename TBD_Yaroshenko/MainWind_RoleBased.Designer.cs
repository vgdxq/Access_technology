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
            button_SaveFileLevel = new Button();
            button_SaveUserLevel = new Button();
            comboBox_NewFileLevel = new ComboBox();
            comboBox_NewUserLevel = new ComboBox();
            label2 = new Label();
            label1 = new Label();
            comboBox_Files = new ComboBox();
            comboBox_Users = new ComboBox();
            button_LogOut = new Button();
            button_Settings = new Button();
            button_FileManager = new Button();
            SettingMenu.SuspendLayout();
            SuspendLayout();
            // 
            // SettingMenu
            // 
            SettingMenu.Controls.Add(button_SaveFileLevel);
            SettingMenu.Controls.Add(button_SaveUserLevel);
            SettingMenu.Controls.Add(comboBox_NewFileLevel);
            SettingMenu.Controls.Add(comboBox_NewUserLevel);
            SettingMenu.Controls.Add(label2);
            SettingMenu.Controls.Add(label1);
            SettingMenu.Controls.Add(comboBox_Files);
            SettingMenu.Controls.Add(comboBox_Users);
            SettingMenu.Location = new Point(286, 28);
            SettingMenu.Name = "SettingMenu";
            SettingMenu.Size = new Size(469, 192);
            SettingMenu.TabIndex = 11;
            SettingMenu.TabStop = false;
            // 
            // button_SaveFileLevel
            // 
            button_SaveFileLevel.Location = new Point(388, 144);
            button_SaveFileLevel.Name = "button_SaveFileLevel";
            button_SaveFileLevel.Size = new Size(75, 23);
            button_SaveFileLevel.TabIndex = 11;
            button_SaveFileLevel.Text = "Save";
            button_SaveFileLevel.UseVisualStyleBackColor = true;
            // 
            // button_SaveUserLevel
            // 
            button_SaveUserLevel.Location = new Point(121, 144);
            button_SaveUserLevel.Name = "button_SaveUserLevel";
            button_SaveUserLevel.Size = new Size(75, 23);
            button_SaveUserLevel.TabIndex = 10;
            button_SaveUserLevel.Text = "Save";
            button_SaveUserLevel.UseVisualStyleBackColor = true;
            // 
            // comboBox_NewFileLevel
            // 
            comboBox_NewFileLevel.FormattingEnabled = true;
            comboBox_NewFileLevel.Location = new Point(292, 94);
            comboBox_NewFileLevel.Name = "comboBox_NewFileLevel";
            comboBox_NewFileLevel.Size = new Size(171, 23);
            comboBox_NewFileLevel.TabIndex = 9;
            // 
            // comboBox_NewUserLevel
            // 
            comboBox_NewUserLevel.FormattingEnabled = true;
            comboBox_NewUserLevel.Location = new Point(10, 94);
            comboBox_NewUserLevel.Name = "comboBox_NewUserLevel";
            comboBox_NewUserLevel.Size = new Size(186, 23);
            comboBox_NewUserLevel.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(292, 47);
            label2.Name = "label2";
            label2.Size = new Size(30, 15);
            label2.TabIndex = 7;
            label2.Text = "Files";
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
            // comboBox_Files
            // 
            comboBox_Files.FormattingEnabled = true;
            comboBox_Files.Location = new Point(337, 44);
            comboBox_Files.Name = "comboBox_Files";
            comboBox_Files.Size = new Size(126, 23);
            comboBox_Files.TabIndex = 5;
            // 
            // comboBox_Users
            // 
            comboBox_Users.FormattingEnabled = true;
            comboBox_Users.Location = new Point(70, 44);
            comboBox_Users.Name = "comboBox_Users";
            comboBox_Users.Size = new Size(126, 23);
            comboBox_Users.TabIndex = 4;
            // 
            // button_LogOut
            // 
            button_LogOut.Location = new Point(51, 394);
            button_LogOut.Name = "button_LogOut";
            button_LogOut.Size = new Size(125, 23);
            button_LogOut.TabIndex = 10;
            button_LogOut.Text = "LogOut";
            button_LogOut.UseVisualStyleBackColor = true;
            // 
            // button_Settings
            // 
            button_Settings.Location = new Point(30, 72);
            button_Settings.Name = "button_Settings";
            button_Settings.Size = new Size(135, 23);
            button_Settings.TabIndex = 9;
            button_Settings.Text = "Settings";
            button_Settings.UseVisualStyleBackColor = true;
            // 
            // button_FileManager
            // 
            button_FileManager.Location = new Point(28, 28);
            button_FileManager.Name = "button_FileManager";
            button_FileManager.Size = new Size(137, 23);
            button_FileManager.TabIndex = 8;
            button_FileManager.Text = "FileManager";
            button_FileManager.UseVisualStyleBackColor = true;
            // 
            // MainWind_RoleBased
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(SettingMenu);
            Controls.Add(button_LogOut);
            Controls.Add(button_Settings);
            Controls.Add(button_FileManager);
            Name = "MainWind_RoleBased";
            Text = "MainWind_RoleBased";
            SettingMenu.ResumeLayout(false);
            SettingMenu.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox SettingMenu;
        private Button button_SaveFileLevel;
        private Button button_SaveUserLevel;
        private ComboBox comboBox_NewFileLevel;
        private ComboBox comboBox_NewUserLevel;
        private Label label2;
        private Label label1;
        private ComboBox comboBox_Files;
        private ComboBox comboBox_Users;
        private Button button_LogOut;
        private Button button_Settings;
        private Button button_FileManager;
    }
}