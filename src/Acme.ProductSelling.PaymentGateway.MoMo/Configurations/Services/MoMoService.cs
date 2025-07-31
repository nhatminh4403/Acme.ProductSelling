//using Newtonsoft.Json;
using Acme.ProductSelling.PaymentGateway.MoMo.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace Acme.ProductSelling.PaymentGateway.MoMo.Services
{
    public class MoMoService : IMoMoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly string _partnerCode;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _requestType;


        public MoMoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _endpoint = configuration["MOMO:Endpoint"];
            _partnerCode = configuration["MOMO:PartnerCode"];
            _accessKey = configuration["MOMO:AccessKey"];
            _secretKey = configuration["MOMO:SecretKey"];
            _requestType = configuration["MOMO:RequestType"];
        }

        public async Task<MoMoPaymentResponse> CreatePaymentAsync(MoMoPaymentRequest request)
        {
            request.ExtraData = request.ExtraData ?? string.Empty;
            request.PartnerCode = _partnerCode;
            request.RequestId = request.OrderId;
            string secretKey = _secretKey;
            request.AccessKey = _accessKey;
            request.RequestType = _requestType;


            var rawHash = "accessKey=" + request.AccessKey +
                          "&amount=" + request.Amount.ToString() +
                          "&extraData=" + request.ExtraData +
                          "&ipnUrl=" + request.IpnUrl +
                          "&orderId=" + request.OrderId +
                          "&orderInfo=" + request.OrderInfo +
                          "&partnerCode=" + request.PartnerCode +
                          "&redirectUrl=" + request.RedirectUrl +
                          "&requestId=" + request.RequestId +
                          "&requestType=" + request.RequestType;

            request.Signature = ComputeHmacSha256(rawHash, secretKey);

            var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });


            // 6. Gửi request
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {

                return JsonSerializer.Deserialize<MoMoPaymentResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            throw new Exception($"Lỗi khi gọi API MoMo ({response.StatusCode}): {responseContent}");
        }

        public async Task<bool> ValidateSignature(
            string partnerCode, string requestId, string orderId, string orderInfo,
            string orderType, long transId, string message,
            int resultCode, string payType, long amount,
            string extraData, string signatureFromCallback, long responseTime)
        {
            try
            {

                var rawHash = "accessKey=" + _accessKey +
                              "&amount=" + amount +
                              "&extraData=" + extraData +
                              "&message=" + message +
                              "&orderId=" + orderId +
                              "&orderInfo=" + orderInfo +
                              "&orderType=" + orderType +
                              "&partnerCode=" + partnerCode +
                              "&payType=" + payType +
                              "&requestId=" + requestId +
                               "&responseTime=" + responseTime +
                              "&resultCode=" + resultCode +
                              "&transId=" + transId;



                var calculatedSignature = ComputeHmacSha256(rawHash, _secretKey);


                // So sánh signature nhận được với signature tính toán
                return signatureFromCallback == calculatedSignature;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ValidateSignature - Callback] Error validating signature: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidateIPNRequest(MomoIPNRequest request)
        {
            try
            {
                // Tạo chuỗi raw hash từ thông tin IPN
                var rawHash = "accessKey=" + _accessKey +
                              "&amount=" + request.Amount +
                              "&extraData=" + request.ExtraData +
                              "&message=" + request.Message +
                              "&orderId=" + request.OrderId +
                              "&orderInfo=" + request.OrderInfo +
                              "&orderType=" + request.OrderType +
                              "&partnerCode=" + request.PartnerCode +
                              "&payType=" + request.PayType +
                              "&requestId=" + request.RequestId +
                              "&responseTime=" + request.ResponseTime +
                              "&resultCode=" + request.ResultCode +
                              "&transId=" + request.TransId + "&ipnUrl=" + request.IpnUrl;

                // Tính toán signature từ raw hash
                var calculatedSignature = ComputeHmacSha256(rawHash, _secretKey);

                // So sánh signature nhận được với signature tính toán
                return request.Signature == calculatedSignature;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string ComputeHmacSha256(string message, string secretKey)
        {
            // Chuyển secretKey và message thành mảng byte
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Tạo HMACSHA256 từ key
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmacsha256.ComputeHash(messageBytes);
                // Chuyển sang hex string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
