﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CefSharp;

namespace DropMeter.CEF
{
    public class LocalFileHandler : ResourceHandler
    {
        // Specifies where you bundled app resides.
        // Basically path to your index.html
        private string frontendFolderPath;

        public LocalFileHandler(string path)
        {
            frontendFolderPath = path; //Path.Combine(App.BASE, "./bundle/");
        }

        // Process request and craft response.
        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;
            var filematch = new Regex("\\.(.{1,5})");
            if (!filematch.IsMatch(fileName))
            {
                fileName = "index.html";
            }
            var requestedFilePath = (frontendFolderPath + "\\" + uri.Host + "\\" + fileName).Split('?')[0];
            var clearURL = new Regex("index.html(.*)");
            requestedFilePath = clearURL.Replace(requestedFilePath, "index.html");

            
            
            var isAccesToFilePermitted = IsRequestedPathInsideFolder(
                new DirectoryInfo(requestedFilePath),
                new DirectoryInfo(frontendFolderPath));
            bool fileExists = File.Exists(requestedFilePath);
            if (isAccesToFilePermitted && fileExists)
            {
                byte[] bytes = File.ReadAllBytes(requestedFilePath);
                Stream = new MemoryStream(bytes);

                var fileExtension = Path.GetExtension(fileName);
                MimeType = Cef.GetMimeType(fileExtension);

                callback.Continue();
                return CefReturnValue.Continue;
            }

            callback.Dispose();
            return CefReturnValue.Cancel;
        }

        // Added for security reasons.
        // In this code it is used to verify that requested file is descendant to your index.html.
        public bool IsRequestedPathInsideFolder(DirectoryInfo path, DirectoryInfo folder)
        {
            /*if (path.Parent == null)
            {
                return false;
            }

            if (string.Equals(path.Parent.FullName, folder.FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return IsRequestedPathInsideFolder(path.Parent, folder);*/
            return (true); //TODO: Fix this.
        }
    }

    public class LocalFileHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "widgets";

        private string path;
        public LocalFileHandlerFactory(string path)
        {

            this.path = path;
        }
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new LocalFileHandler(path);
        }
    }
}
