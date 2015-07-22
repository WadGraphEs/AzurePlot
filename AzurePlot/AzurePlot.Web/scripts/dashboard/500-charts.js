"use strict";
(function (dashboard) {
	var Chart = function(uri) {
        var $chart,$loader,interval;

		$AssertChartElementAvailable();

		setNotRendered();

		var setInterval = function (newInterval) {
		    interval = newInterval;
		}

		var buildUri = function () {
		    return ChartUriBuilder.FromUri(uri).WithInterval(interval).Build();
		}

	    var render = function () {
	        var uri = buildUri();

	        setLoading();

	        var me = this;
	        return $.ajax({
	            url: '/api/charts/get-chart-data?uri=' + encodeURIComponent(uri)
	        })
			.done(function (data) {
			    draw(data);
			})
			.fail(function(result) {
			    displayError(JSON.parse(result.responseText));
			});
			
	    }

	    function $AssertChartElementAvailable() {
	        if(!$chart) {
	            initChartElement();
	        }
	    }

	    function initChartElement() {
	        $chart = $getChartElement();

	        $chart.hover(function () {
	            $chart.addClass('hover');
	        }, function () {
	            $chart.removeClass('hover');
	        });

	        $('#chart-container').append($chart);
	    }

	    var draw = function(data) {
	        setLoaded();

	        var series = $.map(data.Series, function (serie) {
	            return {
	                name: serie.Name,
	                data: $.map(serie.DataPoints, function (point) {
	                    return [[
							Common.DateTime.FromISOUTCString(point.Timestamp).AsJSDate().getTime(),
							point.Value
	                    ]];
	                })
	            }
	        });

	        $chart.find('.chart-area').highcharts({
	            plotOptions: {
	                series: {
	                    animation: false
	                }
	            },
	            title: {
	                text: data.Name
	            },
	            xAxis: {
	                type: 'datetime'
	            },
	            yAxis: {
	                min: 0,
	            },
	            series: series
	        })
	    }

	    var displayError = function(msg) {
	        new ErrorBox($chart, msg.Message, msg.ExceptionMessage);
	    }

	    var onRemoveClick = function (cb) {
	        $chart.find('.dropdown .remove-chart').on('click', cb);
	    }
	    var remove = function () {
	        $chart.remove();
	    }
	    function setNotRendered() {
	    }
	    var setLoading = function() {
	        assertLoaderElement();
	        $chart.removeClass('loaded');
	        $chart.find('.load-error-box').remove();
	    }
	    var setLoaded = function() {
	        $chart.addClass('loaded');
	    }
	    var assertLoaderElement = function () {
	        if($loader) {
	            return;
	        }
	        var $el = $('<div class="chart-loader"></div>');
	        $el.css({
	            width: $chart.width(),
	            height: $chart.height(),
	        });
	        $chart.append($el);
	        $loader = $el;
	    }

	    this.SetInterval = setInterval;
	    this.Render = render;
        this.OnRemoveClick = onRemoveClick;
        this.Remove = remove;
	}

	
	Chart.FromURI = function(uri) {
		return new Chart(uri);
	}

	var ChartUriBuilder = function (uri, interval) {
	    if(uri.indexOf("?")>=0) {
	        throw "uri can't contain path" + uri;
	    }
	    this.WithInterval = function (newInterval) {
            return new ChartUriBuilder(uri,newInterval)
	    }

	    this.Build = function () {
	        var uriBuilder = uri;
	        if (interval) {
	            uriBuilder += "?interval=" + interval.value + "&unit=" + interval.unit;
	        }
	        return uriBuilder;
	    }
	}

	ChartUriBuilder.FromUri = function (uri) {
	    return new ChartUriBuilder(uri);
	}

	var DashboardChart = function (chartInfo) {
	    var chart = Chart.FromURI(chartInfo.Uri);

	    chart.SetInterval(dashboard.IntervalSelector.getCurrentInterval());

		setupEvents();

		function render() {
		    return chart.Render();
		}

		function setupEvents() {
		    dashboard.IntervalSelector.onIntervalChanged(function (newInterval) {
		        chart.SetInterval(newInterval);
		        chart.Render();
		    });

	        chart.OnRemoveClick(function (ev) {
	            ev.preventDefault();
	            chart.Remove();
	            remove();
	        });
	    }

	    function remove() {
	        $.ajax({
	            'url': '/dashboard/remove-chart',
	            'data': { chartId: chartInfo.Id },
	            'method': 'post'
	        });
	    }

	    this.Render = render;
	}
	
	DashboardChart.FromData = function (data) {
		return new DashboardChart(data);
	}

	$.extend(true,window,{ Charts: { DashboardChart: DashboardChart  }});

	$(function() {
		$('.load-chart').each(function(idx, item) {
			$(this).remove();
			return DashboardChart.FromData(JSON.parse(item.value)).Render();
		});
	});

	var $getChartElement = function () {
		var $result = $('#chart-template').clone();
		
		return $result.attr('id', '');
	}
})(window.Dashboard);