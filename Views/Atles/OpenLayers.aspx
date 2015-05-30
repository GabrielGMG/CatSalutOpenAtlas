<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $( document ).ready(function() {

            map = init();

            // Capa OSM
            var osmSource = new ol.source.OSM();
            var osmLayer = new ol.layer.Tile({source: osmSource, title: 'OSM'});
            map.addLayer(osmLayer);

        // Tancament on ready
        }); 
    </script>
</asp:Content>
