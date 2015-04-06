"use strict";
(function() {

	//var substringMatcher=function() {
	//	return function findMatches(q,cb) {
	//		var matches,substrRegex;

	//		// an array that will be populated with substring matches
	//		matches=[];

	//		// regex used to determine if a string contains the substring `q`
	//		substrRegex=new RegExp(q,'i');

	//		// iterate through the pool of strings and for any string that
	//		// contains the substring `q`, add it to the `matches` array
	//		$.each(charts,function(i,chart) {
	//			if(substrRegex.test(chart.Name)) {
	//				// the typeahead jQuery plugin expects suggestions to a
	//				// JavaScript object, refer to typeahead docs for more info
	//				matches.push(chart);
	//			}
	//		});

	//		cb(matches);
	//	};
	//};

	//var charts=[];

	var chartData = [];

	$(function() {
		var bloodhound = new Bloodhound({
			datumTokenizer: function(resource) {
				var result = [];
				$.each(['Name', 'ResourceName', 'ResourceType'], function(idx, property) {
					$.merge(result, Bloodhound.tokenizers.whitespace(resource[property]));
				});
				return result;
				//Bloodhound.tokenizers.obj.whitespace('Name')
			},
			queryTokenizer: Bloodhound.tokenizers.whitespace,
			local: []
		});
		bloodhound.initialize();
		var adapter = bloodhound.ttAdapter();

		$('#chartlist').typeahead({
				hint: false,
				highlight: true,
				minLength: 0
			},
			{
				name: 'states',
				displayKey: 'value',
				source: function(query,cb) {
					//from https://github.com/twitter/typeahead.js/pull/719#issuecomment-43083651
					if(query=="") {
						cb(chartData);
						return;
					}
					adapter(query,cb);
				},
				templates: {
					suggestion: Handlebars.compile('<strong>{{Name}}</strong><br/><small>Name: {{ResourceName}}</small><br/><small>Type: {{ResourceType}}</small>')
				}
			}
		);
		
		$('#chartlist').on('focus', function() {
			//from http://jsfiddle.net/5xxYq/
			var ev = $.Event("keydown");
			ev.keyCode = ev.which = 40;
			$(this).trigger(ev);
			return true;
		});

		getCachedAjax('/api/list-all-charts',function(data) {
			bloodhound.clear();
			chartData = data;
			bloodhound.add(chartData);
		})
		.fail(function(xhr) {
			alert('failed fetching charts: '+xhr.statusText + ' ('+xhr.status+')');
		});
	});
})();