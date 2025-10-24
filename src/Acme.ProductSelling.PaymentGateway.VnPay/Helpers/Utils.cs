using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Acme.ProductSelling.PaymentGateway.VnPay.Helpers
{
    public class Utils
    {
        public static string HmacSHA512(string key, string inputData)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            if (string.IsNullOrWhiteSpace(inputData))
                throw new ArgumentException("Input data cannot be null or empty", nameof(inputData));

            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        // IMPROVEMENT: Better IP address extraction
        public static string GetIpAddress(HttpContext context)
        {
            if (context == null)
                return "127.0.0.1";

            try
            {
                // Check X-Forwarded-For header first (for proxies/load balancers)
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    // Take the first IP if multiple IPs are present
                    var firstIp = forwardedFor.Split(',')[0].Trim();
                    if (IPAddress.TryParse(firstIp, out var parsedIp))
                    {
                        return parsedIp.ToString();
                    }
                }

                // Check X-Real-IP header
                var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(realIp) && IPAddress.TryParse(realIp, out var parsedRealIp))
                {
                    return parsedRealIp.ToString();
                }

                // Fall back to RemoteIpAddress
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    // Convert IPv6 localhost to IPv4
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        if (remoteIpAddress.Equals(IPAddress.IPv6Loopback))
                        {
                            return "127.0.0.1";
                        }

                        // Try to get IPv4 address
                        var ipv4 = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                        if (ipv4 != null)
                        {
                            return ipv4.ToString();
                        }
                    }

                    return remoteIpAddress.ToString();
                }
            }
            catch (Exception)
            {
                // Log if needed, but don't expose exception details
            }

            return "127.0.0.1"; // Default fallback
        }
    }
}