using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePhotoOrganizer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool result;
            var mutex = new System.Threading.Mutex(true, "UniqueAppId", out result);

            if (!result)
            {
                MessageBox.Show("Another instance is already running.");
                return;
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (String.IsNullOrWhiteSpace(GetGooglePhotosFolder.GetGooglePhotoFolderId()))
                return;
            Application.Run(new MainForm());

            GC.KeepAlive(mutex);
        }
    }
}
