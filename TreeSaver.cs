using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace GooglePhotoOrganizer
{
    static class TreeSaver
    {

        private const string _xmlNodeTag = "node";
        private const string _xmlNodeTextAtt = "text";
        private const string _xmlNodeTagAtt = "tag";
        private const string _xmlNodeCheckedAtt = "checked";
        private const string _xmlNodeBgColorAtt = "color";
        private const string _xmlNodeIsExpandedAtt = "expanded";
        

        static private string GetFilePathNearModule(string shortFileName)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), shortFileName);
        }

        private static string GetFileName()
        {
            return GetFilePathNearModule("directory.xml");
        }

        static public void SaveTreeView(TreeView treeView)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(GetFileName(), System.Text.Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("TreeView");

            SaveNodesRecursive(treeView.Nodes, xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }

        static private void SaveNodesRecursive(TreeNodeCollection nodes, XmlTextWriter xmlWriter)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode node = nodes[i];
                xmlWriter.WriteStartElement(_xmlNodeTag);
                xmlWriter.WriteAttributeString(_xmlNodeTextAtt, node.Text );
                if (node.Tag != null)
                    xmlWriter.WriteAttributeString(_xmlNodeTagAtt, node.Tag.ToString());
                xmlWriter.WriteAttributeString(_xmlNodeCheckedAtt, node.Checked?"true":"false");
                xmlWriter.WriteAttributeString(_xmlNodeBgColorAtt, node.ForeColor == Color.Gray ? "Gray" : "Default");
                xmlWriter.WriteAttributeString(_xmlNodeIsExpandedAtt, node.IsExpanded ? "true" : "false");
                if (node.Nodes.Count > 0)
                    SaveNodesRecursive(node.Nodes, xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }

        static public TreeNode LoadTreeView(TreeView treeView, out List<TreeNode> loadedNodes)
        {
            var fileName = GetFileName();
            loadedNodes = new List<TreeNode>();
            if (!File.Exists(fileName))
                return null;

            TreeNode rootNode = null;
            XmlTextReader xmlReader = null;
            try
            {
                // disabling re-drawing of treeview till all nodes are added
                treeView.BeginUpdate();
                xmlReader = new XmlTextReader(fileName);
                TreeNode parentNode = null;
                
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    { 
                        case XmlNodeType.Element:
                            if (xmlReader.Name == _xmlNodeTag)
                            {
                                TreeNode newNode = new TreeNode();
                                bool isEmptyElement = xmlReader.IsEmptyElement;

                                // loading node attributes
                                int attributeCount = xmlReader.AttributeCount;
                                if (attributeCount > 0)
                                {
                                    for (int i = 0; i < attributeCount; i++)
                                    {
                                        xmlReader.MoveToAttribute(i);
                                        switch (xmlReader.Name)
                                        {
                                            case _xmlNodeTextAtt:
                                                newNode.Text = xmlReader.Value;
                                                break;
                                            case _xmlNodeTagAtt:
                                                newNode.Tag = xmlReader.Value;
                                                break;
                                            case _xmlNodeCheckedAtt:
                                                if (xmlReader.Value == "true")
                                                    newNode.Checked = true;
                                                else
                                                    newNode.Checked = false;
                                                break;
                                            case _xmlNodeBgColorAtt:
                                                if (xmlReader.Value == "Gray")
                                                    newNode.ForeColor = Color.Gray;
                                                break;
                                            case _xmlNodeIsExpandedAtt:
                                                if (xmlReader.Value == "true")
                                                    newNode.Expand();
                                                break;
                                        }
                                    }
                                }
                          
                                if (parentNode != null)
                                    parentNode.Nodes.Add(newNode);
                                else
                                    treeView.Nodes.Add(newNode);
                                
                                if (rootNode == null)
                                    rootNode = newNode;
                                else
                                    loadedNodes.Add(newNode);

                                if (!isEmptyElement)
                                {
                                    parentNode = newNode;
                                }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (xmlReader.Name == _xmlNodeTag)
                                parentNode = parentNode.Parent;                            
                            break;
                        case XmlNodeType.XmlDeclaration:
                            break;
                        case XmlNodeType.Text:
                            parentNode.Nodes.Add(xmlReader.Value);
                            break;
                        case XmlNodeType.None:
                            break;
                    };
                }
            }
            finally
            {
                // enabling redrawing of treeview after all nodes are added
                if (rootNode != null)
                    rootNode.ExpandAll();
                rootNode.EnsureVisible();

                treeView.EndUpdate();
                xmlReader.Close();
            }
            return rootNode;

        }

    }
}
