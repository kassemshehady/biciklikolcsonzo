$(document).ready(function() {

	// Dropdown menu
	$(".dropdown-group").click( function() {
		var showing = $(this).hasClass("open");
		$(document).click();
		
		if (!showing) {
			$(this).addClass("open");
		}
		
		return false;
	});

	// Hide dropdown menu if user clicks on anything other
	$(document).click( function() {
		if ($(".dropdown-group").hasClass("open")) {
			$(".dropdown-group").removeClass("open");
		}
		return true;
	});
	
	// Dropdown element
	$(".dropdown-menu > a").click( function() {
		var url = $(this).attr("href");
		if (url != "" && url != "#") {
			window.location.href = url;
		}
		return true;
	});

});
