namespace Cardon___Exportacion_SIFERE
{
    partial class VentanaSifere
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VentanaSifere));
            this.DTPFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.DTPFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.BtnExportar = new System.Windows.Forms.Button();
            this.LblFechaDesde = new System.Windows.Forms.Label();
            this.LblFechaHasta = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DTPFechaDesde
            // 
            this.DTPFechaDesde.Location = new System.Drawing.Point(13, 29);
            this.DTPFechaDesde.Name = "DTPFechaDesde";
            this.DTPFechaDesde.Size = new System.Drawing.Size(200, 20);
            this.DTPFechaDesde.TabIndex = 0;
            // 
            // DTPFechaHasta
            // 
            this.DTPFechaHasta.Location = new System.Drawing.Point(13, 90);
            this.DTPFechaHasta.Name = "DTPFechaHasta";
            this.DTPFechaHasta.Size = new System.Drawing.Size(200, 20);
            this.DTPFechaHasta.TabIndex = 1;
            // 
            // BtnExportar
            // 
            this.BtnExportar.Location = new System.Drawing.Point(239, 126);
            this.BtnExportar.Name = "BtnExportar";
            this.BtnExportar.Size = new System.Drawing.Size(75, 23);
            this.BtnExportar.TabIndex = 2;
            this.BtnExportar.Text = "Exportar";
            this.BtnExportar.UseVisualStyleBackColor = true;
            this.BtnExportar.Click += new System.EventHandler(this.BtnExportar_Click);
            // 
            // LblFechaDesde
            // 
            this.LblFechaDesde.AutoSize = true;
            this.LblFechaDesde.Location = new System.Drawing.Point(13, 13);
            this.LblFechaDesde.Name = "LblFechaDesde";
            this.LblFechaDesde.Size = new System.Drawing.Size(71, 13);
            this.LblFechaDesde.TabIndex = 3;
            this.LblFechaDesde.Text = "Fecha Desde";
            // 
            // LblFechaHasta
            // 
            this.LblFechaHasta.AutoSize = true;
            this.LblFechaHasta.Location = new System.Drawing.Point(16, 71);
            this.LblFechaHasta.Name = "LblFechaHasta";
            this.LblFechaHasta.Size = new System.Drawing.Size(68, 13);
            this.LblFechaHasta.TabIndex = 4;
            this.LblFechaHasta.Text = "Fecha Hasta";
            // 
            // VentanaSifere
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 161);
            this.Controls.Add(this.LblFechaHasta);
            this.Controls.Add(this.LblFechaDesde);
            this.Controls.Add(this.BtnExportar);
            this.Controls.Add(this.DTPFechaHasta);
            this.Controls.Add(this.DTPFechaDesde);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(350, 200);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 200);
            this.Name = "VentanaSifere";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cardon - Exportación SIFERE";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker DTPFechaDesde;
        private System.Windows.Forms.DateTimePicker DTPFechaHasta;
        private System.Windows.Forms.Button BtnExportar;
        private System.Windows.Forms.Label LblFechaDesde;
        private System.Windows.Forms.Label LblFechaHasta;
    }
}

