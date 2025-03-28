namespace TBD_Yaroshenko
{
    partial class radioNoInfo
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            довідкаToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button1 = new Button();
            errorProvider1 = new ErrorProvider(components);
            errorProvider2 = new ErrorProvider(components);
            groupBox1 = new GroupBox();
            button2 = new Button();
            checkBox2 = new CheckBox();
            button3 = new Button();
            checkBox1 = new CheckBox();
            panel1 = new Panel();
            label3 = new Label();
            comboBoxAccessControl = new ComboBox();
            groupBoxBruteForce = new GroupBox();
            btnStopDictionary = new Button();
            btnStartDictionary = new Button();
            chkNoInfo = new CheckBox();
            lblBruteStatus = new Label();
            txtBruteLog = new TextBox();
            lblProgress = new Label();
            progressBarBrute = new ProgressBar();
            btnStopBrute = new Button();
            btnStartBrute = new Button();
            chkDigits = new CheckBox();
            chkCyrillic = new CheckBox();
            chkUpperLatin = new CheckBox();
            chkSpecial = new CheckBox();
            chkLowerLatin = new CheckBox();
            lblCharSets = new Label();
            chkExactLength = new CheckBox();
            lblLength = new Label();
            numLength = new NumericUpDown();
            lblBruteUser = new Label();
            txtBruteUser = new TextBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider2).BeginInit();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            groupBoxBruteForce.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLength).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { довідкаToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(624, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // довідкаToolStripMenuItem
            // 
            довідкаToolStripMenuItem.Font = new Font("SimSun", 10.8F);
            довідкаToolStripMenuItem.Name = "довідкаToolStripMenuItem";
            довідкаToolStripMenuItem.Size = new Size(51, 20);
            довідкаToolStripMenuItem.Text = "Info";
            довідкаToolStripMenuItem.Click += довідкаToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("SimSun", 10.8F);
            label1.Location = new Point(20, 41);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 1;
            label1.Text = "Login";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("SimSun", 10.8F);
            label2.Location = new Point(20, 72);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 2;
            label2.Text = "Password";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(95, 38);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(156, 23);
            textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(95, 65);
            textBox2.Margin = new Padding(3, 2, 3, 2);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(156, 23);
            textBox2.TabIndex = 1;
            textBox2.UseSystemPasswordChar = true;
            // 
            // button1
            // 
            button1.Font = new Font("SimSun", 10.8F);
            button1.Location = new Point(0, 165);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(135, 31);
            button1.TabIndex = 2;
            button1.Text = "SignIn";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // errorProvider2
            // 
            errorProvider2.ContainerControl = this;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.FromArgb(224, 224, 224);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(checkBox2);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Location = new Point(10, 23);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(302, 237);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            // 
            // button2
            // 
            button2.Font = new Font("SimSun", 10.8F);
            button2.Location = new Point(0, 131);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(135, 30);
            button2.TabIndex = 6;
            button2.Text = "SignUp";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("SimSun", 10.8F);
            checkBox2.Location = new Point(6, 102);
            checkBox2.Margin = new Padding(3, 2, 3, 2);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(146, 19);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "Strong password";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Font = new Font("SimSun", 10.8F);
            button3.Location = new Point(0, 200);
            button3.Margin = new Padding(3, 2, 3, 2);
            button3.Name = "button3";
            button3.Size = new Size(135, 33);
            button3.TabIndex = 4;
            button3.Text = "Change password";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("SimSun", 10.8F);
            checkBox1.Location = new Point(156, 102);
            checkBox1.Margin = new Padding(3, 2, 3, 2);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(130, 19);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Show password";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(comboBoxAccessControl);
            panel1.Location = new Point(10, 266);
            panel1.Name = "panel1";
            panel1.Size = new Size(302, 103);
            panel1.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 17);
            label3.Name = "label3";
            label3.Size = new Size(229, 15);
            label3.TabIndex = 3;
            label3.Text = "Please select the type of access restriction:";
            // 
            // comboBoxAccessControl
            // 
            comboBoxAccessControl.FormattingEnabled = true;
            comboBoxAccessControl.Location = new Point(39, 49);
            comboBoxAccessControl.Name = "comboBoxAccessControl";
            comboBoxAccessControl.Size = new Size(177, 23);
            comboBoxAccessControl.TabIndex = 2;
            // 
            // groupBoxBruteForce
            // 
            groupBoxBruteForce.Controls.Add(btnStopDictionary);
            groupBoxBruteForce.Controls.Add(btnStartDictionary);
            groupBoxBruteForce.Controls.Add(chkNoInfo);
            groupBoxBruteForce.Controls.Add(lblBruteStatus);
            groupBoxBruteForce.Controls.Add(txtBruteLog);
            groupBoxBruteForce.Controls.Add(lblProgress);
            groupBoxBruteForce.Controls.Add(progressBarBrute);
            groupBoxBruteForce.Controls.Add(btnStopBrute);
            groupBoxBruteForce.Controls.Add(btnStartBrute);
            groupBoxBruteForce.Controls.Add(chkDigits);
            groupBoxBruteForce.Controls.Add(chkCyrillic);
            groupBoxBruteForce.Controls.Add(chkUpperLatin);
            groupBoxBruteForce.Controls.Add(chkSpecial);
            groupBoxBruteForce.Controls.Add(chkLowerLatin);
            groupBoxBruteForce.Controls.Add(lblCharSets);
            groupBoxBruteForce.Controls.Add(chkExactLength);
            groupBoxBruteForce.Controls.Add(lblLength);
            groupBoxBruteForce.Controls.Add(numLength);
            groupBoxBruteForce.Controls.Add(lblBruteUser);
            groupBoxBruteForce.Controls.Add(txtBruteUser);
            groupBoxBruteForce.Location = new Point(355, 23);
            groupBoxBruteForce.Name = "groupBoxBruteForce";
            groupBoxBruteForce.Size = new Size(246, 426);
            groupBoxBruteForce.TabIndex = 5;
            groupBoxBruteForce.TabStop = false;
            groupBoxBruteForce.Text = "Brute Force Password Cracker";
            // 
            // btnStopDictionary
            // 
            btnStopDictionary.Location = new Point(139, 321);
            btnStopDictionary.Name = "btnStopDictionary";
            btnStopDictionary.Size = new Size(75, 23);
            btnStopDictionary.TabIndex = 20;
            btnStopDictionary.Text = "Stop";
            btnStopDictionary.UseVisualStyleBackColor = true;
            btnStopDictionary.Click += btnStopDictionary_Click;
            // 
            // btnStartDictionary
            // 
            btnStartDictionary.Location = new Point(6, 321);
            btnStartDictionary.Name = "btnStartDictionary";
            btnStartDictionary.Size = new Size(113, 23);
            btnStartDictionary.TabIndex = 19;
            btnStartDictionary.Text = "Start Dictionary";
            btnStartDictionary.UseVisualStyleBackColor = true;
            btnStartDictionary.Click += btnStartDictionary_Click_1;
            // 
            // chkNoInfo
            // 
            chkNoInfo.AutoSize = true;
            chkNoInfo.Location = new Point(9, 256);
            chkNoInfo.Name = "chkNoInfo";
            chkNoInfo.Size = new Size(153, 19);
            chkNoInfo.TabIndex = 18;
            chkNoInfo.Text = "No info about password";
            chkNoInfo.UseVisualStyleBackColor = true;
            chkNoInfo.CheckedChanged += chkNoInfo_CheckedChanged_1;
            // 
            // lblBruteStatus
            // 
            lblBruteStatus.AutoSize = true;
            lblBruteStatus.Location = new Point(7, 405);
            lblBruteStatus.Name = "lblBruteStatus";
            lblBruteStatus.Size = new Size(39, 15);
            lblBruteStatus.TabIndex = 16;
            lblBruteStatus.Text = "Status";
            // 
            // txtBruteLog
            // 
            txtBruteLog.Location = new Point(7, 379);
            txtBruteLog.Name = "txtBruteLog";
            txtBruteLog.ReadOnly = true;
            txtBruteLog.ScrollBars = ScrollBars.Vertical;
            txtBruteLog.Size = new Size(205, 23);
            txtBruteLog.TabIndex = 15;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Location = new Point(156, 358);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(23, 15);
            lblProgress.TabIndex = 14;
            lblProgress.Text = "0%";
            // 
            // progressBarBrute
            // 
            progressBarBrute.Location = new Point(7, 350);
            progressBarBrute.Name = "progressBarBrute";
            progressBarBrute.Size = new Size(143, 23);
            progressBarBrute.TabIndex = 13;
            // 
            // btnStopBrute
            // 
            btnStopBrute.Location = new Point(139, 292);
            btnStopBrute.Name = "btnStopBrute";
            btnStopBrute.Size = new Size(75, 23);
            btnStopBrute.TabIndex = 11;
            btnStopBrute.Text = "Stop";
            btnStopBrute.UseVisualStyleBackColor = true;
            btnStopBrute.Click += btnStopBrute_Click_1;
            // 
            // btnStartBrute
            // 
            btnStartBrute.Location = new Point(6, 292);
            btnStartBrute.Name = "btnStartBrute";
            btnStartBrute.Size = new Size(113, 23);
            btnStartBrute.TabIndex = 10;
            btnStartBrute.Text = "Start Brute Force";
            btnStartBrute.UseVisualStyleBackColor = true;
            btnStartBrute.Click += btnStartBrute_Click_1;
            // 
            // chkDigits
            // 
            chkDigits.AutoSize = true;
            chkDigits.Checked = true;
            chkDigits.CheckState = CheckState.Checked;
            chkDigits.Location = new Point(9, 168);
            chkDigits.Name = "chkDigits";
            chkDigits.Size = new Size(84, 19);
            chkDigits.TabIndex = 9;
            chkDigits.Text = "Digits (0-9)";
            chkDigits.UseVisualStyleBackColor = true;
            // 
            // chkCyrillic
            // 
            chkCyrillic.AutoSize = true;
            chkCyrillic.Location = new Point(9, 218);
            chkCyrillic.Name = "chkCyrillic";
            chkCyrillic.Size = new Size(121, 19);
            chkCyrillic.TabIndex = 7;
            chkCyrillic.Text = "Cyrillic Characters";
            chkCyrillic.UseVisualStyleBackColor = true;
            // 
            // chkUpperLatin
            // 
            chkUpperLatin.AutoSize = true;
            chkUpperLatin.Checked = true;
            chkUpperLatin.CheckState = CheckState.Checked;
            chkUpperLatin.Location = new Point(9, 143);
            chkUpperLatin.Name = "chkUpperLatin";
            chkUpperLatin.Size = new Size(141, 19);
            chkUpperLatin.TabIndex = 8;
            chkUpperLatin.Text = "Uppercase Latin (A-Z)";
            chkUpperLatin.UseVisualStyleBackColor = true;
            // 
            // chkSpecial
            // 
            chkSpecial.AutoSize = true;
            chkSpecial.Checked = true;
            chkSpecial.CheckState = CheckState.Checked;
            chkSpecial.Location = new Point(9, 193);
            chkSpecial.Name = "chkSpecial";
            chkSpecial.Size = new Size(163, 19);
            chkSpecial.TabIndex = 7;
            chkSpecial.Text = "Special Characters (!@#...)";
            chkSpecial.UseVisualStyleBackColor = true;
            // 
            // chkLowerLatin
            // 
            chkLowerLatin.AutoSize = true;
            chkLowerLatin.Checked = true;
            chkLowerLatin.CheckState = CheckState.Checked;
            chkLowerLatin.Location = new Point(9, 118);
            chkLowerLatin.Name = "chkLowerLatin";
            chkLowerLatin.Size = new Size(137, 19);
            chkLowerLatin.TabIndex = 6;
            chkLowerLatin.Text = "Lowercase Latin (a-z)";
            chkLowerLatin.UseVisualStyleBackColor = true;
            // 
            // lblCharSets
            // 
            lblCharSets.AutoSize = true;
            lblCharSets.Location = new Point(9, 95);
            lblCharSets.Name = "lblCharSets";
            lblCharSets.Size = new Size(85, 15);
            lblCharSets.TabIndex = 5;
            lblCharSets.Text = "Character Sets:";
            // 
            // chkExactLength
            // 
            chkExactLength.AutoSize = true;
            chkExactLength.Checked = true;
            chkExactLength.CheckState = CheckState.Checked;
            chkExactLength.Location = new Point(131, 82);
            chkExactLength.Name = "chkExactLength";
            chkExactLength.Size = new Size(93, 19);
            chkExactLength.TabIndex = 4;
            chkExactLength.Text = "Exact Length";
            chkExactLength.UseVisualStyleBackColor = true;
            // 
            // lblLength
            // 
            lblLength.AutoSize = true;
            lblLength.Location = new Point(-2, 55);
            lblLength.Name = "lblLength";
            lblLength.Size = new Size(100, 15);
            lblLength.TabIndex = 3;
            lblLength.Text = "Password Length:";
            // 
            // numLength
            // 
            numLength.Location = new Point(104, 53);
            numLength.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            numLength.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numLength.Name = "numLength";
            numLength.Size = new Size(120, 23);
            numLength.TabIndex = 2;
            numLength.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // lblBruteUser
            // 
            lblBruteUser.AutoSize = true;
            lblBruteUser.Location = new Point(6, 29);
            lblBruteUser.Name = "lblBruteUser";
            lblBruteUser.Size = new Size(63, 15);
            lblBruteUser.TabIndex = 1;
            lblBruteUser.Text = "Username:";
            // 
            // txtBruteUser
            // 
            txtBruteUser.Location = new Point(75, 24);
            txtBruteUser.Name = "txtBruteUser";
            txtBruteUser.Size = new Size(149, 23);
            txtBruteUser.TabIndex = 0;
            // 
            // radioNoInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(624, 461);
            Controls.Add(groupBoxBruteForce);
            Controls.Add(panel1);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 2, 3, 2);
            Name = "radioNoInfo";
            Text = "TBD_Yaroshenko";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider2).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupBoxBruteForce.ResumeLayout(false);
            groupBoxBruteForce.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numLength).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem довідкаToolStripMenuItem;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private ErrorProvider errorProvider1;
        private ErrorProvider errorProvider2;
        private GroupBox groupBox1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Button button3;
        private Button button2;
        private Panel panel1;
        private Label label3;
        private ComboBox comboBoxAccessControl;
        private GroupBox groupBoxBruteForce;
        private Label lblBruteUser;
        private TextBox txtBruteUser;
        private CheckBox chkDigits;
        private CheckBox chkCyrillic;
        private CheckBox chkUpperLatin;
        private CheckBox chkSpecial;
        private CheckBox chkLowerLatin;
        private Label lblCharSets;
        private CheckBox chkExactLength;
        private Label lblLength;
        private NumericUpDown numLength;
        private Button btnStartBrute;
        private Button btnStopBrute;
        private ProgressBar progressBarBrute;
        private Label lblProgress;
        private TextBox txtBruteLog;
        private Label lblBruteStatus;
        private CheckBox chkNoInfo;
        private Button btnStopDictionary;
        private Button btnStartDictionary;
    }
}
