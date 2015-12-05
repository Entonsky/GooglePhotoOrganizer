using Google.Apis.Drive.v2.Data;
using Google.GData.Client;
using Google.GData.Photos;
using GooglePhotoOrganizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooglePhotoOrganizer
{
    class Synchronizer
    {

        ProgressBar _progressBar;
        RichTextBox _richTextBox;

        public Synchronizer(ProgressBar progressBar, RichTextBox richTextBox)
        {
            _progressBar = progressBar;
            _richTextBox = richTextBox;
        }


        private void LogText(string text)
        {
            Action action = delegate() { _richTextBox.AppendText(text + "\r\n"); _richTextBox.ScrollToCaret(); };
            _richTextBox.Invoke(action);
        }

        private void IncreaseProgress()
        {
            Action action = delegate () { _progressBar.Value++; };
            _progressBar.Invoke(action);
        }

        private void ResetProgress(int maxValue)
        {
            Action action = delegate () { _progressBar.Value = 0; _progressBar.Maximum = maxValue; };
            _progressBar.Invoke(action);
        }

        delegate string ExtractKeyDelegate(object item);
        Dictionary<string, List<object>> MakeKeyDict(IEnumerable<object> list, ExtractKeyDelegate KeyExtractor)
        {
            var result = new Dictionary<string, List<object>>();
            foreach (var item in list)
            {
                var key = KeyExtractor(item);
                if (key == null)
                    key = "";
                if (!result.ContainsKey(key))
                    result.Add(key, new List<object>());
                result[key].Add(item);
            }
            return result;
        }

        class FileDesc
        {
            public string path;
            public string relPath;
            public string album;

            public FileDesc(string path, string relPath, string album)
            {
                this.path = path;
                this.relPath = relPath;
                this.album = album;
            }
        }

        
        Dictionary<string, List<FileDesc>> GetFilesFromNodes(string pathRoot, List<TreeNode> nodes)
        {
            var result = new Dictionary<string, List<FileDesc>>();
            foreach (var node in nodes)
            {
                var directory = (string)node.Tag;

                if (!System.IO.Directory.Exists(directory))
                    continue;
                //Files in folder according to node
                var files = System.IO.Directory.GetFiles(directory);

                var albumName = "Default";
                var curNode = node;
                while (curNode!= null && curNode.Parent != null)
                {
                    if (curNode.ForeColor!= Color.Gray)
                    {
                        albumName = curNode.Text;
                        break;
                    }
                    curNode = curNode.Parent;
                }

                foreach (var file in files)
                {
                    var fName = System.IO.Path.GetFileName(file);
                    var relPath = FileWorker.GetRelativePathDirectory(file, pathRoot);
                    if (!result.ContainsKey(fName))
                        result.Add(fName, new List<FileDesc>());
                    //Add filePath, 
                    result[fName].Add(new FileDesc(file, relPath, albumName));
                }
            }

            return result;
        }



        void AddPicasaFiles(PicasaClient picasa, 
            Dictionary<string, List<PicasaEntry>> picasaFiles, HashSet<string> picasaFilesId, 
            string albumId = null, string searchText = null)
        {
            var pf = picasa.GetPhotos(albumId, searchText);
            foreach (var entry in pf)
                AddPicasaFile(entry, picasaFiles, picasaFilesId);            
        }


        void AddPicasaFile(PicasaEntry newFile,
            Dictionary<string, List<PicasaEntry>> picasaFiles, HashSet<string> picasaFilesId)
        {
            var photo = new PhotoAccessor(newFile);
            if (picasaFilesId.Contains(photo.Id))
                return; //This file already in picasa files
            picasaFilesId.Add(photo.Id);
            if (!picasaFiles.ContainsKey(photo.PhotoTitle))
                picasaFiles.Add(photo.PhotoTitle, new List<PicasaEntry>());
            picasaFiles[photo.PhotoTitle].Add(newFile);
        }


        public void Organize(string pathRoot, List<TreeNode> nodes, string drivePhotoDirId, bool driveOrg = true, bool albumOrg = true, bool useDateTag = true)
        {
            if (!System.IO.Directory.Exists(pathRoot))
                return;

            GoogleDriveClient drive = null;
            if (driveOrg)
                drive = new GoogleDriveClient();
            PicasaClient picasa = null;
            if (albumOrg)
                picasa = new PicasaClient();

            LogText("Search for files in '" + pathRoot+"'");
            var localFiles = GetFilesFromNodes(pathRoot, nodes);
            if (localFiles.Count ==0)
            {
                LogText("No files found");
                return;
            }

            LogText("Search for files already in Google Photos directory on Google Drive. It can take a long time...");
            var googleFilesLst = drive.GetFiles(null, drivePhotoDirId);
            var googleFiles = MakeKeyDict(googleFilesLst,
                (object file) => { return ((File)file).OriginalFilename; });
            if (googleFilesLst.Count == 0)
            {
                LogText("No files found");
                return;
            }
            else
                LogText("Found " + googleFilesLst.Count + " files");
            
            LogText("Search for exist photo albums");
            var picasaAlbumsLst = picasa.GetAlbums();
            //Album by name 
            var picasaAlbumsByName = new Dictionary<string, AlbumAccessor>();
            foreach (var picasaAlbum in picasaAlbumsLst)
            {
                if (!picasaAlbumsByName.ContainsKey(picasaAlbum.AlbumTitle))
                    picasaAlbumsByName.Add(picasaAlbum.AlbumTitle, picasaAlbum);
            }
            LogText("Found " + picasaAlbumsLst.Count + " albums");

            var picasaFiles = new Dictionary<string, List<PicasaEntry>>();
            var picasaFilesId = new HashSet<string>();

            LogText("MOVING FILES...");
            ResetProgress(googleFiles.Count);
            foreach (var googleFilePair in googleFiles)
            {
                if (!localFiles.ContainsKey(googleFilePair.Key))
                {
                    IncreaseProgress();
                    continue;
                }

                //Try get picasa file by google file name        
                foreach (var googleFile in googleFilePair.Value)
                {
                    //Add by 
                    var googleFileName = ((File)googleFile).OriginalFilename;
                    if (!picasaFiles.ContainsKey(googleFileName))
                    {
                        var foundAlbums = new HashSet<string>();
                        var picasaFoundFiles = picasa.GetPhotos(null, googleFileName);
                        foreach (var file in picasaFoundFiles)
                        {
                            AddPicasaFile(file, picasaFiles, picasaFilesId);
                            var picasaFoto = new PhotoAccessor(file);
                            foundAlbums.Add(picasaFoto.AlbumId);
                        }
                        //Also try to add any files from album of found files
                        foreach (var albumId in foundAlbums)
                            AddPicasaFiles(picasa, picasaFiles, picasaFilesId, albumId);
                    }
                }
                
                //Has fileName matching
                //Seek for exif data matching
                var exifLocal = MakeKeyDict(localFiles[googleFilePair.Key],
                (object file) => 
                {
                    string exifDate = "";
                    if (useDateTag)
                        exifDate = ImageWorker.GetExifDateInGoogleFormat(((FileDesc)file).path);
                    return exifDate;
                });
                
                var exifGoogle = MakeKeyDict(googleFilePair.Value,
                (object file) =>
                {
                    string exifDate = "";
                    if (useDateTag)
                    {
                        if (((File)file).ImageMediaMetadata!=null && ((File)file).ImageMediaMetadata!=null)
                            exifDate = ((File)file).ImageMediaMetadata.Date;
                    }
                    return exifDate;
                });

                Dictionary<string, List<object>> exifPicasa = new Dictionary<string, List<object>>();
                if (picasaFiles.ContainsKey(googleFilePair.Key))
                {
                    exifPicasa =  MakeKeyDict(picasaFiles[googleFilePair.Key],
                    (object file) =>
                    {
                        string exifDate = "";
                        if (useDateTag)
                        {
                            if (((PicasaEntry)file).Exif != null && ((PicasaEntry)file).Exif.Time != null)
                            {
                                var timeStr = ((PicasaEntry)file).Exif.Time.Value;
                                long value;
                                if (Int64.TryParse(timeStr, out value))
                                {
                                    var time = new DateTime(1970, 1, 1, 0, 0, 0);
                                    time = time.AddMilliseconds(value);
                                    exifDate = ImageWorker.ExifDateToGoogleFormat(time.ToString());
                                }
                            }
                        }
                        return exifDate;
                    });
                }


                Dictionary<string, string> googleDirs = new Dictionary<string, string>();

                foreach (var exifListPair in exifLocal)
                {
                    //No such files on google photos
                    if (!exifGoogle.ContainsKey(exifListPair.Key) && !exifPicasa.ContainsKey(exifListPair.Key))
                        continue;
                    
                    if (exifListPair.Value.Count>1)
                    {
                        LogText("The same files in different folders. Skipping them:");
                        foreach (FileDesc file in exifListPair.Value)
                            LogText("   "+file.path);
                        continue;
                    }

                    if (exifListPair.Value.Count == 0)
                        continue;

                    var localFile = (FileDesc)exifListPair.Value[0];

                    //Move files to google dirs
                    if (exifGoogle.ContainsKey(exifListPair.Key) && exifGoogle[exifListPair.Key].Count != 0)
                    {
                        string googleDirId;
                        if (googleDirs.ContainsKey(localFile.relPath))
                            googleDirId = googleDirs[localFile.relPath];
                        else
                        {
                            googleDirId = drive.CreateCascadeDirectory(localFile.relPath).Id;
                            googleDirs.Add(localFile.relPath, googleDirId);
                        }
                        
                        if (exifGoogle[exifListPair.Key].Count > 1)
                            LogText("Has " + exifGoogle[exifListPair.Key].Count + " dublicates with name '" + ((File)exifGoogle[exifListPair.Key][0]).OriginalFilename + "' among the google files. All of them will move to '" + localFile.relPath + "'");

                        foreach (File googleFile in exifGoogle[exifListPair.Key])
                        {
                            bool hasSameParent = false;
                            foreach (var parent in googleFile.Parents)
                            {
                                if (parent.Id == googleDirId)
                                {
                                    hasSameParent = true;
                                    break;
                                }
                            }
                            if (!hasSameParent) //If not already in this folder
                                drive.MoveFileToDirectory(googleFile.Id, googleDirId);

                            if (String.IsNullOrWhiteSpace(exifListPair.Key))
                            {
                                //No Exif date for file.
                                //Will change google CreationDate
                                var creationDate = System.IO.File.GetCreationTime(localFile.path);
                                if (googleFile.CreatedDate > creationDate)
                                    drive.SetCreationDate(googleFile.Id, creationDate);
                            }

                        }
                    }

                    //Move files to picasa album
                    if (exifPicasa.ContainsKey(exifListPair.Key) && exifPicasa[exifListPair.Key].Count != 0)
                    {
                        AlbumAccessor picasaAlbum;
                        if (picasaAlbumsByName.ContainsKey(localFile.album))
                            picasaAlbum = picasaAlbumsByName[localFile.album];
                        else
                        {
                            picasaAlbum = picasa.CreateAlbum(localFile.album, localFile.relPath);
                            picasaAlbumsByName.Add(localFile.album, picasaAlbum);
                        }

                        if (exifPicasa[exifListPair.Key].Count > 1)
                        {
                            var photo = new PhotoAccessor((PicasaEntry)exifPicasa[exifListPair.Key][0]);
                            LogText("Has " + exifPicasa[exifListPair.Key].Count + " dublicates with name '" + photo.PhotoTitle + "' among the Google Photo files. All of them will move to album '" + localFile.album + "'");
                        }

                        foreach (PicasaEntry picasaFile in exifPicasa[exifListPair.Key])
                        {
                            var photo = new PhotoAccessor(picasaFile);
                            if (photo.AlbumId!= picasaAlbum.Id) //Not in that album yet
                                picasa.MovePhotoToAlbum(picasaFile, picasaAlbum.Id);
                        }
                    }
                }
                
                IncreaseProgress();
            }
            LogText("\r\nREADY");
            ResetProgress(100);
        }





        public void Upload(List<TreeNode> dirNodes, string drivePhotoDirId)
        {
            /*
            var google = new GoogleDriveClient();

            if (dirNodes == null)
                return;
            var checkedNodes = new List<TreeNode>();
            foreach (var node in dirNodes)
                if (node.Checked)
                    checkedNodes.Add(node);

            if (checkedNodes.Count == 0)
                return;

            var drive = new GoogleDriveClient();
            //var drivePhotoDirId = GetGooglePhotoId();

            LogText("Search for albums in web...");
            var picasa = new PicasaClient();
            var albums = picasa.GetAlbums();
            LogText("Found " + albums.Count + " albumns");

            LogText("Search for files on disk...");
            int totalFiles = 0;
            var allFiles = FileWorker.GetFilesForNodes(checkedNodes, out totalFiles);
            //progressBar.Maximum = totalFiles;
            LogText("Found " + totalFiles + " files");


            foreach (var pair in allFiles)
            {
                if (pair.Value.Count == 0)
                    continue;//Пустой каталог

                //1. Проверим, есть ли абльбом с таким названием, и если нет - создадим
                string albumId;
                var albumnTitle = pair.Key.Text;
                if (!albums.ContainsKey(albumnTitle))
                    try
                    {
                        LogText("Create album '" + albumnTitle + "'");
                        albumId = picasa.CreateAlbum(albumnTitle, (string)pair.Key.Tag).Id;
                    }
                    catch (Exception ex)
                    {
                        LogText("Can't create album '" + albumnTitle + "'\r\n" + ex.ToString());
                        continue;
                    }

                else
                {
                    LogText("Process album '" + albumnTitle + "'");
                    albumId = albums[albumnTitle].Id;
                }

                //Получим файлы, которые уже в альбоме
                Dictionary<string, PicasaEntry> filesAlreadyInAlbumn;
                try
                {
                    filesAlreadyInAlbumn = picasa.GetPhotos(albumId);
                }
                catch (Exception ex)
                {
                    LogText("Can't get photos in album '" + albumnTitle + "'\r\n" + ex.ToString());
                    continue;
                }

                foreach (var file in pair.Value)
                {
                    var fName = System.IO.Path.GetFileName(file);
                    if (!filesAlreadyInAlbumn.ContainsKey(fName))
                    {
                        try
                        {
                            var about = google.GetAbout();
                            var hasSpace = (about.QuotaBytesTotal - about.QuotaBytesUsedAggregate) / 1024 / 1024;
                            //LogText("Has space " + hasSpace+" Mb");
                            picasa.UploadPhoto(file, albumId);
                            LogText("Uploading '" + file + "'...");
                        }
                        catch (GDataRequestException ex)
                        {
                            LogText("Can't upload '" + file + "': " + ex.ResponseString);
                        }
                    }
                    IncreaseProgress();
                    Application.DoEvents();
                }
                pair.Key.BackColor = Color.Yellow;
            }

            //progressBar.Value = 0;*/
        }




    }
}
