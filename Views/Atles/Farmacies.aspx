<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.Hidden("parameter") %>
    <%= Html.Hidden("layerTitle") %>

    <script type="text/javascript">
        //$( document ).ready(function() {
        
            var jsondataFiltered;

            $("#cerca").removeClass("hidden");
            $('#cerca').keypress(function(event){
              if(event.keyCode == 13){
                $('#btnCerca').click();
              }
            });

            function cerca(str, field){
                jsondataFiltered = clone(jsondata);
                jsondataFiltered.features = $.grep(jsondataFiltered.features, function(element, index){return element.properties[field].toLowerCase().search(str.toLowerCase())>-1});
                jsondataFiltered.totalFeatures = jsondataFiltered.features.length;
                if (str == ""){
                    jsondataFiltered = clone(jsondata);
                }
                vectorLayer.setSource(new ol.source.Vector({
                    projection: 'EPSG:3857',
                    features: (new ol.format.GeoJSON()).readFeatures(jsondataFiltered)
                }));
            }

            var jsondata = <%= ViewData["json"] %>;

            var fields = Object.keys(jsondata.features[0].properties);
            for (var key in fields){
                console.log(fields[key]);
                $("#selCerca").append("<option val="+fields[key]+">"+fields[key]+"</option>");
            }

            $("#btnCerca").click(function(){cerca($("#txtCerca").val(), $("#selCerca").val())});

            map = init();

            // Capa OSM
            var osmSource = new ol.source.OSM();
            var osmLayer = new ol.layer.Tile({source: osmSource, title: 'OSM'});
            map.addLayer(osmLayer);

            var colorSpacing = <%= ViewData["colorSpacing"] %>;
            var min = <%= ViewData["min"] %>;
            var max = <%= ViewData["max"] %>;

            var styles = [];
            styles = <%= ViewData["colors"] %>;
            var styleCache = {};

            function styleFunction(feature, resolution) {
                if (!styleCache[0]) {
                    styleCache[0] = new ol.style.Style({
                        image : new ol.style.Circle({
                            fill: new ol.style.Fill({
                                color: 'rgba'+styles[0].Color,
                            }),
                            stroke : new ol.style.Stroke({
                                color: 'white'
                            }),
                            radius : 5
                        })
                    });
                }
                return [styleCache[0]];
            }

            var vectorLayer = new ol.layer.Vector({
                title: $("input#layerTitle").val(),
                source: new ol.source.Vector({
                    projection: 'EPSG:3857',
                    features: (new ol.format.GeoJSON()).readFeatures(jsondata)
                })
            })
            map.addLayer(vectorLayer);

            vectorLayer.setStyle(styleFunction);

            // Popup showing the position the user clicked
            var element = document.getElementById('popup');
            var popup = new ol.Overlay({
              element: element
            });
            map.addOverlay(popup);

            map.on('click', function(evt) {
                var feature = map.forEachFeatureAtPixel(evt.pixel, function(feature, layer) {
                    return feature;
                });

                if (feature) {
                    var geometry = feature.getGeometry();
                    var coord = geometry.getCoordinates();
                    popup.setPosition(coord);
                    var props = feature.getProperties();
                    var info = "<h1>"+props["nom"]+"</h1><br />";
                        info += "<p><em>Adreça</em>: " +coord+ props["carrer"] + ". " + props["municipi"] + "</p>";
                }

                $(element).popover({
                    'placement': 'top',
                    'animation': true,
                    'html': true,
                    'content': info
                });
                $(element).popover('show');
                if (feature==null){
                    $(element).popover('destroy');
                }
            });


            // Llegenda amb D3
            var cubeSide = 18;
            var legendWidth = 175;
            var legend = d3.select(".legend").attr("width", legendWidth).attr("height", cubeSide * 6);
            legend.append("text").text("Llegenda").attr("y", cubeSide/2);

            var cube = legend.selectAll("g").data(styles).enter().append("g").attr("transform", function(d, i) { return "translate(0," + (i+1) * cubeSide  + ")"; });;
            cube.append("rect").attr("height", cubeSide).attr("width", cubeSide).style("fill", function(d, i){ return 'rgba'+d.Color });
            cube.append("text").text(function(d,i){ return (d.Desc)}).attr("dy", ".35em").attr("x", cubeSide+5).attr("y", cubeSide/2);

            // Redibuixa el mapa (única manera d'arreglar bug de click de features desplaçat)
            map.updateSize();
        // Tancament on ready
        //});
        
    </script>
</asp:Content>