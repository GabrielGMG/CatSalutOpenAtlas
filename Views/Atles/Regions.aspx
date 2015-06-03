<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.Hidden("parameter") %>
    <%= Html.Hidden("layerTitle") %>

    <script type="text/javascript">
        $( document ).ready(function() {
        
        var jsondata = <%= ViewData["json"] %>;

        map = init();

        var colorSpacing = <%= ViewData["colorSpacing"] %>;
        var min = <%= ViewData["min"] %>;
        var max = <%= ViewData["max"] %>;

        var styles = [];
        styles = colorbrewer.Blues[7];
        var styleCache = {};

        function styleFunction(feature, resolution) {
            var level = feature.get($("input#parameter").val());
            var i = 0;
            var s = min;
            while (i < styles.length){
                if (level > s + colorSpacing) {
                    s = s + colorSpacing;
                } else {
                    var result = i;
                    i = styles.length;
                }
                i = i + 1;
            }
            if (!styleCache[level]) {
                styleCache[level] = new ol.style.Style({
                    fill: new ol.style.Fill({
                        color: styles[result==null?i-1:result]
                    }),
                    stroke : new ol.style.Stroke({
                        color: 'steelblue'
                    })
                });
            }
            return [styleCache[level]];
        }

        var vectorLayer = new ol.layer.Vector({
            title: $("input#layerTitle").val(),
            source: new ol.source.Vector({
                projection: 'EPSG:3857',
                features: (new ol.format.GeoJSON()).readFeatures(jsondata)
            })
        })
        map.addLayer(vectorLayer);

        if ($("input#parameter").val() != ""){
            vectorLayer.setStyle(styleFunction);
        }
          var highlightStyle = new ol.style.Style({
            stroke: new ol.style.Stroke({
              color: [255,0,0,0.6],
              width: 2
            }),
            fill: new ol.style.Fill({
              color: [255,0,0,0.2]
            }),
            zIndex: 1
          });
        var featureOverlay = new ol.FeatureOverlay({
            map: map,
            style: highlightStyle
        });
    
        // Hover highlight
        map.on('pointermove', function(browserEvent) {
            featureOverlay.getFeatures().clear();
            var coordinate = browserEvent.coordinate;
            var pixel = browserEvent.pixel;
            map.forEachFeatureAtPixel(pixel, function(feature, layer) {
                featureOverlay.addFeature(feature);
            });
        });

        // Popup showing the position the user clicked
        var popup = new ol.Overlay({
          element: document.getElementById('popup')
        });
        map.addOverlay(popup);

        map.on('click', function(evt) {
            var element = popup.getElement();
            var coordinate = evt.coordinate;

            $(element).popover('destroy');
            popup.setPosition(coordinate);
            // the keys are quoted to prevent renaming in ADVANCED mode.

            // Attempt to find a feature in one of the visible vector layers
            var feature = map.forEachFeatureAtPixel(evt.pixel, function(feature, layer) {
                return feature;
            });

            if (feature) {
                var props = feature.getProperties();
                var info = "<h1>"+props["regio"]+"</h1><br />";
                    info += "<p><em>Assegurats en aquesta regió</em>: " + props["Assegurats"].toLocaleString() + "</p>";
                //$("div#featureInfo").html(info);
            }

            $(element).popover({
                'placement': 'top',
                'animation': false,
                'html': true,
                'content': info
            });
            $(element).popover('show');
            if (feature==null){
                $(element).popover('destroy');
            }
        });

        // Charting amb D3
        /*var width = 150, height = 150;
        var radius = Math.min(width, height) / 2;
        var colorbrew = d3.scale.ordinal().range(colorbrewer.Set3[7]);

        var svg = d3.select('.chart')
          .append('svg')
          .attr('width', width)
          .attr('height', height)
          .append('g')
          .attr('transform', 'translate(' + (width / 2) +  ',' + (height / 2) + ')');

        var arc = d3.svg.arc().outerRadius(radius);
        var pie = d3.layout.pie().value(function(d) { return d.properties.Assegurats; }).sort(null);
        var path = svg.selectAll('path')
          .data(pie(jsondata.features))
          .enter()
          .append('path')
          .attr('d', arc)
          .attr('fill', function(d, i) { 
            return colorbrew(d.data.properties.Assegurats);
          });*/

        // Llegenda amb D3
        var cubeSide = 18;
        var legendWidth = 175;
        var legend = d3.select(".legend").attr("width", legendWidth).attr("height", cubeSide * (jsondata.features.length+1));
        legend.append("text").text("Llegenda (assegurats)").attr("y", cubeSide/2);

        var cube = legend.selectAll("g").data(styles).enter().append("g").attr("transform", function(d, i) { return "translate(0," + (i+1) * cubeSide  + ")"; });;
        cube.append("rect").attr("height", cubeSide).attr("width", cubeSide).style("fill", function(d){ return d });
        cube.append("text").text(function(d,i){ return (min+(colorSpacing*i)).toLocaleString()+" - "+(min+(colorSpacing*(i+1))).toLocaleString()}).attr("dy", ".35em").attr("x", cubeSide+5).attr("y", cubeSide/2);

        // Redibuixa el mapa (única manera d'arreglar bug de click de features desplaçat)
        map.updateSize();
        // Tancament on ready
        }); 
    </script>
</asp:Content>