using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;
using Google.GData.Photos;
using Google.Apis.Drive.v2.Data;

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


        delegate string ExtractKeyDelegate(object item);
        static Dictionary<string, List<object>> MakeKeyDict(IEnumerable<object> list, ExtractKeyDelegate KeyExtractor)
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


        delegate string ExtractKeyDelegate2(object item, Dictionary<string, List<object>> exifGoogle);
        static Dictionary<string, List<object>> MakeKeyDict(IEnumerable<object> list, Dictionary<string, List<object>> exifGoogle, ExtractKeyDelegate2 KeyExtractor)
        {
            var result = new Dictionary<string, List<object>>();
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
            var result = new Dictionary<string, MatchedFiles>();

            var localNotMatched = new HashSet<FileDesc>(localFiles);
            var googleNotMatched = new HashSet<File>(googleFiles);
            var picasaNotMatched = new HashSet<PicasaEntry>(picasaFiles);

            //Match by exif date and video size
            MatchFilesWithTheSameNameByType(result,
                localNotMatched, googleNotMatched, picasaNotMatched,
                (object file) =>
                {
                    return MediaKeyMatcher.GetExifImageDateOrMovieLength(((FileDesc)file).path);
                },
                (object file) =>
                {
                    return MediaKeyMatcher.GetExifImageDateOrMovieLength((File)file);
                },
                (object file, Dictionary<string, List<object>> exifGoogle) =>
                {
                    return MediaKeyMatcher.GetExifImageDateOrMovieLength((PicasaEntry)file, true, exifGoogle);
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
                (object file, Dictionary<string, List<object>> exifGoogle) =>
                {
                    return MediaKeyMatcher.GetMinDateKey((PicasaEntry)file, exifGoogle);
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

        private static void MatchFilesWithTheSameNameByType(Dictionary<string, MatchedFiles> result, 
            HashSet<FileDesc> localFiles, HashSet<File> googleFiles, HashSet<PicasaEntry> picasaFiles,
            ExtractKeyDelegate localExtractor, ExtractKeyDelegate googleExtractor, ExtractKeyDelegate2 picasaExtractor)
        { 
            
            //Get different localfile keys
            var exifLocal = MakeKeyDict(localFiles, localExtractor);
            var exifGoogle = MakeKeyDict(googleFiles, googleExtractor);
            var exifPicasa = MakeKeyDict(picasaFiles, exifGoogle, picasaExtractor);
            

            //Try matching by exif
            foreach (var pair in exifLocal)
            {
                if (String.IsNullOrWhiteSpace(pair.Key))
                    continue;
                if (pair.Value.Count > 1)
                    continue; //Bad matching - many file in this basked
                if (!exifGoogle.ContainsKey(pair.Key) && !exifPicasa.ContainsKey(pair.Key))
                    continue; //Nothing to match
                if (!result.ContainsKey(pair.Key))
                    result.Add(pair.Key, new MatchedFiles());

                foreach (FileDesc fileDesc in pair.Value)
                    result[pair.Key].localFiles.Add(fileDesc);
                 
                if (exifGoogle.ContainsKey(pair.Key) && exifPicasa.ContainsKey(pair.Key))
                {
                    //Delete if both matched
                    foreach (FileDesc fileDesc in pair.Value)
                        localFiles.Remove(fileDesc);
                }

                if (exifGoogle.ContainsKey(pair.Key))
                {
                    foreach (File file in exifGoogle[pair.Key])
                    {
                        result[pair.Key].googleFiles.Add(file);
                        googleFiles.Remove(file);
                    }
                }
                if (exifPicasa.ContainsKey(pair.Key))
                {
                    foreach (PicasaEntry file in exifPicasa[pair.Key])
                    {
                        result[pair.Key].picasaFiles.Add(file);
                        picasaFiles.Remove(file);
                    }
                }
            }
        }



        static object _diskReadLock = new Object();

        //Keydata for file
        public static string GetExifImageDateOrMovieLength(string filePath)
        {
            lock (_diskReadLock)
            {
                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                if (IsImage(fs))
                {
                    try
                    {
                        BitmapSource img = BitmapFrame.Create(fs);
                        BitmapMetadata md = (BitmapMetadata)img.Metadata;
                        string date = md.DateTaken;
                        if (!String.IsNullOrWhiteSpace(date))
                            return ExifDateToGoogleFormat(date);
                        //null date. Try width and height
                        if (img.Width != 0 && img.Height != 0) //Google usually transforms images more the 4000 pixels
                            return img.Width.ToString() + "&&" + img.Height.ToString();
                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                }

                try
                {
                    var videoDuration = GetMediaParams(filePath);
                    return videoDuration.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public static string GetMinDateKey(string filePath)
        {
            lock (_diskReadLock)
            {

                var creationDate = System.IO.File.GetCreationTime(filePath);
                var fi = new System.IO.FileInfo(filePath);
                if (creationDate > fi.LastAccessTime) //Get min among creation and modified date
                    creationDate = fi.LastWriteTime;
                return ExifDateToGoogleFormat(creationDate.ToString());
            }
        }


        public static string GetMinDateKey(File googleFile)
        {
            return ExifDateToGoogleFormat(googleFile.CreatedDate.ToString());
        }

        public static string GetMinDateKey(PicasaEntry picasaEntry, Dictionary<string, List<object>> driveKeys)
        {
            var photo = new PhotoAccessor(picasaEntry);
            /*
            
            var value = photo.Timestamp;
            var time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            time = time.AddMilliseconds(value);
            return MediaKeyMatcher.ExifDateToGoogleFormat(time.ToString());*/

            //Picasa API do not have video metadata.
            //Try to match video by size (size on google drive are the same as on picasa)

            //Make dictionary Size -> Keys from driveKeys
            var sizeToKeys = new Dictionary<string, List<string>>();
            foreach (var pair in driveKeys)
            {
                foreach (object googleObject in pair.Value)
                {
                    var googleFile = (File)googleObject;
                    long size = googleFile.FileSize.HasValue ? googleFile.FileSize.Value : 0;
                    var sizeKey = size.ToString() + "&&" + googleFile.OriginalFilename;
                    if (!sizeToKeys.ContainsKey(sizeKey))
                        sizeToKeys.Add(sizeKey, new List<string>());
                    sizeToKeys[sizeKey].Add(pair.Key);
                }
            }

            //Get key by size
            var photoKey = photo.Size.ToString() + "&&" + photo.PhotoTitle;

            if (sizeToKeys.ContainsKey(photoKey) && sizeToKeys[photoKey].Count == 1)
                return sizeToKeys[photoKey][0];

            return null;

        }
        

        public static long GetFileSize(string filePath)
        {
            lock (_diskReadLock)
            {
                var f = new System.IO.FileInfo(filePath);
                return f.Length;
            }
        }



        public static string GetExifImageDateOrMovieLength(PicasaEntry picasaEntry, bool useExif, Dictionary<string, List<object>> driveKeys)
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
                            return MediaKeyMatcher.ExifDateToGoogleFormat(time.ToString());
                        }
                    }
                }
            }

            //Picasa API do not have video metadata.
            //Try to match video by size (size on google drive are the same as on picasa)

            //Make dictionary Size -> Keys from driveKeys
            var sizeToKeys = new Dictionary<string, List<string>>();
            foreach (var pair in driveKeys)
            {
                foreach (object googleObject in pair.Value)
                {
                    var googleFile = (File)googleObject;
                    long size = googleFile.FileSize.HasValue?googleFile.FileSize.Value:0;
                    var sizeKey = size.ToString() + "&&" + googleFile.OriginalFilename;
                    if (!sizeToKeys.ContainsKey(sizeKey))
                        sizeToKeys.Add(sizeKey, new List<string>());
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


        public static string GetExifImageDateOrMovieLength(File googleFile)
        {
            if (googleFile.ImageMediaMetadata != null)
            {
                if (!String.IsNullOrWhiteSpace(googleFile.ImageMediaMetadata.Date))
                    return googleFile.ImageMediaMetadata.Date;
                //No data. Will use size
                var height = googleFile.ImageMediaMetadata.Height;
                var width = googleFile.ImageMediaMetadata.Width;

                if (width != null && height != null && width != 0 && height != 0 &&
                        width < 4000 && height < 4000) //Google usually transforms images more the 4000 pixels
                    return width.ToString() + "&&" + height.ToString();
            }

            if (googleFile.VideoMediaMetadata != null && ((File)googleFile).VideoMediaMetadata.DurationMillis != null)
            {
                long duration = (long)Math.Round(googleFile.VideoMediaMetadata.DurationMillis.Value / 10.0);
                return duration.ToString();
            }
            return null;
        }



        private static long? GetMediaParams(string file)
        {
            ShellFile so = ShellFile.FromFilePath(file);
            double nanoseconds = 0;
            if (so.Properties.System == null || so.Properties.System.Media == null || so.Properties.System.Media.Duration == null)
                return null;
            double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds);
            return (long)Math.Ceiling(nanoseconds * 0.00001); //*0.0001
        }



        public static string ExifDateToGoogleFormat(string date)
        {
            DateTime dateParsed;
            if (DateTime.TryParse(date, out dateParsed))
                date = dateParsed.ToString("yyyy:MM:dd HH:mm:ss");
            return date;
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
