"use strict";
(function() {
	//var chartDatums = [];

	//$(function() {
	//	var bloodhound = new Bloodhound({
	//		datumTokenizer: function(resource) {
	//			var result = [];
	//			$.each(['Name', 'ResourceName', 'ResourceType'], function(idx, property) {
	//				$.merge(result, Bloodhound.tokenizers.whitespace(resource[property]));
	//			});
	//			return result;
	//			//Bloodhound.tokenizers.obj.whitespace('Name')
	//		},
	//		queryTokenizer: Bloodhound.tokenizers.whitespace,
	//		local: []
	//	});
	//	bloodhound.initialize();
	//	var adapter = bloodhound.ttAdapter();

	//	$('#chartlist').typeahead({
	//			hint: false,
	//			highlight: true,
	//			minLength: 0
	//		},
	//		{
	//			name: 'charts',
	//			displayKey: function(chartData) {
	//				return chartData.Name;
	//			},
	//			source: function(query,cb) {
	//				//from https://github.com/twitter/typeahead.js/pull/719#issuecomment-43083651
	//				if(query=="") {
	//					cb(chartDatums);
	//					return;
	//				}
	//				adapter(query,cb);
	//			},
	//			templates: {
	//				suggestion: Handlebars.compile('<strong>{{Name}}</strong><br/><small>Name: {{ResourceName}}</small><br/><small>Type: {{ResourceType}}</small>')
	//			}
	//		}
	//	);
		
	//	//$('#chartlist').on('focus', function() {
	//	//	//from http://jsfiddle.net/5xxYq/
	//	//	var ev = $.Event("keydown");
	//	//	ev.keyCode = ev.which = 40;
	//	//	$(this).trigger(ev);
	//	//	return true;
	//	//});

	//	$('#chartlist').on('typeahead:selected', function(ev, chartdata) {
	//		$(this).typeahead('val', '');
	//		$.ajax({
	//			url: '/dashboard/add-chart',
	//			data: {
	//				uri: chartdata.Uri
	//			},
	//			method: 'post',
	//		})
	//		.done(function() {
	//			console.log(arguments);
	//			window.Charts.Chart.FromURI(chartdata.Uri).Render();
	//		})
	//		.fail(function(xhr) {
	//			alertXhrError('failed adding to dashboard',xhr);
	//		});
	//	});

	//	getCachedAjax('/api/list-all-charts',function(data) {
	//		chartDatums = data;
	//		bloodhound.clear();
	//		bloodhound.add(chartDatums);
	//	})
	//	.fail(function(xhr) {
	//		alertXhrError('failed fetching charts',xhr);
	//	});

	//	var alertXhrError = function(msg, xhr) {
	//		alert(msg+': '+xhr.statusText + ' ('+xhr.status+')');
	//	}
	//});

	var addChartsModel = function () {
		var toAdd = [];
		var me = this;
		var onSelected = [];
		var onDeselected = [];


		var raise = function (callbacks, chart) {
			$.each(callbacks, function (idx, cb) {
				cb(chart);
			});
		}

		var hasChart = function (chart) {
			var count = $.grep(toAdd, function (item) {
				return item.Uri == chart.Uri;
			}).length;
			return count >= 1;
		}

		this.add = function (chart) {
			if (hasChart(chart)) {
				return;
			}
			toAdd.push(chart);
			raise(onSelected, chart);
		}

		this.remove = function (chart) {
			if (!hasChart(chart)) {
				return;
			}
			toAdd = $.grep(toAdd, function (item) {
				return item.Uri != chart.Uri;
			});
			
			raise(onDeselected, chart);
		}

		this.toggle = function (chart) {
			if (me.contains(chart)) {
				me.remove(chart);
				return;
			}

			me.add(chart);
		}

		this.onSelected = function (cb) {
			onSelected.push(cb);
		}

		this.onDeselected = function (cb) {
			onDeselected.push(cb);
		}

		var addChartApi = function (chart) {
			return $.ajax({
				url: '/dashboard/add-chart',
				data: {
					uri: chart.Uri
				},
				method: 'post',
			});
		}

		this.commit = function () {
			var when = [];
			$.each(toAdd, function (idx,chart) {
				when.push(
					addChartApi(chart)
					.then(function () {
						return window.Charts.Chart.FromURI(chart.Uri).Render();
					})
				);
			});
			return when;
		}

		this.clear = function () {
			$.each(toAdd, function (idx, chart) {
				me.remove(chart);
			});
		}

		this.contains = hasChart;
	}


	var initChartList = function () {
		var toAddModel = new addChartsModel();
		$.ajax({
			url: '/api/list-all-charts'
		})
		.done(function (result) {
			var $chartContainer = $('.available-charts');

			$.each(result, function (idx, chart) {
				var $row = $('<a href="#" class="list-group-item">' + chart.Name + '</a>');
				
				toAddModel.onSelected(function (selectedChart) {
					if(selectedChart.Uri != chart.Uri) {
						return;
					}
					$row.addClass('active');
				});

				toAddModel.onDeselected(function (selectedChart) {
					if (selectedChart.Uri != chart.Uri) {
						return;
					}
					$row.removeClass('active');
				});

				$row.on('click', function (ev) {
					ev.preventDefault();
					
					toAddModel.toggle(chart);
				});

				$chartContainer.append($row);
			});

		});

		return toAddModel;
	}


	$(function () {
		var toAddModel = initChartList();

		var $theModal =$('#add-to-chart-modal');

		$('.add-to-dashboard').on('click', function () {
			$theModal.modal('show');
		});

		$('.add-to-dashboard-submit').on('click', function () {
			$.when.apply($,toAddModel.commit()).done(function () {
				toAddModel.clear();

				$theModal.hide();
			});
		});
	});
})();