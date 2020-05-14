using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IO;
using System.Net;


namespace Cardon___Exportacion_SIFERE.Controller
{
    public class OtrosPagosController
    {

        //,string Proveedor
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
                Program.ConsoleLog("Comprobante pago Controller: " + e.Message);
                throw e;
            }
        }




    }
}
