using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MisDatosSDK
{
    [ComVisible(true)]
    public class MdApi2
    {
        // Propiedades públicas autogeneradas que respetan el estilo y nombres de Python
        public string UltimoMensajeError { get; set; } = "";
        public string Usuario { get; set; } = "";
        public string Password { get; set; } = "";
        public int Modo { get; set; } = 1; // 1 = Producción, otro valor = Test
        public string Revision { get; } = "010.90";
        public JsonNode Respuesta { get; set; }

        // Propiedades de red expuestas como solo lectura en paridad con las @property de Python
        private readonly string _endpoint = "https://api.misdatos.com.ar/";
        private readonly string _endpointtest = "http://127.0.0.1:8000/gae2api/";
        private HttpClient _service = null;

        public string Endpoint => _endpoint;
        public string Endpointtest => _endpointtest;
        public HttpClient Service => _service;

        public MdApi2() { }

        public bool conectar()
        {
            UltimoMensajeError = "";
            bool bresultado = false;

            if (string.IsNullOrEmpty(Password))
            {
                UltimoMensajeError = "Debe asignar un token antes de conectar";
                return false;
            }

            try
            {
                // 1 y 2. Configuramos el manejador base y la estrategia de reintentos nativa (Equivalente a Retry de Python)
                var httpClientHandler = new HttpClientHandler();
                var retryHandler = new RetryDelegatingHandler(httpClientHandler, maxRetries: 3, backoffFactorSeconds: 0.5);

                // 3. Instanciamos la sesión (HttpClient) inyectando el adaptador de reintentos
                _service = new HttpClient(retryHandler);

                // 4. Actualizamos los headers (Tu código original)
                _service.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{Usuario} {Password}");
                _service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                bresultado = true;
            }
            catch (Exception e)
            {
                UltimoMensajeError = $"Error al preparar conexión API: {e.Message}";
            }

            return bresultado;
        }

        public string LeerPropiedad(string cmetodo = "", string cpropiedad = "", int nindice = 0, int nsubindice = 0, int nsubsubindice = 0)
        {
            string cresultado = "";
            UltimoMensajeError = "";
            cpropiedad = cpropiedad.Trim();
            cmetodo = cmetodo.ToLower().Trim();
            string[] lpropiedad = cpropiedad.Split('.');

            // Lista de compatibilidad estricta con Python
            if (cmetodo == "mdwhatsappenlacev01" || cmetodo == "mdwhatssappenviarplantilla01v01" || cmetodo == "mdsumarv01" || cmetodo == "respuesta")
            {
                if (Respuesta != null)
                {
                    try
                    {
                        JsonNode current = Respuesta;
                        foreach (var prop in lpropiedad)
                        {
                            if (prop == "diccionario" || prop == "lista") continue;

                            if (current is JsonObject obj && obj.ContainsKey(prop))
                            {
                                current = obj[prop];
                            }
                            else if (current is JsonArray arr && int.TryParse(prop, out int idx) && idx < arr.Count)
                            {
                                current = arr[idx];
                            }
                            else
                            {
                                current = null;
                                break;
                            }
                        }
                        cresultado = current?.ToString() ?? "";
                    }
                    catch (Exception e)
                    {
                        UltimoMensajeError = $"Error objeto: {e.Message}";
                    }
                }
            }
            else
            {
                UltimoMensajeError = "Método no definido o no compatible con leerPropiedad";
            }

            return cresultado;
        }

        // NUEVO: Método traducido fielmente desde Python (anteriormente omitido)
        public string mdwhatsappenlaceurlv01(int nid_enlace = 0, string cruta_archivo = "")
        {
            UltimoMensajeError = "";
            string cresultado = "0";

            if (_service == null)
            {
                UltimoMensajeError = "No hay conexión. Ejecute conectar() primero.";
                return cresultado;
            }

            if (!File.Exists(cruta_archivo))
            {
                UltimoMensajeError = $"El archivo no existe: {cruta_archivo}";
                return cresultado;
            }

            // Validación de tamaño en el lado del cliente (aprox 1000 MB)
            long LIMITE_KB = 1000000;
            long LIMITE_BYTES = LIMITE_KB * 1024;
            long peso_archivo = new FileInfo(cruta_archivo).Length;

            if (peso_archivo > LIMITE_BYTES)
            {
                UltimoMensajeError = $"El archivo supera el tamaño máximo permitido de {LIMITE_KB} KB.";
                return cresultado;
            }

            try
            {
                byte[] contenido_bytes = File.ReadAllBytes(cruta_archivo);
                string archivo_b64 = Convert.ToBase64String(contenido_bytes);
                string nombre_archivo = Path.GetFileName(cruta_archivo);

                var payload = new Dictionary<string, object>
                {
                    { "id_enlace", nid_enlace },
                    { "archivo_b64", archivo_b64 },
                    { "nombre_archivo", nombre_archivo }
                };

                string url = (Modo == 1 ? _endpoint : _endpointtest) + "mdwhatsappenlaceurlv01";
                string responseBody = PostRequest(url, payload);

                if (responseBody != null)
                {
                    var datosRespuesta = JsonNode.Parse(responseBody);
                    Respuesta = datosRespuesta;
                    if (datosRespuesta?["status"]?.ToString() == "ok")
                    {
                        cresultado = datosRespuesta["resultado"]?.ToString() ?? "0";
                    }
                    else
                    {
                        UltimoMensajeError = datosRespuesta?["error"]?.ToString() ?? "Error lógico en servidor";
                    }
                }
            }
            catch (Exception e)
            {
                UltimoMensajeError = $"Error local al ejecutar mdwhatsappenlaceurlv01: {e.Message}";
            }

            return cresultado;
        }

        // NUEVO: Método traducido fielmente desde Python (anteriormente omitido)
        public string mdobteneraccesov02(int nproveedor = 0, string cplan = "gratuito", string cservicio = "", string csubservicio = "", string cdestino = "", int ncantidad = 1)
        {
            UltimoMensajeError = "";
            string cresultado = "0";

            if (_service == null)
            {
                UltimoMensajeError = "No hay conexión. Ejecute conectar() primero.";
                return cresultado;
            }

            try
            {
                var payload = new Dictionary<string, object>
                {
                    { "proveedor", nproveedor },
                    { "plan", cplan },
                    { "servicio", cservicio },
                    { "subservicio", csubservicio },
                    { "destino", cdestino },
                    { "cantidad", ncantidad }
                };

                string url = (Modo == 1 ? _endpoint : _endpointtest) + "mdobteneraccesov02";
                string responseBody = PostRequest(url, payload);

                if (responseBody != null)
                {
                    var datosRespuesta = JsonNode.Parse(responseBody);
                    Respuesta = datosRespuesta;
                    if (datosRespuesta?["status"]?.ToString() == "ok")
                    {
                        cresultado = datosRespuesta["resultado"]?.ToString() ?? "0";
                    }
                    else
                    {
                        UltimoMensajeError = datosRespuesta?["error"]?.ToString() ?? "Error lógico en servidor";
                    }
                }
            }
            catch (Exception e)
            {
                UltimoMensajeError = $"Error local al ejecutar mdobteneraccesov02: {e.Message}";
            }

            return cresultado;
        }

        public float MdSumarV01(float numero1 = 0, float numero2 = 0)
        {
            UltimoMensajeError = "";
            if (_service == null) { UltimoMensajeError = "No hay conexión."; return 0.0f; }

            try
            {
                var payload = new { numero1, numero2 };
                string url = (Modo == 1 ? _endpoint : _endpointtest) + "mdsumarv01";
                string responseBody = PostRequest(url, payload);

                if (responseBody != null)
                {
                    var datosRespuesta = JsonNode.Parse(responseBody);
                    Respuesta = datosRespuesta;

                    if (datosRespuesta?["status"]?.ToString() == "ok") return (float)datosRespuesta["resultado"];

                    UltimoMensajeError = datosRespuesta?["error"]?.ToString() ?? "Error lógico en servidor";
                }
            }
            catch (Exception e) { UltimoMensajeError = $"Error al ejecutar sumar: {e.Message}"; }
            return 0.0f;
        }

        public string MdWhatsappDestinoV01(string cdestino = "", string ccodigo = "", string cemail = "", string nestadoproveedor = "", string cfecha = "", string cnombre = "", string caccionclave = "")
        {
            UltimoMensajeError = "";
            string cresultado = "0";
            if (_service == null) { UltimoMensajeError = "No hay conexión."; return cresultado; }

            try
            {
                var payload = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(cdestino)) payload["destino"] = cdestino;
                if (!string.IsNullOrEmpty(cemail)) payload["email"] = cemail;
                if (!string.IsNullOrEmpty(nestadoproveedor)) payload["estadoproveedor"] = nestadoproveedor;
                if (!string.IsNullOrEmpty(cfecha)) payload["fecha"] = cfecha;
                if (!string.IsNullOrEmpty(cnombre)) payload["nombre"] = cnombre;
                if (!string.IsNullOrEmpty(ccodigo)) payload["codigo"] = ccodigo;
                if (!string.IsNullOrEmpty(caccionclave)) payload["accionclave"] = caccionclave;

                string url = (Modo == 1 ? _endpoint : _endpointtest) + "mdwhatsappdestinov01";
                string responseBody = PostRequest(url, payload);

                if (responseBody != null)
                {
                    var datosRespuesta = JsonNode.Parse(responseBody);
                    Respuesta = datosRespuesta;
                    if (datosRespuesta?["status"]?.ToString() == "ok") cresultado = datosRespuesta["resultado"]?.ToString() ?? "0";
                    else UltimoMensajeError = datosRespuesta?["error"]?.ToString() ?? "Error lógico en servidor";
                }
            }
            catch (Exception e) { UltimoMensajeError = $"Error local al ejecutar mdwhatsappdestinov01: {e.Message}"; }
            return cresultado;
        }

        // Wrapper original de Python preservado para mantener compatibilidad de firmas de llamada
        public string mdwhatsappenlacev01(string ctipo = "", string ccodigo = "", string cnombre = "", string cfecha = "", string cenlace = "", string cusuariosql = "", string cdescripcion = "", string ntotal = "", string nprocesado = "", string caccionclave = "")
        {
            return mdhwatsappenlacev01(ctipo, ccodigo, cnombre, cfecha, cenlace, cusuariosql, cdescripcion, ntotal, nprocesado, caccionclave);
        }

        public string mdhwatsappenlacev01(string ctipo = "", string ccodigo = "", string cnombre = "", string cfecha = "", string cenlace = "", string cusuariosql = "", string cdescripcion = "", string ntotal = "", string nprocesado = "", string caccionclave = "")
        {
            UltimoMensajeError = "";
            string cresultado = "0";
            if (_service == null) { UltimoMensajeError = "No hay conexión."; return cresultado; }

            try
            {
                var payload = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(ctipo)) payload["tipo"] = ctipo;
                if (!string.IsNullOrEmpty(ccodigo)) payload["codigo"] = ccodigo;
                if (!string.IsNullOrEmpty(cnombre)) payload["nombre"] = cnombre;
                if (!string.IsNullOrEmpty(cfecha)) payload["fecha"] = cfecha;
                if (!string.IsNullOrEmpty(cenlace)) payload["enlace"] = cenlace;
                if (!string.IsNullOrEmpty(cusuariosql)) payload["usuariosql"] = cusuariosql;
                if (!string.IsNullOrEmpty(cdescripcion)) payload["descripcion"] = cdescripcion;
                if (!string.IsNullOrEmpty(ntotal)) payload["total"] = ntotal;
                if (!string.IsNullOrEmpty(nprocesado)) payload["procesado"] = nprocesado;
                if (!string.IsNullOrEmpty(caccionclave)) payload["accionclave"] = caccionclave;

                string url = (Modo == 1 ? _endpoint : _endpointtest) + "mdhwatsappenlacev01";
                string responseBody = PostRequest(url, payload);

                if (responseBody != null)
                {
                    var datosRespuesta = JsonNode.Parse(responseBody);
                    Respuesta = datosRespuesta;
                    if (datosRespuesta?["status"]?.ToString() == "ok") cresultado = datosRespuesta["resultado"]?.ToString() ?? "0";
                    else UltimoMensajeError = datosRespuesta?["error"]?.ToString() ?? "Error lógico en servidor";
                }
            }
            catch (Exception e) { UltimoMensajeError = $"Error local al ejecutar mdhwatsappenlacev01: {e.Message}"; }
            return cresultado;
        }

        // Renombrado a mdwhatssappenviarplantilla01v01 (con doble 's') para paridad total con Python
        public float mdwhatssappenviarplantilla01v01(string cdestino = "", string ccodigo = "", string caccion = "")
        {
            UltimoMensajeError = "";
            if (_service == null) { UltimoMensajeError = "No hay conexión."; return 0.0f; }

            try
            {
                var payload = new { destino = cdestino, codigo = ccodigo, accion = caccion };
                string url = (Modo == 1 ? _endpoint : _endpointtest) + "mdswhatssappenviarplantilla01v01";
                string responseBody = PostRequest(url, payload);

                if (responseBody != null)
                {
                    var datosRespuesta = JsonNode.Parse(responseBody);
                    Respuesta = datosRespuesta;
                    if (datosRespuesta?["status"]?.ToString() == "ok") return (float)datosRespuesta["resultado"];
                    UltimoMensajeError = datosRespuesta?["error"]?.ToString() ?? "Error lógico en servidor";
                }
            }
            catch (Exception e) { UltimoMensajeError = $"Error al ejecutar mdwhatssappenviarplantilla: {e.Message}"; }
            return 0.0f;
        }

        private string PostRequest(string endpointUrl, object payload)
        {
            var jsonString = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Bloqueo síncrono compatible con Mono. Se recomienda precaución en hilos de UI de escritorio.
            var response = _service.PostAsync(endpointUrl, content).GetAwaiter().GetResult();
            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode) return responseBody;

            UltimoMensajeError = $"HTTP {(int)response.StatusCode}: {responseBody}";
            try
            {
                var errorNode = JsonNode.Parse(responseBody);
                if (errorNode?["error"] != null) UltimoMensajeError = errorNode["error"].ToString();
            }
            catch { }

            return null;
        }
    }

    // -------------------------------------------------------------------------
    // CLASE DE APOYO: Emula el comportamiento de urllib3.util.retry.Retry de Python
    // -------------------------------------------------------------------------
    public class RetryDelegatingHandler : DelegatingHandler
    {
        private readonly int _maxRetries;
        private readonly double _backoffFactorSeconds;

        public RetryDelegatingHandler(HttpMessageHandler innerHandler, int maxRetries = 3, double backoffFactorSeconds = 0.5)
            : base(innerHandler)
        {
            _maxRetries = maxRetries;
            _backoffFactorSeconds = backoffFactorSeconds;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            for (int i = 0; i <= _maxRetries; i++)
            {
                try
                {
                    response = await base.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        return response; // Si la petición es exitosa, se detiene el reintento
                    }

                    int statusCode = (int)response.StatusCode;
                    // Códigos para forzar el reintento (Equivalente al status_forcelist de Python)
                    if (statusCode != 500 && statusCode != 502 && statusCode != 503 && statusCode != 504)
                    {
                        return response;
                    }
                }
                catch (Exception ex) when (ex is TaskCanceledException || ex is HttpRequestException)
                {
                    if (i == _maxRetries) throw;
                }

                if (i < _maxRetries)
                {
                    double delaySeconds = _backoffFactorSeconds * Math.Pow(2, i);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
                }
            }

            return response;
        }
    }
}
