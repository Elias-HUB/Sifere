using System;
using System.Collections.Generic;
using System.Linq;
using Cardon___Exportacion_SIFERE.Model;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Cardon___Exportacion_SIFERE.Services
{
    public class OtrosPagosServices
    {
        private static List<Model.ConceptoPago> lConceptosOtrosPagos = new List<ConceptoPago>();
        internal static List<Model.ConceptoPago> LConceptosOtrosPagos { get => lConceptosOtrosPagos; set => lConceptosOtrosPagos = value; }

        private static List<Model.Proveedor> lproveedores = new List<Proveedor>();
        internal static List<Model.Proveedor> Lproveedores { get => lproveedores; set => lproveedores = value; }

        private static List<Model.Sifere> lSifere = new List<Model.Sifere>();
        internal static List<Sifere> LSifere { get => lSifere; set => lSifere = value; }

        private static List<Model.Sifere> lSifereConProblemas = new List<Model.Sifere>();
        internal static List<Sifere> LSifereConProblemas { get => LSifereConProblemas; set => LSifereConProblemas = value; }


        public static void GetOtrosPagos(string baseDeDatos, string FechaDesde, string FechaHasta, Singleton singleton)
        {
            string Siguiente = "Siguiente";
            int Page = 1;
            Proveedor Proveedor = null;

            foreach (string ProveedoresSingleton in singleton.Proveedores)
            {
                var responseProv = Controller.ProveedorController.Proveedor(ProveedoresSingleton, baseDeDatos);
                switch (responseProv.StatusCode)
                {
                    case HttpStatusCode.OK:
                        {
                            try
                            {
                                var jsonProv = JObject.Parse(responseProv.Content);
                                GuardarProveedorEnListado(jsonProv, Proveedor, singleton);
                            }

                            catch (Exeptions.Exceptions Ex)
                            {
                                throw Ex;
                            }
                            catch (Exception e)
                            {
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



            while (Siguiente != "")
            {
                var responseRet = Controller.OtrosPagosController.Comprobantepago(Page++, baseDeDatos);
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
                                        if (Lproveedores.Any(item => item.Codigo == codProveedor))
                                        {

                                            bool JuridiccionExiste = false;
                                            int IndiceConceptoDetalle = 0;
                                            ConceptoPago ConceptoPago = null;

                                            foreach (JObject Recorre in jsonRet["Resultados"][ret]["ComprobantePagoDetalle"])
                                            {
                                                var CodigoConcepto = jsonRet["Resultados"][ret]["ComprobantePagoDetalle"][IndiceConceptoDetalle]["Concepto"].ToString();

                                                JuridiccionExiste = false;
                                                JuridiccionExiste = GuardarConceptoPagoEnObjeto(ref ConceptoPago, CodigoConcepto);

                                                if (!JuridiccionExiste)
                                                {
                                                    var responseConceptoPago = Controller.ConceptoOtrosPagosController.ConceptoOtrosPagos(CodigoConcepto, baseDeDatos);

                                                    switch (responseConceptoPago.StatusCode)
                                                    {
                                                        case HttpStatusCode.OK:
                                                            {
                                                                try
                                                                {
                                                                    var jsonConceptoPago = JObject.Parse(responseConceptoPago.Content);
                                                                    GuardarConceptoPagoEnListado(jsonConceptoPago, ConceptoPago);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Program.ConsoleLog("getConceptoPago: " + e.Message);
                                                                    throw e;
                                                                }
                                                                break;
                                                            }
                                                        case HttpStatusCode.NotFound:
                                                            break;
                                                        default:
                                                            Program.ConsoleLog("GetConceptoPago: " + responseConceptoPago.StatusCode + " " + responseConceptoPago.StatusDescription);
                                                            break;
                                                    }
                                                }
                                                //Se guarda el Listado de la Clase sifere
                                                GuardarSifereEnListado(codProveedor, CodigoConcepto, jsonRet, ret, IndiceConceptoDetalle, fecha);
                                                IndiceConceptoDetalle++;
                                                //LMAL

                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
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

        private static void GuardarProveedorEnListado(JObject jsonProv, Proveedor proveedor, Singleton singleton)
        {
            try
            {
                string Cuit = jsonProv["CUIT"].ToString();
                string Fax = jsonProv["Fax"].ToString();
                if (Cuit.Length != 11)
                {
                    Program.ConsoleLog("El Proveedor " + jsonProv["Codigo"].ToString() + " en el campo CUIT \rno contiene la cantidad de caracteres correspondiente. \r\rUna vez corregido intente nuevamente.");
                    throw new Exeptions.Exceptions("El Proveedor " + jsonProv["Codigo"].ToString() + " en el campo CUIT \rno contiene la cantidad de caracteres correspondiente. \r\rUna vez corregido intente nuevamente.");
                }
                if (Fax.Length != 24)
                {
                    Program.ConsoleLog("El Proveedor " + jsonProv["Codigo"].ToString() + " en el campo FAX \rdebe de contener 24 caracteres. \n\r" + "Verificar si el CBU o el Tipo de cuenta tiene la cantidad de caracteres correspondiente.");
                    throw new Exeptions.Exceptions("El Proveedor " + jsonProv["Codigo"].ToString() + " en el campo FAX \rdebe de contener 24 caracteres. \n\r" + "Verificar si el CBU o el Tipo de cuenta tiene la cantidad de caracteres correspondiente.");
                }

                proveedor = new Proveedor();
                proveedor.Codigo = jsonProv["Codigo"].ToString();

                proveedor.Cuit = Cuit.ToString().Substring(0, Math.Min(11, Cuit.ToString().Length));
                proveedor.Cuit = string.Concat(Enumerable.Repeat(" ", 11 - proveedor.Cuit.Length)) + proveedor.Cuit;

                proveedor.Cbu = Fax.ToString().Substring(0, Math.Min(22, Fax.ToString().Length));
                proveedor.Cbu = string.Concat(Enumerable.Repeat(" ", 22 - proveedor.Cbu.Length)) + proveedor.Cbu;

                proveedor.TipoCuenta = jsonProv["Fax"].ToString().Substring(22, Math.Min(2, jsonProv["Fax"].ToString().Length));
                if (!singleton.TipoCuenta.Any(item => item == proveedor.TipoCuenta))
                {
                    throw new Exeptions.Exceptions("El Proveedor " + jsonProv["Codigo"].ToString() + " en el campo FAX \rno tiene el Tipo de Cuenta de manera correcta. \r\r" + "Verificar los dos ultimos digitos del campo FAX. \nSi es un nuevo Tipo de Cuenta agregarlo en el archivo XML. \rHa ingresado " + proveedor.TipoCuenta);
                }

                Lproveedores.Add(proveedor);
            }
            catch (Exeptions.Exceptions Ex)
            {
                throw Ex;
            }
            catch (Exception ex)
            {
                Program.ConsoleLog("GuardarProveedorEnListado: " + ex.Message);
                throw ex;
            }


        }


        private static void GuardarConceptoPagoEnListado(JObject jsonConceptoPagoEnListado, ConceptoPago conceptoPago)
        {
            try
            {
                string CodidoJurisdiccion = jsonConceptoPagoEnListado["Clasificacion"].ToString().Substring(0, Math.Min(3, jsonConceptoPagoEnListado["Clasificacion"].ToString().Length));
                conceptoPago = new ConceptoPago();
                conceptoPago.Codigo = jsonConceptoPagoEnListado["Codigo"].ToString();
                conceptoPago.CodigoJurisdiccion = string.Concat(Enumerable.Repeat(" ", 3 - CodidoJurisdiccion.Length)) + CodidoJurisdiccion;

                LConceptosOtrosPagos.Add(conceptoPago);
            }
            catch (Exception ex)
            {
                Program.ConsoleLog("GuardarConceptoPagoEnListado: " + ex.Message);
                throw ex;
            }
        }


        private static void GuardarSifereEnListado(string CodigoProveedor, string CodigoConceptoOtrosPagos, JObject jsonRet, int IndiceJsonret, int IndiceConceptoDetalle, DateTime fecha)
        {
            try
            {
                Model.Sifere sifere = new Model.Sifere();

                foreach (ConceptoPago FconceptoPago in LConceptosOtrosPagos)
                {
                    if (FconceptoPago.Codigo == CodigoConceptoOtrosPagos)
                    {
                        sifere.CodigoJurisdiccion = FconceptoPago.CodigoJurisdiccion;
                        break;
                    }
                }

                foreach (Proveedor proveedor in Lproveedores)
                {
                    if (proveedor.Codigo == CodigoProveedor)
                    {
                        sifere.Cuit = proveedor.Cuit;
                        sifere.Cbu = proveedor.Cbu;
                        sifere.TipoCuenta = proveedor.TipoCuenta;
                        break;
                    }
                }

                //string Cuit = jsonProv["CUIT"].ToString().Substring(0, Math.Min(11, jsonProv["CUIT"].ToString().Length));
                //string Cbu = jsonProv["Fax"].ToString().Substring(0, Math.Min(22, jsonProv["Fax"].ToString().Length));
                //string TipoCuenta = jsonProv["Fax"].ToString().Substring(22, Math.Min(2, jsonProv["Fax"].ToString().Length))


                sifere.PeriodoRetencion = fecha.ToString("yyyy/MM");
                sifere.TipoMoneda = jsonRet["Resultados"][IndiceJsonret]["MonedaComprobante"].ToString().Substring(0, Math.Min(1, jsonRet["Resultados"][IndiceJsonret]["MonedaComprobante"].ToString().Length));
                sifere.ImporteRetenido = jsonRet["Resultados"][IndiceJsonret]["ComprobantePagoDetalle"][IndiceConceptoDetalle]["Monto"].ToString();
                sifere.ImporteRetenido = Convert.ToDouble(sifere.ImporteRetenido).ToString("N2").Replace(".", "");
                sifere.ImporteRetenido = string.Concat(Enumerable.Repeat("0", 10 - sifere.ImporteRetenido.Length)) + sifere.ImporteRetenido;
                LSifere.Add(sifere);
            }
            catch (Exception ex)
            {
                Program.ConsoleLog("GuardarSifereEnListado: " + ex.Message);
                throw ex;
            }
        }


        private static bool GuardarProveedorEnObjeto(ref Proveedor Proveedor, string codProveedor)
        {
            try
            {
                foreach (Proveedor FProveedor in Lproveedores)
                {
                    if (FProveedor.Codigo == codProveedor)
                    {
                        Proveedor = new Proveedor();
                        Proveedor.Codigo = FProveedor.Codigo;
                        Proveedor.Cuit = FProveedor.Cuit;
                        Proveedor.Cbu = FProveedor.Cbu;
                        Proveedor.TipoCuenta = FProveedor.TipoCuenta;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Program.ConsoleLog("GuardarProveedorEnObjeto: " + ex.Message);
                throw ex;
            }

        }


        private static bool GuardarConceptoPagoEnObjeto(ref ConceptoPago ConceptoPago, string CodigoConcepto)
        {
            try
            {
                foreach (ConceptoPago conceptoPago in LConceptosOtrosPagos)
                {
                    if (conceptoPago.Codigo == CodigoConcepto)
                    {
                        ConceptoPago = new ConceptoPago();
                        ConceptoPago.Codigo = conceptoPago.Codigo;
                        ConceptoPago.CodigoJurisdiccion = conceptoPago.CodigoJurisdiccion;
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Program.ConsoleLog("GuardarConceptoPagoEnObjeto: " + ex.Message);
                throw ex;
            }

        }
    }
}


