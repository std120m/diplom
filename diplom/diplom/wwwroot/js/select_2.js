"use strict";
$(document).ready(function() {
    $(".select2").select2().on("change", function (e) {
        shareIds = [];
        sectorIds = [];
        for (var i = 0; i < $(".select2 option:selected").length; i++) {
            shareIds.push($(".select2 option:selected")[i].value);
            sectorIds.push($(".select2 option:selected")[i].label);
        }
        console.log(sectorIds);
        if (shareIds.length > 0) {
            drawTrend(ChartTypes.TREND);
        }
        FilterSector();
    });
});