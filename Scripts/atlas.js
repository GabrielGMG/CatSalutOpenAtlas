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
    var stamenTonerLayer = new ol.layer.Tile({ source: stamenTonerSource, title: 'Stamen: Toner', type: 'base' });

    // Capa Map Quest Sat
    var mqsatSource = new ol.source.MapQuest({ layer: 'sat' });
    var mqsatLayer = new ol.layer.Tile({ source: mqsatSource, title: 'Map Quest: Satellite', type: 'base' });

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
        type: 'base'
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