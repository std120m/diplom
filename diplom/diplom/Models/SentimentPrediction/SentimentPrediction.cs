using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace diplom.Models.SentimentPrediction
{
    public class SentimentPredictionModel
    {
        public SentimentPredictionModel()
        {
            var newSample = new SentimentData
            {
                SentimentText = @"В офисах Сбера начали тестировать технологию помощи посетителям в экстренных ситуациях. «Зеленая кнопка» будет
                    в зонах круглосуточного обслуживания офисов банка в Воронеже, Санкт-Петербурге, Подольске, Пскове, Орле и Ярославле.
                    В них находятся стенды с сенсорными кнопками, обеспечивающие связь с операторами центра мониторинга службы безопасности
                    банка. Получив сигнал о помощи, оператор центра может подключиться к объекту по голосовой связи. С помощью камер
                    видеонаблюдения он оценит обстановку и при необходимости вызовет полицию или скорую помощь. «Зеленой кнопкой» можно
                    воспользоваться в нерабочее для отделения время, если возникла угроза жизни или здоровью. В остальных случаях помочь
                    клиентам готовы сотрудники отделения банка. «Одно из направлений нашей работы в области ESG и устойчивого развития
                    — это забота об обществе. И здоровье людей как высшая ценность является его основой. Поэтому задача банка в области
                    безопасности гораздо масштабнее, чем обеспечение только финансовой безопасности клиентов. Этот пилотный проект
                    приурочен к 180-летию Сбербанка: мы хотим, чтобы, приходя в банк, клиент чувствовал, что его жизнь и безопасность
                    — наша ценность», — отметил заместитель председателя правления Сбербанка Станислав Кузнецов."
            };


            var trainers = new List<ITrainerBase>
            {
                new LbfgsMaximumEntropyTrainer(),
                new NaiveBayesTrainer(),
                new OneVersusAllTrainer(),
                new SdcaMaximumEntropyTrainer(),
                new SdcaNonCalibratedTrainer()
            };

            trainers.ForEach(t => TrainEvaluatePredict(t, newSample));
        }

        static void TrainEvaluatePredict(ITrainerBase trainer, SentimentData newSample)
        {
            Console.WriteLine("*******************************");
            Console.WriteLine($"{trainer.Name}");
            Console.WriteLine("*******************************");

            //trainer.Fit(Path.Combine(Environment.CurrentDirectory, "Models\\SentimentPrediction\\Model", "doc_comment_summary.txt"));
            trainer.Fit(Path.Combine(Environment.CurrentDirectory, "Models\\SentimentPrediction\\Model", "doc.txt"));

            var modelMetrics = trainer.Evaluate();

            Console.WriteLine($"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
                              $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
                              $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
                              $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}");

            trainer.Save();

            var predictor = new Predictor();
            var prediction = predictor.Predict(newSample);
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Prediction: {prediction.PredictedLabel:#.##}");
            Console.WriteLine("------------------------------");
        }
    }

    /// <summary>
    /// Models Palmer Penguins Binary Data.
    /// </summary>
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1)]
        public string Sentiment;
    }

    /// <summary>
    /// Models Palmer Penguins Binary Prediction.
    /// </summary>
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; }
    }

    public interface ITrainerBase
    {
        string Name { get; }
        void Fit(string trainingFileName);
        MulticlassClassificationMetrics Evaluate();
        void Save();
    }

    /// <summary>
    /// Base class for Trainers.
    /// This class exposes methods for training, evaluating and saving ML Models.
    /// Classes that inherit this class need to assing concrete model and name; and to implement data pre-processing.
    /// </summary>
    public abstract class TrainerBase<TParameters> : ITrainerBase
        where TParameters : class
    {
        public string Name { get; protected set; }

        protected static string ModelPath => Path.Combine(AppContext.BaseDirectory, "classification.mdl");

        protected readonly MLContext MlContext;

        protected DataOperationsCatalog.TrainTestData _dataSplit;
        protected ITrainerEstimator<MulticlassPredictionTransformer<TParameters>, TParameters> _model;
        protected ITransformer _trainedModel;

        protected TrainerBase()
        {
            MlContext = new MLContext(111);
        }

        /// <summary>
        /// Train model on defined data.
        /// </summary>
        /// <param name="trainingFileName"></param>
        public void Fit(string trainingFileName)
        {
            if (!File.Exists(trainingFileName))
            {
                throw new FileNotFoundException($"File {trainingFileName} doesn't exist.");
            }

            _dataSplit = LoadAndPrepareData(trainingFileName);
            var dataProcessPipeline = BuildDataProcessingPipeline();
            var trainingPipeline = dataProcessPipeline
                                    .Append(_model)
                                    .Append(MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _trainedModel = trainingPipeline.Fit(_dataSplit.TrainSet);
        }

        /// <summary>
        /// Evaluate trained model.
        /// </summary>
        /// <returns>RegressionMetrics object which contain information about model performance.</returns>
        public MulticlassClassificationMetrics Evaluate()
        {
            var testSetTransform = _trainedModel.Transform(_dataSplit.TestSet);

            return MlContext.MulticlassClassification.Evaluate(testSetTransform);
        }

        /// <summary>
        /// Save Model in the file.
        /// </summary>
        public void Save()
        {
            MlContext.Model.Save(_trainedModel, _dataSplit.TrainSet.Schema, ModelPath);
        }

        /// <summary>
        /// Feature engeneering and data pre-processing.
        /// </summary>
        /// <returns>Data Processing Pipeline.</returns>
        private EstimatorChain<NormalizingTransformer> BuildDataProcessingPipeline()
        {
            var dataProcessPipeline = MlContext.Transforms.Conversion.MapValueToKey(inputColumnName: nameof(SentimentData.Sentiment), outputColumnName: "Label")
               .Append(MlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText)))
               .Append(MlContext.Transforms.NormalizeMinMax("Features", "Features"))
               .AppendCacheCheckpoint(MlContext);

            return dataProcessPipeline;
        }

        private DataOperationsCatalog.TrainTestData LoadAndPrepareData(string trainingFileName)
        {
            var trainingDataView = MlContext.Data.LoadFromTextFile<SentimentData>(trainingFileName, hasHeader: false, separatorChar: '\t');
            return MlContext.Data.TrainTestSplit(trainingDataView, testFraction: 0.3);
        }
    }
    public class LbfgsMaximumEntropyTrainer : TrainerBase<MaximumEntropyModelParameters>
    {
        public LbfgsMaximumEntropyTrainer() : base()
        {
            Name = "LBFGS Maximum Entropy";
            _model = MlContext.MulticlassClassification.Trainers
              .LbfgsMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features");
        }
    }

    public class NaiveBayesTrainer : TrainerBase<NaiveBayesMulticlassModelParameters>
    {
        public NaiveBayesTrainer() : base()
        {
            Name = "Naive Bayes";
            _model = MlContext.MulticlassClassification.Trainers
                      .NaiveBayes(labelColumnName: "Label", featureColumnName: "Features");
        }
    }

    public class OneVersusAllTrainer : TrainerBase<OneVersusAllModelParameters>
    {
        public OneVersusAllTrainer() : base()
        {
            Name = "One Versus All";
            _model = MlContext.MulticlassClassification.Trainers
          .OneVersusAll(binaryEstimator: MlContext.BinaryClassification.Trainers.SgdCalibrated());
        }
    }

    public class SdcaMaximumEntropyTrainer : TrainerBase<MaximumEntropyModelParameters>
    {
        public SdcaMaximumEntropyTrainer() : base()
        {
            Name = "Sdca Maximum Entropy";
            _model = MlContext.MulticlassClassification.Trainers
              .SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features");
        }
    }

    public class SdcaNonCalibratedTrainer : TrainerBase<LinearMulticlassModelParameters>
    {
        public SdcaNonCalibratedTrainer() : base()
        {
            Name = "Sdca NonCalibrated";
            _model = MlContext.MulticlassClassification.Trainers
              .SdcaNonCalibrated(labelColumnName: "Label", featureColumnName: "Features");
        }
    }
    public class Predictor
    {
        protected static string ModelPath => Path.Combine(AppContext.BaseDirectory, "classification.mdl");
        private readonly MLContext _mlContext;

        private ITransformer _model;

        public Predictor()
        {
            _mlContext = new MLContext(111);
        }

        /// <summary>
        /// Runs prediction on new data.
        /// </summary>
        /// <param name="newSample">New data sample.</param>
        /// <returns>PalmerPenguinsData object, which contains predictions made by model.</returns>
        public SentimentPrediction Predict(SentimentData newSample)
        {
            LoadModel();

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);

            return predictionEngine.Predict(newSample);
        }

        private void LoadModel()
        {
            if (!File.Exists(ModelPath))
            {
                throw new FileNotFoundException($"File {ModelPath} doesn't exist.");
            }

            using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _model = _mlContext.Model.Load(stream, out _);
            }

            if (_model == null)
            {
                throw new Exception($"Failed to load Model");
            }
        }
    }
}
