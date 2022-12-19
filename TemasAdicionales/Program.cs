
using Entidades;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

Console.WriteLine("¡TEMAS ADICIONALES!\n");

/* GetAsync */
var httpClient = new HttpClient();
var resp = await httpClient.GetAsync("https://localhost:7103/WeatherForecast");
var cuerpo = await resp.Content.ReadAsStringAsync();


Console.WriteLine("---CUERPO---\n" + cuerpo);
Console.WriteLine("\n--CABECERA---");

foreach (var cabecera in resp.Headers) {
    Console.WriteLine($"{ cabecera.Key }: { cabecera.Value.First() }");
}

Console.WriteLine($"\n---CODIGO DE ESTATUS---");
Console.WriteLine(resp.StatusCode);
Console.WriteLine(resp.IsSuccessStatusCode);

/* GetStringAsync */
var respString = await httpClient.GetStringAsync("https://localhost:7103/WeatherForecast");
Console.WriteLine($"\nRespuesta String: { respString }");

/* Serializando respuesta con GetFromJsonAsync */
var url = "https://localhost:7103/WeatherForecast";
var respSeriealizada = await httpClient.GetFromJsonAsync<List<WeatherForecast>>(url);

Console.WriteLine($"\nNumero de elementos: { respSeriealizada!.Count }");

/* Post usando PostAsJsonAsync */
var wf = new WeatherForecast() { 
    Date = DateTime.Now,
    Summary = "¡Que calor!",
    TemperatureC = 40
};

var respWF = await httpClient.PostAsJsonAsync(url, wf);

if (resp.IsSuccessStatusCode) {
    var cuerpoWF = await respWF.Content.ReadAsStringAsync();

    Console.WriteLine($"El cuerpo de la respuesta es: { cuerpoWF }");
}

/* Post usando PostAsync */
var tempSeriealizada = JsonConvert.SerializeObject(wf);
var stringContent = new StringContent(tempSeriealizada, Encoding.UTF8, "application/json");
var resp3 = await httpClient.PostAsync(url, stringContent);

if (resp3.IsSuccessStatusCode) {
    var cuerpoWF = await respWF.Content.ReadAsStringAsync();

    Console.WriteLine($"El cuerpo de la respuesta es: { cuerpoWF }");
}