﻿using System;
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


        private bool RunAction(ThreadStart act, bool waitForResult = true)
        {
            labelTimer.Text = "0 sec";
            DateTime start = DateTime.Now;
            SetEnabled(false);
            Application.DoEvents();
            active = true;
            Exception lastEx = null;

            bool isAborted = false;

            var th = new Thread(() =>
            {
                try
                {
                    act();
                }
                catch (ThreadAbortException ex1)
                {
                    isAborted = true;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                }
            });
            th.Start();


            var thMonitor = new Thread(() =>
            {
                while (!th.Join(100))
                {
                    Action action = delegate () { labelTimer.Text = Math.Round(DateTime.Now.Subtract(start).TotalSeconds) + " sec"; };
                    labelTimer.Invoke(action);
                    if (!active && !isAborted)
                    {
                        isAborted = true;
                        th.Abort();
                        break;
                    }
                }

                progressBar.Invoke((Action)(delegate () { progressBar.Value = 0; SetEnabled(true); }));

                if (!waitForResult)
                {
                    active = false;
                    if (lastEx != null)
                    {
                        //Show error if not wait for result
                        Action action = delegate ()
                        {
                            richTextBoxLog.AppendText("Error. Check for internet connection. If problem still exists with internet connection, send question to author.\r\n" + lastEx.ToString());
                            richTextBoxLog.ScrollToCaret();
                        };
                        richTextBoxLog.Invoke(action);
                    }

                    if (isAborted)
                    {
                        //Show error if not wait for result
                        Action action = delegate ()
                        {
                            richTextBoxLog.AppendText("Execution canceled");
                            richTextBoxLog.ScrollToCaret();
                        };
                        richTextBoxLog.Invoke(action);
                    }
                }
            });
            thMonitor.Start();

            if (waitForResult)
            {
                while (!thMonitor.Join(10))
                {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }

                if (lastEx != null)
                    throw lastEx;

                if (active && !isAborted)
                {
                    active = false;
                    return true;
                }
                else
                {
                    active = false;
                    return false;
                }
            }
            else
                return false;
        }
        
                                
      



        bool alreadyShown = false;

        List<TreeNode> _nodes = null;


        private void LogText(string text)
        {
            Action action = delegate () { richTextBoxLog.AppendText(text + "\r\n"); richTextBoxLog.ScrollToCaret(); };
            richTextBoxLog.Invoke(action);
        }


        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!alreadyShown)
            {
                alreadyShown = true;
                string photoId = null;
                if (!RunAction(() => 
                {
                    photoId = GetGooglePhotosFolder.GetGooglePhotoFolderId(LogText);
                }))

                if (String.IsNullOrWhiteSpace(photoId))
                {
                    MessageBox.Show("Can't open google photos folder.");
                    this.Close();
                    return;
                }

                LogText("Seek for folders on local drive...");
                Application.DoEvents();
                _nodes = PathTreeWorker.LoadTreeView(treeViewDirectories);
                LogText("Ready.");
            }

        }
        
        private void buttonGetDirectories_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            
            if (!Directory.Exists(folderBrowserDialog.SelectedPath))
            {
                MessageBox.Show("Directory '" + folderBrowserDialog.SelectedPath + "' not found");
                return;
            }
            
            _nodes = PathTreeWorker.AddDirToTreeView(folderBrowserDialog.SelectedPath, treeViewDirectories);
            PathTreeWorker.SaveTreeView(treeViewDirectories);
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
                var rootNodes = new List<TreeNode>();
                foreach (TreeNode node in treeViewDirectories.Nodes)
                    rootNodes.Add(node);

                RunAction(() => { syncronizer.Organize(rootNodes, drivePhotoDirId, diskOrg, albumnOrg, checkBoxRecheckSubfolders.Checked); }, false);
                
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
                PathTreeWorker.SaveTreeView(treeViewDirectories);
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

            deleteFolderToolStripMenuItem.Visible = treeViewDirectories.SelectedNode.Parent == null;
            
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
            /*
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
            }*/
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PicasaClient picasa = new PicasaClient();
            var alreadyFoundExt = new HashSet<string>();

            //picasa.Test();


            var x = picasa.GetPhotos(null, "P1150295.JPG");
            var y = picasa.GetPhotos(null, "P11");
            var z = picasa.GetPhotos(null, "P1150295");
            var z1 = picasa.GetPhotos(null, "P");

            var foundAlbums = new HashSet<string>();
            var picasaFoundFiles = picasa.GetPhotos(null, ".avi");
            PicasaEntry pe = null;
            foreach (var file in picasaFoundFiles)
            {
                var ph = new PhotoAccessor(file);

                if (ph.PhotoTitle.ToLower()== "2005-12-16Mafia.avi".ToLower())
                {
                    pe = file;
                    var value = ph.Timestamp;
                    var tmp = new DateTime(1970, 1, 1, 0, 0, 0);
                    tmp = tmp.AddMilliseconds(value);

                    break;
                }
            }


            picasa.SetPhotoCreationDate(pe, DateTime.Now);





            MessageBox.Show(pe.Title.ToString());
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            treeViewDirectories.Nodes.Clear();
            PathTreeWorker.SaveTreeView(treeViewDirectories, false);
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!treeViewDirectories.Enabled || treeViewDirectories.SelectedNode == null)
                return;

            if (treeViewDirectories.SelectedNode.Parent!=null)
            {
                MessageBox.Show("Can delete only root nodes");
                return;
            }

            treeViewDirectories.SelectedNode.Remove();
        }

        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(GoogleDriveClient.CredPath))
            {
                Directory.Delete(GoogleDriveClient.CredPath, true);
                MessageBox.Show("Application will be closed. Start it again to select new google account.");
                this.Close();
            }
            {
                MessageBox.Show("Could not found '"+ GoogleDriveClient.CredPath+"' directory. Already logout?");
            }
        }

        private void buttonFixVideoTime_Click(object sender, EventArgs e)
        {
            //This did not work, because cant change creation date
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
                var rootNodes = new List<TreeNode>();
                foreach (TreeNode node in treeViewDirectories.Nodes)
                    rootNodes.Add(node);

                if (!RunAction(() => { syncronizer.FixVideoDates(rootNodes, drivePhotoDirId); }))
                    return;
            }
            catch (Exception ex)
            {
                richTextBoxLog.AppendText("Error. Check for internet connection. If problem still exists with internet connection, send question to author.\r\n" + ex.ToString());
                richTextBoxLog.ScrollToCaret();
            }
        }
    }
    
}
