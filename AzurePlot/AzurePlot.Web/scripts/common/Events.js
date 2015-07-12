(function() {
	var $el = $("<div></div>");

	var register = function(name, callback) {
		$el.on(name, function(ev, data) {
			callback.apply(null,data.args);
		});
	}

	var trigger = function(name, data) {
		$el.trigger(name, {args: Arguments.copy(arguments).slice(1) });
	}

	$.extend(true, window, {
		Events: {
			Register: register,
			Trigger: trigger
		}
	});
})();