﻿using diplom.Data;
using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using MySqlConnector;
using System.Data;
using System.Data.Odbc;

namespace diplom.Models
{
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

    public class ForecastingModel
    {
        const string connectionString = "server=localhost;database=test;user=root;password=1234";
        public enum ForecastHorizon
        {
            week,
            month,
            halfYear
        }
        private ForecastHorizon horizon;

        public ForecastingModel(ForecastHorizon horizon = ForecastHorizon.week)
        {
            this.horizon = horizon;
        }

        private string getFileName()
        {
            switch (this.horizon)
            {
                case ForecastHorizon.week:
                    return "MLModel.zip";
                case ForecastHorizon.month:
                    return "MLModel_month.zip";
                case ForecastHorizon.halfYear:
                    return "MLModel_half_year.zip";
                default:
                    return "MLModel.zip";
            }
        }

        private int getHorizonValue()
        {
            switch (this.horizon)
            {
                case ForecastHorizon.week:
                    return 7*24;
                case ForecastHorizon.month:
                    return 24*30;
                case ForecastHorizon.halfYear:
                    return 24*180;
                default:
                    return 7*24;
            }
        }

        private string getModelPath()
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string fileName = this.getFileName();
            return Path.Combine(rootDir, fileName);
        }

        private SsaForecastingEstimator GetForecastingPipline(MLContext mlContext)
        {
            SsaForecastingEstimator forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedClose",
                inputColumnName: "Close",
                windowSize: 365,
                seriesLength: 180 * 24,
                trainSize: 365 * 5,
                horizon: getHorizonValue(),
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundClose",
                confidenceUpperBoundColumn: "UpperBoundClose"
            );

            return forecastingPipeline;
        }

        private SsaForecastingTransformer getForecaster(long shareId, out MLContext mlContext, out IDataView data, out DatabaseLoader loader)
        {
            string modelPath = this.getModelPath();
            string query = $@"SELECT close, time FROM candles where shareId = {shareId};";
            SsaForecastingTransformer forecaster = null;

            DatabaseSource source = new DatabaseSource(MySqlConnectorFactory.Instance, ForecastingModel.connectionString, query);
            mlContext = new MLContext();
            MLContext ctx = new MLContext();

            loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();
            DatabaseLoader.Options options = new DatabaseLoader.Options();
            options.Columns = new[] { new DatabaseLoader.Column("Close", DbType.Single, 0), new DatabaseLoader.Column("Time", DbType.DateTime, 1) };
            loader = ctx.Data.CreateDatabaseLoader(options);

            data = loader.Load(source);

            //if (File.Exists(modelPath))
            //{
            //    using (var file = File.OpenRead(modelPath))
            //    {
            //        forecaster = (SsaForecastingTransformer)mlContext.Model.Load(file, out DataViewSchema schema);
            //    }
            //}
            //else
            //{
                forecaster = this.GetForecastingPipline(mlContext).Fit(data);
                mlContext.Model.Save(forecaster, loader, getFileName());
            //}

            return forecaster;
        }

        public void GetForecast(IConfiguration configuration, diplomContext context, int shareId)
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string modelPath = Path.Combine(rootDir, getFileName());
            string connectionString = ForecastingModel.connectionString;

            MLContext mlContext = new MLContext();

            IDataView data = null, evaluateDate = null;
            //SsaForecastingTransformer forecaster = null;
            ITransformer forecaster = null;
            TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecastEngine = null;
            foreach (Share share in context.Shares.Where(share => share.Id == shareId).ToList())
            {
                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();

                long candlesCount = context.Candles.Where(candle => candle.Share.Id == share.Id).Count();
                if (candlesCount < 30)
                    continue;
                
                string query = @$"select closeByDay.day as Time, closeByDay.close from (
                        select
                          DATE_FORMAT(Time, '%Y-%m-%d') as day,
                          CAST(substring_index(group_concat(cast(close as CHAR) order by Time desc), ',', 1 ) AS REAL) as close
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

                //using (var file = File.OpenRead(modelPath))
                //{
                //    forecaster = mlContext.Model.Load(file, out DataViewSchema schema);
                //    //forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
                //}

                SsaForecastingEstimator forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "ForecastedClose",
                    inputColumnName: "Close",
                    windowSize: 365,
                    seriesLength: 180 * 24,
                    trainSize: 365 * 5,
                    horizon: getHorizonValue(),
                    confidenceLevel: 0.95f,
                    confidenceLowerBoundColumn: "LowerBoundClose",
                    confidenceUpperBoundColumn: "UpperBoundClose");

                forecaster = forecastingPipeline.Fit(data);

                Evaluate(data, forecaster, mlContext);
                forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);

                mlContext.Model.Save(forecaster, loader, this.getFileName());
            }

            if (data != null && forecastEngine != null && forecaster != null)
            {
                forecastEngine.CheckPoint(mlContext, modelPath);
                Forecast(data, getHorizonValue(), forecastEngine, mlContext);
            }
        }

        public void Fit(long shareId)
        {
            SsaForecastingTransformer forecaster = this.getForecaster(shareId, out MLContext mlContext, out IDataView data, out DatabaseLoader loader);

            forecaster = this.GetForecastingPipline(mlContext).Fit(data);
            Evaluate(data, forecaster, mlContext);
            mlContext.Model.Save(forecaster, loader, this.getFileName());
        }

        public ModelOutput GetForecast(long shareId)
        {
            SsaForecastingTransformer forecaster = this.getForecaster(shareId, out MLContext mlContext, out IDataView data, out DatabaseLoader loader);

            TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
            if (data != null && forecastEngine != null && forecaster != null)
            {
                forecastEngine.CheckPoint(mlContext, this.getModelPath());
                return Forecast(data, getHorizonValue(), forecastEngine, mlContext);
            }
            return null;
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

        private ModelOutput Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
        {
            ModelOutput forecast = forecaster.Predict();

            IEnumerable<string> forecastOutput =
                mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
                    .TakeLast(horizon)
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
            return forecast;
        }
    }
}
