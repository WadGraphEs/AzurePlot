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


	var initChartList = function () {
		$.ajax({
			url: '/api/list-all-charts'
		})
		.done(function (result) {
			var $chartContainer = $('.available-charts');
			$.each(result, function (idx, chart) {
				$chartContainer.append('<a href="#" class="list-group-item">' + chart.Name + '</a>');
			});

		});
	}


	$(function () {
		
		initChartList();

		$('.add-to-dashboard').on('click', function () {
			$('#add-to-chart-modal').modal();
		});
	});
})();