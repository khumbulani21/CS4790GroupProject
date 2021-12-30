// Google Custom Search

//added 10/02/14
(function () {
	var cx = '001912136203379195774:c8pjud2lumc';
	var gcse = document.createElement('script');
	gcse.type = 'text/javascript';
	gcse.async = true;
	//gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') +
	//    '//www.google.com/cse/cse.js?cx=' + cx;
	gcse.src = 'https://cse.google.com/cse/cse.js?cx=' + cx;
	var s = document.getElementsByTagName('script')[0];
	s.parentNode.insertBefore(gcse, s);
})();

// JavaScript Document
$(document).ready(function (e) {

	$(".show-w-number > a").click(function () {
		$(".show-w-number").hide();
		$(".w-number").show();
	});
	$(".show-w-number > a").keypress(function (e) {
		if (e.which == 13) {
			$(".show-w-number").hide();
			$(".w-number").show();
		}
	});


	$(".photo-holder").hover(function () {
		$(".edit-picture").toggle();
	});


	// Keeps dropdown on screen
	DetectDropdownEdge();
	$(window).resize(function () {
		DetectDropdownEdge();
	});
	function DetectDropdownEdge() {
		if ($(window).width() >= 1008) {
			$("li.has-dropdown ul").each(function () {
				if (isEntirelyVisible($(this))) {
					//Dropdown shows completely on screen
					$(this).removeClass('edge');
				} else {
					//Dropdown is off-screen
					$(this).addClass('edge');
				}
			});
		} else {
			//Remove .edge for mobile views
			$("li.has-dropdown ul.edge").removeClass("edge");
		}
	}
	function isEntirelyVisible(obj) {
		var objOffset = obj.offset();

		return (objOffset.left - obj.width() > 0);
	}

	//if icon cover is clicked flash highlight on button to indicate action required
	$(".cover-icons").click(function () {
		$(".wsu-alert-desktop .button").addClass("cover-clicked");

		var delay = setTimeout(function () {
			$(".wsu-alert-desktop .button").removeClass("cover-clicked");
		}, 200)
	});

	$(document).click(function (e) {
		var container = $("#notificationsDropdown, #notificationsButton");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#notificationsDropdown").hide();
			if ($("#notificationsButton").hasClass("active-icon")) {
				$("#notificationsButton").toggleClass("active-icon");
			};
		}
	});

	$(document).click(function (e) {
		var container = $("#searchDropdown, .gsc-completion-container, .gssb_c, #searchButton, .inpage-search-link");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			if (!findCNadsTerminalParent(e.target)) {
				$("#searchDropdown, .gsc-completion-container, .gssb_c").hide();
				if ($("#searchButton").hasClass("active-icon")) {
					$("#searchButton").toggleClass("active-icon");
				};
			}
		}
	});

	$(document).click(function (e) {
		var container = $("#profileDropdown, #btnProfile");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#profileDropdown").hide();
			if ($("#btnProfile").hasClass("active-icon")) {
				$("#btnProfile").toggleClass("active-icon");
			};
		}
	});

	$(document).click(function (e) {
		var container = $("#azDropdown, #azButton");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#azDropdown").hide();
			$(".a-z-list ul").remove();
			$(".a-z-list button").removeClass("active-a-z-letter");
			if ($("#azButton").hasClass("active-icon")) {
				$("#azButton").toggleClass("active-icon");
			};
		}
	});

	$(".a-z-letters button").click(function () {
		$(".a-z-letters button").removeClass("active-a-z-letter");
		$(this).addClass("active-a-z-letter");
	});

	$("#notificationsButton").click(function () {
		$("#notificationsDropdown").fadeToggle("fast");
		$("#notificationsDropdown").focus();
		$(this).toggleClass("active-icon");
	});

	$("#azButton").click(function () {
		$('html, body').animate({ scrollTop: 0 }, 0);
		$("#azDropdown").fadeToggle("fast");
		$("#azButton").blur();
		$("#azDropdown").focus();
		//$("#azButton").blur();
		$(this).toggleClass("active-icon");
	});

	$("#searchButton, .inpage-search-link").click(function () {
		$(".search-dropdown").fadeToggle("fast");
		$("#searchButton").toggleClass("active-icon");
		$("#gsc-i-id1").focus();
		$(".gssb_c, .gsc-completion-container").show();
		beginActiveSearchListener();
	});

	$("#btnProfile").click(function () {
		$(this).toggleClass("active-icon");
		$("#profileDropdown").fadeToggle("fast");
		$("#profileDropdown").focus();
	});

	/*  Start Mega Menu   */

	$("#top-link-admissions").click(function () {
		$(this).addClass("top-link-active");
		$("#admissions-menu").show();
		$(".menu-line").show();
	});

	$("#top-link-academics").click(function () {
		$(this).addClass("top-link-active");
		$("#academics-menu").show();
		$(".menu-line").show();
	});

	$("#top-link-student-life").click(function () {
		$(this).addClass("top-link-active");
		$("#student-life-menu").show();
		$(".menu-line").show();
	});

	$("#top-link-about-wsu").click(function () {
		$(this).addClass("top-link-active");
		$("#about-wsu-menu").show();
		$(".menu-line").show();
	});


	/*   Setting Tab Focus   */
	$("#top-link-admissions").click(function (e) {
		$("#admissions-menu a:first").focus();
		$("#admissions-menu a:last").blur(function (e) {
			$("#top-link-admissions").focus();
		});
	});

	$("#top-link-academics").click(function (e) {
		$("#academics-menu a:first").focus();
		$("#academics-menu a:last").blur(function (e) {
			$("#top-link-academics").focus();
		});
	});

	$("#top-link-student-life").click(function (e) {
		$("#student-life-menu a:first").focus();
		$("#student-life-menu a:last").blur(function (e) {
			$("#top-link-student-life").focus();
		});
	});

	$("#top-link-about-wsu").click(function (e) {
		$("#about-wsu-menu a:first").focus();
		$("#about-wsu-menu a:last").blur(function (e) {
			$("#top-link-about-wsu").focus();
		});
	});

	$(".mega-menu-top-links a:last").blur(function (e) {

		$(".menu-line").hide();
		$("#admissions-menu").hide();
		$("#academics-menu").hide();
		$("#student-life-menu").hide();
		$("#about-wsu-menu").hide();

		$("#top-link-admissions").removeClass("top-link-active");
		$("#top-link-academics").removeClass("top-link-active");
		$("#top-link-student-life").removeClass("top-link-active");
		$("#top-link-about-wsu").removeClass("top-link-active");

		$("#skipToContent").focus();
	});

	$(document).click(function (e) {

		var container = $(".top-link");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$(".menu-line").hide();
		}

		var container = $("#admissions-menu, #top-link-admissions");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#admissions-menu").hide();
			container.removeClass("top-link-active");
		}

		var container = $("#academics-menu, #top-link-academics");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#academics-menu").hide();
			container.removeClass("top-link-active");
		}

		var container = $("#student-life-menu, #top-link-student-life");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#student-life-menu").hide();
			container.removeClass("top-link-active");
		}

		var container = $("#about-wsu-menu, #top-link-about-wsu");
		if (!container.is(e.target) && container.has(e.target).length === 0) {
			$("#about-wsu-menu").hide();
			container.removeClass("top-link-active");
		}

	});


});

