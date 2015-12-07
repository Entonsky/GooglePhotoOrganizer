using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePhotoOrganizer
{
    public partial class FolderSelectorForm : Form
    {
        public FolderSelectorForm()
        {
            InitializeComponent();
        }


        static FolderSelectorForm _thisForm;

        public DialogResult dialogResult;

        public static DialogResult SelectTheFolder(string fileName, List<string> folders, out int checkedIndex, out bool forAllChecked)
        {
            if (_thisForm == null || _thisForm.IsDisposed)
                _thisForm = new FolderSelectorForm();
            _thisForm.radioListBox.Items.Clear();
            foreach (var item in folders)
            {
                _thisForm.radioListBox.Items.Add(item);
            }
            _thisForm.checkBoxAlwaysThis.Checked = false;
            _thisForm.labelFileName.Text = fileName;

            _thisForm.ShowDialog();
            if (_thisForm.radioListBox.CheckedItems.Count != 1)
                checkedIndex = -1;
            else
                checkedIndex = _thisForm.radioListBox.CheckedIndices[0];

            if (checkedIndex >= 0)
                forAllChecked = _thisForm.checkBoxAlwaysThis.Checked;
            else
                forAllChecked = false;

            return _thisForm.dialogResult;
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (radioListBox.CheckedItems.Count!=1)
            {
                MessageBox.Show("Select one element");
                return;
            }

            dialogResult = DialogResult.OK;
            this.Close();
        }

        private void FolderSelectorForm_Load(object sender, EventArgs e)
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

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            dialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (radioListBox.SelectedIndex < 0)
                return;
            Clipboard.SetText(radioListBox.SelectedItem.ToString());

        }

        private void viewFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}
