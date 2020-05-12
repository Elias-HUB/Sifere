using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cardon___Exportacion_SIFERE
{
    public partial class VentanaSifere : Form
    {
        static Model.Singleton singleton;
        

        public VentanaSifere()
        {
            try
            {
                singleton = Model.Singleton.Instance;

                InitializeComponent();
                DTPFechaDesde.Value = new DateTime(DTPFechaDesde.Value.Year, DTPFechaDesde.Value.Month, 1);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al exportar.");
                Program.ConsoleLog("VentanaSifere: " + e.Message);
            }

        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (singleton.basesDeDatos == null || singleton.rutaExportacion == "")
            {
                MessageBox.Show("Error en configuración. Verifique el log.");
            }
            else
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    foreach (string baseDeDatos in singleton.basesDeDatos)
                    {                        
                        Controller.OtrosPagosController.GetOtrosPagos(baseDeDatos, DTPFechaDesde.Text, DTPFechaHasta.Text);

                        Controller.OtrosPagosController.AProveedores.Clear();
                        Controller.OtrosPagosController.AConceptosOtrosPagos.Clear();
                    }
                    var nameFile = "SifereSujetos_" + DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-") + "_" + DateTime.Now.ToString("HHmmssffff") + ".txt";

                    //FOREACH LIST
                    foreach(Model.Sifere sifere in Controller.OtrosPagosController.LSifere)
                    {
                        using (StreamWriter file = new StreamWriter(singleton.rutaExportacion + nameFile, true))
                        {
                            file.WriteLine(sifere.CodigoJurisdiccion + sifere.Cuit + sifere.PeriodoRetencion + sifere.Cbu + sifere.TipoCuenta + sifere.TipoMoneda + sifere.ImporteRetenido);
                        }                            
                    }


                    MessageBox.Show("Exportacion finalizada.");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error al exportar.");
                    Program.ConsoleLog("buttonExportar: " + exc.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.AppStarting;
                }

            }
        }
    }
}
