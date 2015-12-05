using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePhotoOrganizer
{
    class FileWorker
    {


        static public List<TreeNode> FillTreeViewWithDirs(string rootDirectory, TreeView treeView)
        {
            var result = new List<TreeNode>();
            treeView.Nodes.Clear();
            var treeNode = new TreeNode(rootDirectory);
            treeNode.Checked = true;           
            FillRecursive(treeNode, rootDirectory, result);
            treeNode.Tag = rootDirectory;
            treeView.Nodes.Add(treeNode);
            treeNode.ExpandAll();
            return result;
        }

        static void FillRecursive(TreeNode curNode, string directory, List<TreeNode> result)
        {
            var dirs = Directory.GetDirectories(directory);
            foreach (var dir in dirs)
            {
                var treeNode = new TreeNode(Path.GetFileName(dir));
                treeNode.Tag = dir;
                treeNode.Checked = true;
                curNode.Nodes.Add(treeNode);
                result.Add(treeNode);
                FillRecursive(treeNode, dir, result);
            }
        }

        /*static public Dictionary<string, List<string>> GetFilesInAllFolders(List<string> folder)
        {
            var result = new Dictionary<string, List<string>>();

            //Сперва найдем все файлы в папке и их относительный путь
            var files = Directory.GetFiles(textBoxLocalPath.Text, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fname = Path.GetFileName(file);
                var relPath = GetRelativePathDirectory(file, textBoxLocalPath.Text);
                if (!result.ContainsKey(fname))
                    result.Add(fname, new List<string>());
                result[fname].Add(relPath);
            }

            return result;
        }*/


        static public Dictionary<TreeNode, List<string>> GetFilesForNodes(List<TreeNode> nodes, out int allFilesCnt)
        {
            allFilesCnt = 0;
            var result = new Dictionary<TreeNode, List<string>>();
            foreach (var node in nodes)
            {
                var files = Directory.GetFiles((string)node.Tag);
                var lst = new List<string>();
                foreach (var file in files)
                {
                    if (!file.ToLower().EndsWith(".jpg") && !file.ToLower().EndsWith(".jpeg"))
                        continue;
                    lst.Add(file);
                    allFilesCnt++;
                }
                result.Add(node, lst);
            }
            return result;
        }



        public static string GetRelativePathDirectory(string fullPath, string pathRoot)
        {
            if (!fullPath.ToLower().StartsWith(pathRoot.ToLower()))
                throw new Exception("Path '" + fullPath + "' does not start witn '" + pathRoot + "'");
            var result = fullPath.Substring(pathRoot.Length);
            result = Path.GetDirectoryName(result);
            if (result.StartsWith("\\"))
            {
                result = result.Substring(1);
            }
            if (result.EndsWith("\\"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }



    }
}
