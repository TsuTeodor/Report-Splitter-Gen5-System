using System.Drawing;
using System.Windows.Forms;

namespace SplitterGen5_SystemTest_v2
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.HTML_input = new System.Windows.Forms.TextBox();
            this.html_output = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_NoTcStarts = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.NoTcStarts_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(147, 210);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 78);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(356, 67);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 22);
            this.button2.TabIndex = 3;
            this.button2.Text = "select HTML input";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(356, 107);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 22);
            this.button3.TabIndex = 4;
            this.button3.Text = "select output folder";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // HTML_input
            // 
            this.HTML_input.Location = new System.Drawing.Point(147, 71);
            this.HTML_input.Name = "HTML_input";
            this.HTML_input.Size = new System.Drawing.Size(191, 20);
            this.HTML_input.TabIndex = 5;
            // 
            // html_output
            // 
            this.html_output.Location = new System.Drawing.Point(147, 108);
            this.html_output.Name = "html_output";
            this.html_output.Size = new System.Drawing.Size(191, 20);
            this.html_output.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(435, 360);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "HatzzTools";
            // 
            // textBox_NoTcStarts
            // 
            this.textBox_NoTcStarts.Location = new System.Drawing.Point(147, 148);
            this.textBox_NoTcStarts.Name = "textBox_NoTcStarts";
            this.textBox_NoTcStarts.Size = new System.Drawing.Size(191, 20);
            this.textBox_NoTcStarts.TabIndex = 8;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(356, 146);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(99, 22);
            this.button4.TabIndex = 9;
            this.button4.Text = "select NoTcStarts";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button5.Location = new System.Drawing.Point(12, 351);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(116, 31);
            this.button5.TabIndex = 9;
            this.button5.Text = "about NoTcStarts";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // NoTcStarts_Label
            // 
            this.NoTcStarts_Label.AutoSize = true;
            this.NoTcStarts_Label.Location = new System.Drawing.Point(144, 183);
            this.NoTcStarts_Label.Name = "NoTcStarts_Label";
            this.NoTcStarts_Label.Size = new System.Drawing.Size(61, 13);
            this.NoTcStarts_Label.TabIndex = 10;
            this.NoTcStarts_Label.Text = "NoTcStarts";
            this.NoTcStarts_Label.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 390);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.NoTcStarts_Label);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox_NoTcStarts);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.html_output);
            this.Controls.Add(this.HTML_input);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "ReportSpliterGen5";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private TextBox HTML_input;
        private TextBox html_output;
        private Label label1;
        private TextBox textBox_NoTcStarts;
        private Button button4;
        private Label NoTcStarts_Label;
        private Button button5;
    }
}

