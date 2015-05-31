"use strict";
(function() {
	var Chart = function(uri) {
		this.uri = uri;
	}

	Chart.prototype = {
		Render: function() {
			var me = this;
			return $.ajax({
				url: '/api/charts/get-chart-data?uri=' + this.uri
			})
			.done(function (data) {
				me.Draw(data);
			});
			
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
			var $chart = this.$AssertChartElementAvailable();

			$chart.find('.chart-area').highcharts({
				title: {
					text: data.Name
				},
				xAxis: {
				},
				yAxis: {
					
				},
				series: $.map(data.Series, function(serie) {
					return {
						name: serie.Name,
						data: $.map(serie.DataPoints, function(point) { return point.Value; })
					}
				})
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

			return me.chart.Render().done(function () {
				console.log(arguments);
				me.setupEvents();
			});
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