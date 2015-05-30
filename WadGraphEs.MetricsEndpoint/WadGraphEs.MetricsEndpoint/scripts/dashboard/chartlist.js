"use strict";
(function() {
	var addChartsModel = function () {
		var toAdd = [];
		var me = this;
		var onSelected = [];
		var onDeselected = [];


		var raise = function (callbacks, chart) {
			$.each(callbacks, function (idx, cb) {
				cb(chart);
			});
		}

		var hasChart = function (chart) {
			var count = $.grep(toAdd, function (item) {
				return item.Uri == chart.Uri;
			}).length;
			return count >= 1;
		}

		this.add = function (chart) {
			if (hasChart(chart)) {
				return;
			}
			toAdd.push(chart);
			raise(onSelected, chart);
		}

		this.remove = function (chart) {
			if (!hasChart(chart)) {
				return;
			}
			toAdd = $.grep(toAdd, function (item) {
				return item.Uri != chart.Uri;
			});
			
			raise(onDeselected, chart);
		}

		this.toggle = function (chart) {
			if (me.contains(chart)) {
				me.remove(chart);
				return;
			}

			me.add(chart);
		}

		this.onSelected = function (cb) {
			onSelected.push(cb);
		}

		this.onDeselected = function (cb) {
			onDeselected.push(cb);
		}

		var addChartApi = function (chart) {
			return $.ajax({
				url: '/dashboard/add-chart',
				data: {
					uri: chart.Uri
				},
				method: 'post',
			});
		}

		this.commit = function () {
			var when = [];
			$.each(toAdd, function (idx,chart) {
				when.push(
					addChartApi(chart)
					.then(function () {
						return window.Charts.Chart.FromURI(chart.Uri).Render();
					})
				);
			});
			return when;
		}

		this.clear = function () {
			$.each(toAdd, function (idx, chart) {
				me.remove(chart);
			});
		}

		this.contains = hasChart;
	}


	var initChartList = function () {
		var toAddModel = new addChartsModel();
		$.ajax({
			url: '/api/list-all-charts'
		})
		.done(function (result) {
			var $chartContainer = $('.available-charts');
			$chartContainer.empty();

			$.each(result, function (idx, chart) {
				var $row = $('<a href="#" class="list-group-item">' + chart.Name + '</a>');
				
				toAddModel.onSelected(function (selectedChart) {
					if(selectedChart.Uri != chart.Uri) {
						return;
					}
					$row.addClass('active');
				});

				toAddModel.onDeselected(function (selectedChart) {
					if (selectedChart.Uri != chart.Uri) {
						return;
					}
					$row.removeClass('active');
				});

				$row.on('click', function (ev) {
					ev.preventDefault();
					
					toAddModel.toggle(chart);
				});

				$chartContainer.append($row);
			});

		});

		return toAddModel;
	}


	var waitAll = function(deferreds, onSuccess, onFailed, onAlways){
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

	$(function () {
		var toAddModel = initChartList();

		var $theModal =$('#add-to-chart-modal');

		$('.add-to-dashboard').on('click', function () {
			$theModal.modal('show');
		});

		$('.add-to-dashboard-submit').on('click', function () {
			$('body').addClass('loading');
			
			new waitAll(toAddModel.commit(), function(){}, function() {}, function(success, failed) {
				if(failed.length) {
					alert("Failed "+ failed.length+" items");
				}
				$('body').removeClass('loading');
				$theModal.modal('hide');
			});
		});
	});
})();