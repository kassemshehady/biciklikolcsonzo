var geocoder = new google.maps.Geocoder();
var marker = null;
var circle = null;
var map = null;
var zone_mode = false;

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
    if (document.getElementById('address') != null) {
        document.getElementById('address').value = str;
    } else if (document.getElementById('addressDiv') != null) {
        document.getElementById('addressDiv').innerHTML = str;
    }
}

function init_gmaps_single_marker(readonly) {

    var coords_from_input = true;               // use input field to init
    var input_latitude = parseFloat(document.getElementById('latitude').value);
    var input_longitude = parseFloat(document.getElementById('longitude').value);

    if ((input_latitude == null) || (Math.abs(input_latitude) < 0.001 )) {
        input_latitude = 47.47;         // default latitude
        coords_from_input = false;
    }

    if ((input_longitude == null) || (Math.abs(input_longitude) < 0.001)) {
        input_longitude = 19.06;        // default longitude
        coords_from_input = false;
    }

    var latLng = new google.maps.LatLng(input_latitude, input_longitude);

    if (readonly) {
        map = new google.maps.Map(document.getElementById('mapCanvas'), {
            zoom: 16,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });

        marker = new google.maps.Marker({
            position: latLng,
            map: map,
            draggable: false
        });

        if (zone_mode) {
            geocodePosition(latLng);
        }
    } else {
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

        if (!coords_from_input) {
            geocodePosition(latLng);        // do not replace address on init when editing
        } else {
            map.setZoom(16);                // if the coordinates could be read out then zoom in
        }

        // Add dragging event listeners.
        google.maps.event.addListener(marker, 'drag', function () {
            updateMarkerPosition(marker.getPosition());
        });

        google.maps.event.addListener(marker, 'dragend', function () {
            updateMarkerPosition(marker.getPosition());
            geocodePosition(marker.getPosition());
        });

        // Add map click listener
        google.maps.event.addListener(map, 'click', function (event) {
            marker.setPosition(event.latLng);
            updateMarkerPosition(marker.getPosition());
            geocodePosition(marker.getPosition());
        });
    }

    if (zone_mode) {
        var input_radius = parseFloat(document.getElementById('radius').value);

        if ((input_radius == null) || (Math.abs(input_radius) < 0.001)) {
            input_radius = 250;
            document.getElementById('radius').value = input_radius;
        }

        circle = new google.maps.Circle({
            map: map,
            radius: input_radius,
            fillColor: '#AA0000'
        });
        circle.bindTo('center', marker, 'position');

        geocodePosition(marker.getPosition());
    }
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

    var rad = parseFloat($("#radius").val());
    if (isNaN(rad)) {
        rad = circle.getRadius();
    }

    marker.setPosition(new google.maps.LatLng(lat, lng));
    circle.setRadius(rad);
    updateMarkerPosition(marker.getPosition());
    map.setCenter(marker.getPosition());
    map.setZoom(14);
    geocodePosition(marker.getPosition());
}

function init_gmaps_picker() {
    init_gmaps_single_marker(false);
    $("#latitude").change(updateGmapsView);
    $("#longitude").change(updateGmapsView);
}

function init_gmaps_readonly() {
    init_gmaps_single_marker(true);
}

function init_gmaps_zones_picker() {
    zone_mode = true;
    init_gmaps_single_marker(false);
    $("#latitude").change(updateGmapsView);
    $("#longitude").change(updateGmapsView);
    $("#radius").change(updateGmapsView);
}

function init_gmaps_zones_readonly() {
    zone_mode = true;
    init_gmaps_single_marker(true);
}