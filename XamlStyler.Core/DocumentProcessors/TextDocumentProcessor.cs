﻿// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Xavalon.XamlStyler.Core.Extensions;
using Xavalon.XamlStyler.Core.Parser;
using Xavalon.XamlStyler.Core.Services;

namespace Xavalon.XamlStyler.Core.DocumentProcessors
{
    internal class TextDocumentProcessor : IDocumentProcessor
    {
        private readonly IndentService indentService;

        public TextDocumentProcessor(IndentService indentService)
        {
            this.indentService = indentService;
        }

        public void Process(XmlReader xmlReader, StringBuilder output, ElementProcessContext elementProcessContext)
        {
            elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.SINGLE_LINE_TEXT_ONLY);

            var xmlEncodedContent = xmlReader.Value.ToXmlEncodedString(ignoreCarrier: true);
            if (elementProcessContext.Current.IsPreservingSpace)
            {
                output.Append(xmlEncodedContent.Replace("\n", Environment.NewLine));
            }
            else
            {
                string currentIndentString = this.indentService.GetIndentString(xmlReader.Depth);
                IEnumerable<string> textLines = xmlEncodedContent.Trim().Split('\n').Where(_ => _.Trim().Length > 0).ToList();

                foreach (var line in textLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.Length > 0)
                    {
                        output.Append(Environment.NewLine).Append(currentIndentString).Append(trimmedLine);
                    }
                }
            }

            if (xmlEncodedContent.Any(_ => _ == '\n'))
            {
                elementProcessContext.UpdateParentElementProcessStatus(ContentTypeEnum.MULTI_LINE_TEXT_ONLY);
            }
        }
    }
}