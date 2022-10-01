//// <SnippetAddUsings>
//using Microsoft.ML;
//using Microsoft.ML.Data;
//using Pullenti.Ner;
//using static Microsoft.ML.DataOperationsCatalog;
//// </SnippetAddUsings>

//namespace diplom.Models.SentimentPrediction
//{
//    // <SnippetDeclareTypes>
//    public class SentimentData
//    {
//        [LoadColumn(0)]
//        public string SentimentText;

//        [LoadColumn(1), ColumnName("Label")]
//        public float Sentiment;
//    }

//    public class SentimentPrediction : SentimentData
//    {

//        [ColumnName("PredictedLabel")]
//        public float Prediction { get; set; }

//        public float Probability { get; set; }

//        public float[] Score { get; set; }
//    }
//    // </SnippetDeclareTypes>

//    public class EntityFrequency
//    {
//        public Referent Entity { get; set; }
//        public List<int> SentenseIds { get; set; }
//        public int MentionsCount { get; set; }
//    }

//    class SentimentPrediction2
//    {
//        // <SnippetDeclareGlobalVariables>
//        //static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Models\\SentimentPrediction\\Model", "yelp_labelled.txt");
//        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Models\\SentimentPrediction\\Model", "doc_comment_summary.txt");
//        // </SnippetDeclareGlobalVariables>

//        public string Text { get; set; }
//        public string[] Sentenses { get; set; }
//        public List<EntityFrequency> EntitiesFrequency { get; set; }

//        public SentimentPrediction2()
//        {
//            string text = @"В офисах Сбера начали тестировать технологию помощи посетителям в экстренных ситуациях. «Зеленая кнопка» будет
//                в зонах круглосуточного обслуживания офисов банка в Воронеже, Санкт-Петербурге, Подольске, Пскове, Орле и Ярославле.
//                В них находятся стенды с сенсорными кнопками, обеспечивающие связь с операторами центра мониторинга службы безопасности
//                банка. Получив сигнал о помощи, оператор центра может подключиться к объекту по голосовой связи. С помощью камер
//                видеонаблюдения он оценит обстановку и при необходимости вызовет полицию или скорую помощь. «Зеленой кнопкой» можно
//                воспользоваться в нерабочее для отделения время, если возникла угроза жизни или здоровью. В остальных случаях помочь
//                клиентам готовы сотрудники отделения банка. «Одно из направлений нашей работы в области ESG и устойчивого развития
//                — это забота об обществе. И здоровье людей как высшая ценность является его основой. Поэтому задача банка в области
//                безопасности гораздо масштабнее, чем обеспечение только финансовой безопасности клиентов. Этот пилотный проект
//                приурочен к 180-летию Сбербанка: мы хотим, чтобы, приходя в банк, клиент чувствовал, что его жизнь и безопасность
//                — наша ценность», — отметил заместитель председателя правления Сбербанка Станислав Кузнецов.";
//            this.Text = text;

//            this.Sentenses = text.Split(". ", StringSplitOptions.RemoveEmptyEntries);
//            this.EntitiesFrequency = new List<EntityFrequency>();

//            Pullenti.Sdk.InitializeAll();
//            for (int i = 0; i < this.Sentenses.Length; i++)
//            {
//                // создаём экземпляр процессора со стандартными анализаторами
//                Processor processor = ProcessorService.CreateProcessor();
//                // запускаем на тексте text
//                AnalysisResult result = processor.Process(new SourceOfAnalysis(this.Sentenses[i]));
//                // получили выделенные сущности
//                foreach (Referent entity in result.Entities)
//                {
//                    if (entity.InstanceOf.Name != "ORGANIZATION")
//                        continue;

//                    bool needToCreateNewEntityFrequency = true;
//                    foreach (EntityFrequency entityFrequency in this.EntitiesFrequency)
//                    {
//                        if (entity.CanBeEquals(entityFrequency.Entity))
//                        {
//                            entityFrequency.MentionsCount++;
//                            entityFrequency.SentenseIds.Add(i);
//                            needToCreateNewEntityFrequency = false;
//                            continue;
//                        }
//                    }
//                    if (needToCreateNewEntityFrequency)
//                    {
//                        this.EntitiesFrequency.Add(new EntityFrequency
//                        {
//                            Entity = entity,
//                            MentionsCount = 1,
//                            SentenseIds = new List<int>() { i }
//                        });
//                    }

