using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.IO;


namespace Helper
{
    public class DropBoxBase
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        #region Variables  
        private DropboxClient DBClient;
        #endregion

        #region Constructor  
        public DropBoxBase(string ApiKey, string ApiSecret, string ApplicationName = "385_fisk")
        {
            try
            {
                AppKey = ApiKey;
                AppSecret = ApiSecret;
                AppName = ApplicationName;
                DBClient = new DropboxClient("sl.BueT67SQi7txpXTRrGltWOoIIM7ojb5YqFhnb7432ZNNZ9A1Mrtk90DUkw1TgX4YRzDT7aS5T7y8a7K0fz6P4yYUOqkr70fuL5HGrNwbhe6sv4LL2SRiH5HR_T26S2NaQ9fYGLyBkv3CDaU");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Properties  
        public string AppName
        {
            get; private set;
        }
        public string AuthenticationURL
        {
            get; private set;
        }
        public string AppKey
        {
            get; private set;
        }

        public string AppSecret
        {
            get; private set;
        }

        public string AccessTocken
        {
            get; private set;
        }
        public string Uid
        {
            get; private set;
        }
        #endregion

        #region UserDefined Methods  


   

        /// <summary>  
        /// Method to create new folder on Dropbox  
        /// </summary>  
        /// <param name="path"> path of the folder we want to create on Dropbox</param>  
        /// <returns></returns>  
        public bool CreateFolder(string path)
        {
            try
            {
                if (AccessTocken == null)
                {
                    throw new Exception("AccessToken not generated !");
                }
                if (AuthenticationURL == null)
                {
                    throw new Exception("AuthenticationURI not generated !");
                }

                var folderArg = new CreateFolderArg(path);
                var folder = DBClient.Files.CreateFolderAsync(folderArg);
                var result = folder.Result;
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>  
        /// Method is to check that whether folder exists on Dropbox or not.  
        /// </summary>  
        /// <param name="path"> Path of the folder we want to check for existance.</param>  
        /// <returns></returns>  
        public bool FolderExists(string path)
        {
            try
            {
                if (AccessTocken == null)
                {
                    throw new Exception("AccessToken not generated !");
                }
                if (AuthenticationURL == null)
                {
                    throw new Exception("AuthenticationURI not generated !");
                }

                var folders = DBClient.Files.ListFolderAsync(path);
                var result = folders.Result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>  
        /// Method to delete file/folder from Dropbox  
        /// </summary>  
        /// <param name="path">path of file.folder to delete</param>  
        /// <returns></returns>  
        public bool Delete(string path)
        {
            try
            {

                var folders = DBClient.Files.DeleteAsync(path);
                var result = folders.Result;
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>  
        /// Method to upload files on Dropbox  
        /// </summary>  
        /// <param name="UploadfolderPath"> Dropbox path where we want to upload files</param>  
        /// <param name="UploadfileName"> File name to be created in Dropbox</param>  
        /// <param name="SourceFilePath"> Local file path which we want to upload</param>  
        /// <returns></returns>  
        public bool Upload(string UploadfolderPath, string UploadfileName, string SourceFilePath)

        {
            
            try
            {
                using (var stream = new MemoryStream(File.ReadAllBytes(SourceFilePath)))
                {
                    var response = DBClient.Files.UploadAsync(UploadfolderPath + "/" + UploadfileName, WriteMode.Overwrite.Instance, body: stream);
                    var rest = response.Result; //Added to wait for the result from Async method  
                }
                log.Debug("uploaded config file to dropbox "+ UploadfileName);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error in upoading to dropbox",ex);
                return false;
            }

        }

        /// <summary>  
        /// Method to Download files from Dropbox  
        /// </summary>  
        /// <param name="DropboxFolderPath">Dropbox folder path which we want to download</param>  
        /// <param name="DropboxFileName"> Dropbox File name availalbe in DropboxFolderPath to download</param>  
        /// <param name="DownloadFolderPath"> Local folder path where we want to download file</param>  
        /// <param name="DownloadFileName">File name to download Dropbox files in local drive</param>  
        /// <returns></returns>  
        public bool Download(string DropboxFolderPath, string DropboxFileName, string DownloadFolderPath, string DownloadFileName)
        {
            try
            {
                var response = DBClient.Files.DownloadAsync(DropboxFolderPath + "/" + DropboxFileName);
                var result = response.Result.GetContentAsStreamAsync(); //Added to wait for the result from Async method  

                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

    }
}