using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Google.GData.Photos;
using Google.GData.Client;
using System.Net.Security;
using System.Net.Sockets;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading;

namespace GooglePhotoOrganizer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        bool active = true;

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            active = false;
        }

        private void SetEnabled(bool enabled)
        {
            buttonCancel.Visible = !enabled;
            foreach (Control control in this.Controls)
            {
                if (control == buttonCancel || control == labelTimer || control == richTextBoxLog)
                    continue;
                control.Enabled = enabled;
            }
        }

        
        private bool RunAction(ThreadStart act)
        {
            labelTimer.Text = "0 sec";
            DateTime start = DateTime.Now;
            SetEnabled(false);
            Application.DoEvents();
            active = true;
            Exception lastEx = null;
            
            var th = new Thread(() =>
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                }
            });
            th.Start();
            while (!th.Join(100))
            {
                labelTimer.Text = Math.Round(DateTime.Now.Subtract(start).TotalSeconds) + " sec";
                Application.DoEvents();
                if (!active)
                    break;
            }

            if (lastEx != null)
                throw lastEx;

            if (!active)
                th.Abort();


            SetEnabled(true);
            if (active)
            {
                active = false;
                return true;
            }
            else
                return false;

        }
        
                                
      

        private void button1_Click(object sender, EventArgs e)
        {

        }



        bool alreadyShown = false;

        List<TreeNode> _nodes = null;


        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!alreadyShown)
            {
                try
                {
                    var res = TreeSaver.LoadTreeView(treeViewDirectories, out _nodes);
                    textBoxLocalPath.Text = res.Text;
                }
                catch   { };
                

                alreadyShown = true;
            }

        }
        
        private void buttonGetDirectories_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxLocalPath.Text))
            {
                MessageBox.Show("Directory '" + textBoxLocalPath.Text + "' not found");
                return;
            }

            _nodes = FileWorker.FillTreeViewWithDirs(textBoxLocalPath.Text, treeViewDirectories);
            TreeSaver.SaveTreeView(treeViewDirectories);
        }
        
        private void treeViewDirectories_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;

            foreach (TreeNode subNode in node.Nodes)
                subNode.Checked = node.Checked;
        }

        private string GetGooglePhotoId()
        {
            List<string> sugestionNames = new List<string>() { "Google Photos", "Google Фото", "Google Photo" };
            var photoFolders = new List<Google.Apis.Drive.v2.Data.File>();

            try
            {
                var google = new GoogleDriveClient();
                foreach (var sg in sugestionNames)
                {
                    photoFolders = google.GetDirectories(sg, "root");
                    if (photoFolders.Count > 0)
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Can't connect to google drive. No internet connection?");
                return null;
            }
            
            if (photoFolders.Count == 0)
            {
                MessageBox.Show(@"Can't find 'Google Photos' folder on google drive.\r\n" +
                    "You should activate it:\r\n" +
                    "1. Sign in to Google Drive with your Google Account. \r\n" +
                    "2. Click on the Cog icon located at the top right corner of your screen\r\n" +
                    "and then select Settings > General." +
                    "3. Scroll to Create a Google Photos folder and tick the Automatically put your\r\n" +
                    "Google Photos into a folder in My Drive checkbox and that's it.\r\n\r\n" +
                    "If something goes wrong - google for: 'How to add a Google Photos Folder to Google Drive'");
                return null;
            }
            if (photoFolders.Count > 1)
            {
                MessageBox.Show("Unexpected error. Too many photo folders.\r\n Remove 'Google photos' folders, which is not needed.");
                return null;
            }
            return photoFolders[0].Id;
        }
        
                
        
        private void buttonUpload_Click(object sender, EventArgs e)
        {
            
        }


        private void buttonOrganize_Click(object sender, EventArgs e)
        {
            bool diskOrg = true;
            bool albumnOrg = true;

            var drivePhotoDirId = GetGooglePhotoId();
            if (String.IsNullOrWhiteSpace(drivePhotoDirId))
            {
                return;
            }

            var syncronizer = new Synchronizer(progressBar, richTextBoxLog);

            //syncronizer.Organize(textBoxLocalPath.Text, _nodes, drivePhotoDirId, diskOrg, albumnOrg);
            try
            {
                if (!RunAction(() => { syncronizer.Organize(textBoxLocalPath.Text, _nodes, drivePhotoDirId, diskOrg, albumnOrg); }))
                    return;
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendText("Error. Check for internet connection. If problem still exists with internet connection, send question to author.\r\n"+ex.ToString());
                richTextBoxLog.ScrollToCaret();
            }
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TreeSaver.SaveTreeView(treeViewDirectories);
        }

        private void treeViewDirectories_ContextMenuStripChanged(object sender, EventArgs e)
        {
        }

        private void contextMenuStripTree_Opening(object sender, CancelEventArgs e)
        {
            if (!treeViewDirectories.Enabled || treeViewDirectories.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }

            bool visible = true;
            if (treeViewDirectories.SelectedNode.ForeColor == Color.Gray)
                visible = false;

            doNotMakeAlbumWithFolderNameToolStripMenuItem.Visible = visible;
            makeAlbumWithFolderNameToolStripMenuItem.Visible = !visible;            
            doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem.Visible = treeViewDirectories.SelectedNode.Nodes.Count>0;
            makeAlbumsWithSubfolderNamesToolStripMenuItem.Visible = treeViewDirectories.SelectedNode.Nodes.Count > 0;
        }

        private void treeViewDirectories_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && !e.Node.IsSelected)
                treeViewDirectories.SelectedNode = e.Node;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void makeThisDirectoryAsAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!treeViewDirectories.Enabled || treeViewDirectories.SelectedNode == null)
                return;
            
            if (!(sender is ToolStripMenuItem))
                return;

            var snd = (ToolStripMenuItem)sender;
            
            if (snd.Tag == "false")
                treeViewDirectories.SelectedNode.ForeColor = Color.Black;
            else
                treeViewDirectories.SelectedNode.ForeColor = Color.Gray;
        }

      

        private void makeThisDictionaryAndSubsAsAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!treeViewDirectories.Enabled || treeViewDirectories.SelectedNode == null)
                return;

            if (!(sender is ToolStripMenuItem))
                return;

            var snd = (ToolStripMenuItem)sender;

            Color color;
            if (snd.Tag == "false")
                color = Color.Black;
            else
                color = Color.Gray;
            ChangeColorRecursive(treeViewDirectories.SelectedNode, color);
        }

        void ChangeColorRecursive(TreeNode node, Color newColor)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.ForeColor = newColor;
                ChangeColorRecursive(child, newColor);
            }
        }
    }
    
}
