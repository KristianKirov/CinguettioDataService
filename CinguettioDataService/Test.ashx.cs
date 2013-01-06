using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinguettioDataService
{
    /// <summary>
    /// Summary description for Test
    /// </summary>
    public class Test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string filePath = context.Request["f"];
            if (!string.IsNullOrEmpty(filePath))
            {
                string filePathFull = context.Server.MapPath(filePath);
                context.Response.Write(filePathFull);
                context.Response.Write(Environment.NewLine);
                context.Response.Write(System.IO.File.Exists(filePathFull));
            }

            string directoryPath = context.Request["d"];
            if (!string.IsNullOrEmpty(directoryPath))
            {
                context.Response.Write(Environment.NewLine);
                string directoryPathFull = context.Server.MapPath(directoryPath);
                context.Response.Write(directoryPathFull);
                context.Response.Write(Environment.NewLine);
                context.Response.Write(System.IO.Directory.Exists(directoryPathFull));
            }

            string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (!string.IsNullOrEmpty(dataDirectory))
            {
                context.Response.Write(Environment.NewLine);
                string dataDirectoryFull = dataDirectory;
                context.Response.Write(dataDirectoryFull);
                context.Response.Write(Environment.NewLine);
                context.Response.Write(System.IO.Directory.Exists(dataDirectoryFull));
                context.Response.Write(Environment.NewLine);

                string file = System.IO.Path.Combine(dataDirectoryFull, "4cdbb3038252b4cc8a2e2799e402d1f99b0d15ad-privatekey.p12");
                context.Response.Write(file);
                context.Response.Write(Environment.NewLine);
                context.Response.Write(System.IO.File.Exists(file));

                context.Response.Write(Environment.NewLine);
                context.Response.Write(Environment.NewLine);

                foreach (string fp in System.IO.Directory.GetFiles(dataDirectoryFull))
                {
                    context.Response.Write(fp);

                    context.Response.Write(Environment.NewLine);
                }
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