using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IO;
using System.Net;
using Cardon___Exportacion_SIFERE.Model;

namespace Cardon___Exportacion_SIFERE.Controller
{
    public class OtrosPagosController
    {

        static public JArray AProveedores = new JArray();
        static public JArray AConceptosOtrosPagos = new JArray();
        private static List<Model.Sifere> lSifere = new List<Model.Sifere>();

        internal static List<Sifere> LSifere { get => lSifere; set => lSifere = value; }

        static public void GetOtrosPagos(string baseDeDatos, string FechaDesde, string FechaHasta)
        {
            string Siguiente = "Siguiente";
            int Page = 1;

            while (Siguiente != "")
            {
                var responseRet = Comprobantepago(Page++, baseDeDatos);
                switch (responseRet.StatusCode)
                {
                    case HttpStatusCode.OK:
                        {
                            try
                            {
                                var jsonRet = JObject.Parse(responseRet.Content);
                                
                                Siguiente = jsonRet["Siguiente"].ToString();

                                for (int ret = 0; ret < int.Parse(jsonRet["CountResultado"].ToString()); ret++)
                                {
                                    var codProveedor = jsonRet["Resultados"][ret]["Proveedor"].ToString();
                                    var fecha = DateTime.Parse(jsonRet["Resultados"][ret]["Fecha"].ToString());

                                    if ((fecha >= DateTime.Parse(FechaDesde)) && (fecha <= DateTime.Parse(FechaHasta)))
                                    {
                                        if (codProveedor != "")
                                        {
                                            bool provExiste = false;
                                            //JObject ObjProveedor = null;
                                            dynamic ObjProveedor = null;
                                            foreach (JObject proveedor in AProveedores)
                                            {
                                                if (proveedor.GetValue("Codigo").ToString() == codProveedor)
                                                {
                                                    ObjProveedor = new JObject();
                                                    ObjProveedor.Codigo = proveedor.GetValue("Codigo").ToString();
                                                    ObjProveedor.Cuit = proveedor.GetValue("Cuit").ToString();
                                                    ObjProveedor.Cbu = proveedor.GetValue("Cbu").ToString();
                                                    ObjProveedor.TipoCuenta = proveedor.GetValue("TipoCuenta").ToString();
                                                    provExiste = true;

                                                   // ObjProveedor.Add(new JProperty("Codigo",proveedor.))
                                                    break;
                                                }
                                            }
                                            if (!provExiste)
                                            {
                                                var responseProv = ProveedorController.Proveedor(codProveedor, baseDeDatos);

                                                switch (responseProv.StatusCode)
                                                {
                                                    case HttpStatusCode.OK:
                                                        {
                                                            try
                                                            {
                                                                var jsonProv = JObject.Parse(responseProv.Content);

                                                                string Cuit = jsonProv["CUIT"].ToString().Substring(0, Math.Min(11, jsonProv["CUIT"].ToString().Length));
                                                                string Cbu = jsonProv["Fax"].ToString().Substring(0, Math.Min(22, jsonProv["Fax"].ToString().Length));
                                                                string TipoCuenta = jsonProv["Fax"].ToString().Substring(22, Math.Min(2, jsonProv["Fax"].ToString().Length));

                                                                ObjProveedor = new JObject();
                                                                ObjProveedor.Codigo = jsonProv["Codigo"].ToString();
                                                                ObjProveedor.Cuit = string.Concat(Enumerable.Repeat(" ", 11 - Cuit.Length)) + Cuit;
                                                                ObjProveedor.Cbu = string.Concat(Enumerable.Repeat(" ", 22 - Cbu.Length)) + Cbu;
                                                                ObjProveedor.TipoCuenta = string.Concat(Enumerable.Repeat(" ", 2 - TipoCuenta.Length)) + TipoCuenta;

                                                                AProveedores.Add(ObjProveedor);

                                                            }
                                                            catch (Exception e)
                                                            {
                                                                //MessageBox.Show("Error al exportar.");
                                                                Program.ConsoleLog("getProveedores: " + e.Message);
                                                                throw e;
                                                            }
                                                            break;
                                                        }
                                                    case HttpStatusCode.NotFound:
                                                        break;
                                                    default:
                                                        Program.ConsoleLog("getProveedores: " + responseProv.StatusCode + " " + responseProv.StatusDescription);
                                                        break;
                                                }
                                            }


                                            //----------------------------------------------------------------------------------
                                            //BANDERA PROV

                                            bool JuridiccionExiste = false;
                                            int IndiceConceptoDetalle = 0;
                                            dynamic ObjComprobantePagoDetalle = null;

                                            foreach (JObject Recorre in jsonRet["Resultados"][ret]["ComprobantePagoDetalle"])
                                            {
                                                var CodigoConcepto = jsonRet["Resultados"][ret]["ComprobantePagoDetalle"][IndiceConceptoDetalle]["Concepto"].ToString();
                                                
                                                JuridiccionExiste = false;

                                                foreach (JObject Jurisdiccion in AConceptosOtrosPagos)
                                                {
                                                    if (Jurisdiccion.GetValue("Codigo").ToString() == CodigoConcepto)
                                                    {
                                                        ObjComprobantePagoDetalle = new JObject();
                                                        ObjComprobantePagoDetalle.Codigo = Jurisdiccion.GetValue("Codigo").ToString();
                                                        ObjComprobantePagoDetalle.CodigoJurisdiccion = Jurisdiccion.GetValue("CodigoJurisdiccion").ToString();
                                                        JuridiccionExiste = true;
                                                        break;
                                                    }
                                                }
                                                if (!JuridiccionExiste)
                                                {
                                                    var responseJurisdiccion = ConceptoOtrosPagosController.ConceptoOtrosPagos(CodigoConcepto, baseDeDatos);

                                                    switch (responseJurisdiccion.StatusCode)
                                                    {
                                                        case HttpStatusCode.OK:
                                                            {
                                                                try
                                                                {
                                                                    var jsonProv = JObject.Parse(responseJurisdiccion.Content);

                                                                    string CodidoJurisdiccion = jsonProv["Clasificacion"].ToString().Substring(0, Math.Min(3, jsonProv["Clasificacion"].ToString().Length));

                                                                    ObjComprobantePagoDetalle = new JObject();
                                                                    ObjComprobantePagoDetalle.Codigo = jsonProv["Codigo"].ToString();
                                                                    ObjComprobantePagoDetalle.CodigoJurisdiccion = string.Concat(Enumerable.Repeat(" ", 3 - CodidoJurisdiccion.Length)) + CodidoJurisdiccion;

                                                                    AConceptosOtrosPagos.Add(ObjComprobantePagoDetalle);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    //MessageBox.Show("Error al exportar.");
                                                                    Program.ConsoleLog("getJurisdiccion: " + e.Message);
                                                                    throw e;
                                                                }
                                                                break;
                                                            }
                                                        case HttpStatusCode.NotFound:
                                                            break;
                                                        default:
                                                            Program.ConsoleLog("getJurisdiccion: " + responseJurisdiccion.StatusCode + " " + responseJurisdiccion.StatusDescription);
                                                            break;
                                                    }
                                                }

                                                //ACA SE CREA TODO EL OBJETO
                                                Model.Sifere sifere = new Model.Sifere();

                                                foreach (JObject Jurisdiccion in AConceptosOtrosPagos)
                                                {
                                                    if (Jurisdiccion.GetValue("Codigo").ToString() == CodigoConcepto)
                                                    {
                                                        sifere.CodigoJurisdiccion = Jurisdiccion.GetValue("CodigoJurisdiccion").ToString();
                                                    }
                                                }

                                                foreach (JObject proveedor in AProveedores)
                                                {
                                                    if (proveedor.GetValue("Codigo").ToString() == codProveedor)
                                                    {
                                                        sifere.Cuit = proveedor.GetValue("Cuit").ToString();
                                                        sifere.Cbu = proveedor.GetValue("Cbu").ToString();
                                                        sifere.TipoCuenta = proveedor.GetValue("TipoCuenta").ToString();
                                                    }
                                                }
                                                sifere.PeriodoRetencion = fecha.ToString("yyyy/MM");
                                                sifere.TipoMoneda = jsonRet["Resultados"][ret]["MonedaComprobante"].ToString().Substring(0, Math.Min(2, jsonRet["Resultados"][ret]["MonedaComprobante"].ToString().Length));
                                                sifere.TipoMoneda = string.Concat(Enumerable.Repeat(" ", 2 - sifere.TipoMoneda.Length)) + sifere.TipoMoneda;
                                                sifere.ImporteRetenido = jsonRet["Resultados"][ret]["ComprobantePagoDetalle"][IndiceConceptoDetalle]["Monto"].ToString();
                                                sifere.ImporteRetenido = string.Concat(Enumerable.Repeat("0", 9 - sifere.ImporteRetenido.Length)) + sifere.ImporteRetenido;
                                                IndiceConceptoDetalle++;
                                                LSifere.Add(sifere);
                                                //LMAL

                                            }

                                            //----------------------------------------------------------------------------------

                                        }
                                    }
                                    else
                                    {
                                        //break;
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                // MessageBox.Show("Error al exportar.");
                                Program.ConsoleLog("GetOtrosPagos: " + e.Message);
                                throw e;
                            }
                            break;
                        }
                    case HttpStatusCode.NotFound:
                        break;
                    default:
                        Program.ConsoleLog("GetOtrosPagos: " + responseRet.StatusCode + " " + responseRet.StatusDescription);
                        return;
                }
            }
        }


        static public IRestResponse Comprobantepago(int page, string baseDeDatos)
        {
            try
            {

                var url = new RestClient(Program.singleton.urlDragonfish + "/api.Dragonfish/Comprobantepago/");
                var request = new RestRequest(Method.GET);
                request.AddParameter("page", page);
                request.AddParameter("limit", 1000);
                request.AddParameter("sort", "-Numero");
                request.AddHeader("idCliente", Program.singleton.clienteDragonfish);
                request.AddHeader("Authorization", Program.singleton.tokenDragonfish);
                request.AddHeader("BaseDeDatos", baseDeDatos);

                var response = url.Execute(request);

                return response;

            }
            catch (Exception e)
            {
                //MessageBox.Show("Error al exportar.");
                Program.ConsoleLog("Comprobante pago Controller: " + e.Message);
                throw e;
            }
        }




    }
}
