using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Google.Apis.Drive.v2;

namespace CinguettioDataService
{
    /// <summary>
    /// Summary description for UploadImage
    /// </summary>
    public class UploadImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];
                file.InputStream.Position = 0;

                DriveService ds = DriveServiceFactory.BuildService();

                var uploadedFile = DriveServiceInvoker.InsertFile(ds, file.ContentType, file.FileName, file.InputStream);
                
                context.Response.Write(string.Format("https://googledrive.com/host/0BySX8GhV2wf8enhxSU56N25Rbnc/{0}", file.FileName));
            }
            else
            {
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}