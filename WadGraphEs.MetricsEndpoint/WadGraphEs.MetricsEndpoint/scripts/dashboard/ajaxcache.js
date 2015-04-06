"use strict";
(function() {
	var store = function(key, data) {
		if(!localStorage) {
			return;
		}
		localStorage.setItem(key, JSON.stringify(data));
	}

	var get = function(key) {
		if(!localStorage) {
			return [];
		}
		var value = localStorage.getItem(key);
		if(!value) {
			return null;
		}

		return JSON.parse(value);
	}

	var defaultOptions = {
		dataType: 'json'
	}

	window.getCachedAjax = function(url, loadData, options) {
		options = $.extend({}, defaultOptions, options || {});

		var item = get(url);
		if(item) {
			loadData(item);
		}

		return $.ajax({
			url: url,
			dataType: options.dataType,
			method: 'get'
		})
		.done(function(data) {
			store(url, data);
			loadData(data);
		});
	}
})();