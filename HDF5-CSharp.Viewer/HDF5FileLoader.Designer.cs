namespace Hdf5DotnetWrapper.Viewer
{
    partial class HDF5FileLoader
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label18 = new System.Windows.Forms.Label();
            this.txtbHDF5 = new System.Windows.Forms.TextBox();
            this.btnHDF5ReadFile = new System.Windows.Forms.Button();
            this.btnHDF5Browse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 8);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 18);
            this.label18.TabIndex = 10;
            this.label18.Text = "HDF5 File:";
            // 
            // txtbHDF5
            // 
            this.txtbHDF5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbHDF5.Location = new System.Drawing.Point(97, 4);
            this.txtbHDF5.Margin = new System.Windows.Forms.Padding(4);
            this.txtbHDF5.Name = "txtbHDF5";
            this.txtbHDF5.Size = new System.Drawing.Size(539, 26);
            this.txtbHDF5.TabIndex = 11;
            this.txtbHDF5.Text = "D:\\Data\\9_pig.h5";
            // 
            // btnHDF5ReadFile
            // 
            this.btnHDF5ReadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHDF5ReadFile.Location = new System.Drawing.Point(722, 1);
            this.btnHDF5ReadFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnHDF5ReadFile.Name = "btnHDF5ReadFile";
            this.btnHDF5ReadFile.Size = new System.Drawing.Size(100, 32);
            this.btnHDF5ReadFile.TabIndex = 13;
            this.btnHDF5ReadFile.Text = "Load Data";
            this.btnHDF5ReadFile.UseVisualStyleBackColor = true;
            this.btnHDF5ReadFile.Click += new System.EventHandler(this.btnHDF5ReadFile_Click);
            // 
            // btnHDF5Browse
            // 
            this.btnHDF5Browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHDF5Browse.Location = new System.Drawing.Point(644, 1);
            this.btnHDF5Browse.Margin = new System.Windows.Forms.Padding(4);
            this.btnHDF5Browse.Name = "btnHDF5Browse";
            this.btnHDF5Browse.Size = new System.Drawing.Size(69, 32);
            this.btnHDF5Browse.TabIndex = 12;
            this.btnHDF5Browse.Text = "Browse";
            this.btnHDF5Browse.UseVisualStyleBackColor = true;
            this.btnHDF5Browse.Click += new System.EventHandler(this.btnHDF5Browse_Click);
            // 
            // HDF5FileLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtbHDF5);
            this.Controls.Add(this.btnHDF5ReadFile);
            this.Controls.Add(this.btnHDF5Browse);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "HDF5FileLoader";
            this.Size = new System.Drawing.Size(826, 550);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtbHDF5;
        private System.Windows.Forms.Button btnHDF5ReadFile;
        private System.Windows.Forms.Button btnHDF5Browse;
    }
}
