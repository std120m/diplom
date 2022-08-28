using diplom.Data;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using MySqlConnector;
using System.Data;
using System.Data.Odbc;

namespace diplom.Models
{
    public class ForecastingModel
    {
        public void GetForecast(IConfiguration configuration, diplomContext context)
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string modelPath = Path.Combine(rootDir, "MLModel.zip");
            string connectionString = "server=localhost;database=test;user=root;password=1234";

            MLContext mlContext = new MLContext();

            IDataView data = null;
            SsaForecastingTransformer forecaster = null;
            TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecastEngine = null;
            foreach (Share share in context.Shares.Where(share => share.Id == 3).ToList())
            {
                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();

                long candlesCount = context.Candles.Where(candle => candle.Share.Id == share.Id).Count();
                if (candlesCount < 30)
                    continue;
                string query = @$"select closeByDay.close from (
                        select
                          DATE_FORMAT(Time, '%Y-%m-%d') as day,
                          CAST(substring_index(group_concat(cast(close as CHAR) order by Time desc), ',', 1 ) AS DECIMAL( 9, 2 )) as close
                        from
                          candles
                          where shareId = {share.Id}
                        group by
                          DATE_FORMAT(day, '%Y-%m-%d'), ShareId
                        order by
                          day
                        ) as closeByDay;";
                query = $@"SELECT close, time FROM candles where shareId = {share.Id};";
                DatabaseSource source = new DatabaseSource(MySqlConnectorFactory.Instance, connectionString, query);
                MLContext ctx = new MLContext();
                DatabaseLoader.Options options = new DatabaseLoader.Options();
                options.Columns = new[] { new DatabaseLoader.Column("Close", DbType.Single, 0), new DatabaseLoader.Column("Time", DbType.DateTime, 1) };
                loader = ctx.Data.CreateDatabaseLoader(options);

                data = loader.Load(source);

                var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedClose",
                    inputColumnName: "Close",
                    windowSize: 24,
                    seriesLength: 30*24,
                    trainSize: 6917,
                    horizon: 24,
                    confidenceLevel: 0.95f,
                    confidenceLowerBoundColumn: "LowerBoundClose",
                    confidenceUpperBoundColumn: "UpperBoundClose");

                forecaster = forecastingPipeline.Fit(data);

                Evaluate(data, forecaster, mlContext);
                forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
            }

            if (data != null && forecastEngine != null && forecaster != null)
            {
                forecastEngine.CheckPoint(mlContext, modelPath);
                Forecast(data, 7, forecastEngine, mlContext);
            }
        }

        private void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        {
            IDataView predictions = model.Transform(testData);

            IEnumerable<float> actual =
                mlContext.Data.CreateEnumerable<ModelInput>(testData, true)
                    .Select(observed => observed.Close);

            IEnumerable<float> forecast =
                mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true)
                    .Select(prediction => prediction.ForecastedClose[0]);

            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            var MAE = metrics.Average(error => Math.Abs(error));
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2)));
            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
        }

        private void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
        {
            ModelOutput forecast = forecaster.Predict();

            IEnumerable<string> forecastOutput =
                mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
                    .Take(horizon)
                    .Select((ModelInput input, int index) =>
                    {
                        float actualClose = input.Close;
                        float lowerEstimate = Math.Max(0, forecast.LowerBoundClose[index]);
                        float estimate = forecast.ForecastedClose[index];
                        float upperEstimate = forecast.UpperBoundClose[index];
                        return $"Date: {input.Time}\n" +
                            $"Actual Rentals: {actualClose}\n" +
                            $"Lower Estimate: {lowerEstimate}\n" +
                            $"Forecast: {estimate}\n" +
                            $"Upper Estimate: {upperEstimate}\n";
                    });

            Console.WriteLine("Rental Forecast");
            Console.WriteLine("---------------------");
            foreach (var prediction in forecastOutput)
            {
                Console.WriteLine(prediction);
            }
        }
    }

    public class ModelInput
    {
        public float Close { get; set; }
        public DateTime Time { get; set; }
    }

    public class ModelOutput
    {
        public float[] ForecastedClose { get; set; }
        public float[] LowerBoundClose { get; set; }
        public float[] UpperBoundClose { get; set; }
    }
}