var nMaxNads = 7;
function findCNadsTerminalParent(obj) {
	if ($(obj).hasClass('gsq_a')) {
		nMaxNads = 7;
		return true;
	}

	if (obj.parentElement == null) {
		nMaxNads = 7;
		return false;
	}
	else {
		nMaxNads -= 1;
		if (nMaxNads > 0) {
			return findCNadsTerminalParent(obj.parentElement);
		}
		else { return false; }
	}

	//return $(obj.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement).hasClass('gsq_a');

}

var oWatcher;
function beginActiveSearchListener() { oWatcher = setInterval(function () { activeSearchPaneListener(); }, 100); }
function endActiveSearchListener() { clearInterval(oWatcher); }

function activeSearchPaneListener() {
	if ($(".gsc-results-close-btn") != null) {
		$(".gsc-results-close-btn").attr("style", "position:fixed !important;");
		endActiveSearchListener();
	}
}

if (!Modernizr.svg) {
	var imgs = document.getElementsByTagName('img');
	var svgExtension = /.*\.svg$/
	var l = imgs.length;
	for (var i = 0; i < l; i++) {
		if (imgs[i].src.match(svgExtension)) {
			imgs[i].src = imgs[i].src.slice(0, -3) + 'png';
			console.log(imgs[i].src);
		}
	}
}