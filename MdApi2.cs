using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;

namespace MisDatosSDK
{
    [ComVisible(true)]
    public class MdApi2
    {
        public string UltimoMensajeError { get; set; } = "";
        public string Usuario { get; set; } = "";
        public string Password { get; set; } = "";
        public int Modo { get; set; } = 1;
        public string Revision { get; } = "010.90";
        public JsonNode Respuesta { get; set; }

        private readonly string _endpoint = "https://api.misdatos.com.ar/";
        private readonly string _endpointtest = "http://127.0.0.1:8000/gae2api/";
        private HttpClient _service = null;

        public MdApi2() { }

        public bool Conectar()
        {
            UltimoMensajeError = "";
            if (string.IsNullOrEmpty(Password))
            {
                UltimoMensajeError = "Debe asignar un token antes de conectar";
                return false;
            }

            try
            {
                if (_service == null) _service = new HttpClient();
                
                _service.DefaultRequestHeaders.Clear();
                _service.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{Usuario} {Password}");
                _service.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                return true;
            }
            catch (Exception e)
            {
                UltimoMensajeError = $"Error al preparar conexión API: {e.Message}";
                return false;
            }
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

        public string MdWhatsappEnlaceV01(string ctipo = "", string ccodigo = "", string cnombre = "", string cfecha = "", string cenlace = "", string cusuariosql = "", string cdescripcion = "", string ntotal = "", string nprocesado = "", string caccionclave = "")
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

        public float MdWhatsappEnviarPlantilla01V01(string cdestino = "", string ccodigo = "", string caccion = "")
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

        public string LeerPropiedad(string cmetodo = "", string cpropiedad = "", int nindice = 0, int nsubindice = 0, int nsubsubindice = 0)
        {
            string cresultado = "";
            UltimoMensajeError = "";
            cpropiedad = cpropiedad.Trim();
            cmetodo = cmetodo.ToLower().Trim();
            string[] lpropiedad = cpropiedad.Split('.');

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
                            
                            if (current is JsonObject obj && obj.ContainsKey(prop)) current = obj[prop];
                            else if (current is JsonArray arr && int.TryParse(prop, out int idx) && idx < arr.Count) current = arr[idx];
                            else { current = null; break; }
                        }
                        cresultado = current?.ToString() ?? "";
                    }
                    catch (Exception e) { UltimoMensajeError = $"Error objeto: {e.Message}"; }
                }
            }
            else { UltimoMensajeError = "Método no definido o no compatible con leerPropiedad"; }

            return cresultado;
        }

        private string PostRequest(string endpointUrl, object payload)
        {
            var jsonString = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

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
}