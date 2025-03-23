namespace TBD_Yaroshenko
{
    partial class MainWind_Discretionary
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
            checkBox_Execute = new CheckBox();
            checkBox_Writing = new CheckBox();
            checkBox_Reading = new CheckBox();
            button_GrantAccess = new Button();
            label2 = new Label();
            label1 = new Label();
            comboBox_Files = new ComboBox();
            comboBox_Users = new ComboBox();
            button_LogOut = new Button();
            button_FileManager = new Button();
            SettingMenu.SuspendLayout();
            SuspendLayout();
            // 
            // SettingMenu
            // 
            SettingMenu.Controls.Add(checkBox_Execute);
            SettingMenu.Controls.Add(checkBox_Writing);
            SettingMenu.Controls.Add(checkBox_Reading);
            SettingMenu.Controls.Add(button_GrantAccess);
            SettingMenu.Controls.Add(label2);
            SettingMenu.Controls.Add(label1);
            SettingMenu.Controls.Add(comboBox_Files);
            SettingMenu.Controls.Add(comboBox_Users);
            SettingMenu.Location = new Point(309, 30);
            SettingMenu.Name = "SettingMenu";
            SettingMenu.Size = new Size(357, 192);
            SettingMenu.TabIndex = 7;
            SettingMenu.TabStop = false;
            // 
            // checkBox_Execute
            // 
            checkBox_Execute.AutoSize = true;
            checkBox_Execute.Location = new Point(233, 96);
            checkBox_Execute.Name = "checkBox_Execute";
            checkBox_Execute.Size = new Size(66, 19);
            checkBox_Execute.TabIndex = 13;
            checkBox_Execute.Text = "Execute";
            checkBox_Execute.UseVisualStyleBackColor = true;
            // 
            // checkBox_Writing
            // 
            checkBox_Writing.AutoSize = true;
            checkBox_Writing.Location = new Point(233, 71);
            checkBox_Writing.Name = "checkBox_Writing";
            checkBox_Writing.Size = new Size(65, 19);
            checkBox_Writing.TabIndex = 12;
            checkBox_Writing.Text = "Writing";
            checkBox_Writing.UseVisualStyleBackColor = true;
            // 
            // checkBox_Reading
            // 
            checkBox_Reading.AutoSize = true;
            checkBox_Reading.Location = new Point(233, 46);
            checkBox_Reading.Name = "checkBox_Reading";
            checkBox_Reading.Size = new Size(69, 19);
            checkBox_Reading.TabIndex = 11;
            checkBox_Reading.Text = "Reading";
            checkBox_Reading.UseVisualStyleBackColor = true;
            // 
            // button_GrantAccess
            // 
            button_GrantAccess.Location = new Point(121, 163);
            button_GrantAccess.Name = "button_GrantAccess";
            button_GrantAccess.Size = new Size(75, 23);
            button_GrantAccess.TabIndex = 10;
            button_GrantAccess.Text = "Save";
            button_GrantAccess.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 102);
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
            comboBox_Files.Location = new Point(70, 102);
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
            button_LogOut.Location = new Point(12, 404);
            button_LogOut.Name = "button_LogOut";
            button_LogOut.Size = new Size(137, 23);
            button_LogOut.TabIndex = 6;
            button_LogOut.Text = "LogOut";
            button_LogOut.UseVisualStyleBackColor = true;
            button_LogOut.Click += button_LogOut_Click;
            // 
            // button_FileManager
            // 
            button_FileManager.Location = new Point(22, 30);
            button_FileManager.Name = "button_FileManager";
            button_FileManager.Size = new Size(137, 23);
            button_FileManager.TabIndex = 4;
            button_FileManager.Text = "FileManager";
            button_FileManager.UseVisualStyleBackColor = true;
            button_FileManager.Click += button_FileManager_Click;
            // 
            // MainWind_Discretionary
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(692, 439);
            Controls.Add(SettingMenu);
            Controls.Add(button_LogOut);
            Controls.Add(button_FileManager);
            Name = "MainWind_Discretionary";
            Text = "MainWind_Discretionary";
            SettingMenu.ResumeLayout(false);
            SettingMenu.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox SettingMenu;
        private Button button_GrantAccess;
        private Label label2;
        private Label label1;
        private ComboBox comboBox_Files;
        private ComboBox comboBox_Users;
        private Button button_LogOut;
        private Button button_FileManager;
        private CheckBox checkBox_Execute;
        private CheckBox checkBox_Writing;
        private CheckBox checkBox_Reading;
    }
}