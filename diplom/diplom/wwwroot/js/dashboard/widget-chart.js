'use strict';
$(document).ready(function () {
    function getData() {
        $.ajax({
            type: "GET",
            url: 'https://localhost:7170/api/sectors/stats',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log(response["$values"]);
                drawChart(response["$values"]);
            },
        });
    }
    getData();

    function drawChart(data) {
        // deal-analytic-chart
        var chart = AmCharts.makeChart("deal-analytic-chart", {
            "type": "serial",
            "theme": "light",
            "dataDateFormat": "YYYY-MM-DD",
            "precision": 2,
            "valueAxes": [{
                "id": "v1",
                "position": "left",
                "autoGridCount": false,
                "labelFunction": function (value) {
                    return "$" + Math.round(value) + "M";
                }
            }, {
                "id": "v2",
                "gridAlpha": 0,
                "autoGridCount": false
            }],
            "graphs": [{
                "id": "g1",
                "valueAxis": "v2",
                "bullet": "round",
                "bulletBorderAlpha": 1,
                "bulletColor": "#FFFFFF",
                "bulletSize": 8,
                "hideBulletsCount": 50,
                "lineThickness": 3,
                "lineColor": "#2ed8b6",
                "title": "Market Days",
                "useLineColorForBulletBorder": true,
                "valueField": "Consumer",
                "balloonText": "[[title]]<br /><b style='font-size: 130%'>[[value]]</b>"
            }, {
                "id": "g2",
                "valueAxis": "v2",
                "bullet": "round",
                "bulletBorderAlpha": 1,
                "bulletColor": "#FFFFFF",
                "bulletSize": 8,
                "hideBulletsCount": 50,
                "lineThickness": 3,
                "lineColor": "#e95753",
                "title": "Market Days ALL",
                "useLineColorForBulletBorder": true,
                "valueField": "It",
                "balloonText": "[[title]]<br /><b style='font-size: 130%'>[[value]]</b>"
            }],
            "chartCursor": {
                "pan": true,
                "valueLineEnabled": true,
                "valueLineBalloonEnabled": true,
                "cursorAlpha": 0,
                "valueLineAlpha": 0.2
            },
            "categoryField": "date",
            "categoryAxis": {
                "parseDates": true,
                "dashLength": 1,
                "minorGridEnabled": true
            },
            "legend": {
                "useGraphSettings": true,
                "position": "top"
            },
            "balloon": {
                "borderThickness": 1,
                "shadowAlpha": 0
            },
            "dataProvider": data
        });
    }
});
