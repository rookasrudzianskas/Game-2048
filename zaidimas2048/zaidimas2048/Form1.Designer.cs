namespace zaidimas2048
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
            this.bnt4x4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bnt4x4
            // 
            this.bnt4x4.Location = new System.Drawing.Point(12, 12);
            this.bnt4x4.Name = "bnt4x4";
            this.bnt4x4.Size = new System.Drawing.Size(334, 24);
            this.bnt4x4.TabIndex = 0;
            this.bnt4x4.Text = "4x4";
            this.bnt4x4.UseVisualStyleBackColor = true;
            this.bnt4x4.Click += new System.EventHandler(this.bnt4x4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 46);
            this.Controls.Add(this.bnt4x4);
            this.Name = "Form1";
            this.Text = "2048";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnt4x4;
    }
}

