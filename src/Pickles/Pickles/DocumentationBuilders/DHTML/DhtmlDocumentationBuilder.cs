﻿#region License

/*
    Copyright [2011] [Jeffrey Cameron]

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

#endregion

using System.IO.Abstractions;
using NLog;
using NGenerics.DataStructures.Trees;
using PicklesDoc.Pickles.DirectoryCrawler;
using PicklesDoc.Pickles.DocumentationBuilders.JSON;
using PicklesDoc.Pickles.TestFrameworks;
using System.Reflection;

namespace PicklesDoc.Pickles.DocumentationBuilders.DHTML
{
    public class DhtmlDocumentationBuilder : IDocumentationBuilder
    {
        private static readonly Logger log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        private readonly Configuration configuration;
        private readonly ITestResults testResults;

        private readonly IFileSystem fileSystem;

        public DhtmlDocumentationBuilder(Configuration configuration, ITestResults testResults, IFileSystem fileSystem)
        {
            this.configuration = configuration;
            this.testResults = testResults;
            this.fileSystem = fileSystem;
        }

        #region IDocumentationBuilder Members

        public void Build(GeneralTree<INode> features)
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Writing DHTML files to {0}", this.configuration.OutputFolder.FullName);
            }

            log.Info("WriteResources");
            this.WriteResources();
            
            log.Info("UtilizeJsonBuilderToDumpJsonFeatureFileNextToDthmlResources");
            UtilizeJsonBuilderToDumpJsonFeatureFileNextToDthmlResources(features);

            log.Info("Tweak Json file");
            TweakJsonFile();
        }

        private void UtilizeJsonBuilderToDumpJsonFeatureFileNextToDthmlResources(GeneralTree<INode> features)
        {
            var jsonBuilder = new JSONDocumentationBuilder(configuration, testResults, this.fileSystem);
            jsonBuilder.Build(features);
        }

        private void WriteResources()
        {
            var dhtmlResourceWriter = new DhtmlResourceWriter(fileSystem);
            dhtmlResourceWriter.WriteTo(configuration.OutputFolder.FullName);
        }

        private void TweakJsonFile()
        {
            var jsonBuilder = new JSONDocumentationBuilder(configuration, testResults, this.fileSystem);
            var jsonFilePath = jsonBuilder.OutputFilePath;

            var tweaker = new JsonTweaker(this.fileSystem);
            tweaker.AddJsonPWrapperTo(jsonFilePath);
            tweaker.RenameFileTo(jsonFilePath, jsonFilePath.Replace(".json", ".js"));
        }
        #endregion
    }
}