$(document).ready(function () {
    if ($("#navDropdown .ContainerPanel").length <= 0) { //check that you are not in edit mode
        $("#navDesktop ul").addClass("side-nav");

        if ($(".side-nav").length > 0) {
            //console.log("has side menu");

            if ($("#main-menu").length <= 0) {
                //console.log("no main menu");

                if ($("#navMobile").length <= 0) {
                    //console.log("no mobile nav");

                    var sNav = "";

                    sNav += "<div class=\"top-bar-holder\">";
                    sNav += "<div class=\"row collapse\">";
                    sNav += "<div class=\"column\">";
                    sNav += "<div class=\"title-bar\" data-responsive-toggle=\"main-menu\" data-hide-for=\"medium-up\">";
                    sNav += "<div class=\"right\">";
                    sNav += "<button type=\"button\" data-toggle>";
                    sNav += "<img src=\"/ui/images/icons/random/drilldown-menu.svg\" alt=\"Navigation Menu\"></button>";
                    sNav += "</div>";
                    sNav += "</div>";
                    sNav += "<div class=\"top-bar\" id=\"main-menu\">";
                    sNav += "<div class=\"top-bar-right\">";
                    sNav += "<ul class=\"menu\" id=\"drilldown-menu\" data-auto-height=\"true\" data-responsive-menu=\"drilldown xlarge-dropdown\">";
                    sNav += "</ul>";
                    sNav += "</div>";
                    sNav += "</div>";
                    sNav += "</div>";
                    sNav += "</div>";
                    sNav += "</div>";

                    $("#navDropdown").html(sNav);

                    $("#navDropdown").addClass("show-for-medium-down");

                    if ($(".top-bar").length > 0) {
                        $("#navDesktop li").each(function () {
                            var lnk = "<li>" + $(this).html() + "</li>";
                            $("#drilldown-menu").append(lnk);
                        });

                        $(document).foundation();
                    }
                }
            }
            $("#navMobile a").addClass("nav-title");
        }
    }
});