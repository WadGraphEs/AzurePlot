"use strict";
(function () {
	var Chart = function(uri) {
		this.uri = uri;
		this.$AssertChartElementAvailable();
	}

	Chart.prototype = {
		Render: function () {
			this.showLoader();

			var me = this;
			return $.ajax({
				url: '/api/charts/get-chart-data?uri=' + this.uri
			})
			.done(function (data) {
				me.Draw(data);
			});
			
		},
		showLoader: function () {
			var $el = $('<div class="chart-loader"></div>');
			$el.css({
				width: this.$chart.width(),
				height: this.$chart.height(),
			});
			this.$chart.append($el);
		},
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

			$chart.addClass('loaded');

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

			//console.log(series);

			$chart.find('.chart-area').highcharts({
				title: {
					text: data.Name
				},
				xAxis: {
					type: 'datetime'
				},
				yAxis: {
					
				},
				series: series
			})
		},
		onRemoveClick: function (cb) {
			this.$chart.find('.dropdown .remove-chart').on('click', cb);
		},
		remove: function () {
			this.$chart.remove();
		}
	}

	Chart.FromURI = function(uri) {
		return new Chart(uri);
	}

	var DashboardChart = function (chartInfo) {
		this.chartInfo = chartInfo;
	}

	DashboardChart.prototype = {
		Render: function () {
			var me = this;

			me.chart = Chart.FromURI(this.chartInfo.Uri);
			me.setupEvents();
			return me.chart.Render();
		},
		setupEvents: function () {
			var me = this;
			this.chart.onRemoveClick(function (ev) {
				ev.preventDefault();
				me.chart.remove();
				me.remove();
			});
		},
		remove: function () {
			$.ajax({
				'url': '/dashboard/remove-chart',
				'data': { chartId: this.chartInfo.Id },
				'method': 'post'
			});
		},
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
})();