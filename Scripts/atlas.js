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

function creaVoronoi(jsondata, map, colorSpacingV, minV, layerColors, parameter, title) {

    var styles = [];
    styles = colorbrewer["Oranges"][4];
    var styleCacheV = {};

    function styleFunctionV(feature, resolution) {
        var level = feature.get('area');
        var i = 0;
        var s = minV;
        while (i < styles.length) {
            if (level > s + colorSpacingV) {
                s = s + colorSpacingV;
            } else {
                var result = i;
                i = styles.length;
            }
            i = i + 1;
        }
        console.log(result);
        if (!styleCacheV[level]) {
            styleCacheV[level] = new ol.style.Style({
                fill: new ol.style.Fill({
                    color: styles[result == null ? i - 1 : result]
                }),
                stroke: new ol.style.Stroke({
                    color: 'white'
                })
            });
        }
        return [styleCacheV[level]];
    }

    var vectorLayer = new ol.layer.Vector({
        title: title,
        source: new ol.source.Vector({
            projection: 'EPSG:3857',
            features: (new ol.format.GeoJSON()).readFeatures(jsondata)
        })
    })
    map.addLayer(vectorLayer);

    vectorLayer.setStyle(styleFunctionV);

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

    return map;
}