"use strict";
(function() {
	var waitAll = function (deferreds, onSuccess, onFailed, onAlways) {
		if (deferreds.length == 0) {
			onSuccess([]);
			onAlways([], []);
			return;
		}

		var successes = [];
		var failed = [];
			
		$.each(deferreds, function(idx, res) {
			res.done(function() {
				success(res, arguments);
			});
			res.fail(function() {
				fail(res, arguments);
			});
		});

		var success = function(res, args) {
			successes.push({res: res, args: args});
			checkDone();
		}
		var fail = function(res, args) {
			failed.push({res: res, args: args});
			checkDone();
		}
		var done = function() {
			if(onSuccess && successes.length) {
				onSuccess(successes);
			}
			if(onFailed && failed.length) {
				onFailed(failed);
			}
			if(onAlways) {
				onAlways(successes,failed);
			}
		}
		var checkDone = function() {
			if(!(successes.length + failed.length >= deferreds.length)) {
				return;
			}
			done();
		}
	}

	$.extend(true, window, {
		Common: {
			WaitAll: waitAll
		}
	});
})();