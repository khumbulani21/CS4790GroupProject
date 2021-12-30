$(document).ready(function (e) {

	// off-canvas menu
	$("span.switch").click(function (e) {

		$(".slick-track").hide();

		if ($(".body-copy")[0]) {
			var viewportWidth = ($(window).width() - 10);
			$(".body-copy").css("width", viewportWidth);
		}

		if ($('body').hasClass("offcanvas")) {
			$(".slick-track").show();
			var offset = -parseInt($('body').css('top'));
			$('body').toggleClass('offcanvas').css('top', 0).scrollTop(offset);
		} else {
			$('body').css('top', -window.pageYOffset + 'px').toggleClass('offcanvas');
		}
	});

	$(".overlay").click(function () {
		if ($(".body-copy")[0]) {
			$(".body-copy").css("width", "100%");
		}
		if ($('body').hasClass("offcanvas")) {
			$(".slick-track").show();
			var offset = -parseInt($('body').css('top'));
			$('body').toggleClass('offcanvas').css('top', 0).scrollTop(offset);
		}
	});

	// Logout/Edit Profile Dropdown for Portal
	$(document).mouseup(function (e) {
		var container = $(".login-dropdown-mobile");

		if (!container.is(e.target) && container.has(e.target).length === 0) {
			container.hide();
			if ($("#profile").hasClass("profile-active")) {
				$("#profile").toggleClass("profile profile-active");
			};
			if ($("#login-profile-mobile-link").hasClass("login-profile-mobile-active")) {
				$("#login-profile-mobile-link").toggleClass("login-profile-mobile login-profile-mobile-active");
			};
		}
	});

	$(".top-nav-menu-icon").click(function () {

		$(".top-nav-dropdown").toggle("fast");

	});

	$("#duo_iframe").width("100%");
});