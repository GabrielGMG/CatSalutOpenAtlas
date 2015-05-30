using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace CatSalutOpenAtlas.Models
{
    public class Layer
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string getGeoJSON()
        {
            WebRequest req = WebRequest.Create(this.Url);
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream dataStream = req.GetRequestStream();
            dataStream.Close();
            WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string responseDoc = string.Empty;
            responseDoc = sr.ReadToEnd();
            sr.Close();

            return responseDoc;
        }
    }
}