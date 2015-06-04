/*
*********************************************************************************************
*
*   Fitxer: atlas.js
*   Versió: 1.0
*   Autor: Gabriel Garcia-Mascaraque Gil
*
*********************************************************************************************
*   Aquest fitxer conté les funcions i crides globals que es fan servir en totes les pàgines
*   de l'aplicació.
*
*   Requereix les llibreries ol.js o ol-debug.js (OpenLayers 3) i ol3-layerswitcher (Plugin 
*   per a OpenLayers).
*
*********************************************************************************************
*/


/*
*   Inicialització del mapa amb els controls bàsics
*/
function init() {
    // Creació del mapa
        var projection = ol.proj.get('EPSG:3857');
        var view = new ol.View({
            center: [195000, 5100000],
            projection: projection,
            zoom: 8
        });
        var map = new ol.Map({ target: 'map', view: view });

    // Control que dóna les coordenades del cursor
    var mousePositionControl = new ol.control.MousePosition({
        coordinateFormat: ol.coordinate.createStringXY(4),
        projection: 'EPSG:3857',
        undefinedHTML: '&nbsp;'
    });
    map.addControl(mousePositionControl);

    // Control per a mostrar/ocultar les capes carregades al mapa
    // Requereix que les capes tinguin informat l'atribut "title"
    var layerSwitcher = new ol.control.LayerSwitcher({
        tipLabel: 'Selector de capes'
    });
    map.addControl(layerSwitcher);

    

    // Capa Stamen Toner
    var stamenTonerSource = new ol.source.Stamen({ layer: 'toner' });
    var stamenTonerLayer = new ol.layer.Tile({ source: stamenTonerSource, title: 'Stamen: Toner', type: 'base', visible: false });

    // Capa Map Quest Sat
    var mqsatSource = new ol.source.MapQuest({ layer: 'sat' });
    var mqsatLayer = new ol.layer.Tile({ source: mqsatSource, title: 'Map Quest: Satellite', type: 'base', visible: false });

    // Capa OSM
    var osmSource = new ol.source.OSM();
    var osmLayer = new ol.layer.Tile({ source: osmSource, title: 'OSM', type: 'base' });

    // Capa ICGC
    var icgcLayer = new ol.layer.Tile({
        source: new ol.source.TileWMS({
            url: 'http://geoserveis.icc.cat/icc_mapesmultibase/noutm/wms/service?',
            params: { 'LAYERS': 'topo' }
        }),
        title: 'ICGC',
        type: 'base',
        visible: false
    })

    var baselayers = new ol.layer.Group({
        title: 'Cartografia base',
        layers: [stamenTonerLayer, mqsatLayer, icgcLayer, osmLayer]
    });

    map.addLayer(baselayers);

    return map;
}

function clone(obj) {
    if (obj == null || typeof (obj) != 'object')
        return obj;

    var temp = new obj.constructor();
    for (var key in obj)
        temp[key] = clone(obj[key]);

    return temp;
}

