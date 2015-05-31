"use strict";
(function() {
	var CommonTime = function (seconds) {
		this.getSeconds = function () {
			return seconds;
		}
	}

	CommonTime.FromString = function(timeString) {
		if(timeString.indexOf('.')!=-1) {
			timeString = timeString.substring(0, timeString.indexOf('.'));
		}
		var parts = timeString.split(':');
		return new CommonTime(parseInt(parts[0],10) * 3600 + parseInt(parts[1],10) * 60 + parseInt(parts[2],10));
	}

	var CommonDate = function(jsDate) {
		this.AsJSDate = function () {
			return new Date(jsDate.getTime());
		};
	}

	CommonDate.FromString = function(dateString) {
		var parts = dateString.split('-');
		return CommonDate.FromJSDate(new Date(Date.UTC(parts[0],parts[1]-1,parts[2])));
	}

	CommonDate.FromJSDate = function (jsDate) {
		return new CommonDate(jsDate);
	}

	var CommonDateTime = function(date,time) {
		this.AsJSDate = function () {
			var dt = date.AsJSDate();
			dt.setSeconds(dt.getSeconds() + time.getSeconds());
			return dt;
		};
	}

	
	CommonDateTime.FromISOUTCString = function(isoDateTimeString) {
		if(isoDateTimeString[isoDateTimeString.length-1] != 'Z') {
			throw "non-utc not supported";
		}
		var parts = isoDateTimeString.split('T');
		var date = CommonDate.FromString(parts[0]);
		var time = CommonTime.FromString(parts[1].substring(0,parts[1].length-1));

		return new CommonDateTime(date,time);
	}

	$.extend(true, window, {
		Common: {
			Time: CommonTime,
			Date: CommonDate,
			DateTime: CommonDateTime
		}
	});
})();