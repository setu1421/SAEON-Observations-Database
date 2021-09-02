// Errors

function ErrorInFunc(method: string, status: string, error: string) {
    alert("Error in " + method + " Status: " + status + " Error: " + error);
}

// Map

interface MapPoint {
    title: string;
    latitude: number;
    longitude: number;
    elevation: number;
    url: string;
}

let map: google.maps.Map;
let markers: google.maps.Marker[] = [];
let mapPoints: MapPoint[];
let mapBounds: google.maps.LatLngBounds;
let mapFitted = false;

export function FitMap(override = false) {
    if (override || (!mapFitted && (mapBounds !== null) && !mapBounds.isEmpty())) {
        map.setCenter(mapBounds.getCenter());
        map.fitBounds(mapBounds);
        mapFitted = true;
    }
}

export function WaitForMap() {
    if (typeof mapBounds !== "undefined") {
        FitMap();
    }
    else {
        setTimeout(WaitForMap, 250);
    }
}

export function UpdateMap() {
    $.getJSON("/Home/GetMapPoints")
        .done(function (json) {
            for (let i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];
            mapPoints = json;
            mapBounds = new google.maps.LatLngBounds();
            for (let i = 0; i < mapPoints.length; i++) {
                const mapPoint = mapPoints[i];
                //if (mapPoint.latitude === null || mapPoint.longitude === null) alert("Null");
                const marker = new google.maps.Marker({
                    position: { lat: mapPoint.latitude, lng: mapPoint.longitude },
                    map: map,
                    title: mapPoint.title
                });
                markers.push(marker);
                mapBounds.extend(marker.getPosition() as google.maps.LatLng);
                marker.setIcon('/Images/green-dot.png');
            }
        })
        .fail(function (jqXHR, status, error) {
            ErrorInFunc("UpdateMap", status, error)
        });
}

export function InitMap() {
    const mapOpts: google.maps.MapOptions = {
        center: new google.maps.LatLng(-34, 25.5),
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.SATELLITE
    };
    map = new google.maps.Map(document.getElementById('mapLocations') as Element, mapOpts);
    UpdateMap();
}

export function FixMap() {
    UpdateMap();
    FitMap(true);
}

