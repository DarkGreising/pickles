﻿using System;

using NUnit.Framework;

using PicklesDoc.Pickles.ObjectModel;
using PicklesDoc.Pickles.TestFrameworks;

using NFluent;

namespace PicklesDoc.Pickles.Test.TestFrameworks
{
    [TestFixture]
    public class WhenParsingNUnitResultsFile : WhenParsingTestResultFiles<NUnitResults>
    {
        public WhenParsingNUnitResultsFile()
            : base("results-example-nunit.xml")
        {
        }

        [Test]
        public void ThenCanReadFeatureResultSuccessfully()
        {
            // Write out the embedded test results file
            var results = ParseResultsFile();

            var feature = new Feature { Name = "Addition" };
            TestResult result = results.GetFeatureResult(feature);

            Check.That(result.WasExecuted).IsTrue();
            Check.That(result.WasSuccessful).IsFalse();
        }

        [Test]
        public void ThenCanReadScenarioOutlineResultSuccessfully()
        {
            var results = ParseResultsFile();

            var feature = new Feature { Name = "Addition" };

            var scenarioOutline = new ScenarioOutline { Name = "Adding several numbers", Feature = feature };
            TestResult result = results.GetScenarioOutlineResult(scenarioOutline);

            Check.That(result.WasExecuted).IsTrue();
            Check.That(result.WasSuccessful).IsTrue();

            TestResult exampleResult1 = results.GetExampleResult(scenarioOutline, new[] { "40", "50", "90" });
            Check.That(exampleResult1.WasExecuted).IsTrue();
            Check.That(exampleResult1.WasSuccessful).IsTrue();

            TestResult exampleResult2 = results.GetExampleResult(scenarioOutline, new[] { "60", "70", "130" });
            Check.That(exampleResult2.WasExecuted).IsTrue();
            Check.That(exampleResult2.WasSuccessful).IsTrue();
        }

        [Test]
        public void WithoutExampleSignatureBuilderThrowsInvalidOperationException()
        {
            var results = ParseResultsFile();
            results.SetExampleSignatureBuilder(null);

            var feature = new Feature { Name = "Addition" };

            var scenarioOutline = new ScenarioOutline { Name = "Adding several numbers", Feature = feature };

            Check.ThatCode(() => results.GetExampleResult(scenarioOutline, new[] { "40", "50", "90" })).Throws<InvalidOperationException>();
        }

        [Test]
        public void ThenCanReadSuccessfulScenarioResultSuccessfully()
        {
            var results = ParseResultsFile();

            var feature = new Feature { Name = "Addition" };

            var passedScenario = new Scenario { Name = "Add two numbers", Feature = feature };
            TestResult result = results.GetScenarioResult(passedScenario);

            Check.That(result.WasExecuted).IsTrue();
            Check.That(result.WasSuccessful).IsTrue();
        }

        [Test]
        public void ThenCanReadFailedScenarioResultSuccessfully()
        {
            var results = ParseResultsFile();
            var feature = new Feature { Name = "Addition" };
            var scenario = new Scenario { Name = "Fail to add two numbers", Feature = feature };
            TestResult result = results.GetScenarioResult(scenario);

            Check.That(result.WasExecuted).IsTrue();
            Check.That(result.WasSuccessful).IsFalse();
        }

        [Test]
        public void ThenCanReadIgnoredScenarioResultSuccessfully()
        {
            var results = ParseResultsFile();
            var feature = new Feature { Name = "Addition" };
            var ignoredScenario = new Scenario { Name = "Ignored adding two numbers", Feature = feature };
            var result = results.GetScenarioResult(ignoredScenario);

            Check.That(result.WasExecuted).IsFalse();
            Check.That(result.WasSuccessful).IsFalse();
        }

        [Test]
        public void ThenCanReadInconclusiveScenarioResultSuccessfully()
        {
            var results = ParseResultsFile();
            var feature = new Feature { Name = "Addition" };
            var inconclusiveScenario = new Scenario
            {
                Name = "Not automated adding two numbers",
                Feature = feature
            };
            var result = results.GetScenarioResult(inconclusiveScenario);

            Check.That(result.WasExecuted).IsFalse();
            Check.That(result.WasSuccessful).IsFalse();
        }

