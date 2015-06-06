<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CatSalutOpenAtlas.Models.Layer>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var map = init();
        var layers = [];

        // Càrrega de capes
        <% foreach (CatSalutOpenAtlas.Models.Layer layer in Model) {
        %> 
            var json = <%= layer.Json %>;
            var colorSpacing = <%= layer.CalculateColorSpacing().ToString().Replace(',','.') %>;
            var min = <%= layer.GetMinValue(layer.Parameter).ToString().Replace(',','.') %>;
            var max = <%= layer.GetMaxValue(layer.Parameter).ToString().Replace(',','.') %>;
            var layerColors = colorbrewer["RdYlGn"][<%= layer.Agrupadors %>];
            var parameter = <%= layer.Parameter == null ? "null" : "'"+layer.Parameter+"'" %>;
            var parameterDesc = <%= layer.ParameterDesc == null ? "null" : "'"+layer.ParameterDesc+"'" %>;
            var title = '<%= layer.Name %>';
            var layertype = '<%= layer.Layertype %>';
            var startVisible = <%= layer.StartVisible.ToString().ToLower() %>;

            // Si es tracta d'una capa de punts, crea un mapa de calor
            if (layertype == "POINT"){
                map = heatmap(json, map, parameter);
                var layerColors = colorbrewer["Set1"][<%= layer.Agrupadors %>];
            }

            var vectorlayer = creaCapa(json, map, colorSpacing, min, layerColors, parameter, title, layertype, startVisible);

            var distinctValues = getDistinctValues(json, parameter, parameterDesc);

            if ('<%= ViewData["LegendLayer"] %>' == title && layertype == "POINT"){
                creaLlegendaCategorica(layerColors, distinctValues);
            }
            if ('<%= ViewData["LegendLayer"] %>' == title && layertype == "POLYGON"){
                creaLlegendaContinua(layerColors, min, colorSpacing);
            }
            layers.push({layer:vectorlayer,layertitle:title,data:json});
        <%
        } %>

        // Càrrega del cercador
        <% if (ViewData["Cercador"] != null){ %>;
            var jsondataFiltered;
            var currentlayer = $.grep(layers, function(e){ return e.layertitle == "<%= ViewData["Cercador"] %>"; })[0];

            $("#cerca").removeClass("hidden");
            $('#cerca').keypress(function(event){
              if(event.keyCode == 13){
                $('#btnCerca').click();
              }
            });

            function cerca(str, field){
                jsondataFiltered = clone(currentlayer.data);
                jsondataFiltered.features = $.grep(jsondataFiltered.features, function(element, index){return element.properties[field].toLowerCase().search(str.toLowerCase())>-1});
                jsondataFiltered.totalFeatures = jsondataFiltered.features.length;
                if (str == ""){
                    jsondataFiltered = clone(currentlayer.data);
                }
                currentlayer.layer.setSource(new ol.source.Vector({
                    projection: 'EPSG:3857',
                    features: (new ol.format.GeoJSON()).readFeatures(jsondataFiltered)
                }));
            }
            var fields = Object.keys(currentlayer.data.features[0].properties);
            for (var key in fields){
                $("#selCerca").append("<option val="+fields[key]+">"+fields[key]+"</option>");
            }

            $("#btnCerca").click(function(){cerca($("#txtCerca").val(), $("#selCerca").val())});
        <%} %>

        // Càrrega de chart D3
        <% if (ViewData["BarChart"] != null){ %>
            var currentlayer = $.grep(layers, function(e){ return e.layertitle == "<%= ViewData["BarChart"] %>"; })[0];
            creaBarChart(currentlayer.data, parameter, max);
        <% } %>

        // Càrrega de finestra emergent
        var element = document.getElementById('popup');
        var popup = new ol.Overlay({
            element: element
        });
        map.addOverlay(popup);

        map.on('click', function(evt) {
            var element = popup.getElement();
            var coordinate = evt.coordinate;

            $(element).popover('destroy');
            popup.setPosition(coordinate);

            var feature = map.forEachFeatureAtPixel(evt.pixel, function(feature, layer) {
                return feature;
            });

            if (feature) {
                var props = feature.getProperties();
                <%= ViewData["PopupContent"] %>
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

        // Redibuixa el mapa (única manera d'arreglar bug de click de features desplaçat)
        map.updateSize();
        
    </script>

</asp:Content>

