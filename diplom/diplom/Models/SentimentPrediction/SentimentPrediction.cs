using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Pullenti.Ner;

namespace diplom.Models.SentimentPrediction
{
    public class EntityFrequency
    {
        public Referent Entity { get; set; }
        public List<int> SentenseIds { get; set; }
        public int MentionsCount { get; set; }
    }

    public class SentimentPredictionModel
    {
        public static string DatasetPath = Path.Combine(Environment.CurrentDirectory, "Models\\SentimentPrediction\\Model", "train.txt");
        public string Text { get; set; }
        public string[] Sentenses { get; set; }
        public List<EntityFrequency> EntitiesFrequency { get; set; }
        protected ITrainerBase trainer;
        public SentimentPredictionModel()
        {
            //var newSample = new SentimentData
            //{
            //    SentimentText = @"Руководство страны начало военную операцию."
            //};

            this.trainer = new OneVersusAllTrainer();
        }

        public void Train()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine($"{trainer.Name}");
            Console.WriteLine("*******************************");

            trainer.Load();
            trainer.Fit(SentimentPredictionModel.DatasetPath);
            var modelMetrics = trainer.Evaluate();

            Console.WriteLine($"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
                                $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
                                $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
                                $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}");
            trainer.Save();
        }

        public List<EntitySentimentPrediction> Predict(string text)
        {
            Console.WriteLine("*******************************");
            Console.WriteLine($"{trainer.Name}");
            Console.WriteLine("*******************************");

            List<SentimentData> sentimentData = new List<SentimentData>();
            string[] sentenses = text.Split(". ", StringSplitOptions.RemoveEmptyEntries);
            List<EntityFrequency> entities = GetTextEntities(sentenses);

            trainer.Load();
            var predictor = new Predictor();
            List<EntitySentimentPrediction> predictions = new List<EntitySentimentPrediction>();

            foreach (EntityFrequency entity in entities)
            {
                SentimentData data = new SentimentData(GetTextWithReplaceEntity(entity, sentenses));

                var prediction = predictor.Predict(data);
                EntitySentimentPrediction entityPrediction = new EntitySentimentPrediction(entity, prediction);
                predictions.Add(entityPrediction);
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Text: {data.SentimentText}");
                Console.WriteLine($"Prediction: {prediction.PredictedLabel:#.##}");
                Console.WriteLine("------------------------------");
            }

            return predictions;
        }

        protected List<EntityFrequency> GetTextEntities(string[] sentenses)
        {
            List<EntityFrequency> entitiesFrequency = new List<EntityFrequency>();

            Pullenti.Sdk.InitializeAll();
            for (int i = 0; i < sentenses.Length; i++)
            {
                // создаём экземпляр процессора со стандартными анализаторами
                Processor processor = ProcessorService.CreateProcessor();
                // запускаем на тексте text
                AnalysisResult result = processor.Process(new SourceOfAnalysis(sentenses[i]));
                // получили выделенные сущности
                foreach (Referent entity in result.Entities)
                {
                    if (entity.InstanceOf.Name != "ORGANIZATION")
                        continue;

                    bool needToCreateNewEntityFrequency = true;
                    foreach (EntityFrequency entityFrequency in entitiesFrequency)
                    {
                        if (entity.CanBeEquals(entityFrequency.Entity))
                        {
                            entityFrequency.MentionsCount++;
                            entityFrequency.SentenseIds.Add(i);
                            needToCreateNewEntityFrequency = false;
                            continue;
                        }
                    }
                    if (needToCreateNewEntityFrequency)
                    {
                        entitiesFrequency.Add(new EntityFrequency
                        {
                            Entity = entity,
                            MentionsCount = 1,
                            SentenseIds = new List<int>() { i }
                        });
                    }

                    Console.WriteLine(entity.ToString());
                }
            }
            Console.WriteLine(entitiesFrequency);
            return entitiesFrequency;
        }

        protected string GetTextWithReplaceEntity(EntityFrequency entityFrequency, string[] sentenses, string replacer = "X")
        {
            string text = String.Empty;
            foreach (int sentenseIndex in entityFrequency.SentenseIds)
            {
                text += sentenses[sentenseIndex];
            }
            foreach (TextAnnotation annotation in entityFrequency.Entity.Occurrence)
            {
                text = text.Replace(annotation.ToString(), replacer);
            }
            return text;
        }

        public void TrainEvaluatePredict(ITrainerBase trainer, SentimentData newSample, bool needToFit = false)
        {
            Console.WriteLine("*******************************");
            Console.WriteLine($"{trainer.Name}");
            Console.WriteLine("*******************************");

            trainer.Load();

            if (needToFit)
            {
                trainer.Fit(SentimentPredictionModel.DatasetPath);
                var modelMetrics = trainer.Evaluate();

                Console.WriteLine($"Macro Accuracy: {modelMetrics.MacroAccuracy:#.##}{Environment.NewLine}" +
                                  $"Micro Accuracy: {modelMetrics.MicroAccuracy:#.##}{Environment.NewLine}" +
                                  $"Log Loss: {modelMetrics.LogLoss:#.##}{Environment.NewLine}" +
                                  $"Log Loss Reduction: {modelMetrics.LogLossReduction:#.##}{Environment.NewLine}");
                trainer.Save();
            }

            var predictor = new Predictor();
            var prediction = predictor.Predict(newSample);
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Prediction: {prediction.PredictedLabel:#.##}");
            Console.WriteLine("------------------------------");
        }
    }

    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1)]
        public string Sentiment;

        public SentimentData(string text)
        {
            SentimentText = text;
        }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; }
    }

    public class EntitySentimentPrediction
    {
        public SentimentPrediction Prediction;

        public EntityFrequency Entity;

        public EntitySentimentPrediction(EntityFrequency entity, SentimentPrediction prediction)
        {
            Prediction = prediction;
            Entity = entity;
        }
    }

    public interface ITrainerBase
    {
        string Name { get; }
        void Fit(string trainingFileName);
        MulticlassClassificationMetrics Evaluate();
        void Save();
        void Load();
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
        /// Load Model from the file.
        /// </summary>
        public void Load()
        {
            DataViewSchema schema;
            _trainedModel = MlContext.Model.Load(ModelPath, out schema);
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
