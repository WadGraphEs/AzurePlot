"use strict";
(function() {

	var substringMatcher=function() {
		return function findMatches(q,cb) {
			var matches,substrRegex;

			// an array that will be populated with substring matches
			matches=[];

			// regex used to determine if a string contains the substring `q`
			substrRegex=new RegExp(q,'i');

			// iterate through the pool of strings and for any string that
			// contains the substring `q`, add it to the `matches` array
			$.each(states,function(i,str) {
				if(substrRegex.test(str)) {
					// the typeahead jQuery plugin expects suggestions to a
					// JavaScript object, refer to typeahead docs for more info
					matches.push({ value: str });
				}
			});

			cb(matches);
		};
	};

	var states=[];
  //  ['Alabama','Alaska','Arizona','Arkansas','California',
  //'Colorado','Connecticut','Delaware','Florida','Georgia','Hawaii',
  //'Idaho','Illinois','Indiana','Iowa','Kansas','Kentucky','Louisiana',
  //'Maine','Maryland','Massachusetts','Michigan','Minnesota',
  //'Mississippi','Missouri','Montana','Nebraska','Nevada','New Hampshire',
  //'New Jersey','New Mexico','New York','North Carolina','North Dakota',
  //'Ohio','Oklahoma','Oregon','Pennsylvania','Rhode Island',
  //'South Carolina','South Dakota','Tennessee','Texas','Utah','Vermont',
  //'Virginia','Washington','West Virginia','Wisconsin','Wyoming'
  //  ];

	$(function() {
		$('#chartlist').typeahead({
				hint: false,
				highlight: true,
				minLength: 0
			},
			{
				name: 'states',
				displayKey: 'value',
				source: substringMatcher()
			}
		);


		getCachedAjax('/api/list-all-charts',function(data) {
			states = $.map(data, function(item) {
				return item.Name;
			});
		})
		.fail(function(xhr) {
			alert('failed fetching charts: '+xhr.statusText + ' ('+xhr.status+')');
		});
	});
})();