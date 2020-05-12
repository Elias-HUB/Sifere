using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Cardon___Exportacion_SIFERE
{
    static class Program
    {
        static public Model.Singleton singleton = null;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            singleton = Model.Singleton.Instance;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Verificar APi
            Application.Run(new VentanaSifere());
        }

        static public void ConsoleLog(string Message)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(singleton.log, true))
                {
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + Message);
                }
            }
            catch
            {

            }
        }
    }
}
