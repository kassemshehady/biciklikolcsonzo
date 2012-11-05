var geocoder = new google.maps.Geocoder();
var marker = null;
var map = null;

function geocodePosition(pos) {
    geocoder.geocode({
        latLng: pos
    }, function (responses) {
        if (responses && responses.length > 0) {
            updateMarkerAddress(responses[0].formatted_address);
        } else {
            updateMarkerAddress('');
        }
    });
}

function updateMarkerPosition(latLng) {
    document.getElementById('latitude').value = latLng.lat();
    document.getElementById('longitude').value = latLng.lng();
}

function updateMarkerAddress(str) {
    document.getElementById('address').value = str;
}

function init_gmaps_single_marker() {

    var edit_mode = true;               // use input field to init
    var input_latitude = parseFloat(document.getElementById('latitude').value);
    var input_longitude = parseFloat(document.getElementById('longitude').value);

    if (input_latitude == null || Math.abs(input_latitude) < 0.001) {
        input_latitude = 47.47;         // default latitude
        edit_mode = false;
    }

    if (input_longitude == null || Math.abs(input_longitude) < 0.001) {
        input_longitude = 19.06;        // default longitude
        edit_mode = false;
    }

    var latLng = new google.maps.LatLng(input_latitude, input_longitude);

    map = new google.maps.Map(document.getElementById('mapCanvas'), {
        zoom: 8,
        center: latLng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });

    marker = new google.maps.Marker({
        position: latLng,
        map: map,
        draggable: true
    });

    // Update current position info.
    updateMarkerPosition(latLng);

    if (!edit_mode) {
        geocodePosition(latLng);
    } else {
        map.setZoom(15);
    }

    // Add dragging event listeners.
    google.maps.event.addListener(marker, 'drag', function () {
        updateMarkerPosition(marker.getPosition());
    });

    google.maps.event.addListener(marker, 'dragend', function () {
        updateMarkerPosition(marker.getPosition());
        geocodePosition(marker.getPosition());
    });

    google.maps.event.addListener(map, 'click', function (event) {
        marker.setPosition(event.latLng);
        updateMarkerPosition(marker.getPosition());
        geocodePosition(marker.getPosition());
    });
}

function updateGmapsView() {
    var lat = parseFloat($("#latitude").val());
    if (isNaN(lat)) {
        lat = marker.getPosition().lat();
    }

    var lng = parseFloat($("#longitude").val());
    if (isNaN(lng)) {
        lng = marker.getPosition().lng();
    }

    marker.setPosition(new google.maps.LatLng(lat, lng));
    updateMarkerPosition(marker.getPosition());
    map.setCenter(marker.getPosition());
    map.setZoom(15);
    geocodePosition(marker.getPosition());
}

function init_gmaps_picker() {
    init_gmaps_single_marker();
    $("#latitude").change(updateGmapsView);
    $("#longitude").change(updateGmapsView);
}