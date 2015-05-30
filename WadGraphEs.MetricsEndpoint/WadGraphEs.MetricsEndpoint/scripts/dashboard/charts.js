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
				this.$chart = $('<div class="chart"></div>');
				$('#chart-container').append(this.$chart);
			}
			return this.$chart;		
		},
		Draw: function(data) {
			var $chart = this.$AssertChartElementAvailable();

			$chart.highcharts({
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
		}
	}

	Chart.FromURI = function(uri) {
		return new Chart(uri);
	}

	$.extend(true,window,{ Charts: { Chart: Chart }});
})();