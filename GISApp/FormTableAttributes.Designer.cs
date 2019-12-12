namespace GISApp
{
    partial class FormTableAttributes
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
            this.listAttributes = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listAttributes
            // 
            this.listAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listAttributes.FullRowSelect = true;
            this.listAttributes.GridLines = true;
            this.listAttributes.HideSelection = false;
            this.listAttributes.Location = new System.Drawing.Point(0, 0);
            this.listAttributes.Name = "listAttributes";
            this.listAttributes.Size = new System.Drawing.Size(458, 364);
            this.listAttributes.TabIndex = 0;
            this.listAttributes.UseCompatibleStateImageBehavior = false;
            this.listAttributes.View = System.Windows.Forms.View.Details;
            // 
            // FormTableAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 364);
            this.Controls.Add(this.listAttributes);
            this.Name = "FormTableAttributes";
            this.Text = "Attributes Of Features";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listAttributes;
    }
}