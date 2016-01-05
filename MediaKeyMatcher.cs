using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;
using Google.GData.Photos;
using Google.Apis.Drive.v2.Data;
using System.Drawing;

namespace GooglePhotoOrganizer
{
    class MediaKeyMatcher
    {

        public class MatchedFiles
        {
            public List<FileDesc> localFiles = new List<FileDesc>();
            public List<PicasaEntry> picasaFiles = new List<PicasaEntry>();
            public List<File> googleFiles = new List<File>();
        }


        delegate object ExtractKeyDelegate(object item);
        static Dictionary<object, List<object>> MakeKeyDict(IEnumerable<object> list, ExtractKeyDelegate KeyExtractor)
        {
            var result = new Dictionary<object, List<object>>();
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


        delegate object ExtractKeyDelegate2(object item, Dictionary<object, List<object>> exifGoogle);
        static Dictionary<object, List<object>> MakeKeyDict(IEnumerable<object> list, Dictionary<object, List<object>> exifGoogle, ExtractKeyDelegate2 KeyExtractor)
        {
            var result = new Dictionary<object, List<object>>();
            foreach (var item in list)
            {
                var key = KeyExtractor(item, exifGoogle);
                if (key == null)
                    key = "";
                if (!result.ContainsKey(key))
                    result.Add(key, new List<object>());
                result[key].Add(item);
            }
            return result;
        }


        public static List<MatchedFiles> MatchFilesWithTheSameName(List<FileDesc> localFiles, List<File> googleFiles, List<PicasaEntry> picasaFiles)
        {
            var result = new Dictionary<object, MatchedFiles>();

            var localNotMatched = new HashSet<FileDesc>(localFiles);
            var googleNotMatched = new HashSet<File>(googleFiles);
            var picasaNotMatched = new HashSet<PicasaEntry>(picasaFiles);

            //Match by exif date
            MatchFilesWithTheSameNameByType(result,
                localNotMatched, googleNotMatched, picasaNotMatched,
                (object file) =>
                {
                    return MediaKeyMatcher.GetExifImageDate(((FileDesc)file).path);
                },
                (object file) =>
                {
                    return MediaKeyMatcher.GetExifImageDate((File)file);
                },
                (object file, Dictionary<object, List<object>> exifGoogle) =>
                {
                    return MediaKeyMatcher.GetPicasaKeyByDriveKey((PicasaEntry)file, exifGoogle);
                });
            
            //match by creationDate
            MatchFilesWithTheSameNameByType(result,
                localNotMatched, googleNotMatched, picasaNotMatched,
                (object file) =>
                {
                    return MediaKeyMatcher.GetMinDateKey(((FileDesc)file).path);
                },
                (object file) =>
                {
                    return MediaKeyMatcher.GetMinDateKey((File)file);
                },
                (object file, Dictionary<object, List<object>> exifGoogle) =>
                {
                    return MediaKeyMatcher.GetPicasaKeyByDriveKey((PicasaEntry)file, exifGoogle);
                });


            //Match by image Size or movielength
            MatchFilesWithTheSameNameByType(result,
                localNotMatched, googleNotMatched, picasaNotMatched,
                (object file) =>
                {
                    return MediaKeyMatcher.GetImageSizeOrMovieLength(((FileDesc)file).path);
                },
                (object file) =>
                {
                    return MediaKeyMatcher.GetImageSizeOrMovieLength((File)file);
                },
                (object file, Dictionary<object, List<object>> exifGoogle) =>
                {
                    return MediaKeyMatcher.GetPicasaKeyByDriveKey((PicasaEntry)file, exifGoogle);
                });


            if (localNotMatched.Count>0 || googleNotMatched.Count>0 || picasaNotMatched.Count>0)
            {
                var defaultKey = "Default";
                if (!result.ContainsKey(defaultKey))
                    result.Add(defaultKey, new MatchedFiles());
                foreach (FileDesc fileDesc in localNotMatched)
                    result[defaultKey].localFiles.Add(fileDesc);
                foreach (File file in googleNotMatched)
                    result[defaultKey].googleFiles.Add(file);
                foreach (PicasaEntry file in picasaNotMatched)
                    result[defaultKey].picasaFiles.Add(file);
            }

            return result.Values.ToList();
        }


        private static bool IsKeyMatched(object key1, object key2)
        {
            if (key1 == null && key2 == null)
                return true;
            if (key1 == null || key2 == null)
                return false;
            if ((key1 is double || key1 is double?) && (key2 is double || key2 is double?))
            {
                double key1Double = key1 is double ? (double)key1 : ((double?)key1).Value;
                double key2Double = key2 is double ? (double)key2 : ((double?)key2).Value;
                if (Math.Abs(key1Double - key2Double) < 1)
                    return true;
                else
                    return false;
            }
            if ((key1 is DateTime || key1 is DateTime?) && (key2 is DateTime || key2 is DateTime?))
            {
                DateTime key1Time = key1 is DateTime ? (DateTime)key1 : ((DateTime?)key1).Value;
                DateTime key2Time = key2 is DateTime ? (DateTime)key2 : ((DateTime?)key2).Value;
                if (Math.Abs(key1Time.Subtract(key2Time).TotalSeconds) < 10)
                    return true;
                else
                    return false;
            }
            if (key1 is String && key2 is String)
                return key1.ToString() == key2.ToString();
            return key1 == key2;
        }


        private static List<object> GetNearKeys(object key, Dictionary<object, List<object>> compareWith, out bool manyKeys)
        {
            var result = new List<object>();
            manyKeys = false;

            foreach (var pair in compareWith)
            {
                if (IsKeyMatched(key, pair.Key))
                {

                    if (result.Count > 0)
                        manyKeys = true;
                    result.AddRange(pair.Value);
                }
            }
            return result;
        }


        /*
        public static List<KeyValuePair<List<object>, List<object>>> ClusteringByNearKey(Dictionary<object, List<object>> exifLocal)
        {
            //Clustering localExif by key
            var exifClusters = new List<KeyValuePair<List<object>, List<object>>>();
            foreach (var pair in exifLocal)
            {
                var newCluster = new KeyValuePair<List<object>, List<object>>(new List<object>(), new List<object>());
                newCluster.Key.Add(pair.Key);
                newCluster.Value.AddRange(pair.Value);

                var matchedCl = new List<KeyValuePair<List<object>, List<object>>>();

                foreach (var cluster in exifClusters)
                {
                    if (IsKeyMatched(pair.Key, cluster.Key))
                    {
                        //Has matching. All to this cluster
                        newCluster.Key.AddRange(cluster.Key);
                        newCluster.Value.AddRange(cluster.Value);
                        matchedCl.Add(cluster);
                    }
                }
                //delete matched
                foreach (var matched in matchedCl)
                    exifClusters.Remove(matched);
                exifClusters.Add(newCluster);
            }
            //End Clustering
            return exifClusters;
        }*/

        private static void MatchFilesWithTheSameNameByType(Dictionary<object, MatchedFiles> result, 
            HashSet<FileDesc> localFiles, HashSet<File> googleFiles, HashSet<PicasaEntry> picasaFiles,
            ExtractKeyDelegate localExtractor, ExtractKeyDelegate googleExtractor, ExtractKeyDelegate2 picasaExtractor)
        { 
            
            //Get different localfile keys
            var exifLocal = MakeKeyDict(localFiles, localExtractor);
            var exifGoogle = MakeKeyDict(googleFiles, googleExtractor);
            var exifPicasa = MakeKeyDict(picasaFiles, exifGoogle, picasaExtractor);
            
            /*
            if (1==1)
            {
                //Get different localfile keys
                var exifLocal1 = MakeKeyDict(localFiles, localExtractor);
                var exifGoogle1 = MakeKeyDict(googleFiles, googleExtractor);
                var exifPicasa1 = MakeKeyDict(picasaFiles, exifGoogle, picasaExtractor);
            }*/

            //Try matching by exif
            foreach (var pair in exifLocal)
            {
                if (pair.Key == null || pair.Key is string && String.IsNullOrWhiteSpace((string)pair.Key))
                    continue;

                //try local Matching
                bool manyLocalKeys;
                var localMatched = GetNearKeys(pair.Key, exifLocal, out manyLocalKeys);
                if (localMatched.Count > 1 || manyLocalKeys)
                    continue; //Bad matching - many file in this basked, or multikeys

                bool manyGoogleKeys;
                var googleMatched = GetNearKeys(pair.Key, exifGoogle, out manyGoogleKeys);
                bool manyPicasaKeys;
                var picasaMatched = GetNearKeys(pair.Key, exifPicasa, out manyPicasaKeys);

                if (googleMatched.Count == 0 && picasaMatched.Count == 0)
                    continue; //Nothing to match

                if (manyGoogleKeys || manyPicasaKeys)
                    continue; //Multimatching? Skip this
                
                if (!result.ContainsKey(pair.Key))
                    result.Add(pair.Key, new MatchedFiles());

                foreach (FileDesc fileDesc in pair.Value)
                    result[pair.Key].localFiles.Add(fileDesc);
                 
                if (googleMatched.Count > 0 && picasaMatched.Count > 0)
                {
                    //Delete if both matched
                    foreach (FileDesc fileDesc in pair.Value)
                        localFiles.Remove(fileDesc);
                }

                if (googleMatched.Count>0)
                {
                    foreach (File file in googleMatched)
                    {
                        result[pair.Key].googleFiles.Add(file);
                        googleFiles.Remove(file);
                    }
                }
                if (picasaMatched.Count>0)
                {
                    foreach (PicasaEntry file in picasaMatched)
                    {
                        result[pair.Key].picasaFiles.Add(file);
                        picasaFiles.Remove(file);
                    }
                }
            }
        }

        
        public static long GetFileSize(string filePath)
        {
            lock (_diskReadLock)
            {
                var f = new System.IO.FileInfo(filePath);
                return f.Length;
            }
        }






        static object _diskReadLock = new Object();

        //Keydata for file
        public static object GetExifImageDate(string filePath)
        {
            lock (_diskReadLock)
            {
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    if (IsImage(fs))
                    {
                        try
                        {
                            BitmapSource img = BitmapFrame.Create(fs);
                            BitmapMetadata md = (BitmapMetadata)img.Metadata;
                            string dateStr = md.DateTaken;
                            DateTime dateResult;
                            if (!String.IsNullOrWhiteSpace(dateStr)&& DateTime.TryParse(dateStr, out dateResult))
                            {
                                fs.Close();
                                return dateResult;
                            }
                        } catch { }
                    }
                    fs.Close();
                    return null;
                }
            }
        }

        
        //Keydata for file
        public static object GetImageSizeOrMovieLength(string filePath)
        {
            lock (_diskReadLock)
            {
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    if (IsImage(fs))
                    {
                        try
                        {

                          
                            using (Image tif = Image.FromStream(stream: fs,
                                                                useEmbeddedColorManagement: false,
                                                                validateImageData: false))
                            {
                                float width = tif.PhysicalDimension.Width;
                                float height = tif.PhysicalDimension.Height;
                                fs.Close();
                                return width.ToString() + "&&" + height.ToString();
                            }
                            /*
                            BitmapSource img = BitmapFrame.Create(fs);
                            //null date. Try width and height
                            if (img.Width != 0 && img.Height != 0) //Google usually transforms images more the 4000 pixels
                            {
                                fs.Close();
                                return img.Width.ToString() + "&&" + img.Height.ToString();
                            }*/

                        }
                        catch {}
                        fs.Close();
                        return null;
                    }
                    else
                    {
                        fs.Close();

                        try
                        {
                            var videoDuration = GetVideoDuration(filePath);
                            return videoDuration;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
        }



        public static DateTime? ExifDateToDateTime(string date)
        {
            var sp = date.Split(':');
            if (sp.Length == 5)
            {
                string dateStr = sp[0] + "-" + sp[1] + "-" + sp[2] + ":" + sp[3] + ":" + sp[4];
                return DateTime.Parse(dateStr);
            }

            DateTime dateParsed;
            if (DateTime.TryParse(date, out dateParsed))
                return dateParsed;
            else
                return null;
        }


        public static DateTime? GetExifDate(string filePath)
        {
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                BitmapSource img = BitmapFrame.Create(fs);
                BitmapMetadata md = (BitmapMetadata)img.Metadata;
                string date = md.DateTaken;
                if (String.IsNullOrWhiteSpace(date))
                {
                    fs.Close();
                    return null;
                }
                fs.Close();
                return ExifDateToDateTime(date);                
            }
        }


        public static object GetPicasaKeyByDriveKey(PicasaEntry picasaEntry, Dictionary<object, List<object>> driveKeys)
        {
            //Make dictionary Size -> Keys from driveKeys
            var sizeToKeys = new Dictionary<string, List<object>>();
            foreach (var pair in driveKeys)
            {
                foreach (object googleObject in pair.Value)
                {
                    var googleFile = (File)googleObject;
                    long size = googleFile.FileSize.HasValue ? googleFile.FileSize.Value : 0;
                    var sizeKey = size.ToString() + "&&" + googleFile.OriginalFilename;
                    if (!sizeToKeys.ContainsKey(sizeKey))
                        sizeToKeys.Add(sizeKey, new List<object>());
                    sizeToKeys[sizeKey].Add(pair.Key);
                }
            }

            //Get key by size
            PhotoAccessor photo = new PhotoAccessor(picasaEntry);
            var photoKey = photo.Size.ToString() + "&&" + photo.PhotoTitle;

            if (sizeToKeys.ContainsKey(photoKey) && sizeToKeys[photoKey].Count == 1)
                return sizeToKeys[photoKey][0];
            return null;
        }
        
        /*
                public static object GetImageSizeOrMovieLength(PicasaEntry picasaEntry, bool useExif, Dictionary<object, List<object>> driveKeys)
                {

                    if (useExif)
                    {
                        if (((PicasaEntry)picasaEntry).Exif != null)
                        {
                            if (picasaEntry.Exif.Time != null)
                            {
                                var timeStr = ((PicasaEntry)picasaEntry).Exif.Time.Value;
                                long value;
                                if (!String.IsNullOrWhiteSpace(timeStr) && Int64.TryParse(timeStr, out value))
                                {
                                    var time = new DateTime(1970, 1, 1, 0, 0, 0);
                                    time = time.AddMilliseconds(value);
                                    return time.ToString();
                                }
                            }
                        }
                    }

                    //Picasa API do not have video metadata.
                    //Try to match video by size (size on google drive are the same as on picasa)

                    //Make dictionary Size -> Keys from driveKeys
                    var sizeToKeys = new Dictionary<string, List<object>>();
                    foreach (var pair in driveKeys)
                    {
                        foreach (object googleObject in pair.Value)
                        {
                            var googleFile = (File)googleObject;
                            long size = googleFile.FileSize.HasValue ? googleFile.FileSize.Value : 0;
                            var sizeKey = size.ToString() + "&&" + googleFile.OriginalFilename;
                            if (!sizeToKeys.ContainsKey(sizeKey))
                                sizeToKeys.Add(sizeKey, new List<object>());
                            sizeToKeys[sizeKey].Add(pair.Key);
                        }
                    }

                    //Get key by size
                    PhotoAccessor photo = new PhotoAccessor(picasaEntry);
                    var photoKey = photo.Size.ToString() + "&&" + photo.PhotoTitle;

                    if (sizeToKeys.ContainsKey(photoKey) && sizeToKeys[photoKey].Count == 1)
                        return sizeToKeys[photoKey][0];

                    return null;
                }*/



        public static object GetExifImageDate(File googleFile)
        {
            if (googleFile.ImageMediaMetadata != null)
            {
                if (!String.IsNullOrWhiteSpace(googleFile.ImageMediaMetadata.Date) && googleFile.ImageMediaMetadata.Date!="0000:00:00 00:00:00")
                {
                    var dateStr = googleFile.ImageMediaMetadata.Date;
                    var sp = dateStr.Split(':');
                    if (sp.Length != 5)
                        throw new Exception("Wrong google date '" + dateStr + "'"); //Wrong exif format
                    return ExifDateToDateTime(dateStr);
                }
                //No data. Will use size
            }
            return null;
        }
        

        public static object GetImageSizeOrMovieLength(File googleFile)
        {
            if (googleFile.ImageMediaMetadata != null)
            {
                //No data. Will use size
                var height = googleFile.ImageMediaMetadata.Height;
                var width = googleFile.ImageMediaMetadata.Width;

                if (width != null && height != null && width != 0 && height != 0 &&
                        width < 4000 && height < 4000) //Google usually transforms images more the 4000 pixels
                    return width.ToString() + "&&" + height.ToString();
            } 
            else if (googleFile.VideoMediaMetadata != null && ((File)googleFile).VideoMediaMetadata.DurationMillis != null)
            {
                double duration = googleFile.VideoMediaMetadata.DurationMillis.Value / 1000.0;
                return duration;
            }
            return null;
        }



        public static DateTime? GetMinDateKey(string filePath)
        {
            lock (_diskReadLock)
            {

                var creationDate = System.IO.File.GetCreationTime(filePath);
                var fi = new System.IO.FileInfo(filePath);
                if (creationDate > fi.LastWriteTime) //Get min among creation and modified date
                    creationDate = fi.LastWriteTime;
                return creationDate;
            }
        }


        public static DateTime? GetMinDateKey(File googleFile)
        {
            return googleFile.CreatedDate;
        }

        public static object GetMinDateKey(PicasaEntry picasaEntry, Dictionary<object, List<object>> driveKeys)
        {
            var photo = new PhotoAccessor(picasaEntry);

            /*
            var value = photo.Timestamp;
            var time = DateTime.FromFileTimeUtc((long)value);
                
            var tmp = new DateTime(1970, 1, 1, 0, 0, 0);
            tmp = tmp.AddMilliseconds(value);
            
            return time;*/

            //Picasa API do not have video metadata.
            //Try to match video by size (size on google drive are the same as on picasa)

            //Make dictionary Size -> Keys from driveKeys
            var sizeToKeys = new Dictionary<string, List<object>>();
            foreach (var pair in driveKeys)
            {
                foreach (object googleObject in pair.Value)
                {
                    var googleFile = (File)googleObject;
                    long size = googleFile.FileSize.HasValue ? googleFile.FileSize.Value : 0;
                    var sizeKey = size.ToString() + "&&" + googleFile.OriginalFilename;
                    if (!sizeToKeys.ContainsKey(sizeKey))
                        sizeToKeys.Add(sizeKey, new List<object>());
                    sizeToKeys[sizeKey].Add(pair.Key);
                }
            }

            //Get key by size
            var photoKey = photo.Size.ToString() + "&&" + photo.PhotoTitle;

            if (sizeToKeys.ContainsKey(photoKey) && sizeToKeys[photoKey].Count == 1)
                return sizeToKeys[photoKey][0];

            return null;

        }

        
        private static double? GetVideoDuration(string file)
        {
            ShellFile so = ShellFile.FromFilePath(file);
            double nanoseconds = 0;
            if (so.Properties.System == null || so.Properties.System.Media == null || so.Properties.System.Media.Duration == null)
                return null;
            double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds);            
            return nanoseconds * 0.00001 / 100.0; //*0.0001
        }


        public static bool IsImage(string filePath)
        {
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                var isImage = IsImage(fs);
                fs.Close();
                return isImage;
            }
        }
        
        private static bool IsImage(System.IO.FileStream stream)
        {
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    return true;
                }
            }
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return false;
        }

        
        


       

    }
}
