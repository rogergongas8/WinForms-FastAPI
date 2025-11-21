using System.Text;
using Newtonsoft.Json;

namespace SuperMarketClient
{
    public class ApiClient
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "http://127.0.0.1:8000";

        // GET
        public async Task<List<Producto>> GetProductosAsync()
        {
            var json = await _client.GetStringAsync($"{_baseUrl}/productos");
            return JsonConvert.DeserializeObject<List<Producto>>(json);
        }

        // GET
        public async Task<List<Venta>> GetVentasAsync()
        {
            var json = await _client.GetStringAsync($"{_baseUrl}/ventas");
            return JsonConvert.DeserializeObject<List<Venta>>(json);
        }

        // POST
        public async Task CrearProductoAsync(Producto p)
        {
            await SendRequestAsync(p, $"{_baseUrl}/productos", HttpMethod.Post);
        }

        // POST
        public async Task RegistrarVentaAsync(Venta v)
        {
            await SendRequestAsync(v, $"{_baseUrl}/ventas", HttpMethod.Post);
        }

        // PUT
        public async Task ActualizarProductoAsync(int id, Producto p)
        {
            await SendRequestAsync(p, $"{_baseUrl}/productos/{id}", HttpMethod.Put);
        }

        // DELETE
        public async Task EliminarProductoAsync(int id)
        {
            var response = await _client.DeleteAsync($"{_baseUrl}/productos/{id}");
            response.EnsureSuccessStatusCode();
        }
        
        private async Task SendRequestAsync<T>(T data, string url, HttpMethod method)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response;
            if (method == HttpMethod.Post) 
                response = await _client.PostAsync(url, content);
            else 
                response = await _client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }
    }
}