//                    Console.WriteLine(entity.ToString());
//                }
//            }
//            Console.WriteLine(this.EntitiesFrequency);



//            // Create ML.NET context/local environment - allows you to add steps in order to keep everything together
//            // as you discover the ML.NET trainers and transforms
//            // <SnippetCreateMLContext>
//            MLContext mlContext = new MLContext();
//            // </SnippetCreateMLContext>

//            // <SnippetCallLoadData>
//            TrainTestData splitDataView = LoadData(mlContext);
//            // </SnippetCallLoadData>

//            // <SnippetCallBuildAndTrainModel>
//            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
//            // </SnippetCallBuildAndTrainModel>

//            // <SnippetCallEvaluate>
//            Evaluate(mlContext, model, splitDataView.TestSet);
//            // </SnippetCallEvaluate>

//            // <SnippetCallUseModelWithSingleItem>
//            UseModelWithSingleItem(mlContext, model);
//            // </SnippetCallUseModelWithSingleItem>

//            // <SnippetCallUseModelWithBatchItems>
//            UseModelWithBatchItems(mlContext, model);
//            // </SnippetCallUseModelWithBatchItems>

//            Console.WriteLine();
//            Console.WriteLine("=============== End of process ===============");
//        }

//        protected string GetTextWithReplaceEntity(EntityFrequency entityFrequency, string replacer = "X")
//        {
//            string text = String.Empty;
//            foreach (int sentenseIndex in entityFrequency.SentenseIds)
//            {
//                text += this.Sentenses[sentenseIndex];
//            }
//            foreach (TextAnnotation annotation in entityFrequency.Entity.Occurrence)
//            {
//                text = text.Replace(annotation.ToString(), replacer);
//            }
//            return text;
//        }

//        public static TrainTestData LoadData(MLContext mlContext)
//        {
//            // Note that this case, loading your training data from a file,
//            // is the easiest way to get started, but ML.NET also allows you
//            // to load data from databases or in-memory collections.
//            // <SnippetLoadData>
//            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
//            // </SnippetLoadData>

//            // You need both a training dataset to train the model and a test dataset to evaluate the model.
//            // Split the loaded dataset into train and test datasets
//            // Specify test dataset percentage with the `testFraction`parameter
//            // <SnippetSplitData>
//            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
//            // </SnippetSplitData>

//            // <SnippetReturnSplitData>
//            return splitDataView;
//            // </SnippetReturnSplitData>
//        }

//        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
//        {
//            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("RiskCategory", "RiskCategory")
//                          .Append(mlContext.Transforms.Categorical.OneHotEncoding(new[] { new InputOutputColumnPair("InspectionType", "InspectionType"), new InputOutputColumnPair("ViolationDescription", "ViolationDescription") }))
//                          .Append(mlContext.Transforms.Concatenate("Features", new[] { "InspectionType", "ViolationDescription" }))
//                          .Append(mlContext.Transforms.NormalizeMinMax("Features", "Features"))
//                          .AppendCacheCheckpoint(mlContext);

//            // Set the training algorithm 
//            var trainer = mlContext.MulticlassClassification.Trainers.OneVersusAll(mlContext.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName: "RiskCategory", numberOfIterations: 10, featureColumnName: "Features"), labelColumnName: "RiskCategory")
//                                      .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
//            var trainingPipeline = dataProcessPipeline.Append(trainer);

//            return trainingPipeline;
//            // Create a flexible pipeline (composed by a chain of estimators) for creating/training the model.
//            // This is used to format and clean the data.
//            // Convert the text column to numeric vectors (Features column)
//            // <SnippetFeaturizeText>
//            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
//            //</SnippetFeaturizeText>
//            // append the machine learning task to the estimator
//            // <SnippetAddTrainer>
//            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"));
//            // </SnippetAddTrainer>

