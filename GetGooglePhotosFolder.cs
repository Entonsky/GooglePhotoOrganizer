using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePhotoOrganizer
{
    public partial class GetGooglePhotosFolder : Form
    {
        public GetGooglePhotosFolder()
        {
            InitializeComponent();
        }

               

        private static string LocalFileName()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GooglePhotosFolder.id");
        }

        private static string GetSavedName()
        {
            try
            {
                var file = LocalFileName();
                if (!File.Exists(file))
                    return null;
                return File.ReadAllText(file);
            }
            catch { return null; }
            
        }


        private static void SetSavedName(string name)
        {
            try
            {
                var file = LocalFileName();
                File.WriteAllText(file, name);
            }
            catch
            {

            }
        }

        static GetGooglePhotosFolder _thisForm;
        static string _googlePhotoFolderId = null;
        
        public static string GetGooglePhotoFolderId(Action<string> logText = null)
        {
            if (_googlePhotoFolderId != null)
                return _googlePhotoFolderId;

            
            List<string> sugestionNames = new List<string>() { "Google Photos", "Google Фото", "Google Photo" };
            var local = GetSavedName();
            if (!String.IsNullOrWhiteSpace(local))
                sugestionNames.Insert(0, local);
            

            var photoFolders = new List<Google.Apis.Drive.v2.Data.File>();
            while (true)
            {
                try
                {
                    if (logText != null)
                    {
                        logText("Login on google.");
                        if (!Directory.Exists(GoogleDriveClient.CredPath))
                            logText("New browser window will appear. You should click accept for work with app.");
                    }
                    var google = new GoogleDriveClient();
                    if (logText != null)
                        logText("Login Ok.");
                    foreach (var sg in sugestionNames)
                    {
                        photoFolders = google.GetDirectories(sg, "root");
                        if (photoFolders.Count > 0)
                            break;
                    }
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    return null;
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show("Can't connect to google drive. Probably something wrong with your internet connection?\r\nTry again please.\r\n"+
                        "\r\nException Code:\r\n"+ex.ToString(),
                        "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) ==
                        DialogResult.Retry)
                        continue;
                    else
                        return null;
                }
                break;
            }

            if (photoFolders.Count == 1)
                return photoFolders[0].Id;

            if (photoFolders.Count > 1)
            {
                MessageBox.Show("Unexpected error. Too many photo folders.\r\n Remove 'Google photos' folders, which is not needed.");
                return null;
            }

            //Can't found photo folder

            if (_thisForm == null || _thisForm.IsDisposed)
                _thisForm = new GetGooglePhotosFolder();

            _thisForm.FillGoogleDirs();

            _thisForm.ShowDialog();
            var result = _thisForm.result;
            var resultName = _thisForm.resultName;
            _thisForm.Dispose();
            if (!String.IsNullOrWhiteSpace(result))
            {
                if (!String.IsNullOrWhiteSpace(resultName))
                    SetSavedName(resultName);
                _googlePhotoFolderId = result;
            }
            return result;
        }


        Dictionary<string, string> _dirs = new Dictionary<string, string>();
        public void FillGoogleDirs()
        {
            var google = new GoogleDriveClient();
            var folders = google.GetDirectories(null, "root");
            _dirs = new Dictionary<string, string>();
            radioListBox.Items.Clear();
            foreach (var folder in folders)
            {
                if (!_dirs.ContainsKey(folder.Title))
                {
                    _dirs.Add(folder.Title, folder.Id);
                    radioListBox.Items.Add(folder.Title);
                }
            } 
        }

        
        private void radioListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        bool _isUiChanged = false;

        private void radioListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_isUiChanged)
                return;
            _isUiChanged = true;
            for (var i = 0; i < radioListBox.Items.Count; i++)
            {
                if (radioListBox.GetItemChecked(i))
                    radioListBox.SetItemChecked(i, false);
            }
            _isUiChanged = false;
        }

        public string result = null;
        public string resultName = null;

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (radioListBox.CheckedItems.Count != 1)
            {
                MessageBox.Show("Select one element");
                return;
            }

            var rText = radioListBox.CheckedItems[0].ToString();

            if (!rText.ToLower().Contains("google"))
            {
                var warningText = "Are you sure want to select folder '" + rText + "' as your Google Photos folder?\r\n" +
                                  "It does not contains the word 'Google'.\r\n" +
                                  "To reset this option in future you should delete file\r\n" +
                                  LocalFileName();

                if (MessageBox.Show(warningText, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)==
                    DialogResult.No)
                {
                    return;
                }
            }

            resultName = rText;
            result = _dirs[rText];
            this.Close();
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            FillGoogleDirs();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            result = null;
            this.Close();
        }
    }
}
