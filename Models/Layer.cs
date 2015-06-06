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
        private Global.PROTOCOLS _protocol;
        private Global.LAYERTYPE _layertype;
        private string _name;
        private string _srs;
        private string _json;
        private string _parameter;
        private string _parameterDesc;
        private int _agrupadors;
        private bool _startVisible;

        // Getters i Setters
        public bool StartVisible
        {
            get { return _startVisible; }
            set { _startVisible = value; }
        }

        public int Agrupadors
        {
            get { return _agrupadors; }
            set { _agrupadors = value; }
        }

        public string ParameterDesc
        {
            get { return _parameterDesc; }
            set { _parameterDesc = value; }
        }

        public string Parameter
        {
            get { return _parameter; }
            set { _parameter = value; }
        }

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

        public Global.LAYERTYPE Layertype
        {
            get { return _layertype; }
            set { _layertype = value; }
        }

        public Global.PROTOCOLS Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        /// <summary>
        /// Constructor amb paràmetres
        /// </summary>
        /// <param name="pProtocol">Protocol</param>
        /// <param name="pLayertype">Tipus de capa</param>
        /// <param name="pName">Nom de la capa</param>
        /// <param name="pSrs">SRS de la capa</param>
        /// <param name="pParameter">Paràmetre clau</param>
        /// <param name="pParameterDesc">Descripció del paràmetre clau</param>
        /// <param name="pAgrupadors">Nombre d'agrupadors</param>
        /// <param name="pStartVisible">Visible a l'inici</param>
        public Layer(Global.PROTOCOLS pProtocol, Global.LAYERTYPE pLayertype, string pName, string pSrs, string pParameter, string pParameterDesc, int pAgrupadors, bool pStartVisible)
        {
            this.Protocol = pProtocol;
            this.Layertype = pLayertype;
            this.Name = pName;
            this.Srs = pSrs;
            this.Parameter = pParameter;
            this.ParameterDesc = pParameterDesc;
            this.Agrupadors = (pAgrupadors < 3) ? 3 : pAgrupadors;
            this.StartVisible = pStartVisible;
            if (pProtocol.Equals(Global.PROTOCOLS.WFS))
            {
                this.GetGeoJSON();
            }
        }

        /// <summary>
        /// Crea una petició al servidor i informa el camp Json amb el resultat
        /// </summary>
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

        /// <summary>
        /// Genera una URL per a la petició al servidor GeoServer
        /// </summary>
        /// <returns>La cadena URL de connexió</returns>
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

        /// <summary>
        /// Calcula el valor màxim d'una propietat del Json d'aquesta capa. Cal que hi hagi dades a l'atribut Json
        /// </summary>
        /// <param name="property">Nom de la propietat a calcular</param>
        /// <returns>El valor màxim de la propietat</returns>
        public Double GetMaxValue(string property)
        {
            try
            {
                JObject o = JObject.Parse(this.Json);
                JArray features = (JArray)o["features"];
                return (Double)features.Max(p => p["properties"][property]);
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Calcula el valor mínim d'una propietat del Json d'aquesta capa. Cal que hi hagi dades a l'atribut Json
        /// </summary>
        /// <param name="property">Nom de la propietat a calcular</param>
        /// <returns>El valor mínim de la propietat</returns>
        public Double GetMinValue(string property)
        {
            try
            {
                JObject o = JObject.Parse(this.Json);
                JArray features = (JArray)o["features"];
                return (Double)features.Min(p => p["properties"][property]);
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Calcula el valor de separació entre grups. Utilitza el valor màxim, mínim i el nombre de grups
        /// </summary>
        /// <returns>Valor numèric del salt entre colors quan es representa un element</returns>
        public double CalculateColorSpacing()
        {
            return (this.GetMaxValue(this.Parameter) - this.GetMinValue(this.Parameter)) / this.Agrupadors;
        }
    }
}