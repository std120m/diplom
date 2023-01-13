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
        am4core.useTheme(am4themes_animated);

        var chart = am4core.create("deal-analytic-chart", am4charts.XYChart);
        chart.language.locale = am4lang_ru_RU;
        chart.data = data;

        chart.dateFormatter.inputDateFormat = "MM/dd/yyyy";

        var categoryAxis = chart.xAxes.push(new am4charts.DateAxis());
        categoryAxis.renderer.grid.template.location = 0;
        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

        var sectorsNamesReadable = {};
        sectorsNamesReadable["Consumer"] = "Потребительские товары и услуги";
        sectorsNamesReadable["It"] = "Информационные технологии";
        sectorsNamesReadable["Health_care"] = "Здравоохранение";
        sectorsNamesReadable["Green_energy"] = "Зеленая энергетика";
        sectorsNamesReadable["Ecomaterials"] = "Материалы для эко-технологи";
        sectorsNamesReadable["Real_estate"] = "Недвижимость";
        sectorsNamesReadable["Materials"] = "Сырьевая промышленность";
        sectorsNamesReadable["Telecom"] = "Телекоммуникации";
        sectorsNamesReadable["Financial"] = "Финансовый сектор";
        sectorsNamesReadable["Electrocars"] = "Электротранспорт и комплектующие";
        sectorsNamesReadable["Utilities"] = "Электроэнергетика";
        sectorsNamesReadable["Energy"] = "Энергетика";
        sectorsNamesReadable["Green_buildings"] = "Энергоэффективные здания";

        Object.keys(data[0]).forEach(element => {
            if (Object.keys(sectorsNamesReadable).includes(element)) {
                var series = chart.series.push(new am4charts.LineSeries());
                series.dataFields.valueY = element;
                series.dataFields.dateX = "date";
                series.name = sectorsNamesReadable[element];
                series.tooltipText = "{name}: [b]{valueY}[/]";
                series.strokeWidth = 2;
            }
        });

        chart.cursor = new am4charts.XYCursor();
        chart.legend = new am4charts.Legend();
    }
});
