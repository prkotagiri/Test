using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Linq;
using System.Security;

namespace SharePointFileDownlaload
{
    class Program
    {
        static void Main(string[] args)
        {
            string UserName = "";
           

            string Pwd = "";
            

            string drive = "D:\\Test";
            
            try
            {
                DownloadFilesFromSharePoint("Test", UserName, Pwd, drive);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            Console.ReadLine();
        }

        private static void DownloadFilesFromSharePoint(string folderName, string UserName, string Pwd, string driveName)
        {
            //Load Libraries from SharePoint
            ClientContext ctxSite = GetSPOContext(UserName, Pwd);
            ctxSite.Load(ctxSite.Web.Lists);
            ctxSite.ExecuteQuery();

            Web web = ctxSite.Web;
            var docLibs = ctxSite.LoadQuery(web.Lists.Where(l => l.BaseTemplate == 101));  //DocumentLibrary only
            ctxSite.ExecuteQuery();

            foreach (var list in docLibs)
            {
                //Console.WriteLine(list.Title);
                ctxSite.Load(list.RootFolder.Folders);
                ctxSite.ExecuteQuery();

                string listTitle = list.Title;


                //Console.WriteLine("List Tile ------------------------------- " + listTitle);
                foreach (Folder folder in list.RootFolder.Folders)
                {
                    ctxSite.Load(folder.Files);
                    ctxSite.ExecuteQuery();

                    if (String.Equals(folder.Name, folderName, StringComparison.OrdinalIgnoreCase))
                    {
                        var folderDestination = driveName + @":\Test\SharePoint\" + listTitle + @"\" + folderName + @"\";
                        ctxSite.Load(folder.Files);
                        ctxSite.ExecuteQuery();

                        foreach (var file in folder.Files)
                        {
                            var fileName = Path.Combine(folderDestination, file.Name);
                            if (!System.IO.File.Exists(fileName))
                            {
                                Directory.CreateDirectory(folderDestination);
                                var fileRef = file.ServerRelativeUrl;
                                var fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(ctxSite, fileRef);
                                using (var fileStream = System.IO.File.Create(fileName))
                                {
                                    fileInfo.Stream.CopyTo(fileStream);
                                }
                            }
                        }
                        Console.WriteLine("Downloaded the file in " + folderDestination);
                    }

                }

            }

        }

        private static ClientContext GetSPOContext(string UserName, string Pwd)
        {

            string spsiteurl = "https://evokemail.sharepoint.com/sites/Microsoft-Devops/";

            var secure = new SecureString();
            foreach (char c in Pwd)
            {
                secure.AppendChar(c);
            }
            ClientContext spoContext = new ClientContext(spsiteurl);
            spoContext.Credentials = new SharePointOnlineCredentials(UserName, secure);
            return spoContext;

        }

        private static void GetAllItemNamesInSP(string UserName, string Pwd)
        {
            //Load Libraries from SharePoint
            ClientContext ctxSite = GetSPOContext(UserName, Pwd);
            ctxSite.Load(ctxSite.Web.Lists);
            ctxSite.ExecuteQuery();

            foreach (List list in ctxSite.Web.Lists)
            {
                string nameTest = list.Title;
                string testVal = list.BaseType.ToString();
                Console.WriteLine(nameTest + " -------------- " + testVal);
                if (list.BaseType.ToString() == "DocumentLibrary")
                {
                }

            }
            Console.ReadLine();
        }
    }
}
