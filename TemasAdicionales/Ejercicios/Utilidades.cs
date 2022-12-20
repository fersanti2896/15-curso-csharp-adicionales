using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemasAdicionales.Ejercicios {
    internal class Utilidades {
        public static Dictionary<string, List<string>> ExtraerErroresWebAPI(string json) { 
            var resp = new Dictionary<string, List<string>>();
            var deserializado = JsonConvert.DeserializeObject<dynamic>(json)!;
            var campoErrores = deserializado.errors;

            foreach (dynamic error in campoErrores) {
                var campo = error.Name;
                var errores = new List<string>();

                foreach (var erro in error.Value) {
                    errores.Add(erro.ToString());
                }

                resp.Add(campo, errores);
            }

            return resp;
        }
    }
}
