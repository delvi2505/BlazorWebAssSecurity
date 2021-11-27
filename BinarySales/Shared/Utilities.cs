using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BinarySales.Shared
{
    public static class Utilities
    {
        //Datos de paginación de listas
        public const int REGISTERSPERPAGE = 100; //Debe ser un valor igual o mayor que RADIO para que no hayan problemas de cálculo
        public const int RADIO = 2;

        //public static System.Globalization.CultureInfo cultureForNumbers = System.Globalization.CultureInfo.InvariantCulture;

        public const string MSGREQUIRED = "El Campo {0} es requerido";
        public const string MSGCREDENTIALSFAILS = "Las credenciales de acceso no son correctas";
        public const string MSGFORMATFAIL = "El Campo {0} no está en un formato adecuado";
        public const string MSGSUSPENDEDUSER = "Este usuario está suspendido";
        public const string MSGCANNOTUSERREGISTER = "No se ha podido registrar el usuario";
        public const string MSGNODATA = "No se encontraron resultados";
        public const string MSGSUCCESS = "La opreción se ha realizado exitosamente";
        public const string MSGSTRINGlENGSHORT = "El Campo {0} requiere más caracteres";
    }
}
