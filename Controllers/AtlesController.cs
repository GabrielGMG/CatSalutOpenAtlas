using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using CatSalutOpenAtlas.Models;
using CatSalutOpenAtlas.Utils;
using Newtonsoft.Json.Linq;

namespace CatSalutOpenAtlas.Controllers
{
    [HandleError]
    public class AtlesController : Controller
    {
        public ActionResult OpenLayers()
        {
            return View();
        }

        public ActionResult Centres()
        {
            Layer centresLayer = new Layer(Global.PROTOCOLS.WFS, "Centres", "EPSG:3857");
            centresLayer.GetGeoJSON();

            List<Feature> features = new List<Feature>();
            features.Add(new Feature(1, "Hospitals", "(228,26,28,1)"));
            features.Add(new Feature(2, "Centres d\'atenció primària (CAP)", "(55,126,184,1)"));
            features.Add(new Feature(3, "Centres sociosanitaris", "(77,175,74,1)"));
            features.Add(new Feature(4, "Centres de salut mental", "(152,78,163,1)"));
            features.Add(new Feature(5, "Centres d'atenció i seguiment a les drogodependències", "(255,127,0,1)"));

            string colors = JsonConvert.SerializeObject(features);
            
            ViewData["parameter"] = "tipus";
            ViewData["colorSpacing"] = 1;
            ViewData["max"] = 5;
            ViewData["min"] = 1;
            ViewData["json"] = centresLayer.Json;
            ViewData["colors"] = colors;
            ViewData["layerInfo"] = "En aquesta capa es mostra la ubicació de tots els centres del sistema sanitari de Catalunya. Es pot activar la capa 'Heatmap' que mostra el mapa de calor o densitat de la capa de centres. Dóna un indicador de la cobertura sanitària de Catalunya i posa en evidència l'estreta relació amb la densitat de població.";
            ViewData["layerTitle"] = "Centres sanitàris";
            return View();
        }

        public ActionResult Farmacies()
        {
            Layer farmaciesLayer = new Layer(Global.PROTOCOLS.WFS, "Farmacies", "EPSG:3857");
            farmaciesLayer.GetGeoJSON();

            List<Feature> features = new List<Feature>();
            features.Add(new Feature(1, "Farmàcies", "(228,26,28,1)"));
            string colors = JsonConvert.SerializeObject(features);        

            ViewData["colorSpacing"] = 1;
            ViewData["max"] = 1;
            ViewData["min"] = 1;
            ViewData["json"] = farmaciesLayer.Json;
            ViewData["colors"] = colors;
            ViewData["layerInfo"] = "En aquesta capa es mostra la ubicació de les farmàcies de Catalunya (Febrer de 2015).";
            ViewData["layerTitle"] = "Farmàcies";
            return View();
        }

        public ActionResult Regions()
        {
            Layer regionsLayer = new Layer(Global.PROTOCOLS.WFS, "RegionsAssegurats", "EPSG:3857");
            regionsLayer.GetGeoJSON();

            ViewData["parameter"] = "Assegurats";
            ViewData["layerColors"] = "[[241,238,246,0.7],[208,209,230,0.7],[166,189,219,0.7],[116,169,207,0.7],[54,144,192,0.7],[5,112,176,0.7],[3,78,123,0.7]]";
            ViewData["colorSpacing"] = (int)(Utilitats.CalculateColorSpacing(7, 5000000, 50000));
            ViewData["max"] = regionsLayer.GetMaxValue("Assegurats").ToString().Replace(',', '.');
            ViewData["min"] = regionsLayer.GetMinValue("Assegurats").ToString().Replace(',', '.');
            ViewData["json"] = regionsLayer.Json;
            ViewData["layerInfo"] = "Aquesta capa representa les diferents regions sanitàries amb informació sobre el nombre d'assegurats a cada una.";
            ViewData["layerTitle"] = "Assegurats per regió sanitària";
            return View();
        }

        public ActionResult Donacions2012()
        {
            Layer donacions2012Layer = new Layer(Global.PROTOCOLS.WFS, "RegionsAssegurats", "EPSG:3857");
            donacions2012Layer.GetGeoJSON();

            ViewData["parameter"] = "donacionsRate2012";
            //ViewData["layerColors"] = Utilitats.GeneratePalette(7, "0.7");
            ViewData["layerColors"] = "[[215,48,39,0.7],[252,141,89,0.7],[254,224,139,0.7],[255,255,191,0.7],[217,239,139,0.7],[145,207,96,0.7],[26,152,80,0.7]]";
            ViewData["colorSpacing"] = Utilitats.CalculateColorSpacing(7, 50, 32).ToString().Replace(',','.');
            ViewData["max"] = donacions2012Layer.GetMaxValue("donacionsRate2012").ToString().Replace(',', '.');
            ViewData["min"] = donacions2012Layer.GetMinValue("donacionsRate2012").ToString().Replace(',', '.');
            ViewData["json"] = donacions2012Layer.Json;
            ViewData["layerInfo"] = "Aquesta capa representa les donacions per cada regió sanitària de Catalunya l'any 2012. Es mostren les donacions totals així com un rati de donacions per cada 1000 assegurats. L'escala de colors s'ha elaborat en base a aquest rati.";
            ViewData["layerTitle"] = "Donacions 2012 (per 1000 hab.)";
            return View();
        }


        public ActionResult Donacions2013()
        {
            Layer donacions2013Layer = new Layer(Global.PROTOCOLS.WFS, "RegionsAssegurats", "EPSG:3857");
            donacions2013Layer.GetGeoJSON();

            ViewData["parameter"] = "donacionsRate2013";
            //ViewData["layerColors"] = Utilitats.GeneratePalette(7, "0.7");
            ViewData["layerColors"] = "[[215,48,39,0.7],[252,141,89,0.7],[254,224,139,0.7],[255,255,191,0.7],[217,239,139,0.7],[145,207,96,0.7],[26,152,80,0.7]]";
            ViewData["colorSpacing"] = Utilitats.CalculateColorSpacing(7, 44, 31).ToString().Replace(',', '.');
            ViewData["max"] = donacions2013Layer.GetMaxValue("donacionsRate2013").ToString().Replace(',', '.');
            ViewData["min"] = donacions2013Layer.GetMinValue("donacionsRate2013").ToString().Replace(',', '.');
            ViewData["json"] = donacions2013Layer.Json;
            ViewData["layerInfo"] = "Aquesta capa representa les donacions per cada regió sanitària de Catalunya l'any 2013. Es mostren les donacions totals així com un rati de donacions per cada 1000 assegurats. L'escala de colors s'ha elaborat en base a aquest rati.";
            ViewData["layerTitle"] = "Donacions 2013 (per 1000 hab.)";
            return View();
        }
    }
}
