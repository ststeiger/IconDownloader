
using System.Windows.Forms;


namespace IconDownloader
{


    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }


        private void btnPost_Click(object sender, System.EventArgs e)
        {
            // Scraoer.Test();
            PostRequest();
        } // End Sub btnPost_Click


/*
<form id="download-form" method="post" action="/download-icon">
	<input type="hidden" id="icon_id" name="icon_id" value="118260" />
	<input type="hidden" id="author" name="author" value="137" />
	<input type="hidden" id="team" name="team" value="137" />
	<input type="hidden" id="keyword" name="keyword" value="pen" />
	<input type="hidden" id="pack" name="pack" value="118131" />
	<input type="hidden" id="style" name="style" value="3" />
	<input type="hidden" id="format" name="format" />
	<input type="hidden" id="color" name="color" />
	<input type="hidden" id="colored" name="colored" value="1"  />
	<input type="hidden" id="size" name="size" />
	<input type="hidden" id="selection" name="selection" value="1" />
	<!--<input type="hidden" id="svg_content" name="svg_content" />-->
</form>
*/


        // https://stackoverflow.com/questions/4015324/http-request-with-post
        public static void PostRequest()
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                System.Collections.Specialized.NameValueCollection values = new System.Collections.Specialized.NameValueCollection();
                values["icon_id"] = "109737";
                values["author"] = "1";
                values["team"] = "1";
                values["keyword"] = "buildings";
                values["pack"] = "112154";
                values["style"] = "3";
                values["format"] = "svg";
                values["color"] = "#D80027";
                values["colored"] = "1";
                values["size"] = "512";
                values["selection"] = "1";


                byte[] response = client.UploadValues("http://www.flaticon.com/download-icon", values);


                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(response))
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.XmlResolver = null;
                    doc.PreserveWhitespace = false;
                    doc.Load(ms);
                    System.Xml.XmlNamespaceManager nsmgr = GetReportNamespaceManager(doc);

                    System.Xml.XmlNodeList nl = doc.SelectNodes("//dft:g", nsmgr);
                    

                    if (nl != null)
                    {
                        System.Console.WriteLine(nl.Count);

                        foreach (System.Xml.XmlNode nd in nl)
                        {
                            System.Console.WriteLine(nd.InnerXml);
                            string strContent = nd.InnerXml.Trim(new char[] { '\r', '\n', ' ', '\t', '\v' });
                            if (string.IsNullOrEmpty(strContent))
                                nd.ParentNode.RemoveChild(nd);    
                        } // Next nd 

                    } // End if (nl != null)

                    System.Console.WriteLine(doc.OuterXml);
                    SaveDocument(doc, @"d:\mytestfile.svg");
                }

                string responseString = System.Text.Encoding.Default.GetString(response);
            }
        } // End Sub PostRequest


        public static void SaveDocument(System.Xml.XmlDocument origDoc, string strSavePath)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.PreserveWhitespace = true;

            string strXml = origDoc.OuterXml.Replace("xmlns=\"\"", "");
            strXml = System.Text.RegularExpressions.Regex.Replace(strXml, @"(\r\n?|\n)+", " ");
            doc.LoadXml(strXml);
            doc.PreserveWhitespace = true;

            // XElement.Parse(str).ToString(SaveOptions.DisableFormatting)

            using (System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(strSavePath, System.Text.Encoding.UTF8))
            {
                xtw.Formatting = System.Xml.Formatting.Indented; // if you want it indented
                xtw.Indentation = 4;
                xtw.IndentChar = ' ';

                // xtw.Formatting = System.Xml.Formatting.None; // if you want it indented
                // doc.PreserveWhitespace = true;
                // doc.PreserveWhitespace = false;



                doc.Save(xtw);
                xtw.Flush();
                xtw.Close();
            } // End Using xtw

            doc = null;
        } // End Sub SaveDocument


        public static System.Xml.XmlNamespaceManager GetReportNamespaceManager(System.Xml.XmlDocument doc)
        {
            if (doc == null)
                throw new System.ArgumentNullException("doc");

            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(doc.NameTable);

            if (doc.DocumentElement != null)
            {
                System.Xml.XPath.XPathNavigator xNav = doc.CreateNavigator();
                while (xNav.MoveToFollowing(System.Xml.XPath.XPathNodeType.Element))
                {
                    System.Collections.Generic.IDictionary<string, string> localNamespaces = xNav.GetNamespacesInScope(System.Xml.XmlNamespaceScope.Local);

                    foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in localNamespaces)
                    {
                        string prefix = kvp.Key;
                        if (string.IsNullOrEmpty(prefix))
                            prefix = "dft";

                        nsmgr.AddNamespace(prefix, kvp.Value);
                    } // Next kvp

                } // Whend

                return nsmgr;
            } // End if (doc.DocumentElement != null)

            nsmgr.AddNamespace("dft", "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");
            // nsmgr.AddNamespace("dft", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");

            return nsmgr;
        } // End Function GetReportNamespaceManager


    } // End Class Form1 : Form


} // End Using Namespace IconDownloader
