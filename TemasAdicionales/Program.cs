
using Entidades;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TemasAdicionales.Clientes;
using TemasAdicionales.Ejercicios;

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
    Console.WriteLine("Todo bien");
} else if (resp3.StatusCode ==  System.Net.HttpStatusCode.BadRequest) {
    var cuerpoWF = await respWF.Content.ReadAsStringAsync();
    var camposErrors = Utilidades.ExtraerErroresWebAPI(cuerpoWF);

    foreach (var campo in camposErrors) {
        Console.WriteLine($"Key: { campo.Key }");
        foreach (var error in campo.Value) {
            Console.WriteLine($"{ error }");
        }
    }
} else { 
    Console.WriteLine("Otro tipo de situación");
}

/* Enviando cabeceras */
var url2 = "https://localhost:7103/WeatherForecast/mayusculas";

Console.WriteLine($"\nEjemplo 1:");
Console.WriteLine(await httpClient.GetStringAsync(url2));

using (var peticion = new HttpRequestMessage(HttpMethod.Get, url2)) {
    peticion.Headers.Add("valor", "Soy Marisol");

    var respPet = await httpClient.SendAsync(peticion);

    Console.WriteLine($"\nEjemplo 2:");
    Console.WriteLine(await respPet.Content.ReadAsStringAsync());
}

/* HttpClientFactory */
var serviceCollection = new ServiceCollection();

configurar(serviceCollection);
var servicios = serviceCollection.BuildServiceProvider();
var httpClientFactory = servicios.GetRequiredService<IHttpClientFactory>();

var httpClientF = httpClientFactory.CreateClient();
var respFac = await httpClientF.GetAsync(url2);

Console.WriteLine($"Ejemplo 1 Existoso: { respFac.IsSuccessStatusCode }");

/* Ejemplo de Clientes Nombrados */
var clienteTareas = httpClientFactory.CreateClient("tareas");
var respTareas = await clienteTareas.GetAsync("");

Console.WriteLine($"Ejemplo (Tareas) Exitoso: { respTareas.IsSuccessStatusCode }");

var clienteWF = httpClientFactory.CreateClient("weatherforecast");
var respWF2 = await clienteWF.GetAsync("mayusculas");

Console.WriteLine($"Ejemplo (WF) Exitoso: { respWF2.IsSuccessStatusCode }");

/* Ejemplo de Clientes Tipados */
var clienteWF2 = servicios.GetRequiredService<IClienteWF>();
var listado = await clienteWF2.Obtener();

Console.WriteLine($"Cantidad WFs: { listado.Count }");

static void configurar(ServiceCollection services) {
    //services.AddHttpClient();

    /* Clientes nombrados */
    services.AddHttpClient("tareas", opc => {
        opc.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/todos/");
    });

    services.AddHttpClient("weatherforecast", opc => {
        opc.BaseAddress = new Uri("https://localhost:7103/WeatherForecast/");
        opc.DefaultRequestHeaders.Add("valor", "Soy un cliente nombrado");
    });

    /* Clientes Tipados */
    services.AddHttpClient<IClienteWF, ClienteWF>();
}

/* Enviando un archivo a la WEB API */
var rutaArchivo = @"C:\Users\josantiago\Documents\Cursos Udemy\Programando en C# de Principiante a Profesional\S15 - Temas Adicionales\TemasAdicionales\Ejemplo.txt";
var nombreArchivo = Path.GetFileName(rutaArchivo);

using (var contenido = new MultipartFormDataContent()) {
    using (var fs = File.OpenRead(rutaArchivo)) {
        contenido.Add(new StreamContent(fs), "file", nombreArchivo);
        
        var resp4 = await clienteWF.PostAsync("archivo", contenido);
        resp4.EnsureSuccessStatusCode();
    }
}

/* Descargando archivo */
var rutaCarpeta = "../../../";
var urlArchivo = @"https://localhost:7103/carpeta/d31387e7-15d7-49ff-ad13-67de43b26722.txt";
var stream = await httpClientF.GetStreamAsync(urlArchivo);
var nombreArchivo2 = Path.GetFileName(urlArchivo);

using (var fs = File.Create(Path.Combine(rutaCarpeta, nombreArchivo2))) {
    await stream.CopyToAsync(fs);
}