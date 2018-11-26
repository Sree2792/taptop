//Referred from http://www.liedman.net/leaflet-routing-machine/
var mapMain = L.map('mapRoute').setView([-37.89, 145.00], 11);
var mapIcon = L.icon({
    iconUrl: '../Image/Map_Pin.png',
    iconSize: [38, 95], // size of the icon
    popupAnchor: [0, -15]
});

L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
	attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
}).addTo(mapMain);

var control = L.Routing.control(L.extend(window.lrmConfig, {
	waypoints: [
	],
	geocoder: L.Control.Geocoder.nominatim(),
	routeWhileDragging: true,
	reverseWaypoints: true,
	showAlternatives: true,
	altLineOptions: {
		styles: [
			{color: 'black', opacity: 0.15, weight: 9},
			{color: 'white', opacity: 0.8, weight: 6},
			{color: 'blue', opacity: 0.5, weight: 2}
		]
	}
})).addTo(mapMain);

L.Routing.errorControl(control).addTo(mapMain);
