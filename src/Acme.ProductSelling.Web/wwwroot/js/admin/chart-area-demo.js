Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';
var l = abp.localization.getResource('ProductSelling');

function number_format(number, decimals, dec_point, thousands_sep) {

    number = (number + '').replace(',', '').replace(' ', '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}
var ctx = document.getElementById("myAreaChart");
function initializeChart() {
    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: allYearlyData[selectedYear].map(item => item.month),
            datasets: [{
                label: l('Admin:Statistics:MonthlyRevenueByYear') + ' ' + selectedYear,
                lineTension: 0.3,
                backgroundColor: "rgba(78, 115, 223, 0.05)",
                borderColor: "rgba(78, 115, 223, 1)",
                pointRadius: 3,
                pointBackgroundColor: "rgba(78, 115, 223, 1)",
                pointBorderColor: "rgba(78, 115, 223, 1)",
                pointHoverRadius: 3,
                pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
                pointHoverBorderColor: "rgba(78, 115, 223, 1)",
                pointHitRadius: 10,
                pointBorderWidth: 2,
                data: allYearlyData[selectedYear].map(item => item.moneyTotal),
            }],
        },
        options: {
            maintainAspectRatio: false,
            responsive: true,
            animation: {
                duration: 1000,
                easing: 'easeInOutQuart'
            },
            layout: {
                padding: {
                    left: 10,
                    right: 25,
                    top: 25,
                    bottom: 0
                }
            },
            scales: {
                xAxes: [{
                    time: {
                        unit: 'date'
                    },
                    gridLines: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        maxTicksLimit: 12,
                        fontStyle: 'bold',
                    }
                }],
                yAxes: [{
                    ticks: {
                        //maxTicksLimit: 10,
                        min: 0,
                        max: 1000000000,
                        stepSize: 100000000,
                        padding: 10,
                        fontStyle: 'bold',
                        // Include a dollar sign in the ticks
                        callback: function (value, index, values) {
                            return number_format(value) + ' VND';
                        }
                    },
                    gridLines: {
                        color: "rgb(234, 236, 244)",
                        zeroLineColor: "rgb(234, 236, 244)",
                        drawBorder: true,
                        borderDash: [2],
                        borderWidth: 2,
                        zeroLineBorderDash: [2]
                    }
                }],
            },
            legend: {
                display: false
            },
            tooltips: {
                backgroundColor: "rgb(255,255,255)",
                bodyFontColor: "#858796",
                titleMarginBottom: 10,
                titleFontColor: '#6e707e',
                titleFontSize: 14,
                borderColor: '#dddfeb',
                borderWidth: 1,
                xPadding: 15,
                yPadding: 15,
                displayColors: false,
                intersect: false,
                mode: 'index',
                caretPadding: 10,
                callbacks: {
                    label: function (tooltipItem, chart) {
                        var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                        return datasetLabel + ': ' + number_format(tooltipItem.yLabel) + ' VND';
                    }
                }
            }
        }
    });
}

function changeYear() {
    var yearSelect = document.getElementById('yearSelect');
    var newYear = parseInt(yearSelect.value);

    document.querySelector('.chart-loading').style.display = 'block';
    document.getElementById('myAreaChart').style.opacity = '0.5';

    selectedYear = newYear;

    var yearData = allYearlyData[newYear] || [];

    document.getElementById('chartTitle').textContent = l('Admin:Statistics:StatisticsForYear') +': ' + newYear;

    var yearTotal = yearData.reduce((sum, month) => sum + month.moneyTotal, 0);
    document.getElementById('yearTotal').textContent = l('Admin:Statistics:TotalRevenueForYear')+' ' + newYear + ': ' + number_format(yearTotal) + ' VND';

    setTimeout(() => {
        initializeChart(yearData);
        document.querySelector('.chart-loading').style.display = 'none';
        document.getElementById('myAreaChart').style.opacity = '1';
    }, 300);
}

document.addEventListener('DOMContentLoaded', function () {
    initializeChart();
});

function refreshWithYear() {
    var yearSelect = document.getElementById('yearSelect');
    var selectedYear = yearSelect.value;
    window.location.href = '/admin?year=' + selectedYear;
}