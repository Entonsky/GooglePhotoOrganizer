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
            this.textBoxLocalPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewDirectories = new System.Windows.Forms.TreeView();
            this.contextMenuStripTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeAlbumWithFolderNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonScanDirectory = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonOrganize = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelTimer = new System.Windows.Forms.Label();
            this.contextMenuStripTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLog.Location = new System.Drawing.Point(-1, 395);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.Size = new System.Drawing.Size(471, 98);
            this.richTextBoxLog.TabIndex = 1;
            this.richTextBoxLog.Text = "";
            // 
            // textBoxLocalPath
            // 
            this.textBoxLocalPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLocalPath.Location = new System.Drawing.Point(73, 10);
            this.textBoxLocalPath.Name = "textBoxLocalPath";
            this.textBoxLocalPath.Size = new System.Drawing.Size(302, 20);
            this.textBoxLocalPath.TabIndex = 3;
            this.textBoxLocalPath.Text = "C:\\Users\\Alexey\\Desktop\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "LocalFolder";
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
            this.doNotMakeAlbumWithFolderNameToolStripMenuItem,
            this.doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem,
            this.makeAlbumWithFolderNameToolStripMenuItem,
            this.makeAlbumsWithSubfolderNamesToolStripMenuItem});
            this.contextMenuStripTree.Name = "contextMenuStripTree";
            this.contextMenuStripTree.Size = new System.Drawing.Size(462, 92);
            this.contextMenuStripTree.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTree_Opening);
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
            // buttonScanDirectory
            // 
            this.buttonScanDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonScanDirectory.Location = new System.Drawing.Point(381, 8);
            this.buttonScanDirectory.Name = "buttonScanDirectory";
            this.buttonScanDirectory.Size = new System.Drawing.Size(84, 23);
            this.buttonScanDirectory.TabIndex = 6;
            this.buttonScanDirectory.Text = "Scan Dir";
            this.buttonScanDirectory.UseVisualStyleBackColor = true;
            this.buttonScanDirectory.Click += new System.EventHandler(this.buttonGetDirectories_Click);
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
            this.buttonOrganize.Location = new System.Drawing.Point(6, 364);
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
            this.labelTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTimer.AutoSize = true;
            this.labelTimer.Location = new System.Drawing.Point(276, 369);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(0, 13);
            this.labelTimer.TabIndex = 11;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 519);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOrganize);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonScanDirectory);
            this.Controls.Add(this.treeViewDirectories);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxLocalPath);
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
        private System.Windows.Forms.TextBox textBoxLocalPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeViewDirectories;
        private System.Windows.Forms.Button buttonScanDirectory;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonOrganize;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTree;
        private System.Windows.Forms.ToolStripMenuItem doNotMakeAlbumWithFolderNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeAlbumWithFolderNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doNotMakeAlbumsWithSubFolderNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeAlbumsWithSubfolderNamesToolStripMenuItem;
    }
}

