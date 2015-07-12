(function() {
	$.extend(true, window, {
		Arguments: {
			copy: function(args) {
				var res = [];
				for(var i=0;i<args.length;i++) {
					res.push(args[i]);
				}

				return res;
			}
		}
	});
})();