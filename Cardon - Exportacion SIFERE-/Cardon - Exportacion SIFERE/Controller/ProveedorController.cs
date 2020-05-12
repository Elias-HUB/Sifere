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
    public class ProveedorController
    {
        static public IRestResponse Proveedor(string idProveedor, string baseDeDatos)
        {
            try
            {
                var url = new RestClient(Program.singleton.urlDragonfish + "/api.Dragonfish/Proveedor/" + idProveedor + "/");
                var request = new RestRequest(Method.GET);
                request.AddHeader("idCliente", Program.singleton.clienteDragonfish);
                request.AddHeader("Authorization", Program.singleton.tokenDragonfish);
                request.AddHeader("BaseDeDatos", baseDeDatos);

                var response = url.Execute(request);

                return response;
            }
            catch (Exception e)
            {
                //MessageBox.Show("Error al exportar.");                
                Program.ConsoleLog("Proveedor Controller: " + e.Message);
                throw e;                
            }
        }
    }
}