//            // Create and train the model based on the dataset that has been loaded, transformed.
//            // <SnippetTrainModel>
//            Console.WriteLine("=============== Create and Train the Model ===============");
//            var model = estimator.Fit(splitTrainSet);
//            Console.WriteLine("=============== End of training ===============");
//            Console.WriteLine();
//            // </SnippetTrainModel>

//            // Returns the model we trained to use for evaluation.
//            // <SnippetReturnModel>
//            return model;
//            // </SnippetReturnModel>
//        }

//        public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
//        {
//            // Evaluate the model and show accuracy stats

//            //Take the data in, make transformations, output the data.
//            // <SnippetTransformData>
//            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
//            IDataView predictions = model.Transform(splitTestSet);
//            // </SnippetTransformData>

//            // BinaryClassificationContext.Evaluate returns a BinaryClassificationEvaluator.CalibratedResult
//            // that contains the computed overall metrics.
//            // <SnippetEvaluate>
//            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
//            // </SnippetEvaluate>

//            // The Accuracy metric gets the accuracy of a model, which is the proportion
//            // of correct predictions in the test set.

//            // The AreaUnderROCCurve metric is equal to the probability that the algorithm ranks
//            // a randomly chosen positive instance higher than a randomly chosen negative one
//            // (assuming 'positive' ranks higher than 'negative').

//            // The F1Score metric gets the model's F1 score.
//            // The F1 score is the harmonic mean of precision and recall:
//            //  2 * precision * recall / (precision + recall).

//            // <SnippetDisplayMetrics>
//            Console.WriteLine();
//            Console.WriteLine("Model quality metrics evaluation");
//            Console.WriteLine("--------------------------------");
//            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
//            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
//            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
//            Console.WriteLine("=============== End of model evaluation ===============");
//            //</SnippetDisplayMetrics>
//        }

//        private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
//        {
//            // <SnippetCreatePredictionEngine1>
//            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
//            // </SnippetCreatePredictionEngine1>

//            // <SnippetCreateTestIssue1>
//            SentimentData sampleStatement = new SentimentData
//            {
//                SentimentText = "This was a very bad steak"
//            };
//            // </SnippetCreateTestIssue1>

//            // <SnippetPredict>
//            var resultPrediction = predictionFunction.Predict(sampleStatement);
//            // </SnippetPredict>
//            // <SnippetOutputPrediction>
//            Console.WriteLine();
//            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

//            Console.WriteLine();
//            //Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");
//            Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {resultPrediction.Prediction} | Probability: {resultPrediction.Probability} ");

//            Console.WriteLine("=============== End of Predictions ===============");
//            Console.WriteLine();
//            // </SnippetOutputPrediction>
//        }

//        public void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
//        {
//            List<SentimentData> sentimentData = new List<SentimentData>();
//            foreach (EntityFrequency entity in this.EntitiesFrequency)
//            {
//                sentimentData.Add(new SentimentData
//                {
//                    SentimentText = this.GetTextWithReplaceEntity(entity)
//                });
//            }

//            // Adds some comments to test the trained model's data points.
//            // <SnippetCreateTestIssues>
//            IEnumerable<SentimentData> sentiments = sentimentData;
//            // </SnippetCreateTestIssues>

//            // Load batch comments just created
//            // <SnippetPrediction>
//            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

//            IDataView predictions = model.Transform(batchComments);

//            // Use model to predict whether comment data is Positive (1) or Negative (0).
//            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
//            // </SnippetPrediction>

//            // <SnippetAddInfoMessage>
//            Console.WriteLine();

//            Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");
//            // </SnippetAddInfoMessage>

//            Console.WriteLine();

//            // <SnippetDisplayResults>
//            foreach (SentimentPrediction prediction in predictedResults)
//            {
//                //Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
//                Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {prediction.Prediction} | Probability: {prediction.Probability} ");
//            }
//            Console.WriteLine("=============== End of predictions ===============");
//            // </SnippetDisplayResults>
//        }
//    }
//}