        [Test]
        public void ThenCanReadInconclusiveFeatureResultSuccessfully()
        {
            var results = ParseResultsFile();
            var result = results.GetFeatureResult(this.InconclusiveFeature());
            Check.That(result).IsEqualTo(TestResult.Inconclusive);
        }


        [Test]
        public void ThenCanReadPassedFeatureResultSuccessfully()
        {
            var results = ParseResultsFile();
            var result = results.GetFeatureResult(this.PassingFeature());
            Check.That(result).IsEqualTo(TestResult.Passed);
        }

        [Test]
        public void ThenCanReadFailedFeatureResultSuccessfully()
        {
            var results = ParseResultsFile();
            var result = results.GetFeatureResult(this.FailingFeature());
            Check.That(result).IsEqualTo(TestResult.Failed);
        }

        private Feature FailingFeature()
        {
            return new Feature {Name = "Failing"};
        }

        private Feature InconclusiveFeature()
        {
            return new Feature { Name = "Inconclusive" };
        }

        private Feature PassingFeature()
        {
            return new Feature { Name = "Passing" };
        }

        [Test]
        public void ThenCanReadNotFoundScenarioCorrectly()
        {
            var results = ParseResultsFile();
            var feature = new Feature { Name = "Addition" };
            var notFoundScenario = new Scenario
            {
                Name = "Not in the file at all!",
                Feature = feature
            };

            var result = results.GetScenarioResult(notFoundScenario);

            Check.That(result.WasExecuted).IsFalse();
            Check.That(result.WasSuccessful).IsFalse();
        }

        [Test]
        public void ThenCanReadNotFoundFeatureCorrectly()
        {
            var results = ParseResultsFile();
            var feature = new Feature {Name = "NotInTheFile"};
            var result = results.GetFeatureResult(feature);
            Check.That(result.WasExecuted).IsFalse();
            Check.That(result.WasSuccessful).IsFalse();
        }

        [Test]
        public void ThenCanReadIndividualResultsFromScenarioOutline_AllPass_ShouldBeTestResultPassed()
        {
          var results = ParseResultsFile();
          results.SetExampleSignatureBuilder(new NUnitExampleSignatureBuilder());

          var feature = new Feature { Name = "Scenario Outlines" };

          var scenarioOutline = new ScenarioOutline { Name = "This is a scenario outline where all scenarios pass", Feature = feature };

          TestResult exampleResultOutline = results.GetScenarioOutlineResult(scenarioOutline);
          Check.That(exampleResultOutline).IsEqualTo(TestResult.Passed);

          TestResult exampleResult1 = results.GetExampleResult(scenarioOutline, new[] { "pass_1" });
          Check.That(exampleResult1).IsEqualTo(TestResult.Passed);

          TestResult exampleResult2 = results.GetExampleResult(scenarioOutline, new[] { "pass_2" });
          Check.That(exampleResult2).IsEqualTo(TestResult.Passed);

          TestResult exampleResult3 = results.GetExampleResult(scenarioOutline, new[] { "pass_3" });
          Check.That(exampleResult3).IsEqualTo(TestResult.Passed);
        }

        [Test]
        public void ThenCanReadIndividualResultsFromScenarioOutline_OneInconclusive_ShouldBeTestResultInconclusive()
        {
          var results = ParseResultsFile();
          results.SetExampleSignatureBuilder(new NUnitExampleSignatureBuilder());

          var feature = new Feature { Name = "Scenario Outlines" };

          var scenarioOutline = new ScenarioOutline { Name = "This is a scenario outline where one scenario is inconclusive", Feature = feature };

          TestResult exampleResultOutline = results.GetScenarioOutlineResult(scenarioOutline);
          Check.That(exampleResultOutline).IsEqualTo(TestResult.Inconclusive);

          TestResult exampleResult1 = results.GetExampleResult(scenarioOutline, new[] { "pass_1" });
          Check.That(exampleResult1).IsEqualTo(TestResult.Passed);

          TestResult exampleResult2 = results.GetExampleResult(scenarioOutline, new[] { "pass_2" });
          Check.That(exampleResult2).IsEqualTo(TestResult.Passed);

          TestResult exampleResult3 = results.GetExampleResult(scenarioOutline, new[] { "inconclusive_1" });
          Check.That(exampleResult3).IsEqualTo(TestResult.Inconclusive);
        }

