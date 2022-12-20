using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TemasAdicionales.Clientes {
    internal interface IClienteWF {
        Task<List<WeatherForecast>> Obtener();
    }

    internal class ClienteWF : IClienteWF {
        private readonly HttpClient httpClient;
        private readonly string urlBase = "https://localhost:7103/WeatherForecast/";

        public ClienteWF(HttpClient httpClient) {
            this.httpClient = httpClient;
        }

        public async Task<List<WeatherForecast>> Obtener() {
            var resp = await httpClient.GetFromJsonAsync<List<WeatherForecast>>(urlBase);

            if (resp is null) {
                return new List<WeatherForecast>();
            }

            return resp;
        }
    }
}
