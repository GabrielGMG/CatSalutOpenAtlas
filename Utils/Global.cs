using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatSalutOpenAtlas.Utils
{
    public static class Global
    {
        /// <summary>
        /// URL base del servidor de mapes.
        /// </summary>
        public static string GEO_SERVER_URL = "http://localhost:8080/geoserver";

        /// <summary>
        /// Nom de l'espai de treball del servidor de mapes on resideixen les capes
        /// </summary>
        public static string GEO_SERVER_WORKSPACE = "TFG";

        /// <summary>
        /// Tipus de protocols suportats
        /// </summary>
        public enum PROTOCOLS { WFS, WMS };

        /// <summary>
        /// Tipus de capes suportades
        /// </summary>
        public enum LAYERTYPE { POINT, POLYGON };
    }
}