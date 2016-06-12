namespace GooglePhotoOrganizer
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.treeViewDirectories = new System.Windows.Forms.TreeView();
            this.contextMenuStripTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeAlbumWithFolderNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonAddDirectory = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonOrganize = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelTimer = new System.Windows.Forms.Label();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.buttonTrashAll = new System.Windows.Forms.Button();
            this.buttonDeleteAllPicasa = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonLogOut = new System.Windows.Forms.Button();
            this.buttonFixVideoTime = new System.Windows.Forms.Button();
            this.checkBoxRecheckSubfolders = new System.Windows.Forms.CheckBox();
            this.contextMenuStripTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLog.Location = new System.Drawing.Point(-1, 395);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(471, 98);
            this.richTextBoxLog.TabIndex = 1;
            this.richTextBoxLog.Text = "";
            // 
            // treeViewDirectories
            // 
            this.treeViewDirectories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewDirectories.ContextMenuStrip = this.contextMenuStripTree;
            this.treeViewDirectories.Location = new System.Drawing.Point(6, 36);
            this.treeViewDirectories.Name = "treeViewDirectories";
            this.treeViewDirectories.Size = new System.Drawing.Size(459, 322);
            this.treeViewDirectories.TabIndex = 5;
            this.treeViewDirectories.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDirectories_AfterCheck);
            this.treeViewDirectories.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewDirectories_NodeMouseClick);
            this.treeViewDirectories.ContextMenuStripChanged += new System.EventHandler(this.treeViewDirectories_ContextMenuStripChanged);
            // 
            // contextMenuStripTree
            // 
            this.contextMenuStripTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFolderToolStripMenuItem,
            this.deleteFolderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem,
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem,
            this.makeAlbumWithFolderNameToolStripMenuItem,
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem});
            this.contextMenuStripTree.Name = "contextMenuStripTree";
            this.contextMenuStripTree.Size = new System.Drawing.Size(462, 142);
            this.contextMenuStripTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTree_Opening);
            // 
            // addFolderToolStripMenuItem
            // 
            this.addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
            this.addFolderToolStripMenuItem.Size = new System.Drawing.Size(461, 22);
            this.addFolderToolStripMenuItem.Text = "Add root folder";
            this.addFolderToolStripMenuItem.Click += new System.EventHandler(this.buttonGetDirectories_Click);
            // 
            // deleteFolderToolStripMenuItem
            // 
            this.deleteFolderToolStripMenuItem.Name = "deleteFolderToolStripMenuItem";
            this.deleteFolderToolStripMenuItem.Size = new System.Drawing.Size(461, 22);
            this.deleteFolderToolStripMenuItem.Text = "Delete root folder";
            this.deleteFolderToolStripMenuItem.Click += new System.EventHandler(this.deleteFolderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(458, 6);
            // 
            // doNotMakeAlbumWithFolderNameToolStripMenuItem
            // 
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem.Name = "doNotMakeAlbumWithFolderNameToolStripMenuItem";
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem.Size = new System.Drawing.Size(461, 22);
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem.Tag = "true";
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem.Text = "Do not make album name from folder (Parent folder will be album name)";
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem.Click += new System.EventHandler(this.makeThisDirectoryAsAlbumToolStripMenuItem_Click);
            // 
            // doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem
            // 
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem.Name = "doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem";
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem.Size = new System.Drawing.Size(461, 22);
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem.Tag = "true";
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem.Text = "Do not make albums with subfolder names";
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem.Click += new System.EventHandler(this.makeThisDictionaryAndSubsAsAlbumToolStripMenuItem_Click);
            // 
            // makeAlbumWithFolderNameToolStripMenuItem
            // 
            this.makeAlbumWithFolderNameToolStripMenuItem.Name = "makeAlbumWithFolderNameToolStripMenuItem";
            this.makeAlbumWithFolderNameToolStripMenuItem.Size = new System.Drawing.Size(461, 22);
            this.makeAlbumWithFolderNameToolStripMenuItem.Tag = "false";
            this.makeAlbumWithFolderNameToolStripMenuItem.Text = "Make album with folder name";
            this.makeAlbumWithFolderNameToolStripMenuItem.Click += new System.EventHandler(this.makeThisDirectoryAsAlbumToolStripMenuItem_Click);
            // 
            // makeAlbumsWithSubfolderNamesToolStripMenuItem
            // 
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem.Name = "makeAlbumsWithSubfolderNamesToolStripMenuItem";
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem.Size = new System.Drawing.Size(461, 22);
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem.Tag = "false";
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem.Text = "Make albums with subfolder names";
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem.Click += new System.EventHandler(this.makeThisDictionaryAndSubsAsAlbumToolStripMenuItem_Click);
            // 
            // buttonAddDirectory
            // 
            this.buttonAddDirectory.Location = new System.Drawing.Point(6, 7);
            this.buttonAddDirectory.Name = "buttonAddDirectory";
            this.buttonAddDirectory.Size = new System.Drawing.Size(75, 23);
            this.buttonAddDirectory.TabIndex = 6;
            this.buttonAddDirectory.Text = "Add Folder";
            this.buttonAddDirectory.UseVisualStyleBackColor = true;
            this.buttonAddDirectory.Click += new System.EventHandler(this.buttonGetDirectories_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(-1, 496);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(471, 23);
            this.progressBar.TabIndex = 8;
            // 
            // buttonOrganize
            // 
            this.buttonOrganize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOrganize.Location = new System.Drawing.Point(95, 364);
            this.buttonOrganize.Name = "buttonOrganize";
            this.buttonOrganize.Size = new System.Drawing.Size(75, 23);
            this.buttonOrganize.TabIndex = 9;
            this.buttonOrganize.Text = "Organize";
            this.buttonOrganize.UseVisualStyleBackColor = true;
            this.buttonOrganize.Click += new System.EventHandler(this.buttonOrganize_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(390, 364);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Visible = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelTimer
            // 
            this.labelTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTimer.AutoSize = true;
            this.labelTimer.Location = new System.Drawing.Point(328, 370);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(0, 13);
            this.labelTimer.TabIndex = 11;
            // 
            // buttonAbout
            // 
            this.buttonAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAbout.Location = new System.Drawing.Point(173, 364);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(75, 23);
            this.buttonAbout.TabIndex = 12;
            this.buttonAbout.Text = "About";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonTrashAll
            // 
            this.buttonTrashAll.Location = new System.Drawing.Point(210, 406);
            this.buttonTrashAll.Name = "buttonTrashAll";
            this.buttonTrashAll.Size = new System.Drawing.Size(75, 23);
            this.buttonTrashAll.TabIndex = 13;
            this.buttonTrashAll.Text = "Trash All Files";
            this.buttonTrashAll.UseVisualStyleBackColor = true;
            this.buttonTrashAll.Visible = false;
            this.buttonTrashAll.Click += new System.EventHandler(this.buttonDeleteAll_Click);
            // 
            // buttonDeleteAllPicasa
            // 
            this.buttonDeleteAllPicasa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteAllPicasa.Location = new System.Drawing.Point(291, 406);
            this.buttonDeleteAllPicasa.Name = "buttonDeleteAllPicasa";
            this.buttonDeleteAllPicasa.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteAllPicasa.TabIndex = 14;
            this.buttonDeleteAllPicasa.Text = "DeleteAllPicasa";
            this.buttonDeleteAllPicasa.UseVisualStyleBackColor = true;
            this.buttonDeleteAllPicasa.Visible = false;
            this.buttonDeleteAllPicasa.Click += new System.EventHandler(this.buttonDeleteAllPicasa_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(281, 435);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(86, 7);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(84, 23);
            this.buttonClear.TabIndex = 16;
            this.buttonClear.Text = "Clear Folders";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonLogOut
            // 
            this.buttonLogOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLogOut.Location = new System.Drawing.Point(390, 7);
            this.buttonLogOut.Name = "buttonLogOut";
            this.buttonLogOut.Size = new System.Drawing.Size(75, 23);
            this.buttonLogOut.TabIndex = 17;
            this.buttonLogOut.Text = "Logout";
            this.buttonLogOut.UseVisualStyleBackColor = true;
            this.buttonLogOut.Click += new System.EventHandler(this.buttonLogOut_Click);
            // 
            // buttonFixVideoTime
            // 
            this.buttonFixVideoTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonFixVideoTime.Location = new System.Drawing.Point(99, 408);
            this.buttonFixVideoTime.Name = "buttonFixVideoTime";
            this.buttonFixVideoTime.Size = new System.Drawing.Size(84, 23);
            this.buttonFixVideoTime.TabIndex = 18;
            this.buttonFixVideoTime.Text = "FixVideoTime";
            this.buttonFixVideoTime.UseVisualStyleBackColor = true;
            this.buttonFixVideoTime.Visible = false;
            this.buttonFixVideoTime.Click += new System.EventHandler(this.buttonFixVideoTime_Click);
            // 
            // checkBoxRecheckSubfolders
            // 
            this.checkBoxRecheckSubfolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxRecheckSubfolders.AutoSize = true;
            this.checkBoxRecheckSubfolders.Location = new System.Drawing.Point(6, 367);
            this.checkBoxRecheckSubfolders.Name = "checkBoxRecheckSubfolders";
            this.checkBoxRecheckSubfolders.Size = new System.Drawing.Size(80, 17);
            this.checkBoxRecheckSubfolders.TabIndex = 19;
            this.checkBoxRecheckSubfolders.Text = "Reorganize";
            this.checkBoxRecheckSubfolders.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 519);
            this.Controls.Add(this.checkBoxRecheckSubfolders);
            this.Controls.Add(this.buttonFixVideoTime);
            this.Controls.Add(this.buttonLogOut);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonDeleteAllPicasa);
            this.Controls.Add(this.buttonTrashAll);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOrganize);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonAddDirectory);
            this.Controls.Add(this.treeViewDirectories);
            this.Controls.Add(this.richTextBoxLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Google Photos Organizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.contextMenuStripTree.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.TreeView treeViewDirectories;
        private System.Windows.Forms.Button buttonAddDirectory;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonOrganize;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTree;
        private System.Windows.Forms.ToolStripMenuItem doNotMakeAlbumWithFolderNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeAlbumWithFolderNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeAlbumsWithSubfolderNamesToolStripMenuItem;
        private System.Windows.Forms.Button buttonAbout;
        private System.Windows.Forms.Button buttonTrashAll;
        private System.Windows.Forms.Button buttonDeleteAllPicasa;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.ToolStripMenuItem addFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Button buttonLogOut;
        private System.Windows.Forms.Button buttonFixVideoTime;
        private System.Windows.Forms.CheckBox checkBoxRecheckSubfolders;
    }
}

