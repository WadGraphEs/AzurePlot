"use strict";
(function() {
	var usagesStorageKey = 'usages';

	var storeUsages = function(data) {
		if(!localStorage) {
			return;
		}
		localStorage.setItem(usagesStorageKey, JSON.stringify(data));
	}

	var getUsagesFromStorage = function() {
		if(!localStorage) {
			return [];
		}
		var value = localStorage.getItem(usagesStorageKey);
		if(!value) {
			return [];
		}

		return JSON.parse(value);
	}

	var groupByCounterName = function(data) {
		var counters = {};
		$.each(data, function(idx, datapoint) {
			if(!(datapoint.GraphiteCounterName in counters)) {
				counters[datapoint.GraphiteCounterName] = {
					name: datapoint.GraphiteCounterName,
					datapoints: []
				};
			}

			var counterData = counters[datapoint.GraphiteCounterName];
			counterData.datapoints.push(datapoint);
		});

		return counters;
	}

	var drawUsages = function(data) {
		var $chartContainer = $('#chart-container');
		var counters = groupByCounterName(data);
		$.each(counters, function(propertyName, counterData) {
			if(!counters.hasOwnProperty(propertyName)) {
				return;
			}
			
			var $chart = $('<div style="width:500px;height:400px;float:left;"></div>');
			$chartContainer.append($chart);
			$chart.highcharts({
				title: {
					text: counterData.name
				},
				xAxis: {
				},
				yAxis: {
					
				},
				series: [{
					name: counterData.name,
					data: $.map(counterData.datapoints, function(point) { return point.Value; })
				}]
			});
		});
	}

	var fetchData = function() {
		$.ajax({
			url: '/usages',
			type: 'get',
			dataType: 'json',
		})
		.fail(function() {
			alert("error");
		})
		.done(function(data) {
			storeUsages(data);
		});
	}

	$(function() {
		fetchData();
		drawUsages(getUsagesFromStorage());
	});
})();