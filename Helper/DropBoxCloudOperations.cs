using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class DropBoxCloudOperations
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Upload file to drop box
        /// </summary>
        /// <param name="filePath">path to file</param>
        public static void CreateFile(string filename, string filePath)
        {
            try
            {
                log.Debug("Upload file " + filename + " to DropBox");
                DropBoxBase dbbase = new DropBoxBase("4ne2tmaa3dnzpry", "4yw9hbnnjcp21ei");
                dbbase.Delete("/" + filename);
                bool ret = dbbase.Upload("", filename, filePath);
                log.Debug("File " + filename + " uploaded");
            }
            catch (Exception e) { log.Error("Uploading file to dropbox failed ",e); }

        }
    }
}
