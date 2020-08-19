using System;
using SelectPdf;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace MccPdfLib
{
    public class H2P_Converter
    {
        public delegate void ErrorEventHandler(object sender, string e);
        public event ErrorEventHandler Error;
        protected virtual void OnError(string e) { if (Error != null) { Error(this, e); }; }

        SelectPdf.HtmlToPdf converter = new HtmlToPdf();
        SelectPdf.PdfDocument pdf = new SelectPdf.PdfDocument();
        List<SelectPdf.PdfDocument> pages_list = new List<SelectPdf.PdfDocument>(); //список включает в себя PDF сконвертированные из Html

        Dictionary<object, string> orientation_list = new Dictionary<object, string>
        {
            {PdfPageOrientation.Portrait,"Portrait"},
            {PdfPageOrientation.Landscape, "Landscape"}
        };

        public bool AddHtmlPagelist(XmlDocument xml)
        {
            try
            {
                XmlNodeList Pages = xml.SelectNodes("//Html_List//Page");
                int i = 0;
                foreach (XmlNode xNode in Pages)
                {
                    string Attribute_id = xNode.Attributes[0].InnerText;

                    pages_list.Add(converter.ConvertUrl(new System.Uri(xml.SelectSingleNode("//Html_List//Page[@id = '" + Attribute_id + "']//Path").InnerText).AbsoluteUri));                                                       //конвертация из Html в PDFDocument
                    pages_list[i].Pages[0].Orientation = (PdfPageOrientation)orientation_list.FirstOrDefault(x => x.Value == xml.SelectSingleNode("//Html_List//Page[@id = '" + Attribute_id + "']//Orintation").InnerText).Key; ;  //ориентация
                    pages_list[i].AddBookmark(xml.SelectSingleNode("//Html_List//Page[@id = '" + Attribute_id + "']//Bookmark").InnerText, new PdfDestination(pages_list[i].Pages[0]));                                            //Bookmarks

                    pdf.Append(pages_list[i]);                          //собираем PDFDocuments в один фаил 
                    i++;
                }
                return true;
            }
            catch (Exception e)
            {
                OnError(e.Message);
                return false;
            }
        }

        public bool Save(string path)
        {
            try
            {
                pdf.Save(path);
                return true;
            }
            catch (Exception e)
            {
                OnError(e.Message);
                return false;
            }
        }
    }
}
