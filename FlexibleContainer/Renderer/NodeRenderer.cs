﻿using FlexibleContainer.Parser.Models;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FlexibleContainer.Renderer
{
    public static class NodeRenderer
    {
        public static string Render(Node rootNode, Func<string, string> contentFormatter = null)
        {
            Assert.ArgumentNotNull(rootNode, nameof(rootNode));

            return RenderInner(rootNode.Children, contentFormatter); 
        }

        private static string RenderInner(IList<Node> nodes, Func<string, string> contentFormatter = null)
        {
            if (nodes == null || !nodes.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                var tag = new TagBuilder(node.Tag);

                if (node.Attributes != null)
                {
                    tag.MergeAttributes(node.Attributes);
                }

                if (node.ClassList != null)
                {
                    foreach (var @class in node.ClassList)
                    {
                        tag.AddCssClass(@class);
                    }
                }

                if (!string.IsNullOrWhiteSpace(node.Id))
                {
                    tag.GenerateId(node.Id);
                }

                if (!string.IsNullOrWhiteSpace(node.Content))
                {
                    var content = contentFormatter?.Invoke(node.Content) ?? node.Content;
                    tag.SetInnerText(content);
                }
                else
                {
                    var innerHtml = RenderInner(node.Children, contentFormatter);
                    tag.InnerHtml = innerHtml;
                }

                sb.Append(tag.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }
    }
}
