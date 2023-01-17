'use strict';
$(document).ready(function () {
    function getData() {
        $.ajax({
            type: "GET",
            url: 'https://localhost:7170/api/sectors/count',
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
        am4core.useTheme(am4themes_animated);

        var chart = am4core.create("pay-analytic-chart", am4charts.PieChart);
        chart.language.locale = am4lang_ru_RU;
        chart.data = data;

        var pieSeries = chart.series.push(new am4charts.PieSeries());
        pieSeries.dataFields.value = "Count";
        pieSeries.dataFields.category = "Sector";
        pieSeries.labels.template.disabled = true;

        chart.legend = new am4charts.Legend();
    }
});
