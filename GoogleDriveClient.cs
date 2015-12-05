using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Google.Apis.Plus.v1domains;


namespace GooglePhotoOrganizer
{
    class GoogleDriveClient
    {

        public GoogleDriveClient()
        {
            GetDriveService();
        }

        string _creditalFile = null;

        public string CreditalFile
        {
            get { return _creditalFile; }
        }
        
        static string[] Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile, "https://picasaweb.google.com/data/",
         //DriveService.Scope.DrivePhotosReadonly,
         "https://www.googleapis.com/auth/plus.stream.write",
         "https://www.googleapis.com/auth/plus.me",
         "https://www.googleapis.com/auth/plus.media.upload"
        //PlusDomainsService.Scope.PlusMe,
        //PlusDomainsService.Scope.PlusMediaUpload
        //"https://www.googleapis.com/resumable/upload/plus/v1whitelisted/mediasets/me.cinstant",
        //"https://www.googleapis.com/resumable/upload/plus/v1whitelisted/mediasets",
        //"https://www.googleapis.com/resumable/upload/plus/v1whitelisted",
        //"https://www.googleapis.com/resumable/upload/plus",
        //"https://www.googleapis.com/resumable/upload",
        //  "https://www.googleapis.com/resumable"
        };

        DriveService _driveService = null;


        public static UserCredential GetCreditals()
        {
            UserCredential credential;

            string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
            using (var stream =
                new System.IO.FileStream("client_secret.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {

                credPath = System.IO.Path.Combine(credPath, ".credentials\\drive-dotnet-quickstart");
                /*if (System.IO.Directory.Exists(credPath))
                {
                    System.IO.Directory.Delete(credPath);
                }*/

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    System.Threading.CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //_creditalFile = credPath;
            }
            return credential;
        }


        private DriveService GetDriveService()
        {
            if (_driveService == null)
            {
                UserCredential credential;

                string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                using (var stream =
                    new System.IO.FileStream("client_secret.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {

                    credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        System.Threading.CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    _creditalFile = credPath;
                }


                // Create Drive API service.
                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Test",
                });
            }

            return _driveService;

        }
        
        const string directoryMime = "application/vnd.google-apps.folder";
        

        private List<File> GetFilesOrDirectories(bool filesTrueOrDirsFalse, string title = null, string parentId = null)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = GetDriveService().Files.List();
            if (filesTrueOrDirsFalse)
                listRequest.Q = "mimeType!='" + directoryMime + "'";
            else
                listRequest.Q = "mimeType='" + directoryMime + "'";
            if (title!= null)
                listRequest.Q+= " and title='" + title + "'";
            if (parentId != null)
                listRequest.Q += " and '" + parentId + "' in parents";

            listRequest.MaxResults = 1000;
            
            // List files.
            var folderFeed = listRequest.Execute();
                        
            var result = new List<File>();
            //// Loop through until we arrive at an empty page
            while (folderFeed.Items != null)
            {
                // Adding each item  to the list.
                foreach (var folder in folderFeed.Items)
                    result.Add(folder);
                

                // We will know we are on the last page when the next page token is
                // null.
                // If this is the case, break.
                if (folderFeed.NextPageToken == null)
                {
                    break;
                }
                //break;//Костыль - удалить

                // Prepare the next page of results
                listRequest.PageToken = folderFeed.NextPageToken;

                // Execute and process the next page request
                folderFeed = listRequest.Execute();
            }
            
            return result;
        }


        public List<File> GetDirectories(string title = null, string parentId = null)
        {
            return GetFilesOrDirectories(false, title, parentId);
        }

        public List<File> GetFiles(string title = null, string parentId = null)
        {
            return GetFilesOrDirectories(true, title, parentId);
        }

        public bool DirectoryExists(string title = null, string parentId = null)
        {
            List<File> files = new List<File>();
            files = GetDirectories(title, parentId);
            
            return files.Count > 0;
        }


        public bool DirectoryExists(string title, string parentId, out File firstDirectory)
        {
            List<File> files = new List<File>();
            files = GetDirectories(title, parentId);
            
            if (files.Count > 0)
                firstDirectory = files[0];
            else
                firstDirectory = null;
            return files.Count > 0;
        }
        
        
        public File CreateDirectory(string title, string parentId = "root" , string description = null)
        {
            var folder = new File();
            folder.Title = title;
            folder.Description = "document description";
            folder.MimeType = "application/vnd.google-apps.folder";
            folder.Parents = new List<ParentReference>() { new ParentReference() { Id = parentId } };

            // service is an authorized Drive API service instance
            return GetDriveService().Files.Insert(folder).Execute();
        }


        public File CreateCascadeDirectory(string relativePath, string parentId = "root")
        {
            var split = relativePath.Split(new string[] { "\\" }, StringSplitOptions.None);

            File curDirectory = new File();
            curDirectory.Id = parentId;

            foreach (var sp in split)
            {
                File newDir;
                if (!DirectoryExists(sp, curDirectory.Id, out newDir))
                    newDir = CreateDirectory(sp, curDirectory.Id);
                curDirectory = newDir;
            }
            return curDirectory;
        }


        public File MoveFileToDirectory(string fileId, string newParentId)
        {
            File file = new File();
            file.Parents = new List<ParentReference>() { new ParentReference() { Id = newParentId } };
            FilesResource.PatchRequest request = GetDriveService().Files.Patch(file, fileId);
            return request.Execute();
        }


        public File SetCreationDate(string fileId, DateTime createdDate)
        {
            File file = new File();
            file.CreatedDate = createdDate;
            FilesResource.PatchRequest request = GetDriveService().Files.Patch(file, fileId);
            return request.Execute();
        }


        public About GetAbout()
        {
            return GetDriveService().About.Get().Execute();
        }


    }
}
