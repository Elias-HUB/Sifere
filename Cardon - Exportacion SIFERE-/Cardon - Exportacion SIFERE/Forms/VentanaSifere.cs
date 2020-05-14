using System;
using System.IO;
using System.Net;
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

                //Verificar APi
                foreach (string baseDeDatos in singleton.basesDeDatos)
                {
                    var response = Controller.VerificadorConexionApi.VerificarConexionApi();

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            {
                                InitializeComponent();
                                DTPFechaDesde.Value = new DateTime(DTPFechaDesde.Value.Year, DTPFechaDesde.Value.Month, 1);
                                break;
                            }
                        case HttpStatusCode.NotFound:
                            {
                                InitializeComponent();
                                DTPFechaDesde.Value = new DateTime(DTPFechaDesde.Value.Year, DTPFechaDesde.Value.Month, 1);
                                break;
                            }
                        case HttpStatusCode.Unauthorized:
                            {
                                MessageBox.Show("Autorizacion invalida \r", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Program.ConsoleLog("Verificador Conexion: " + response.StatusCode);
                                Environment.Exit(0);
                                break;
                            }
                        case HttpStatusCode.BadRequest:
                            {
                                MessageBox.Show("Verificador Conexion \r", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Program.ConsoleLog("Verificador Conexion: " + response.StatusCode);
                                Environment.Exit(0);
                                break;
                            }
                        default:
                            MessageBox.Show("Verificador Conexion \r", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.ConsoleLog("Verificador Conexion: " + response.StatusCode);
                            Environment.Exit(0);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al exportar. \r", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        Services.OtrosPagosServices.GetOtrosPagos(baseDeDatos, DTPFechaDesde.Text, DTPFechaHasta.Text, singleton);

                        Services.OtrosPagosServices.Lproveedores.Clear();
                        Services.OtrosPagosServices.LConceptosOtrosPagos.Clear();
                    }

                    var nameFile = "SifereSujetos_" + DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-") + "_" + DateTime.Now.ToString("HHmmssffff") + ".txt";

                    //FOREACH LIST
                    foreach (Model.Sifere sifere in Services.OtrosPagosServices.LSifere)
                    {
                        using (StreamWriter file = new StreamWriter(singleton.rutaExportacion + nameFile, true))
                        {
                            file.WriteLine(sifere.CodigoJurisdiccion + sifere.Cuit + sifere.PeriodoRetencion + sifere.Cbu + sifere.TipoCuenta + sifere.TipoMoneda + sifere.ImporteRetenido);
                        }
                    }

                    MessageBox.Show("Exportacion finalizada con Exito", "",
                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exeptions.Exceptions Ex)
                {
                    MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Services.OtrosPagosServices.Lproveedores.Clear();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("   Error al exportar", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
