(function() {
	

	var initChartIntervalSelector = function($interval, $unit) {
		var parseCurrentValue = function() {
			return parseInt($interval.val(),10);
		}

		var raiseChange = function() {	
			Events.Trigger("Dashboard.IntervalChanged", currentIntervalValue, currentUnit);
		}

		var currentIntervalValue = parseCurrentValue();
		var currentUnit = $unit.val();

		$interval.on("change", function(ev) {
			var newValue = parseCurrentValue();
			if(newValue<=0) {
				$interval.val(currentIntervalValue);
				return;
			}
			currentIntervalValue = newValue;
			raiseChange();
		});
	}


	$(function() {
		initChartIntervalSelector($("#select-chart-interval"),$("#interval-unit"));
	});
})();