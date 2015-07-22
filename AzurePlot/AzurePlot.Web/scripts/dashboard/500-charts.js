"use strict";
(function (dashboard) {
	var Chart = function(uri) {
		this.$AssertChartElementAvailable();
		this.setNotRendered();

		var interval = null;

		var setInterval = function (newInterval) {
		    interval = newInterval;
		}

		var buildUri = function () {
		    return ChartUriBuilder.FromUri(uri).WithInterval(interval).Build();
		}

	    var render = function () {
	        var uri = buildUri();

	        this.setLoading();

	        var me = this;
	        return $.ajax({
	            url: '/api/charts/get-chart-data?uri=' + encodeURIComponent(uri)
	        })
			.done(function (data) {
			    me.Draw(data);
			})
			.fail(function(result) {
			    me.DisplayError(JSON.parse(result.responseText));
			});
			
	    }

	    this.SetInterval = setInterval;
	    this.Render = render;
	}

	Chart.prototype = {
		
		$AssertChartElementAvailable: function() {
			if(!this.$chart) {
				this.initChartElement();
			}
			return this.$chart;		
		},
		initChartElement: function() {
			var $chart = $getChartElement();

			$chart.hover(function () {
				$chart.addClass('hover');
			}, function () {
				$chart.removeClass('hover');
			});

			$('#chart-container').append($chart);

			this.$chart = $chart;
			
		},
		Draw: function(data) {
			var $chart = this.$chart;

			this.setLoaded();

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
		},
		DisplayError: function(msg) {
			new ErrorBox(this.$chart, msg.Message, msg.ExceptionMessage);
		},
		onRemoveClick: function (cb) {
			this.$chart.find('.dropdown .remove-chart').on('click', cb);
		},
		remove: function () {
			this.$chart.remove();
		},
		showLast: function(interval, unit) {
			this.Render(this.uri+"?interval="+interval+"&unit="+unit);
		},
		setNotRendered: function() {
		},
		setLoading: function() {
			this.assertLoaderElement();
			this.$chart.removeClass('loaded');
			this.$chart.find('.load-error-box').remove();
		},
		setLoaded: function() {
			this.$chart.addClass('loaded');
		},
		assertLoaderElement: function () {
			if(this.$loader) {
				return;
			}
			var $el = $('<div class="chart-loader"></div>');
			$el.css({
				width: this.$chart.width(),
				height: this.$chart.height(),
			});
			this.$chart.append($el);
			this.$loader = $el;
		}
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

	        chart.onRemoveClick(function (ev) {
	            ev.preventDefault();
	            chart.remove();
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