namespace Hdf5DotnetWrapper.Viewer
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
            this.hdF5FileLoader1 = new Hdf5DotnetWrapper.Viewer.HDF5FileLoader();
            this.SuspendLayout();
            // 
            // hdF5FileLoader1
            // 
            this.hdF5FileLoader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hdF5FileLoader1.Location = new System.Drawing.Point(0, 0);
            this.hdF5FileLoader1.Name = "hdF5FileLoader1";
            this.hdF5FileLoader1.Size = new System.Drawing.Size(800, 506);
            this.hdF5FileLoader1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 506);
            this.Controls.Add(this.hdF5FileLoader1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private HDF5FileLoader hdF5FileLoader1;
    }
}

