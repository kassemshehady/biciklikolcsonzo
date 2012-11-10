var map = null;

function init_gmaps() {
    var latLng = new google.maps.LatLng(47.47, 19.06);
    map = new google.maps.Map(document.getElementById('mapCanvas'), {
        zoom: 12,
        center: latLng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });

    gmaps_init_complete();
}

function addMarker(latitude, longitude, info, radius) {
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(latitude, longitude),
        map: map,
        draggable: false
    });

    var markerInfo = new google.maps.InfoWindow({
        content: info
    });

    if (radius != null) {
        var circle = new google.maps.Circle({
            map: map,
            radius: radius,
            fillColor: '#AA0000'
        });
        circle.bindTo('center', marker, 'position');
    }

    if (markerInfo.getContent() != null) {
        google.maps.event.addListener(marker, 'click', function () {
            markerInfo.open(map, marker);
        });
    }
}

function addCircle(latitude, longitude, info, radius) {
    var circle = new google.maps.Circle({
        map: map,
        center: new google.maps.LatLng(latitude, longitude),
        radius: radius,
        fillColor: '#AA0000'
    });

    var circleInfo = new google.maps.InfoWindow({
        content: info
    });

    if (circleInfo.getContent() != null) {
        google.maps.event.addListener(circle, 'click', function () {
            circleInfo.open(map, circle);
        });
    }
}