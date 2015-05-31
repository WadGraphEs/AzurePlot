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

	var DashboardChart = function (chartInfoPromise) {
		this.chartInfoPromise = chartInfoPromise;
	}

	DashboardChart.prototype = {
		Render: function () {
			var me = this;
			return this.chartInfoPromise.then(function (result) {
				me.chartInfo = result;
				me.chart = Chart.FromURI(result.Uri);

				return me.chart.Render().done(function () {
					me.setupEvents();
				});
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

	DashboardChart.FromId = function (id) {
		return new DashboardChart($.ajax({
			url: '/dashboard/get-chart-info?id=' + id,
			method: 'get'
		}));
	}

	DashboardChart.FromData = function (data) {
		var def = $.Deferred();
		def.resolve(data);
		return new DashboardChart(def);
	}

	$.extend(true,window,{ Charts: { Chart: Chart, DashboardChart: DashboardChart  }});

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