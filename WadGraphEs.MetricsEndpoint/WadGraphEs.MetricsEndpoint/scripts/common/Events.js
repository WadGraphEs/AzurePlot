(function() {
	var $el = $("<div></div>");

	var register = function(name, callback) {
		$el.on(name, function(ev, data) {
			console.log(arguments);
			callback(data);
		});
	}

	var trigger = function(name, data) {
		$el.trigger(name, {args: slice(1, arguments) });
	}

	$.extend(true, window, {
		Events: {
			Register: register,
			Trigger: trigger
		}
	});
})();