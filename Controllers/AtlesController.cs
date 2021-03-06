﻿using System;
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
        /// <summary>
        /// Acció Index. Acció d'inici per defecte.
        /// </summary>
        /// <returns>La vista Map amb una llista de capes buida</returns>
        public ActionResult Index()
        {
            ViewData["layerInfo"] = "Trieu una capa del menu lateral.";
            return View("Map", new List<Layer>());
        }

        /// <summary>
        /// Acció Centres.
        /// </summary>
        /// <returns>La vista Map amb una llista de capes a visualitzar</returns>
        public ActionResult Centres()
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POINT, "Centres", "EPSG:3857", "tipus", "Subtema2", 5, true));
            layers.Add(new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "CentresVoronoi", "EPSG:3857", "area", "area", 7, false));

            // Valors a passar a la vista
            // Nom de la capa de la qual es vol veure la llegenda
            ViewData["LegendLayer"] = "Centres";
            // Descripció de la vista
            ViewData["layerInfo"] = "En aquesta capa es mostra la ubicació de tots els centres del sistema sanitari de Catalunya. Es poden activar les capes 'Heatmap' i 'Voronoi' que mostren el mapa de calor o densitat de la capa de centres. Dóna un indicador de la cobertura sanitària de Catalunya i posa en evidència l'estreta relació amb la densitat de població.";
            // Cercador al camp...
            ViewData["Cercador"] = "Centres";
            // Html per incloure a la finestra emergent
            ViewData["PopupContent"] = "var info = '<h1>'+props['Nom']+'</h1><br /><p><em>Adreça</em>: ' + props['Carrer'] + ', ' + props['CP'] + ', ' + props['Municipi'] + '</p><p><em>Tipus de centre</em>: ' + props['Subtema2'] + '</p>';";
            // Títol de la capa per mostrar a la capçalera
            ViewData["layerTitle"] = "Centres sanitaris";
            return View("Map", layers);
        }

        /// <summary>
        /// Acció Farmacies.
        /// </summary>
        /// <returns>La vista Map amb una llista de capes a visualitzar</returns>
        public ActionResult Farmacies()
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POINT, "Farmacies", "EPSG:3857", null, null, 1, true));

            // Valors a passar a la vista
            // Nom de la capa de la qual es vol veure la llegenda
            ViewData["LegendLayer"] = null;
            // Descripció de la vista
            ViewData["layerInfo"] = "En aquesta capa es mostra la ubicació de les farmàcies de Catalunya (Febrer de 2015).";
            // Cercador al camp...
            ViewData["Cercador"] = "Farmacies";
            // Html per incloure a la finestra emergent
            ViewData["PopupContent"] = "var info = '<h1>'+props['nom']+'</h1><br /><p><em>Adreça</em>: ' +props['carrer'] + '. ' + props['municipi'] + '</p>';";
            // Títol de la capa per mostrar a la capçalera
            ViewData["layerTitle"] = "Farmàcies";
            return View("Map", layers);
        }

        /// <summary>
        /// Acció Regions.
        /// </summary>
        /// <returns>La vista Map amb una llista de capes a visualitzar</returns>
        public ActionResult Regions()
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "RegionsAssegurats", "EPSG:3857", "Assegurats", "Assegurats", 9, true));

            // Valors a passar a la vista
            // Nom de la capa de la qual es vol veure la llegenda
            ViewData["LegendLayer"] = "RegionsAssegurats";
            // Descripció de la vista
            ViewData["layerInfo"] = "Aquesta capa representa les diferents regions sanitàries amb informació sobre el nombre d'assegurats a cada una.";
            // Cercador al camp...
            ViewData["Cercador"] = null;
            // Html per incloure a la finestra emergent
            ViewData["PopupContent"] = "var info = '<h1>'+props['regio']+'</h1><br /><p><em>Assegurats en aquesta regió</em>: ' + props['Assegurats'].toLocaleString() + '</p>';";
            // Títol de la capa per mostrar a la capçalera
            ViewData["layerTitle"] = "Regions sanitàries";
            return View("Map", layers);
        }

        /// <summary>
        /// Acció Donacions2012.
        /// </summary>
        /// <returns>La vista Map amb una llista de capes a visualitzar</returns>
        public ActionResult Donacions2012()
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "RegionsAssegurats", "EPSG:3857", "donacionsRate2012", "donacionsRate2012", 8, true));

            // Valors a passar a la vista
            // Nom de la capa de la qual es vol veure la llegenda
            ViewData["LegendLayer"] = "RegionsAssegurats";
            // Descripció de la vista
            ViewData["layerInfo"] = "Aquesta capa representa les donacions per cada regió sanitària de Catalunya l'any 2012. Es mostren les donacions totals així com un rati de donacions per cada 1000 assegurats. L'escala de colors s'ha elaborat en base a aquest rati.";
            // Cercador al camp...
            ViewData["Cercador"] = null;
            ViewData["BarChart"] = "RegionsAssegurats";
            // Html per incloure a la finestra emergent
            ViewData["PopupContent"] = "var info = '<h1>'+props['regio']+'</h1><br /><p><em>Assegurats en aquesta regió</em>: ' + props['Assegurats'].toLocaleString() + '</p><p><em>Donacions de sang en aquesta regió per cada 1000 hab. (2012)</em>: ' + props['donacionsRate2012'].toLocaleString() + '</p><p><em>Donacions de sang totals en aquesta regió (2012)</em>: ' + props['donacions2012'].toLocaleString() + '</p>';";
            // Títol de la capa per mostrar a la capçalera
            ViewData["layerTitle"] = "Donacions de sang (any 2012)";
            return View("Map", layers);
        }

        /// <summary>
        /// Acció Donacions2013.
        /// </summary>
        /// <returns>La vista Map amb una llista de capes a visualitzar</returns>
        public ActionResult Donacions2013()
        {
            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "RegionsAssegurats", "EPSG:3857", "donacionsRate2013", "donacionsRate2013", 8, true));

            // Valors a passar a la vista
            // Nom de la capa de la qual es vol veure la llegenda
            ViewData["LegendLayer"] = "RegionsAssegurats";
            // Descripció de la vista
            ViewData["layerInfo"] = "Aquesta capa representa les donacions per cada regió sanitària de Catalunya l'any 2013. Es mostren les donacions totals així com un rati de donacions per cada 1000 assegurats. L'escala de colors s'ha elaborat en base a aquest rati.";
            // Cercador al camp...
            ViewData["Cercador"] = null;
            ViewData["BarChart"] = "RegionsAssegurats";
            // Html per incloure a la finestra emergent
            ViewData["PopupContent"] = "var info = '<h1>'+props['regio']+'</h1><br /><p><em>Assegurats en aquesta regió</em>: ' + props['Assegurats'].toLocaleString() + '</p><p><em>Donacions de sang en aquesta regió per cada 1000 hab. (2013)</em>: ' + props['donacionsRate2013'].toLocaleString() + '</p><p><em>Donacions de sang totals en aquesta regió (2013)</em>: ' + props['donacions2013'].toLocaleString() + '</p>';";
            // Títol de la capa per mostrar a la capçalera
            ViewData["layerTitle"] = "Donacions de sang (any 2013)";
            return View("Map", layers);
        }

        // Primera versió de les accions
        /*public ActionResult Centres()
        {
            Layer centresLayer = new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POINT, "Centres", "EPSG:3857", "tipus", "Subtema2", 5, true);
            Layer voronoiLayer = new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "CentresVoronoi", "EPSG:3857", "area", "area", 7, true);
            try
            {
                centresLayer.GetGeoJSON();
                voronoiLayer.GetGeoJSON();
            }
            catch (Exception ex)
            {
                ViewData["layerInfo"] = "El servidor no es troba disponible.";
                return View();
            }

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
            ViewData["parameterVoronoi"] = "area";
            ViewData["colorSpacingVoronoi"] = (int)(Utilitats.CalculateColorSpacing(7, voronoiLayer.GetMaxValue("area"), voronoiLayer.GetMinValue("area")));
            ViewData["layerColorsVoronoi"] = "[[241,238,246,0.7],[208,209,230,0.7],[166,189,219,0.7],[116,169,207,0.7],[54,144,192,0.7],[5,112,176,0.7],[3,78,123,0.7]]";
            ViewData["maxVoronoi"] = voronoiLayer.GetMaxValue("area").ToString().Replace(',', '.');
            ViewData["minVoronoi"] = voronoiLayer.GetMinValue("area").ToString().Replace(',', '.');
            ViewData["jsonCentres"] = centresLayer.Json;
            ViewData["jsonVoronoi"] = voronoiLayer.Json;
            ViewData["colors"] = colors;
            ViewData["layerInfo"] = "En aquesta capa es mostra la ubicació de tots els centres del sistema sanitari de Catalunya. Es poden activar les capes 'Heatmap' i 'Voronoi' que mostren el mapa de calor o densitat de la capa de centres. Dóna un indicador de la cobertura sanitària de Catalunya i posa en evidència l'estreta relació amb la densitat de població.";
            ViewData["layerTitle"] = "Centres sanitàris";
            return View();
        }*/

        /*public ActionResult Farmacies()
        {
            Layer farmaciesLayer = new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "Farmacies", "EPSG:3857", null, null, 7, true);
            try
            {
                farmaciesLayer.GetGeoJSON();
            }
            catch (Exception ex)
            {
                ViewData["layerInfo"] = "El servidor no es troba disponible.";
                return View();
            }

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
        }*/

        /*public ActionResult Regions()
        {
            Layer regionsLayer = new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "RegionsAssegurats", "EPSG:3857", "Assegurats", "Assegurats", 7, true);
            try
            {
                regionsLayer.GetGeoJSON();
            }
            catch (Exception ex)
            {
                ViewData["layerInfo"] = "El servidor no es troba disponible.";
                return View();
            }

            ViewData["parameter"] = "Assegurats";
            ViewData["layerColors"] = "[[241,238,246,0.7],[208,209,230,0.7],[166,189,219,0.7],[116,169,207,0.7],[54,144,192,0.7],[5,112,176,0.7],[3,78,123,0.7]]";
            ViewData["colorSpacing"] = (int)(Utilitats.CalculateColorSpacing(7, regionsLayer.GetMaxValue("Assegurats"), regionsLayer.GetMinValue("Assegurats")));
            ViewData["max"] = regionsLayer.GetMaxValue("Assegurats").ToString().Replace(',', '.');
            ViewData["min"] = regionsLayer.GetMinValue("Assegurats").ToString().Replace(',', '.');
            ViewData["json"] = regionsLayer.Json;
            ViewData["layerInfo"] = "Aquesta capa representa les diferents regions sanitàries amb informació sobre el nombre d'assegurats a cada una.";
            ViewData["layerTitle"] = "Assegurats per regió sanitària";
            return View();
        }*/

        /*public ActionResult Donacions2012()
        {
            Layer donacions2012Layer = new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "RegionsAssegurats", "EPSG:3857", "donacionsRate2012", "donacionsRate2012", 7, true);
            try
            {
                donacions2012Layer.GetGeoJSON();
            }
            catch (Exception ex)
            {
                ViewData["layerInfo"] = "El servidor no es troba disponible.";
                return View();
            }

            ViewData["parameter"] = "donacionsRate2012";
            //ViewData["layerColors"] = Utilitats.GeneratePalette(7, "0.7");
            ViewData["layerColors"] = "[[215,48,39,0.7],[252,141,89,0.7],[254,224,139,0.7],[255,255,191,0.7],[217,239,139,0.7],[145,207,96,0.7],[26,152,80,0.7]]";
            ViewData["colorSpacing"] = Utilitats.CalculateColorSpacing(7, donacions2012Layer.GetMaxValue("donacionsRate2012"), donacions2012Layer.GetMinValue("donacionsRate2012")).ToString().Replace(',', '.');
            ViewData["max"] = donacions2012Layer.GetMaxValue("donacionsRate2012").ToString().Replace(',', '.');
            ViewData["min"] = donacions2012Layer.GetMinValue("donacionsRate2012").ToString().Replace(',', '.');
            ViewData["json"] = donacions2012Layer.Json;
            ViewData["layerInfo"] = "Aquesta capa representa les donacions per cada regió sanitària de Catalunya l'any 2012. Es mostren les donacions totals així com un rati de donacions per cada 1000 assegurats. L'escala de colors s'ha elaborat en base a aquest rati.";
            ViewData["layerTitle"] = "Donacions 2012 (per 1000 hab.)";
            return View();
        }*/

        /*public ActionResult Donacions2013()
        {
            Layer donacions2013Layer = new Layer(Global.PROTOCOLS.WFS, Global.LAYERTYPE.POLYGON, "RegionsAssegurats", "EPSG:3857", "donacionsRate2013", "donacionsRate2013", 7, true);
            try
            {
                donacions2013Layer.GetGeoJSON();
            }
            catch (Exception ex)
            {
                ViewData["layerInfo"] = "El servidor no es troba disponible.";
                return View();
            }

            ViewData["parameter"] = "donacionsRate2013";
            //ViewData["layerColors"] = Utilitats.GeneratePalette(7, "0.7");
            ViewData["layerColors"] = "[[215,48,39,0.7],[252,141,89,0.7],[254,224,139,0.7],[255,255,191,0.7],[217,239,139,0.7],[145,207,96,0.7],[26,152,80,0.7]]";
            ViewData["colorSpacing"] = Utilitats.CalculateColorSpacing(7, donacions2013Layer.GetMaxValue("donacionsRate2013"), donacions2013Layer.GetMinValue("donacionsRate2013")).ToString().Replace(',', '.');
            ViewData["max"] = donacions2013Layer.GetMaxValue("donacionsRate2013").ToString().Replace(',', '.');
            ViewData["min"] = donacions2013Layer.GetMinValue("donacionsRate2013").ToString().Replace(',', '.');
            ViewData["json"] = donacions2013Layer.Json;
            ViewData["layerInfo"] = "Aquesta capa representa les donacions per cada regió sanitària de Catalunya l'any 2013. Es mostren les donacions totals així com un rati de donacions per cada 1000 assegurats. L'escala de colors s'ha elaborat en base a aquest rati.";
            ViewData["layerTitle"] = "Donacions 2013 (per 1000 hab.)";
            return View();
        }*/
    }
}
