namespace IconMaker_1._0
{
    partial class NuevoIcono
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NuevoIcono));
            this.comboBox_DimensionHojaTrab = new System.Windows.Forms.ComboBox();
            this.label_DimensionHojaT = new System.Windows.Forms.Label();
            this.button_Crear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox_DimensionHojaTrab
            // 
            this.comboBox_DimensionHojaTrab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DimensionHojaTrab.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_DimensionHojaTrab.FormattingEnabled = true;
            this.comboBox_DimensionHojaTrab.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256",
            "512"});
            this.comboBox_DimensionHojaTrab.Location = new System.Drawing.Point(110, 32);
            this.comboBox_DimensionHojaTrab.MaxLength = 3;
            this.comboBox_DimensionHojaTrab.Name = "comboBox_DimensionHojaTrab";
            this.comboBox_DimensionHojaTrab.Size = new System.Drawing.Size(62, 21);
            this.comboBox_DimensionHojaTrab.TabIndex = 6;
            // 
            // label_DimensionHojaT
            // 
            this.label_DimensionHojaT.AutoSize = true;
            this.label_DimensionHojaT.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label_DimensionHojaT.Location = new System.Drawing.Point(27, 32);
            this.label_DimensionHojaT.Name = "label_DimensionHojaT";
            this.label_DimensionHojaT.Size = new System.Drawing.Size(56, 13);
            this.label_DimensionHojaT.TabIndex = 5;
            this.label_DimensionHojaT.Text = "Dimension";
            // 
            // button_Crear
            // 
            this.button_Crear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.button_Crear.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button_Crear.Location = new System.Drawing.Point(163, 72);
            this.button_Crear.Name = "button_Crear";
            this.button_Crear.Size = new System.Drawing.Size(55, 25);
            this.button_Crear.TabIndex = 4;
            this.button_Crear.Text = "Crear";
            this.button_Crear.UseVisualStyleBackColor = false;
            this.button_Crear.Click += new System.EventHandler(this.button_Crear_Click);
            // 
            // NuevoIcono
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(245, 128);
            this.Controls.Add(this.comboBox_DimensionHojaTrab);
            this.Controls.Add(this.label_DimensionHojaT);
            this.Controls.Add(this.button_Crear);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NuevoIcono";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NuevoIcono";
            this.Load += new System.EventHandler(this.NuevoIcono_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox comboBox_DimensionHojaTrab;
        private System.Windows.Forms.Label label_DimensionHojaT;
        public System.Windows.Forms.Button button_Crear;
    }
}