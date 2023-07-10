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
            checkBox1.Location = new Point(24, 108);
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
            checkBox2.Location = new Point(24, 144);
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
            checkBox3.Location = new Point(320, 108);
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
            checkBox4.Location = new Point(320, 144);
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
            label2.Location = new Point(12, 66);
            label2.Name = "label2";
            label2.Size = new Size(167, 39);
            label2.TabIndex = 5;
            label2.Text = "Secondary";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(319, 66);
            label3.Name = "label3";
            label3.Size = new Size(126, 39);
            label3.TabIndex = 6;
            label3.Text = "Primary";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(16F, 39F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1288, 620);
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
    }
}