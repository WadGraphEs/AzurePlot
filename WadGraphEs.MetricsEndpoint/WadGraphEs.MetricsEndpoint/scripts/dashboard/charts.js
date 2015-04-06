"use strict";
(function() {
	

	var drawChart = function(data) {
		
	}

	

	var Chart = function(uri) {
		this.uri = uri;
	}

	Chart.prototype = {
		Render: function() {
			var me = this;
			getCachedAjax('/api/charts/get-chart-data?uri='+this.uri, function(data) {
				me.Draw(data);
			})
			.fail(function(xhr) {
				alert("Failed getting chart data " +xhr.status);
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