function creaCapa(jsondata, map, colorSpacing, min, layerColors, parameter, title, layertype, startVisible) {

    // Creació d'estils

    // Si parameter es null es una capa sense parametre i no cal fer estils en funció de res
    if (parameter != null){
        var stylesPoly = [];
        stylesPoly = layerColors;
        var styleCachePoly = {};
        function styleFunctionPoly(feature, resolution) {
            var level = feature.get(parameter);
            var i = 0;
            var s = min;
            while (i < stylesPoly.length) {
                if (level > s + colorSpacing) {
                    s = s + colorSpacing;
                } else {
                    var result = i;
                    i = stylesPoly.length;
                }
                i = i + 1;
            }
            if (!styleCachePoly[level]) {
                styleCachePoly[level] = new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: stylesPoly[result == null ? i - 1 : result]
                    }),
                    stroke: new ol.style.Stroke({
                        color: 'white'
                    })
                });
            }
            return [styleCachePoly[level]];
        }

        var stylesPoi = [];
        stylesPoi = layerColors;
        var styleCachePoi = {};

        function styleFunctionPoi(feature, resolution) {
            var level = feature.get(parameter);
            var i = 0;
            var s = min;
            while (i < stylesPoi.length){
                if (level == s + colorSpacing) {
                    s = s + colorSpacing;
                } else {
                    var result = i;
                    i = stylesPoi.length;
                }
                i = i + 1;
            }
            if (!styleCachePoi[level]) {
                styleCachePoi[level] = new ol.style.Style({
                    image : new ol.style.Circle({
                        fill: new ol.style.Fill({
                            color: stylesPoi[level-1],
                        }),
                        stroke : new ol.style.Stroke({
                            color: 'white'
                        }),
                        radius : 5
                    })
                });
            }
            return [styleCachePoi[level]];
        }
    }

    // Estil per a capes d'un sol estil
    var stylesUnivalue = [];
    stylesUnivalue = layerColors;
    var styleCacheUnivalue = {};

    function styleFunctionUnivalue(feature, resolution) {
        styleCacheUnivalue[0] = new ol.style.Style({
            image : new ol.style.Circle({
                fill: new ol.style.Fill({
                    color: stylesUnivalue[0],
                }),
                stroke : new ol.style.Stroke({
                    color: 'white'
                }),
                radius : 5
            })
        });
        return [styleCacheUnivalue[0]];
    }

    // Creació de la capa
    var vectorLayer = new ol.layer.Vector({
        title: title,
        visible: startVisible,
        source: new ol.source.Vector({
            projection: 'EPSG:3857',
            features: (new ol.format.GeoJSON()).readFeatures(jsondata)
        })
    })
    map.addLayer(vectorLayer);

    // Aplica de l'estil que correspongui
    if (parameter == null){
        vectorLayer.setStyle(styleFunctionUnivalue);
    } else
    if (layertype == 'POLYGON'){
        vectorLayer.setStyle(styleFunctionPoly);
    } else 
    if (layertype == 'POINT'){
        vectorLayer.setStyle(styleFunctionPoi);
    }

    // Realçat d'elements poligonals
    var highlightStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: [255, 0, 0, 0.6],
            width: 2
        }),
        fill: new ol.style.Fill({
            color: [255, 0, 0, 0.2]
        }),
        zIndex: 1
    });
    var featureOverlay = new ol.FeatureOverlay({
        map: map,
        style: highlightStyle
    });
    map.on('pointermove', function (browserEvent) {
        featureOverlay.getFeatures().clear();
        var coordinate = browserEvent.coordinate;
        var pixel = browserEvent.pixel;
        map.forEachFeatureAtPixel(pixel, function (feature, layer) {
            featureOverlay.addFeature(feature);
        });
    });

    // Retornem la capa
    return vectorLayer;
}

function heatmap(json, map, parameter) {
    var heatmapLayer = new ol.layer.Heatmap({
    title: 'Heatmap',
    visible: false,
    opacity: 0.7,
    source: new ol.source.Vector({
        projection: 'EPSG:3857',
        features: (new ol.format.GeoJSON()).readFeatures(json)
        })
    })
    map.addLayer(heatmapLayer);
    heatmapLayer.getSource().on('addfeature', function(event) {
        var name = event.feature.get(parameter);
        var magnitude = parseFloat(name.substr(2));
        event.feature.set('weight', magnitude - 5);
    });
    return map;
}

function creaLlegendaCategorica(styles, data){
    var cubeSide = 16;
    var legendWidth = 275;
    var legend = d3.select(".legend").attr("width", legendWidth).attr("height", cubeSide * (styles.length+1));
    legend.append("text").text("Llegenda").attr("y", cubeSide/2);

    var cube = legend.selectAll("g").data(data).enter().append("g").attr("transform", function(d, i) { return "translate(0," + (i+1) * cubeSide  + ")"; });
    cube.append("rect").attr("height", cubeSide).attr("width", cubeSide).style("fill", function(d, i){ return styles[d.id-1] });
    cube.append("text").text(function(d,i){ return (d.desc)}).attr("dy", ".35em").attr("x", cubeSide+5).attr("y", cubeSide/2);
}

function creaLlegendaContinua(styles, min, spacing){
    var cubeSide = 16;
    var legendWidth = 275;
    var legend = d3.select(".legend").attr("width", legendWidth).attr("height", cubeSide * (styles.length+1));
    legend.append("text").text("Llegenda").attr("y", cubeSide/2);

    var cube = legend.selectAll("g").data(styles).enter().append("g").attr("transform", function(d, i) { return "translate(0," + (i+1) * cubeSide  + ")"; });;
    cube.append("rect").attr("height", cubeSide).attr("width", cubeSide).style("fill", function(d) { return d });
    cube.append("text").text(function(d,i){ return (min+(spacing*i)).toLocaleString()+" - "+(min+(spacing*(i+1))).toLocaleString()}).attr("dy", ".35em").attr("x", cubeSide+5).attr("y", cubeSide/2);
}

function getDistinctValues(json, parameterId, parameterNom){
    var lookup = {};
    var items = json.features;
    var result = [];

    for (var item, i = 0; item = items[i++];) {
        var name = item.properties[parameterId];

        if (!(name in lookup)) {
        lookup[name] = 1;
        result.push({id:name, desc:item.properties[parameterNom]});
        }
    }
    return result;
}