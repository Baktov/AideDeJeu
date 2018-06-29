﻿using System.Collections.Generic;
using System.Xml;

namespace AideDeJeuLib
{
    public class Property : Dictionary<string, string>
    {

    }

    public class Properties : Dictionary<string, Property>
    {

    }

    public class Item
    {
        public string Id { get; set; }
        public string IdVO { get; set; }
        public string IdVF { get; set; }
        public string Name { get; set; }
        public string NameVO { get; set; }
        public string NamePHB { get; set; }

        public Properties Properties { get; set; }

        public string Html { get; set; }

        public static IEnumerable<string> NodeListToStringList(IEnumerable<XmlNode> nodes)
        {
            if (nodes == null) return null;
            var strings = new List<string>();
            foreach (var node in nodes)
            {
                strings.Add(node.OuterXml);
            }
            return strings;
        }

        public static XmlNode StringToNode(string str)
        {
            if (str == null) return null;
            var doc = new XmlDocument();
            doc.LoadXml(str);
            return doc.DocumentElement;
        }

        //public static IEnumerable<HtmlNode> StringListToNodeList(IEnumerable<string> strings)
        //{
        //    if (strings == null) return null;
        //    var nodes = new List<HtmlNode>();
        //    foreach (var str in strings)
        //    {
        //        var doc = new HtmlDocument();
        //        doc.LoadHtml(str);
        //        nodes.Add(doc.DocumentNode);
        //    }
        //    return nodes;
        //}

    }
}
