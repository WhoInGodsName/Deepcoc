namespace Deepcoc
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
            label1 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            checkBox4 = new CheckBox();
            label2 = new Label();
            label3 = new Label();
            button1 = new Button();
            checkBox5 = new CheckBox();
            textBox1 = new TextBox();
            checkBox6 = new CheckBox();
            textBox2 = new TextBox();
            checkBox7 = new CheckBox();
            label4 = new Label();
            button2 = new Button();
            button3 = new Button();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(466, 18);
            label1.Margin = new Padding(7, 0, 7, 0);
            label1.Name = "label1";
            label1.Size = new Size(349, 39);
            label1.TabIndex = 0;
            label1.Text = "DEEPCOC MAPHATDIC 0.2";
            label1.Click += label1_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox1.Location = new Point(272, 123);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(125, 30);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "Lock Ammo";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox2.Location = new Point(272, 159);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(142, 30);
            checkBox2.TabIndex = 2;
            checkBox2.Text = "Lock Firerate";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox3.Location = new Point(29, 123);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(125, 30);
            checkBox3.TabIndex = 3;
            checkBox3.Text = "Lock Ammo";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox4.Location = new Point(29, 159);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(145, 30);
            checkBox4.TabIndex = 4;
            checkBox4.Text = "Lock FireRate";
            checkBox4.UseVisualStyleBackColor = true;
            checkBox4.CheckedChanged += checkBox4_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(260, 81);
            label2.Name = "label2";
            label2.Size = new Size(167, 39);
            label2.TabIndex = 5;
            label2.Text = "Secondary";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(28, 81);
            label3.Name = "label3";
            label3.Size = new Size(126, 39);
            label3.TabIndex = 6;
            label3.Text = "Primary";
            // 
            // button1
            // 
            button1.Font = new Font("Impact", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(12, 561);
            button1.Name = "button1";
            button1.Size = new Size(162, 47);
            button1.TabIndex = 7;
            button1.Text = "Reload Addresses";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Font = new Font("Impact", 18F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox5.Location = new Point(26, 251);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(59, 33);
            checkBox5.TabIndex = 8;
            checkBox5.Text = "Fly";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.Location = new Point(26, 215);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(91, 22);
            textBox1.TabIndex = 9;
            textBox1.Text = "Input speed";
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Font = new Font("Impact", 18F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox6.Location = new Point(177, 251);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(92, 33);
            checkBox6.TabIndex = 10;
            checkBox6.Text = "Speed";
            checkBox6.UseVisualStyleBackColor = true;
            checkBox6.CheckedChanged += checkBox6_CheckedChanged;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Arial", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            textBox2.Location = new Point(177, 215);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 22);
            textBox2.TabIndex = 11;
            textBox2.Text = "Input speed";
            // 
            // checkBox7
            // 
            checkBox7.AutoSize = true;
            checkBox7.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox7.Location = new Point(26, 319);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(156, 30);
            checkBox7.TabIndex = 12;
            checkBox7.Text = "Downed Meme";
            checkBox7.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(963, 81);
            label4.Name = "label4";
            label4.Size = new Size(163, 39);
            label4.TabIndex = 13;
            label4.Text = "Teleporter";
            // 
            // button2
            // 
            button2.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            button2.Location = new Point(963, 159);
            button2.Name = "button2";
            button2.Size = new Size(175, 42);
            button2.TabIndex = 14;
            button2.Text = "Save Location";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Font = new Font("Impact", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            button3.Location = new Point(963, 223);
            button3.Name = "button3";
            button3.Size = new Size(175, 36);
            button3.TabIndex = 15;
            button3.Text = "Teleport";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // textBox3
            // 
            textBox3.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            textBox3.Location = new Point(886, 123);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(88, 23);
            textBox3.TabIndex = 16;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // textBox4
            // 
            textBox4.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            textBox4.Location = new Point(999, 123);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(87, 23);
            textBox4.TabIndex = 17;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // textBox5
            // 
            textBox5.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            textBox5.Location = new Point(1109, 123);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(96, 23);
            textBox5.TabIndex = 18;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(16F, 39F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1288, 620);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label4);
            Controls.Add(checkBox7);
            Controls.Add(textBox2);
            Controls.Add(checkBox6);
            Controls.Add(textBox1);
            Controls.Add(checkBox5);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(checkBox4);
            Controls.Add(checkBox3);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label1);
            Font = new Font("Impact", 24F, FontStyle.Bold, GraphicsUnit.Point);
            Margin = new Padding(7);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private Label label2;
        private Label label3;
        private Button button1;
        private CheckBox checkBox5;
        private TextBox textBox1;
        private CheckBox checkBox6;
        private TextBox textBox2;
        private CheckBox checkBox7;
        private Label label4;
        private Button button2;
        private Button button3;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
    }
}