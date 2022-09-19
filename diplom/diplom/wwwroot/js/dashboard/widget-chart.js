'use strict';
$(document).ready(function() {
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
            "labelFunction": function(value) {
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
            "valueField": "market1",
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
            "valueField": "market2",
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
        "dataProvider": [{
            "date": "2013-01-16",
            "market1": 71,
            "market2": 75
        }, {
            "date": "2013-01-17",
            "market1": 80,
            "market2": 84
        }, {
            "date": "2013-01-18",
            "market1": 78,
            "market2": 83
        }, {
            "date": "2013-01-19",
            "market1": 85,
            "market2": 88
        }, {
            "date": "2013-01-20",
            "market1": 87,
            "market2": 85
        }, {
            "date": "2013-01-21",
            "market1": 97,
            "market2": 88
        }, {
            "date": "2013-01-22",
            "market1": 93,
            "market2": 88
        }, {
            "date": "2013-01-23",
            "market1": 85,
            "market2": 80
        }, {
            "date": "2013-01-24",
            "market1": 90,
            "market2": 85
        }]
    });
});
