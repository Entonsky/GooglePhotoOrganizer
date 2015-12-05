using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.GData.Photos;
using Google.GData.Client;
using System.Security.Cryptography.X509Certificates;
using Google.Picasa;


namespace GooglePhotoOrganizer
{
    class PicasaClient
    {

        public PicasaClient()
        {
            GetPicasaService();
        }

        PicasaService _picasaService = null;


        public PicasaService GetPicasaService()
        {
            if (_picasaService == null)
            {
                /*
                const string ServiceAccountEmail = "account-1@praxis-magnet-114019.iam.gserviceaccount.com";
                var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);
                var serviceAccountCredentialInitializer =
                new ServiceAccountCredential.Initializer(ServiceAccountEmail)
                {
                    Scopes = new[] { "https://picasaweb.google.com/data/" }
                }.FromCertificate(certificate);
                var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);
                if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                   throw new InvalidOperationException("Access token request failed.");


                var requestFactory = new GDataRequestFactory(null);
                requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);
                requestFactory.CustomHeaders.Add("Gdata-version: 2");
                PicasaService service = new PicasaService("api-project");
                service.RequestFactory = requestFactory;
 
                PhotoQuery query = new PhotoQuery(PicasaQuery.CreatePicasaUri(_IdUsuari, _albumid));
                PicasaFeed feed = service.Query(query);*/


                var creditals = GoogleDriveClient.GetCreditals();

                var requestFactory = new GDataRequestFactory(null);
                requestFactory.CustomHeaders.Add("Authorization: Bearer " + creditals.Token.AccessToken);
                requestFactory.CustomHeaders.Add("Gdata-version: 2");
                _picasaService = new PicasaService("api-project");
                _picasaService.RequestFactory = requestFactory;
            }
            return _picasaService;
        }



        private List<PicasaEntry> GetItems(string uri, PicasaQuery.Kinds kind, string searchText = null)
        {
            var service = GetPicasaService();
            KindQuery query;
            switch (kind)
            {
                case PicasaQuery.Kinds.album:
                    query = new AlbumQuery(uri);
                    break;
                case PicasaQuery.Kinds.photo:
                    query = new PhotoQuery(uri);
                    if (!String.IsNullOrWhiteSpace(searchText))
                        query.Query = searchText;
                    break;
                default:
                    throw new NotImplementedException("Unknown kind");
            }

            query.NumberToRetrieve = 100;
            
            PicasaFeed feed = service.Query(query);
            
            var result = new List<PicasaEntry>();
            while (feed.Entries!=null)
            {
                foreach (PicasaEntry entry in feed.Entries)
                    result.Add(entry);

                if (String.IsNullOrWhiteSpace(feed.NextChunk))
                    break;

                switch (kind)
                {
                    case PicasaQuery.Kinds.album:
                        query = new AlbumQuery(feed.NextChunk);
                        break;
                    case PicasaQuery.Kinds.photo:
                        query = new PhotoQuery(feed.NextChunk);
                        break;
                    default:
                        throw new NotImplementedException("Unknown kind");
                }
                feed = service.Query(query);
            }
            return result;
       }



        public List<AlbumAccessor> GetAlbums()
        {
            string uri = PicasaQuery.CreatePicasaUri("default");
            var data = GetItems(uri, PicasaQuery.Kinds.album);

            var result = new List<AlbumAccessor>();
            foreach (PicasaEntry entry in data)
            {
                var album = new AlbumAccessor(entry);
                result.Add(album);
            }
            return result;
        }

        
        public List<PicasaEntry> GetPhotos(string albumId = null, string searchText = null)
        {
            string uri;
            if (albumId == null)
                uri = PicasaQuery.CreatePicasaUri("default");
            else
                uri = PicasaQuery.CreatePicasaUri("default", albumId);
            
            var data = GetItems(uri, PicasaQuery.Kinds.photo, searchText);

            var result = new List<PicasaEntry>();
            foreach (var item in data)
                result.Add(item);
            return result;  
        }
        

        public AlbumAccessor CreateAlbum(string title, string description = null, bool isPublic = false)
        {
            var service = GetPicasaService();

            AlbumEntry newEntry = new AlbumEntry();
            newEntry.Title.Text = title;
            if (description!=null)
                newEntry.Summary.Text = description;
            AlbumAccessor ac = new AlbumAccessor(newEntry);

            //set to "private" for a private album
            if (isPublic)
                ac.Access = "public";
            else
                ac.Access = "unlisted";

            Uri feedUri = new Uri(PicasaQuery.CreatePicasaUri("default"));

            PicasaEntry createdEntry = (PicasaEntry)service.Insert(feedUri, newEntry);
            return new AlbumAccessor(createdEntry); 
        }
        
        public void MovePhotoToAlbum(PicasaEntry photo, string newAlbumId)
        {
            PhotoAccessor ac = new PhotoAccessor(photo);
            ac.AlbumId = newAlbumId;
            var a = DateTime.Now.AddYears(-1);
            ac.Timestamp = (ulong)a.Ticks;

            PicasaEntry updatedEntry = (PicasaEntry)photo.Update();
        }
        

        public void UploadPhoto(string filePath, string albumId = "default")
        {
            var service = GetPicasaService();
            Uri postUri = new Uri(PicasaQuery.CreatePicasaUri("default", albumId));
            //postUri = new Uri("https://www.googleapis.com/resumable/upload/plus/v1whitelisted/mediasets/me.cinstant/mediaBackground?uploadType=resumable&imageSize=1024&mediaType=photo&storage=**standard**&remainingMediaCount=111");

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            System.IO.FileStream fileStream = fileInfo.OpenRead();

            PicasaEntry entry = (PicasaEntry)service.Insert(postUri, fileStream, "image/jpeg", filePath);

            //fullRes
            ///resumable/upload/plus/v1whitelisted/mediasets/me.cinstant/mediaBackground?uploadType=resumable&imageSize=1024&mediaType=photo&storage=**full**&remainingMediaCount=111

            //new Uri("https://picasaweb.google.com/data/feed/api/user/default/albumid/default");
            
            fileStream.Close();  
        }


        public PicasaEntry GetItemById(string id)
        {
            PicasaEntry e = new PicasaEntry();
            e.Id = new AtomId(id);
            
            return (PicasaEntry)e.Update();
        }


        public void UploadPhoto2(string filePath, string albumId = "default")
        {
            var service = GetPicasaService();
            //Uri postUri = new Uri(PicasaQuery.CreatePicasaUri("default", albumId));
            Uri postUri = new Uri("https://www.googleapis.com/resumable/upload/plus/v1whitelisted/mediasets/me.cinstant/mediaBackground?uploadType=resumable&imageSize=1024&mediaType=photo&storage=**standard**&remainingMediaCount=111");

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            System.IO.FileStream fileStream = fileInfo.OpenRead();

            PicasaEntry entry = (PicasaEntry)service.Insert(postUri, fileStream, "image/jpeg", filePath);

            //fullRes
            ///resumable/upload/plus/v1whitelisted/mediasets/me.cinstant/mediaBackground?uploadType=resumable&imageSize=1024&mediaType=photo&storage=**full**&remainingMediaCount=111

            //new Uri("https://picasaweb.google.com/data/feed/api/user/default/albumid/default");

            fileStream.Close();
        }


    }
}
