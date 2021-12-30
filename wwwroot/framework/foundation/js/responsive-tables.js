$(document).ready(function () {
    var switched = false;

    function updateTables() {
        if (($(window).width() < 1295) && !switched) {
            switched = true;

            if ($(".responsive").length > 0) {
                $("table.responsive").each(function (i, element) {
                    splitTable($(element));
                });
            }

            if ($(".responsive-anchored").length > 0) {
                $("table.responsive-anchored").each(function (i, element) {
                    splitAnchoredTable($(element));
                });
            }
        }
        else if (switched && ($(window).width() > 1295)) {
            switched = false;

            if ($(".responsive").length > 0) {
                $("table.responsive").each(function (i, element) {
                    unsplitTable($(element));
                });
            }

            if ($(".responsive-anchored").length > 0) {
                $("table.responsive-anchored").each(function (i, element) {
                    unsplitTable($(element));
                });
            }
        }
    }

    $(window).on('load', updateTables());

    $(window).on("redraw", function () {
        switched = false;
        updateTables();
    });

    $(window).on("resize", updateTables);


    function splitTable(original) {
        original.wrap("<div class='table-wrapper' />");
        original.wrap("<div class='scrollable' />");
    }

    function splitAnchoredTable(original) {
        original.wrap("<div class='table-wrapper' />");

        var copy = original.clone();
        copy.find("td:not(:first-child), th:not(:first-child)").css("display", "none");
        copy.removeClass("responsive-anchored");

        original.closest(".table-wrapper").append(copy);
        copy.wrap("<div class='pinned' />");
        original.wrap("<div class='scrollable' />");

        setCellHeights(original, copy);
    }

    function unsplitTable(original) {
        original.closest(".table-wrapper").find(".pinned").remove();
        original.unwrap();
        original.unwrap();
    }

    function setCellHeights(original, copy) {
        var tr = original.find('tr'),
            tr_copy = copy.find('tr'),
            heights = [];

        tr.each(function (index) {
            var self = $(this),
                tx = self.find('th, td');

            tx.each(function () {
                var height = $(this).outerHeight(true);
                heights[index] = heights[index] || 0;
                if (height > heights[index]) heights[index] = height;
            });

        });

        tr_copy.each(function (index) {
            $(this).height(heights[index]);
        });
    }
});