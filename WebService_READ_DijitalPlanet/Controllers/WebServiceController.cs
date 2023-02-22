using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Ders62_WebService_READ_DijitalPlanet.Controllers
{
    public class WebServiceController : Controller
    {
        string ticketno = "";
        dp.IntegrationService client = new dp.IntegrationService();

        // GET: WebService
        public ActionResult Index()
        {

            client.Url = "https://integrationservicewithoutmtom.digitalplanet.com.tr/IntegrationService.asmx";

            try
            {
                ticketno = client.GetFormsAuthenticationTicket("üyelikte size verilen kod buraya", "loginburaya", "şifre");

                if (string.IsNullOrEmpty(ticketno))
                {
                    //login bilgileri yanlış
                }
                else
                {
                    //login girişi OK
                    check_efatura_or_earchive("tckimlik yada vergino");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        public void check_efatura_or_earchive(string tckn_vergino)
        {
            var answer = client.CheckCustomerTaxId("bize verilen ticketno", tckn_vergino);
            if (answer.ServiceResult.ToString() == "Successful")
            {
                //Kurumsal fatura, vergi no ile sipariş alınıyor
                create_kurumsal_XML();
            }
            else
            {
                //Bireysel fatura , tc no ile
                create_bireysel_XML();
            }
        }
        //Gönderilecek XML dosyasını localde oluşturup uygun formaya çevirildi
        public void create_kurumsal_XML()
        {
            // kurumsal xml dosyası fiziksel olarak oluşacak
            XmlDocument dokuman = new XmlDocument();
            XmlElement Invoices = dokuman.CreateElement("Invoices");
            XmlElement Version = dokuman.CreateElement("Version");
            Version.InnerText = "2.1";
            XmlElement Invoice = dokuman.CreateElement("Invoice");

            XmlDeclaration xmldecl;
            xmldecl = dokuman.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            dokuman.AppendChild(xmldecl);
            //<Scenario>TEMELFATURA</Scenario>

            string path = Server.MapPath("XmlData");
            string filename = "\\" + "kurumsal.xml";
            dokuman.AppendChild(Invoice);

            //C:\Users\Mehmet Can Kalabaş\Desktop\C#\Ders62_WebService_READ_DijitalPlanet\Ders62_WebService_READ_DijitalPlanet
            ///XmlData/kurumsal.xml
            dokuman.Save(string.Format("{0}\\{1}", path, filename));

            FileStream fs = new FileStream(path + filename, FileMode.Open, FileAccess.Read);
            byte[] DocumentData = new byte[fs.Length];
            fs.Read(DocumentData, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            SendeInvoiceData(DocumentData);

        }

        //kurumsal faturayı digital planete gönderme işlemi
        public void SendeInvoiceData(byte[]_64Xmldata)
        {
            var answer = client.SendInvoiceData(ticketno, dp.File.Xml, _64Xmldata, "verilen kod", "-1", "verilen nickname");
            if (answer.ToString() == "Successful")
            {
                //xml dosyası digital planete gitti
                //kendi veritabanımzda sipariş tablosunda faturası_oluştumu kolonuna true degeri basarız.
            }
            else
            {
                //xml dosyası digital planete gitmedi
                //kendi veritabanımzda sipariş tablosunda faturası_oluştumu kolonuna false degeri basarız.
                //İlgili personele sms ve email gönder
                //tbl_setting
                //settingID,fatura_bilgi_tel,fatura_bilgi_email
            }
        }
        public void create_bireysel_XML()
        {
            // bireysel xml dosyası fiziksel olarak oluşacak
            // kurumsal xml dosyası fiziksel olarak oluşacak
            XmlDocument dokuman = new XmlDocument();
            XmlElement Invoices = dokuman.CreateElement("Invoices");
            XmlElement Version = dokuman.CreateElement("Version");
            Version.InnerText = "2.1";
            XmlElement Invoice = dokuman.CreateElement("Invoice");

            XmlDeclaration xmldecl;
            xmldecl = dokuman.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            dokuman.AppendChild(xmldecl);
            //<Scenario>EARSİVFATURA</Scenario>

            string path = Server.MapPath("XmlData");
            string filename = "\\" + "bireysel.xml";
            dokuman.AppendChild(Invoice);

            //C:\Users\Mehmet Can Kalabaş\Desktop\C#\Ders62_WebService_READ_DijitalPlanet\Ders62_WebService_READ_DijitalPlanet
            ///XmlData/bireysel.xml
            dokuman.Save(string.Format("{0}\\{1}", path, filename));

            FileStream fs = new FileStream(path + filename, FileMode.Open, FileAccess.Read);
            byte[] DocumentData = new byte[fs.Length];
            fs.Read(DocumentData, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            SendEArchiveData(DocumentData);
        }
        public void SendEArchiveData(byte[] _64Xmldata)
        {
            var answer = client.SendEArchiveData(ticketno, dp.File.Xml, _64Xmldata, "verilen kod", "verilen nickname");
            if (answer.ToString() == "Successful")
            {
                //xml dosyası digital planete gitti
                //kendi veritabanımzda sipariş tablosunda faturası_oluştumu kolonuna true degeri basarız.
            }
            else
            {
           
            }
        }
    }
}