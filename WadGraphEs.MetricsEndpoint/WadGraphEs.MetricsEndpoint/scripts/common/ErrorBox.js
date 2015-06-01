(function() {
	var errorBox = function($appendTo, title, msg) {
		var $box = $('#error-box-template').clone().attr('id','');
		$box.find(".error-title").text(title);
		$box.find(".error-message").text(msg);
		$appendTo.append($box);
	}

	$.extend(true, window, {
		ErrorBox: errorBox
	});
})();