var wportalapps = "https://portalapps.weber.edu";
var wapps = "https://apps.weber.edu";

$(document).ready(function () {
    // Fix for CAS login on mobile
    $(".login-btn").on("click", function () {
        $("#username").val($("#username").val().trim());
    });

    // Sets height of off canvas menu to height of window
    //windowHeight = $(window).height();
    //$('.inner-wrap').css('min-height', windowHeight);

    if ($(location).attr("href").indexOf("-dev.weber") >= 0) {
        wportalapps = "https://portalapps-dev.weber.edu";
        wapps = "https://apps-dev.weber.edu";
    } else if ($(location).attr("href").indexOf("-test.weber") >= 0) {
        wportalapps = "https://portalapps-test.weber.edu";
        wapps = "https://apps-test.weber.edu";
    }

    if ($('.hero').length) {
        $('.hero').slick({
            dots: false,
            autoplay: false,
            autoplaySpeed: 6000,
            slidesToShow: 1,
            fade: true,
            arrows: false,
            swipe: false
        });
    }



    // Makes content of <code> tags selected on click.
    jQuery.fn.selectText = function () {
        var doc = document
            , element = this[0]
            , range, selection
            ;
        if (doc.body.createTextRange) {
            range = document.body.createTextRange();
            range.moveToElementText(element);
            range.select();
        } else if (window.getSelection) {
            selection = window.getSelection();
            range = document.createRange();
            range.selectNodeContents(element);
            selection.removeAllRanges();
            selection.addRange(range);
        }
    };
    $(function () {
        $('code').click(function (e) {
            $(this).selectText();
        });
    });

    // Removes attributes from tags
    jQuery.fn.removeAttributes = function () {
        return this.each(function () {
            var attributes = $.map(this.attributes, function (item) {
                return item.name;
            });
            var img = $(this);
            $.each(attributes, function (i, item) {
                img.removeAttr(item);
            });
        });
    }

    // Adds style to left nav labels
    $('li[data-val="-1"]').addClass("left-nav-label");

    //Remove border from portalapps toolbar
    $(".BackgroundSecondary").removeAttr("style");


    $(".main-content *").css("font-size", "");
    $(".main-content *").css("font-family", "");
    $(".main-content *").removeAttr("font");
    $(".main-content *").removeAttr("face");
    $(".main-content *").removeAttr("size");
    $(".main-content *").removeAttr("pointsize");
    $(".main-content img").css("height", "");
    //$(".main-content font").removeAttributes();
    //$(".main-content span").removeAttributes();
    //$(".main-content div").removeAttributes();
    $(".main-content td, .main-content th, .main-content tbody, .main-content table, .main-content img, .template-header img, .main-content input").removeAttr("height");
    $(".main-content td, .main-content th, .main-content tbody, .main-content table, .main-content img, .template-header img, .main-content input").removeAttr("width");
    $(".main-content td, .main-content th, .main-content tbody, .main-content table, .main-content img, .template-header img, .main-content input").css("width", "");
    $(".main-content td, .main-content th, .main-content tbody, .main-content table, .main-content img, .template-header img, .main-content input").css("height", "");
    //$(".main-content *").removeAttr("style");
    $("#contentBody table").removeAttr("style");

    $(".main-content table").attr("width", "100%");

    $(".main-content td, .main-content th").each(function () {
        var $this = $(this);
        $this.html($this.html().replace(/&nbsp;/g, ' '));
    });

    //Active navigation page
    //topbar
    var sTopUrl = location.href.toLowerCase().split(".");

    $("nav .top-bar .menu li a").each(function () {
        var link = $.trim($(this).prop("href"));

        if (link.indexOf("#") < 1) {
            var sCurrAnchor = this.href.toLowerCase().split(".");

            if (sCurrAnchor[0].indexOf("www") < 1) { //case for lazy urls that skip the www.weber
                if (sTopUrl[1] === sCurrAnchor[1] && sCurrAnchor[1] !== "weber") {
                    AddActive($(this));
                    return false;
                }
            } else {
                if (sTopUrl[2] === sCurrAnchor[2]) {
                    AddActive($(this));
                    return false;
                }
            }
        }
    });

    function AddActive(e) {
        if (!e.parent().hasClass('is-dropdown-submenu-parent')) {
            e.parent().addClass("active");
            e.parents(".is-dropdown-submenu-parent").addClass("active-parent");
        } else {
            e.parent().addClass("active");
        }
    }

    // Prunes empty li's from the menu bars so that foundation doesn't render them in mobile view
    $('.top-bar li[role=menuitem]').filter(function () {
        return $.trim($(this).html()) === '';
    }).remove();

    //// Removes underline from image links with no text
    //$("a > img").parent().filter(function () { return $(this).text().trim() == ""; }).addClass("no-underline");

    //$(".portal-main-content .top-bar input[type=text]").keyup(function (e) {
    //    if (e.keyCode == 13) {
    //        $(this).parent().next().children().first().trigger("click");
    //    }
    //});

    //Spinner
    $(".btn-loader").on("click", function () {
        var src = $(this);

        $(window).bind('beforeunload', function () {
            DisableButton(src);
        });
    });

    function DisableButton(btn) {
        var subtext = '';

        if (btn.is('[data-submit-text]')) {
            subtext = btn.data('submit-text');
        } else {
            subtext = 'Submitting';
        }

        btn.attr("disabled", "true").addClass("disabled spinner").val(subtext);
    }
});

$(document).ready(function () {
    if ($(document).attr('title') === "Page Not Found") {
        $url = window.location.href;

        $('a[href^="mailto:webdevelopment@weber.edu?subject=404 on weber.edu"]').prop("href",
            'mailto:webdevelopment@weber.edu?subject=404 on ' + $url + '&body=Page not found for ' + $url + '%0D%0A%0D%0AOriginating from ' + document.referrer);
    }
});

function translateURL() {
    window.open("https://translate.google.com/translate?sl=en&tl=es&u=" + self.location, "translateWin", "width=900,height=600,left=20,top=20,scrollbars=1,menubar=0,resizable=1,location=0,toolbar=1");
}
