﻿namespace QMS.ExportSisPic
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
            this.textBox4Coils = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox4Coils
            // 
            this.textBox4Coils.Location = new System.Drawing.Point(105, 60);
            this.textBox4Coils.Name = "textBox4Coils";
            this.textBox4Coils.Size = new System.Drawing.Size(359, 319);
            this.textBox4Coils.TabIndex = 0;
            this.textBox4Coils.Text = "";
            this.textBox4Coils.TextChanged += new System.EventHandler(this.textBox4Coils_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(643, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(170, 50);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 607);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox4Coils);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBox4Coils;
        private System.Windows.Forms.Button button1;
    }
}