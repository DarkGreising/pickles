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

using System;
using System.Collections.Generic;

using gherkin;

using PicklesDoc.Pickles.ObjectModel;

namespace PicklesDoc.Pickles.Parser.Builders
{
    internal class StepBuilder
    {
        private readonly I18n nativeLanguageService;
        private readonly TableBuilder tableBuilder;
        private string docString;
        private Keyword keyword;
        private string name;

        private string nativeKeyword;

        public StepBuilder(TableBuilder tableBuilder, I18n nativeLanguageService)
        {
            this.tableBuilder = tableBuilder;
            this.nativeLanguageService = nativeLanguageService;
        }

        public void SetKeyword(string keywordText)
        {
            this.nativeKeyword = keywordText;

            Keyword? keyword = this.TryParseKeyword(keywordText);
            if (keyword.HasValue)
            {
                this.keyword = keyword.Value;
            }
        }

        public Keyword? TryParseKeyword(string keyword)
        {
            if (this.nativeLanguageService.keywords("and").contains(keyword)) return Keyword.And;

            if (this.nativeLanguageService.keywords("given").contains(keyword)) return Keyword.Given;

            if (this.nativeLanguageService.keywords("when").contains(keyword)) return Keyword.When;

            if (this.nativeLanguageService.keywords("then").contains(keyword)) return Keyword.Then;

            if (this.nativeLanguageService.keywords("but").contains(keyword)) return Keyword.But;

            if (!keyword.EndsWith(" ")) return this.TryParseKeyword(keyword + " ");

            return null;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void AddTableRow(IEnumerable<string> cells)
        {
            this.tableBuilder.AddRow(cells);
        }

        public void SetDocString(string docString)
        {
            this.docString = docString;
        }

        public Step GetResult()
        {
            return new Step
                       {
                           Keyword = this.keyword,
                           NativeKeyword = this.nativeKeyword,
                           Name = this.name,
                           DocStringArgument = this.docString,
                           TableArgument = this.tableBuilder.GetResult()
                       };
        }
    }
}