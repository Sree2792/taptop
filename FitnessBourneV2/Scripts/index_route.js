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
var geocodeService = L.esri.Geocoding.geocodeService();

mapMain.on('click', function (e) {
    //var marker = new L.marker(e.latlng).addTo(mapMain);

    geocodeService.reverse().latlng(e.latlng).run(function(error, result) {
        L.marker(result.latlng).addTo(mapMain).bindPopup(result.address.Match_addr).openPopup();
    });

    function callback(targetField1, counter1) {

        //execute things after above loop done
        if (targetField1 == "" && counter1 > 1) {

            //Implement button click for adding new text field
            $('.leaflet-routing-add-waypoint').trigger('click');
        }
    }

    var counter = 0;
    var targetField = ""
    var lastObserved = ""
    var total_length = $('.leaflet-control-container input').length

    $('.leaflet-control-container input').each(function (item,index,array) {

        if (this.value == "") {

            targetField = this.placeholder;
        }
        lastObserved = this.placeholder;
        counter = counter + 1;

        if (counter === total_length) {
            callback(targetField, counter);
        }
    });
    

    
    
});

L.Routing.errorControl(control).addTo(mapMain);