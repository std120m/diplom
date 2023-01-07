//const { log } = require("fullcalendar/src/main");

const Periods = { DAY: 'day', HOUR: 'hour' };
const ChartTypes = { CANDLES: 'candles', TREND: 'trend' };
let currentChartType = ChartTypes.TREND;
let shareIds = [5];
let currentChartPeriod = Periods.DAY;

google.charts.load('current', { 'packages': ['annotationchart'], 'language': 'ru' });
google.charts.setOnLoadCallback(function () { drawTrend(currentChartType) });

function drawTrend(currentChartType) {
    $.ajax({
        type: "GET",
        url: 'https://localhost:7170/home/getCandles',  
        data: {
            currentChartType: currentChartType,
            shareIds: shareIds
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            drawChart(response);
        },
    });
}

function drawChart(json) {
    let shares = json[1];
    let news = json[0];
    let data = new google.visualization.DataTable();
    
    let startDate = new Date(shares[0][1][0].Date);
    let finishDate = new Date(shares[0][1][0].Date);
    data.addColumn('date', 'Date');
    for (let i = 0; i < shares.length; i++) {
        data.addColumn('number', shares[i][0]);
        if (startDate > new Date(shares[i][1][0].Date))
            startDate = new Date(shares[i][1][0].Date);
        if (finishDate < new Date(shares[i][1][shares[i][1].length - 1].Date))
            finishDate = new Date(shares[i][1][shares[i][1].length - 1].Date);
    }
    let rows = [];
    for (let currentData = startDate; currentData < finishDate; currentData = addDays(currentData, 1)) {
        let row = [];
        let skip = true;
        row.push(currentData);
        for (let i = 0; i < shares.length; i++) {
            let shareName = shares[i][0];
            let candles = shares[i][1];
            let candle = candles.filter(currentCandle => new Date(currentCandle.Date).toDateString() == currentData.toDateString())[0];
            let previousCandle = null;
            if (rows.length && rows[rows.length - 1][i + 1]) {
                previousCandle = rows[rows.length - 1][i + 1];
            }
            row.push(candle ? candle.Low : previousCandle);
            if (candle)
                skip = false;
        }
        if (skip)
            continue;
        rows.push(row);
    }
    data.addRows(rows);

    let chart = new google.visualization.AnnotationChart(document.getElementById('chart_div'));
    let options = {
        displayAnnotations: false,
        displayAnnotationsFilter: true
    };
    google.visualization.events.addListener(chart, 'ready', onReady);
    google.visualization.events.addListener(chart, 'select', onSelectDate);
    chart.draw(data, options);
    document.getElementById("chart_div_AnnotationChart_zoomControlContainer_1-hour").remove();
    document.getElementById("chart_div_AnnotationChart_zoomControlContainer_1-day").remove();
    document.getElementById("chart_div_AnnotationChart_zoomControlContainer_5-days").remove();
    document.getElementById("chart_div_AnnotationChart_zoomControlContainer_3-months").remove();
    function onReady() {
        let x = document.getElementById('chart_div_AnnotationChart_zoomControlContainer').getElementsByTagName('button');
        document.getElementById('chart_div_AnnotationChart_zoomControlContainer').style.fontSize = '14px';
        for (let i = 0; i < x.length; i++) {
            x[i].style.height = '40px';
            x[i].style.width = '40px';
            x[i].style.fontSize = '14px';
        }
        //document.getElementById("chart_div_AnnotationChart_zoomControlContainer").innerHTML = document.getElementById("chart_div_AnnotationChart_zoomControlContainer").innerHTML.replace("Zoom:", "Масштаб:");
        document.getElementById("chart_div_AnnotationChart_zoomControlContainer_1-week").innerHTML = "&#1085;&#1077;&#1076;";
        document.getElementById("chart_div_AnnotationChart_zoomControlContainer_1-month").innerHTML = "&#1084;&#1077;&#1089;";
        document.getElementById("chart_div_AnnotationChart_zoomControlContainer_6-months").innerHTML = "6&#1084;&#1077;&#1089;";
        document.getElementById("chart_div_AnnotationChart_zoomControlContainer_1-year").innerHTML = "&#1075;&#1086;&#1076;";
    }
    function onSelectDate() {
        let selection = chart.getSelection();
        let i = selection[0].row;
        let date = rows[i][0];
        $.ajax({
            type: "GET",
            url: 'https://localhost:7170/api/news',
            data: {
                date: new Date(date).getFullYear() + '-' + (new Date(date).getMonth()+1) + '-' + new Date(date).getDate()
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                let newsContainer = document.getElementById('news_container');
                newsContainer.innerHTML = null;
                for (let i = 0; i < response.length; i++) {
                    let newsElement =
                        "<div class=\"feed-item\">" +
                            "<a href=\"/WorldNews/Details/" + response[i].Id + "\">" +
                                "<div class=\"feeds-body\">" +
                                    "<h4 class=\"title text-primary\">" +
                                        response[i].Title +
                                        "<small class=\"float-right text-muted\">" +
                                            new Date(response[i].DateTime).toLocaleDateString() +
                                        "</small>" +
                                    "</h4>" +
                                    "<small>" + response[i].Text.substring(0, 50) +"...</small>" +
                                "</div>" +
                            "</a>" +
                        "</div>";
                    $("#news_container").append(newsElement);
                }
            },
        });

    }

    function addHours(date, hours) {
        var result = new Date(date);
        result.setHours(result.getHours() + hours);
        return result;
    }

    function addDays(date, days) {
        var result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }
}