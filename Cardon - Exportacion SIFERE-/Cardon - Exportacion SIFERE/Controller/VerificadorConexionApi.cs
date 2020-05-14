using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;


namespace Cardon___Exportacion_SIFERE.Controller
{
    class VerificadorConexionApi
    {
        static public IRestResponse VerificarConexionApi()
        {
            try
            {
                var url = new RestClient(Program.singleton.urlDragonfish + "/api.Dragonfish/Senia/");
                var request = new RestRequest(Method.GET);
                request.AddHeader("idCliente", Program.singleton.clienteDragonfish);
                request.AddHeader("Authorization", Program.singleton.tokenDragonfish);

                var response = url.Execute(request);

                return response;
            }
            catch (Exception e)
            {
                Program.ConsoleLog("Concepto Otros Pagos Controller: " + e.Message);
                throw e;
            }
        }
    }
}