        [Test]
        public void ThenCanReadIndividualResultsFromScenarioOutline_OneFailed_ShouldBeTestResultFailed()
        {
          var results = ParseResultsFile();
          results.SetExampleSignatureBuilder(new NUnitExampleSignatureBuilder());

          var feature = new Feature { Name = "Scenario Outlines" };

          var scenarioOutline = new ScenarioOutline { Name = "This is a scenario outline where one scenario fails", Feature = feature };

          TestResult exampleResultOutline = results.GetScenarioOutlineResult(scenarioOutline);
          Check.That(exampleResultOutline).IsEqualTo(TestResult.Failed);

          TestResult exampleResult1 = results.GetExampleResult(scenarioOutline, new[] { "pass_1" });
          Check.That(exampleResult1).IsEqualTo(TestResult.Passed);

          TestResult exampleResult2 = results.GetExampleResult(scenarioOutline, new[] { "pass_2" });
          Check.That(exampleResult2).IsEqualTo(TestResult.Passed);

          TestResult exampleResult3 = results.GetExampleResult(scenarioOutline, new[] { "fail_1" });
          Check.That(exampleResult3).IsEqualTo(TestResult.Failed);
        }

        [Test]
        public void ThenCanReadIndividualResultsFromScenarioOutline_MultipleExampleSections_ShouldBeTestResultFailed()
        {
          var results = ParseResultsFile();
          results.SetExampleSignatureBuilder(new NUnitExampleSignatureBuilder());

          var feature = new Feature { Name = "Scenario Outlines" };

          var scenarioOutline = new ScenarioOutline { Name = "And we can go totally bonkers with multiple example sections.", Feature = feature };

          TestResult exampleResultOutline = results.GetScenarioOutlineResult(scenarioOutline);
          Check.That(exampleResultOutline).IsEqualTo(TestResult.Failed);

          TestResult exampleResult1 = results.GetExampleResult(scenarioOutline, new[] { "pass_1" });
          Check.That(exampleResult1).IsEqualTo(TestResult.Passed);

          TestResult exampleResult2 = results.GetExampleResult(scenarioOutline, new[] { "pass_2" });
          Check.That(exampleResult2).IsEqualTo(TestResult.Passed);

          TestResult exampleResult3 = results.GetExampleResult(scenarioOutline, new[] { "inconclusive_1" });
          Check.That(exampleResult3).IsEqualTo(TestResult.Inconclusive);

          TestResult exampleResult4 = results.GetExampleResult(scenarioOutline, new[] { "inconclusive_2" });
          Check.That(exampleResult4).IsEqualTo(TestResult.Inconclusive);

          TestResult exampleResult5 = results.GetExampleResult(scenarioOutline, new[] { "fail_1" });
          Check.That(exampleResult5).IsEqualTo(TestResult.Failed);

          TestResult exampleResult6 = results.GetExampleResult(scenarioOutline, new[] { "fail_2" });
          Check.That(exampleResult6).IsEqualTo(TestResult.Failed);
        }

      [Test]
      public void ThenCanReadResultsWithBackslashes()
      {
        var results = ParseResultsFile();
        results.SetExampleSignatureBuilder(new NUnitExampleSignatureBuilder());

        var feature = new Feature { Name = "Scenario Outlines" };

        var scenarioOutline = new ScenarioOutline { Name = "Deal correctly with backslashes in the examples", Feature = feature };

        TestResult exampleResultOutline = results.GetScenarioOutlineResult(scenarioOutline);
        Check.That(exampleResultOutline).IsEqualTo(TestResult.Passed);

        TestResult exampleResult1 = results.GetExampleResult(scenarioOutline, new[] { @"c:\Temp\" });
        Check.That(exampleResult1).IsEqualTo(TestResult.Passed);
      }
    }
}