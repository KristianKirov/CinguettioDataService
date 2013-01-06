using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Requests;
using System.Collections.Generic;
using System.Net;
using System;

namespace CinguettioDataService
{
    public static class DriveServiceInvoker
    {
        public static File InsertFile(DriveService service, string mimeType, string filename, System.IO.Stream stream)
        {
            // File's metadata.
            File body = new File();
            body.Title = filename;
            body.MimeType = mimeType;
            body.Parents = new List<ParentReference>() { new ParentReference() { Id = "0BySX8GhV2wf8enhxSU56N25Rbnc" } };

            try
            {
                FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, mimeType);
                request.Upload();

                File file = request.ResponseBody;
                // Uncomment the following line to print the File ID.
                // Console.WriteLine("File ID: " + file.Id);

                return file;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
        }
    }
}