using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatSalutOpenAtlas.Utils
{
    public static class Global
    {
        public static string GEO_SERVER_URL = "http://localhost:8080/geoserver";
        public static string GEO_SERVER_WORKSPACE = "TFG";
        public enum PROTOCOLS { WFS, WMS };
    }
}