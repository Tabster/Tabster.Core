﻿#region

using System;
using System.Collections.Generic;
using System.Xml;

#endregion

namespace Tabster.Core.FileTypes
{
    internal static class XmlDocumentExtensions
    {
        public static XmlNode GetElementByTagName(this XmlDocument doc, string name)
        {
            var elems = doc.GetElementsByTagName(name);
            return elems.Count > 0 ? elems[0] : null;
        }
    }

    public class TabsterXmlDocument
    {
        private readonly string _rootNode;

        private XmlDocument _xmlDoc = new XmlDocument();
        private XmlDocument _xmlDocTemp;

        public TabsterXmlDocument(string rootNode)
        {
            _rootNode = rootNode;
        }

        public Version Version { get; set; }

        private void PrepareTempDocument()
        {
            _xmlDocTemp = new XmlDocument();
            _xmlDocTemp.AppendChild(_xmlDocTemp.CreateXmlDeclaration("1.0", "ISO-8859-1", null));
            var root = _xmlDocTemp.CreateElement(_rootNode);

            var versionAttribute = _xmlDocTemp.CreateAttribute("version");
            versionAttribute.Value = Version.ToString();
            root.Attributes.Append(versionAttribute);

            _xmlDocTemp.AppendChild(root);
        }

        public void Load(string fileName)
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load(fileName);

            Version = new Version(_xmlDoc.DocumentElement.Attributes["version"].Value);
        }

        public void Save(string fileName)
        {
            _xmlDoc = _xmlDocTemp;
            _xmlDoc.Save(fileName);
            _xmlDocTemp = null;
        }

        public string TryReadNodeValue(string name, string defaultValue = null)
        {
            var elem = _xmlDoc.GetElementByTagName(name);
            return elem != null ? elem.InnerText : defaultValue;
        }

        public List<XmlNode> ReadChildNodes(string name)
        {
            var values = new List<XmlNode>();

            var elem = _xmlDoc.GetElementByTagName(name);

            if (elem.HasChildNodes)
            {
                foreach (XmlNode child in elem.ChildNodes)
                {
                    values.Add(child);
                }
            }

            return values;
        }

        public List<string> ReadChildNodeValues(string name)
        {
            var values = new List<string>();

            var elem = _xmlDoc.GetElementByTagName(name);

            if (elem != null && elem.HasChildNodes)
            {
                foreach (XmlNode child in elem.ChildNodes)
                {
                    values.Add(child.InnerText);
                }
            }

            return values;
        }

        public void WriteNode(string name, string value = null, string parentNode = null, SortedDictionary<string, string> attributes = null)
        {
            if (_xmlDocTemp == null)
                PrepareTempDocument();

            XmlNode parent = _xmlDocTemp.DocumentElement;

            if (parentNode != null)
            {
                var elem = _xmlDocTemp.GetElementByTagName(parentNode);

                if (elem != null)
                {
                    parent = elem;
                }
            }

            XmlNode workingNode = _xmlDocTemp.CreateElement(name);

            if (value != null)
                workingNode.InnerText = value;

            if (attributes != null)
            {
                foreach (var kv in attributes)
                {
                    var att = _xmlDocTemp.CreateAttribute(kv.Key);
                    att.Value = kv.Value;

                    workingNode.Attributes.Append(att);
                }
            }

            parent.AppendChild(workingNode);
        }
    }
}