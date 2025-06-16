// Services/LinePayService.cs
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using RestSharp;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;
using prjEvolutionAPI.Models.DTOs.Account;

namespace prjEvolutionAPI.Services
{
    public class LinePayService : ILinePayService
    {
        private readonly LinePayOptions _opt;
        private readonly JsonSerializerOptions _jsonOpts;

        public LinePayService(IOptions<LinePayOptions> opts)
        {
            _opt = opts.Value;
            _jsonOpts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        private RestClient CreateClient() => new RestClient(_opt.Endpoint);

        /* --------------------------------------------------- *
         *  1) 付款預約（RequestPaymentAsync）
         * --------------------------------------------------- */
        public async Task<LinePayRequestResponse> RequestPaymentAsync(
     decimal amount,
     string orderId,
     string productName,
     string confirmUrl,
     string cancelUrl)
        {
            const string path = "/v3/payments/request";

            // 1. 組 body
            var bodyObj = new
            {
                amount,
                currency = "TWD",
                orderId,
                packages = new[]
                {
            new
            {
                id       = orderId,
                amount,
                products = new[]
                {
                    new { name = productName, quantity = 1, price = amount }
                }
            }
        },
                redirectUrls = new { confirmUrl, cancelUrl }
            };
            var bodyJson = JsonSerializer.Serialize(bodyObj, _jsonOpts);

            // 2. 產生簽章所需 nonce
            var nonce = Guid.NewGuid().ToString("N");

            // 3. 計算簽章
            var signatureSource = _opt.ChannelSecret + path + bodyJson + nonce;
            var signature = ComputeSignature(_opt.ChannelSecret, signatureSource);

            // 4. 建立 RestRequest
            var req = new RestRequest(path, Method.Post);
            req.AddHeader("X-LINE-ChannelId", _opt.ChannelId);
            req.AddHeader("X-LINE-Authorization-Nonce", nonce);
            req.AddHeader("X-LINE-Authorization", signature);
            req.AddStringBody(bodyJson, DataFormat.Json);

            // 5. 呼叫並反序列化
            var resp = await CreateClient().ExecuteAsync(req);
            var result = JsonSerializer.Deserialize<LinePayRequestResponse>(resp.Content!, _jsonOpts)
                         ?? throw new InvalidOperationException($"LINE Pay Request 失敗: {resp.Content}");

            return result;
        }


        /* --------------------------------------------------- *
         *  2) 付款確認（ConfirmPaymentAsync）
         * --------------------------------------------------- */
        public async Task<LinePayConfirmResponse> ConfirmPaymentAsync(
     string transactionId,
     decimal amount)
        {
            var path = $"/v3/payments/{transactionId}/confirm";
            var bodyObj = new { amount, currency = "TWD" };
            var bodyJson = JsonSerializer.Serialize(bodyObj, _jsonOpts);

            // 1. 產生 nonce
            var nonce = Guid.NewGuid().ToString("N");

            // 2. 計算簽章
            var signatureSource = _opt.ChannelSecret + path + bodyJson + nonce;
            var signature = ComputeSignature(_opt.ChannelSecret, signatureSource);

            // 3. 建立請求
            var req = new RestRequest(path, Method.Post);
            req.AddHeader("X-LINE-ChannelId", _opt.ChannelId);
            req.AddHeader("X-LINE-Authorization-Nonce", nonce);
            req.AddHeader("X-LINE-Authorization", signature);
            req.AddStringBody(bodyJson, DataFormat.Json);

            // 4. 執行
            var resp = await CreateClient().ExecuteAsync(req);
            if (!resp.IsSuccessful || string.IsNullOrEmpty(resp.Content))
                throw new InvalidOperationException(
                    $"LINE Pay Confirm HTTP 失敗: {(int)resp.StatusCode}");

            // 5. 反序列化與檢查
            var confirm = JsonSerializer.Deserialize<LinePayConfirmResponse>(
                              resp.Content!, _jsonOpts)
                          ?? throw new InvalidOperationException("LINE Pay Confirm 回傳為 null");

            if (confirm.ReturnCode != "0000")
                throw new InvalidOperationException(
                    $"LINE Pay Confirm 回傳失敗: {confirm.ReturnMessage}");

            return confirm;
        }


        /* --------------------------------------------------- *
         *  3) 共用簽章函式
         * --------------------------------------------------- */
        private static string ComputeSignature(string secret, string source)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(source));
            return Convert.ToBase64String(hash);
        }
    }
}
