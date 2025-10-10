using Microsoft.ML.Data;

namespace Acme.ProductSelling.Chatbots.ML
{
    public class IntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Intent { get; set; }

        [ColumnName("Score")]
        public float[] Score { get; set; }
    }
}
