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

            progressBar.Value = 0;

            if (lastEx != null)
            {
                SetEnabled(true);
                throw lastEx;
            }


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
        
                                
      



        bool alreadyShown = false;

        List<TreeNode> _nodes = null;


        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!alreadyShown)
            {
                alreadyShown = true;
                if (String.IsNullOrWhiteSpace(GetGooglePhotosFolder.GetGooglePhotoFolderId()))
                {
                    MessageBox.Show("Can't open google photos folder.");
                    this.Close();
                }
                else
                {
                    try
                    {
                        var res = TreeSaver.LoadTreeView(treeViewDirectories, out _nodes);
                        if (res != null && String.IsNullOrWhiteSpace(res.Text))
                            textBoxLocalPath.Text = res.Text;
                    }
                    catch { };
                }


            }

        }
        
        private void buttonGetDirectories_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            textBoxLocalPath.Text = folderBrowserDialog.SelectedPath;

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

        
        private void buttonUpload_Click(object sender, EventArgs e)
        {
            
        }


        private void buttonOrganize_Click(object sender, EventArgs e)
        {
            bool diskOrg = true;
            bool albumnOrg = true;

            var drivePhotoDirId = GetGooglePhotosFolder.GetGooglePhotoFolderId();
            if (String.IsNullOrWhiteSpace(drivePhotoDirId))
            {
                return;
            }

            richTextBoxLog.Clear();
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



        private void buttonDeleteAll_Click(object sender, EventArgs e)
        {

            var drivePhotoDirId = GetGooglePhotosFolder.GetGooglePhotoFolderId();
            if (String.IsNullOrWhiteSpace(drivePhotoDirId))
            {
                return;
            }

            if (MessageBox.Show("Are you really want move all Google Photo files to trash folder?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            richTextBoxLog.Clear();
            var syncronizer = new Synchronizer(progressBar, richTextBoxLog);

            //syncronizer.Organize(textBoxLocalPath.Text, _nodes, drivePhotoDirId, diskOrg, albumnOrg);
            try
            {
                if (!RunAction(() => { syncronizer.TrashAll(drivePhotoDirId); }))
                    return;
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendText("Error. Check for internet connection. If problem still exists with internet connection, send question to author.\r\n" + ex.ToString());
                richTextBoxLog.ScrollToCaret();
            }

        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (treeViewDirectories.Nodes.Count>0)
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
            
            if (snd.Tag is string && (string)snd.Tag == "false")
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
            if (snd.Tag is string && (string)snd.Tag == "false")
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


      
        
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void buttonDeleteAllPicasa_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxLocalPath.Text))
            {
                MessageBox.Show("Directory not found '" + textBoxLocalPath.Text+"'");
                return;
            }

            if (MessageBox.Show("Are you really want move all Photos from picasa Web Albumns?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            richTextBoxLog.Clear();
            var syncronizer = new Synchronizer(progressBar, richTextBoxLog);

            //syncronizer.Organize(textBoxLocalPath.Text, _nodes, drivePhotoDirId, diskOrg, albumnOrg);
            try
            {
                if (!RunAction(() => { syncronizer.PicasaDeleteAll(textBoxLocalPath.Text); }))
                    return;
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendText("Error. Check for internet connection. If problem still exists with internet connection, send question to author.\r\n" + ex.ToString());
                richTextBoxLog.ScrollToCaret();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }
    }
    
}
