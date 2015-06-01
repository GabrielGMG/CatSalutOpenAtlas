using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using CatSalutOpenAtlas.Utils;
using Newtonsoft.Json.Linq;

namespace CatSalutOpenAtlas.Models
{
    public class Layer
    {
        private static Global.PROTOCOLS _protocol;
        private string _name;
        private string _srs;
        private string _json;

        public string Json
        {
            get { return _json; }
            set { _json = value; }
        }

        public string Srs
        {
            get { return _srs; }
            set { _srs = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Global.PROTOCOLS Protocol
        {
            get { return Layer._protocol; }
            set { Layer._protocol = value; }
        }

        public Layer(Global.PROTOCOLS pProtocol, string pName, string pSrs)
        {
            this.Protocol = pProtocol;
            this.Name = pName;
            this.Srs = pSrs;
        }

        public void GetGeoJSON()
        {
            WebRequest req = WebRequest.Create(this.GenerateRequestUrl());
            req.Method = "POST";
            req.ContentType = "application/json";
            try
            {
                Stream dataStream = req.GetRequestStream();
                dataStream.Close();
                WebResponse resp = req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string responseDoc = string.Empty;
                responseDoc = sr.ReadToEnd();
                sr.Close();
                this.Json = responseDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateRequestUrl()
        {
            string url = "";
            url += Global.GEO_SERVER_URL + "/" + Global.GEO_SERVER_WORKSPACE + "/";
            if (Protocol.Equals(Global.PROTOCOLS.WFS))
            {
                url += "ows?service=WFS";
                url += "&version=2.0";
                url += "&request=GetFeature";
                url += "&typeNames=" + Global.GEO_SERVER_WORKSPACE + ":" + Name;
                url += "&outputFormat=json";
                url += "&srsname=" + Srs;
            } 
            else if (Protocol.Equals(Global.PROTOCOLS.WMS))
            {
                url += "wms?service=WMS&version=1.1.0";
            }
            return url;
        }

        public Double GetMaxValue(string property)
        {
            JObject o = JObject.Parse(this.Json);
            JArray features = (JArray)o["features"];
            return (Double)features.Max(p => p["properties"][property]);
        }

        public Double GetMinValue(string property)
        {
            JObject o = JObject.Parse(this.Json);
            JArray features = (JArray)o["features"];
            return (Double)features.Min(p => p["properties"][property]);
        }
    }
}