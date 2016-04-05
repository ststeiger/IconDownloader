
namespace IconDownloader
{


    public class Scraoer
    {


        public static void DownloadFile(string remoteUri, string fileName)
        {
            // Create a new WebClient instance.
            using (System.Net.WebClient myWebClient = new System.Net.WebClient())
            {
                myWebClient.DownloadFile(remoteUri, fileName);
            }
        } // End Sub DownloadFile 


        public static string MapProjectPath(string path)
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
            string basePath = System.IO.Path.GetDirectoryName(ass.Location);
            basePath = System.IO.Path.Combine(basePath, "../../");
            path = System.IO.Path.Combine(basePath, path);
            return System.IO.Path.GetFullPath(path);
        } // End Function MapProjectPath 


        //<form name="pagination-form" id="" style="display: block;">
        // <span id="pagination-total">65</span>
        // <a href="http://www.flaticon.com/packs/2" class="pagination-next"></a>
        public static int GetMaxPage(HtmlAgilityPack.HtmlDocument doc)
        {
            int iMaxPage = -1;

            foreach (HtmlAgilityPack.HtmlNode span in doc.DocumentNode.SelectNodes("//span[@id=\"pagination-total\"]"))
            {
                bool b = System.Int32.TryParse(span.InnerText, out iMaxPage);
                System.Console.WriteLine(b);
            }

            return iMaxPage;
        }

        public static void Test()
        {
            string path = MapProjectPath("HTML/FlatIconMain.txt");
            System.Console.WriteLine(path);
            

            HtmlAgilityPack.HtmlWeb page = new HtmlAgilityPack.HtmlWeb();
            // HtmlAgilityPack.HtmlDocument doc = page.Load("http://www.flaticon.com/packs");
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(path);


            int iMaxPage = GetMaxPage(doc);


            // http://www.flaticon.com/packs/1
            // http://www.flaticon.com/packs/2
            // ...
            // http://www.flaticon.com/packs/65
            for (int i = 1; i <= iMaxPage; ++i)
            {
                doc = page.Load("http://www.flaticon.com/packs/" + i.ToString());
                // System.Console.WriteLine(doc.DocumentNode.OuterHtml);

                foreach (HtmlAgilityPack.HtmlNode link in doc.DocumentNode.SelectNodes("//article[@class=\"box\"]/a[@href]"))
                {
                    // System.Console.WriteLine(link);
                    HtmlAgilityPack.HtmlAttribute att = link.Attributes["href"];
                    System.Console.WriteLine(att.Value);
                    try
                    {
                        DownloadPack(att.Value);
                        System.Threading.Thread.Sleep(5000);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine("Error on page " + i.ToString());
                        System.Console.WriteLine(ex.Message);
                        System.Console.WriteLine("URL: " + att.Value);
                    }
                    
                } // Next link
            }

        } // End Sub Test


        public static void DownloadPack(string url)
        {
            string harvestPath = MapProjectPath("harvest");
            if (!System.IO.Directory.Exists(harvestPath))
                System.IO.Directory.CreateDirectory(harvestPath);




            // string path = MapProjectPath("HTML/flatIcon_lvl_1.txt");
            // string url = @"http://www.flaticon.com/packs/web-navigation-line-craft";
            HtmlAgilityPack.HtmlWeb page = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = page.Load(url);
            // HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            // doc.Load(path);



            // <section class="list-top">
            // <a href="http://file005.flaticon.com/packs/112154-web-navigation-line-craft.zip" class="btn pull-right track_download_pack" title="Download Pack" data-pack="112154">Download Pack <i class="flaticon-download"></i></a>
            foreach (HtmlAgilityPack.HtmlNode link in doc.DocumentNode.SelectNodes("//section[@class=\"list-top\"]/a[@href]"))
            {
                //System.Console.WriteLine(link);

                HtmlAgilityPack.HtmlAttribute att = link.Attributes["href"];
                string downloadLink = att.Value;

                System.Uri uri = new System.Uri(downloadLink, System.UriKind.Absolute);
                string fn = System.IO.Path.GetFileName(uri.AbsolutePath);
                fn = System.IO.Path.Combine(harvestPath, fn);

                System.Console.WriteLine("Downloading " + downloadLink + ".");
                DownloadFile(downloadLink, fn);
                System.Console.WriteLine("Finished downloading " + downloadLink + ".");
            }
        }


    } // End Class


} 