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
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(452, 24);
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(452, 292);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 2, 3, 2);
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
