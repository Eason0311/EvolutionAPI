using Microsoft.Extensions.Options;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;
using RestSharp;

namespace prjEvolutionAPI.Services
{
    public class LinePayService : ILinePayService
    {
        private readonly LinePayOptions _opt;
        public LinePayService(IOptions<LinePayOptions> opts)
            => _opt = opts.Value;

        private RestClient CreateClient()
            => new RestClient(_opt.Endpoint);

        public async Task<LinePayRequestResponse> RequestPaymentAsync(decimal amount, string orderId, string productName)
        {
            var req = new RestRequest("/v3/payments/request", Method.Post);
            req.AddHeader("X-LINE-ChannelId", _opt.ChannelId);
            req.AddHeader("X-LINE-ChannelSecret", _opt.ChannelSecret);
            req.AddJsonBody(new
            {
                amount,
                currency = "TWD",
                orderId,
                packages = new[] {
                    new {
                        id       = "package-1",
                        amount,
                        name     = productName,
                        products = new[] {
                            new { name = productName, quantity = 1, price = amount }
                        }
                    }
                },
                confirmUrl = _opt.ConfirmUrl,
                cancelUrl = _opt.CancelUrl
            });

            var resp = await CreateClient()
                              .ExecuteAsync<LinePayRequestResponse>(req);
            return resp.Data!;
        }

        public async Task<LinePayConfirmResponse> ConfirmPaymentAsync(string transactionId, decimal amount)
        {
            var req = new RestRequest($"/v3/payments/{transactionId}/confirm", Method.Post);
            req.AddHeader("X-LINE-ChannelId", _opt.ChannelId);
            req.AddHeader("X-LINE-ChannelSecret", _opt.ChannelSecret);
            req.AddJsonBody(new
            {
                amount,
                currency = "TWD"
            });

            var resp = await CreateClient()
                                .ExecuteAsync<LinePayConfirmResponse>(req);
            return resp.Data!;
        }
    }
}
