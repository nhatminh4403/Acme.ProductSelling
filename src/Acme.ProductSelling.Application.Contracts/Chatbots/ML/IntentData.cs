using Microsoft.ML.Data;
namespace Acme.ProductSelling.Chatbots.ML
{
    public class IntentData
    {
        [LoadColumn(0)]
        public string Message { get; set; }
        [LoadColumn(1)]
        public string Intent { get; set; }
    }
}
