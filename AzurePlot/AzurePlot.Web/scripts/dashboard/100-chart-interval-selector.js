(function() {
	var ChartIntervalSelector = function($form, $interval, $unit) {
		var parseCurrentValue = function() {
			return parseInt($interval.val(),10);
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

		$unit.on('change', function(ev) {
			currentUnit = $unit.val();
			raiseChange();
		});

		$form.on("submit", function(ev) {
			ev.preventDefault();
		});

		var getCurrentInterval = function() {
			return {
				value: currentIntervalValue,
				unit: currentUnit
			};
		}

		var intervalChangedListener = Events.newUnnamedListener();

		var onIntervalChanged = function(callback) {
			intervalChangedListener.subscribe(callback);
		}

		var raiseChange = function() {	
			intervalChangedListener.publish(getCurrentInterval());
		}

		this.getCurrentInterval = getCurrentInterval;
		this.onIntervalChanged = onIntervalChanged;
	}

	$(function() {
		var intervalSelector = new ChartIntervalSelector($('#select-dashboard-time-interval'),$("#select-chart-interval"),$("#interval-unit"));
		$.extend(true, window, {
			Dashboard: {
				IntervalSelector: intervalSelector
			}
		});
	});
})();