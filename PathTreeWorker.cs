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
    static class PathTreeWorker
    {

        private const string _xmlNodeTag = "node";
        private const string _xmlNodeCheckedAtt = "checked";
        private const string _xmlNodePathAtt = "path";
        private const string _xmlNodeBgColorAtt = "color";
        private const string _xmlNodeIsExpandedAtt = "expanded";
        private const string _xmlNodeIsBaseLineAtt = "baseline";

        
        public class DirState
        {
            public bool IsExpanded = true;
            public Color Color = Color.Black;
            public bool Checked = true;
        }


        static public List<TreeNode> AddDirToTreeView(string rootDirectory, TreeView treeView, Dictionary<string,DirState> dirStates = null)
        {
            var result = new List<TreeNode>();

            treeView.BeginUpdate();
            var treeNode = new TreeNode(rootDirectory);
            treeView.Nodes.Add(treeNode);
            
            bool isExpanded = true;
            if (dirStates != null && dirStates.ContainsKey(rootDirectory))
            {
                //Set saved attrs
                var attrs = dirStates[rootDirectory];
                treeNode.ForeColor = attrs.Color;
                treeNode.Checked = attrs.Checked;
                isExpanded = attrs.IsExpanded;
            }
            else
            {
                treeNode.Checked = true;
            }
            treeNode.Tag = rootDirectory;

            if (Directory.Exists(rootDirectory))
            {
                FillRecursive(treeNode, rootDirectory, result, dirStates);
            }

            if (isExpanded)
                treeNode.Expand();
            else
                treeNode.Collapse();
            treeView.EndUpdate();

            return result;
        }
        

        static void FillRecursive(TreeNode curNode, string directory, List<TreeNode> result, Dictionary<string, DirState> dirStates = null)
        {
            var dirs = Directory.GetDirectories(directory).ToList();
            dirs.Sort();
            foreach (var dir in dirs)
            {
                var treeNode = new TreeNode(Path.GetFileName(dir));
                treeNode.Tag = dir;
                bool isExpanded = true;
                if (dirStates != null && dirStates.ContainsKey(dir))
                {
                    //Set saved attrs
                    var attrs = dirStates[dir];
                    treeNode.ForeColor = attrs.Color;
                    treeNode.Checked = attrs.Checked;
                    isExpanded = attrs.IsExpanded;
                }
                else
                {
                    treeNode.Checked = true;
                }
                curNode.Nodes.Add(treeNode);
                result.Add(treeNode);
                FillRecursive(treeNode, dir, result, dirStates);
                if (isExpanded)
                    treeNode.Expand();
            }
        }
        
        
        public static string GetRelativePathDirectory(string fullPath, string pathRoot)
        {
            if (!fullPath.ToLower().StartsWith(pathRoot.ToLower()))
                throw new Exception("Path '" + fullPath + "' does not start witn '" + pathRoot + "'");
            pathRoot = Path.GetDirectoryName(pathRoot); //One level up

            var result = "";

            if (pathRoot != null)
                result = fullPath.Substring(pathRoot.Length);
            else
                result = fullPath;
            
            result = Path.GetDirectoryName(result);
            if (result == null)
                result = fullPath;

            if (result.StartsWith("\\"))
            {
                result = result.Substring(1);
            }
            if (result.EndsWith("\\"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result.Replace(":", "");
        }



        static private string GetFilePathNearModule(string shortFileName)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), shortFileName);
        }

        private static string GetStoredFileName()
        {
            return GetFilePathNearModule("Stored.xml");
        }

        static public void SaveTreeView(TreeView treeView, bool savePrev = true)
        {
            try
            {
                var dirStates = new Dictionary<string, DirState>();
                var baseLinePathes = new HashSet<string>();
                GetStatesFromTreeView(treeView.Nodes, dirStates, baseLinePathes);

                if (savePrev)
                {
                    var baseLinePathes2 = new HashSet<string>();
                    var dirStates2 = GetStatesFromXml(out baseLinePathes2);
                    foreach (var b2 in dirStates2)
                    {
                        if (!dirStates.ContainsKey(b2.Key))
                            dirStates.Add(b2.Key, b2.Value);
                    }
                }

                //Get States From TreeView
                SaveStatesToXml(dirStates, baseLinePathes);
            }
            catch
            { }
        }

        

        static private void GetStatesFromTreeView(TreeNodeCollection nodes, Dictionary<string, DirState> dirStates, HashSet<string> baseLinePathes, bool isBaseLine = true)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode node = nodes[i];
                
                string path = null;
                if (isBaseLine)
                {
                    path = node.Text;
                    baseLinePathes.Add(path);
                }
                else
                {
                    if (node.Tag != null)
                        path = node.Tag.ToString();
                }

                if (dirStates.ContainsKey(path))
                    continue;

                var attr = new DirState();
                attr.Color = node.ForeColor;
                attr.Checked = node.Checked;
                attr.IsExpanded = node.IsExpanded;
                dirStates.Add(path, attr);
                if (node.Nodes.Count > 0)
                    GetStatesFromTreeView(node.Nodes, dirStates, baseLinePathes, false);
            }
        }



        static private void SaveStatesToXml(Dictionary<string, DirState> dirStates, HashSet<string> baseLines)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(GetStoredFileName(), System.Text.Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("TreeView");

            foreach (var dirState in dirStates)
            {
                var path = dirState.Key;
                var attr = dirState.Value;

                bool baseLine = baseLines!=null && baseLines.Contains(dirState.Key);

                if (!String.IsNullOrWhiteSpace(path) &&
                    (!attr.Checked || attr.Color == Color.Gray || !attr.IsExpanded || baseLine)) //Any not default feature
                {
                    xmlWriter.WriteStartElement(_xmlNodeTag);
                    xmlWriter.WriteAttributeString(_xmlNodePathAtt, path);
                    if (baseLine)
                        xmlWriter.WriteAttributeString(_xmlNodeIsBaseLineAtt, baseLine ? "true" : "false");
                    if (!attr.Checked)
                        xmlWriter.WriteAttributeString(_xmlNodeCheckedAtt, attr.Checked ? "true" : "false");
                    if (attr.Color == Color.Gray)
                        xmlWriter.WriteAttributeString(_xmlNodeBgColorAtt, attr.Color == Color.Gray ? "Gray" : "Default");
                    if (!attr.IsExpanded)
                        xmlWriter.WriteAttributeString(_xmlNodeIsExpandedAtt, attr.IsExpanded ? "true" : "false");
                    xmlWriter.WriteEndElement();
                }
            }
            
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }
        
        static private Dictionary<string, DirState> GetStatesFromXml(out HashSet<string> baseLinePathes)
        {
            var fileName = GetStoredFileName();
            if (!File.Exists(fileName))
            {
                baseLinePathes = null;
                return null;
            }

            var dirStates = new Dictionary<string, DirState>();
            baseLinePathes = new HashSet<string>();

            using (XmlTextReader xmlReader = new XmlTextReader(fileName))
            {
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xmlReader.Name == _xmlNodeTag)
                            {
                                var dirState = new DirState();
                                var path = "";
                                var isBaseLine = false;

                                // loading node attributes
                                int attributeCount = xmlReader.AttributeCount;
                                if (attributeCount > 0)
                                {
                                    for (int i = 0; i < attributeCount; i++)
                                    {
                                        xmlReader.MoveToAttribute(i);
                                        switch (xmlReader.Name)
                                        {
                                            case _xmlNodeIsBaseLineAtt:
                                                isBaseLine = xmlReader.Value == "true";
                                                break;
                                            case _xmlNodePathAtt:
                                                path = xmlReader.Value;
                                                break;
                                            case _xmlNodeCheckedAtt:
                                                dirState.Checked = xmlReader.Value == "true";
                                                break;
                                            case _xmlNodeBgColorAtt:
                                                if (xmlReader.Value == "Gray")
                                                    dirState.Color = Color.Gray;
                                                break;
                                            case _xmlNodeIsExpandedAtt:
                                                dirState.IsExpanded = xmlReader.Value == "true";
                                                break;
                                        }
                                    }
                                }
                                if (String.IsNullOrWhiteSpace(path))
                                    continue;

                                if (isBaseLine)
                                    baseLinePathes.Add(path);
                                if (!dirStates.ContainsKey(path))
                                    dirStates.Add(path, dirState);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        case XmlNodeType.XmlDeclaration:
                            break;
                        case XmlNodeType.Text:
                            //parentNode.Nodes.Add(xmlReader.Value);
                            break;
                        case XmlNodeType.None:
                            break;
                    };
                }
            }
            return dirStates;
        }



        static public List<TreeNode> LoadTreeView(TreeView treeView)
        {
            HashSet<string> baseLinePathes = null;
            Dictionary<string, DirState> dirStates = null;
            try
            {
                dirStates = GetStatesFromXml(out baseLinePathes);
            }
            catch
            {
                return null;
            }

            
            if (baseLinePathes==null || baseLinePathes.Count==0)
                return null; //Nothing to add

            var result = new List<TreeNode>();
            //Then scan baseLine dirs adn add to list
            try
            {
                treeView.BeginUpdate();
                foreach (var path in baseLinePathes)
                {
                    var foundNodes = AddDirToTreeView(path, treeView, dirStates);
                    result.AddRange(foundNodes);
                }
            }
            catch
            {
                result = null;
                treeView.Nodes.Clear();
            }
            finally
            {
                // enabling redrawing of treeview after all nodes are added
                treeView.EndUpdate();
            }

            return result;

        }

    }
}
