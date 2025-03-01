namespace TBD_Yaroshenko
{
    partial class Form1
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
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider2).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { довідкаToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(516, 26);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // довідкаToolStripMenuItem
            // 
            довідкаToolStripMenuItem.Font = new Font("SimSun", 10.8F);
            довідкаToolStripMenuItem.Name = "довідкаToolStripMenuItem";
            довідкаToolStripMenuItem.Size = new Size(58, 22);
            довідкаToolStripMenuItem.Text = "Info";
            довідкаToolStripMenuItem.Click += довідкаToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("SimSun", 10.8F);
            label1.Location = new Point(23, 55);
            label1.Name = "label1";
            label1.Size = new Size(53, 18);
            label1.TabIndex = 1;
            label1.Text = "Login";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("SimSun", 10.8F);
            label2.Location = new Point(23, 96);
            label2.Name = "label2";
            label2.Size = new Size(80, 18);
            label2.TabIndex = 2;
            label2.Text = "Password";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(109, 51);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(178, 27);
            textBox1.TabIndex = 0;
            textBox1.Leave += textBox1_Leave;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(109, 87);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(178, 27);
            textBox2.TabIndex = 1;
            textBox2.UseSystemPasswordChar = true;
            textBox2.Leave += textBox2_Leave;
            // 
            // button1
            // 
            button1.Font = new Font("SimSun", 10.8F);
            button1.Location = new Point(0, 220);
            button1.Name = "button1";
            button1.Size = new Size(154, 29);
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
            groupBox1.Location = new Point(12, 31);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(345, 316);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Enter += groupBox1_Enter;
            // 
            // button2
            // 
            button2.Font = new Font("SimSun", 10.8F);
            button2.Location = new Point(0, 175);
            button2.Name = "button2";
            button2.Size = new Size(154, 29);
            button2.TabIndex = 6;
            button2.Text = "SignUp";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("SimSun", 10.8F);
            checkBox2.Location = new Point(7, 136);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(165, 22);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "Strong password";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Font = new Font("SimSun", 10.8F);
            button3.Location = new Point(0, 267);
            button3.Name = "button3";
            button3.Size = new Size(154, 29);
            button3.TabIndex = 4;
            button3.Text = "Change password";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("SimSun", 10.8F);
            checkBox1.Location = new Point(178, 136);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(147, 22);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Show password";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(516, 389);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "TBD_Yaroshenko";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider2).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
    }
}
