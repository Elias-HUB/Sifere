using System;
using System.IO;
using System.Xml.Linq;

namespace Cardon___Exportacion_SIFERE.Model
{
    public class Singleton
    {
        public string log = "log.txt";
        public string config = "config.xml";
        public string tokenDragonfish = "";
        public string clienteDragonfish = "";
        public string urlDragonfish = "";
        public string[] basesDeDatos = null;
        public string rutaExportacion = "";
        public string[] Proveedores = null;
        public string[] TipoCuenta = null;
        private static Singleton _instance = null;


        private Singleton()
        {
            getXml();

        }

        public static Singleton Instance
        {
            get
            {
                // The first call will create the one and only instance.
                if (_instance == null)
                {
                    _instance = new Singleton();
                }

                // Every call afterwards will return the single instance created above.
                return _instance;
            }
        }
        public void getXml()
        {
            try
            {
                if (File.Exists(config))
                {

                    string value = File.ReadAllText(config);
                    XElement xmldoc = XElement.Parse(value);

                    tokenDragonfish = xmldoc.Element("tokenDragonfish").Value;
                    clienteDragonfish = xmldoc.Element("clienteDragonfish").Value;
                    urlDragonfish = xmldoc.Element("urlDragonfish").Value;
                    basesDeDatos = xmldoc.Element("basesDeDatos").Value.Split(';');
                    rutaExportacion = xmldoc.Element("RutaExportacion").Value;
                    Proveedores = xmldoc.Element("Proveedores").Value.Split(';');
                    TipoCuenta = xmldoc.Element("TipoCuenta").Value.Split(';');


                    if (tokenDragonfish == "" || clienteDragonfish == "" || urlDragonfish == "" || basesDeDatos == null || Proveedores == null || TipoCuenta == null)
                    {
                        Program.ConsoleLog("Datos incompletos en el archivo config.xml");
                        throw new Exeptions.Exceptions("Datos incompletos en el archivo config.xml");
                    }
                }
                else
                {
                    Program.ConsoleLog("Archivo config.xml no existe");
                    throw new Exeptions.Exceptions("Archivo config.xml no existe");
                }
            }
            catch (Exeptions.Exceptions Ex)
            {
                throw Ex;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }
    }
}
