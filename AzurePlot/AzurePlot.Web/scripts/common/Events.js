(function() {
	var listener = function() {
		var $el = $("<div></div>");

		this.subscribe = function(name, callback) {
			$el.on(name, function(ev, data) {
				callback.apply(null,data.args);
			});
		}

		this.publish = function(name, data) {
			$el.trigger(name, {args: Arguments.copy(arguments).slice(1) });
		}	
	}

	var Events = {
		newListener: function() { return new listener() },
		newUnnamedListener: function() {
			var target = new listener();
			var name ="no-name";
			return {
				subscribe: function() {
					target.subscribe.apply(target, [name].concat(Arguments.copy(arguments)));
				},
				publish: function() {
					target.publish.apply(target,[name].concat(Arguments.copy(arguments)));
				}
			}
		}
	}

	$.extend(true, window, {
		Events: Events
	});
})();