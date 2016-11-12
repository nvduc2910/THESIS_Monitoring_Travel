


var marker, map;
var listMarker = new Array();
var longfake = 0.0000000001;
var lagfake = 0.0000000001
var listPosition = [];
var listInforwindow = [];

var pinColor = "FE7569";
var pinColorSelect = "1FB5AD"

var pinImage = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColor,
    new google.maps.Size(21, 34),
    new google.maps.Point(0, 0),
    new google.maps.Point(10, 34));

var pinImageSelect = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColorSelect,
    new google.maps.Size(21, 34),
    new google.maps.Point(0, 0),
    new google.maps.Point(10, 34));
    
//var pinShadow = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
//    new google.maps.Size(40, 37),
//    new google.maps.Point(0, 0),
//    new google.maps.Point(12, 35));

//var iconImageSelect = {
//    url: "http://localhost:20261/Content/Images/ic_marke.png", // url
//    scaledSize: new google.maps.Size(50, 59), // scaled size
//    anchor: new google.maps.Point(25, 30) // anchor
//};

//var iconImage = {
//    url: "http://localhost:20261/Content/Images/ic_marke.png", // url
//    scaledSize: new google.maps.Size(50, 59), // scaled size
//    anchor: new google.maps.Point(0, 49) // anchor
//};

function initialize() {
    var myLatlng = new google.maps.LatLng(10.824638, 106.627733);
    var mapOptions = {
        zoom: 4,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
    
}
function LoadMarker() {
    $.ajax({
        url: "/Home/CreateMarker/",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        success: function (result) {
            var results = $.parseJSON(result);
            $(results).each(function (index, value) {
                var longitude = this['longitude'];
                var laitude = this['latitude'];
                var title = " " + this['tour_guide_id'];
                var currentLocation = new google.maps.LatLng(laitude, longitude);
                createMarker(index, currentLocation, title);
            });
        },
        error: function (xhr) {
            alert('error');
        }
    });
}
$(document).ready(function () {
        
    $(function () {
        initialize();
        google.maps.event.addListener(map, 'click', function (event) {
            var duration = parseInt($('#durationOption').val());

            if (duration < 0) {
                duration = 1;
                $('#durationOption').val(duration);
            }
            marker.setDuration(duration);
            marker.setEasing($('#easingOption').val());
            marker.setPosition(event.latLng);
        });
           
    });
        
    $('#DemoAjaxClick').click(function () {
        setInterval(function () {
            $.ajax({
                url: "/Home/GetLocationTourGuide/20",
                type: "GET",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                async: true,
                processData: false,
                cache: false,
                success: function (result) {
                    var results = $.parseJSON(result);
                    $(results).each(function (index, value) {

                        var longitude = this['longitude'];
                        var laitude = this['latitude'];
                        var currentLocation = new google.maps.LatLng(laitude, longitude);
                        MoverMarker(currentLocation, index)
                    });
                },
                error: function (xhr) {
                    alert('error');
                }
            });
        }, 2000)
    });
    $("#Search").click(function () {
        var bla = $('#nametour').val();
        $.ajax({
            url: "/Home/SearchTourGuide/" + bla,
            type: "GET",
        })
        .done(function (partialViewResult) {
            $("#list-tour").html(partialViewResult);
        });
    });

});
function LoadMarker() {
    $.ajax({
        url: "/Home/CreateMarker/",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        success: function (result) {

            var results = $.parseJSON(result);
            for (i = 0; i < results.objectArray.length; i++)
            {
                var longitude = results.objectArray[i].TourGuide.longitude;
                var laitude = results.objectArray[i].TourGuide.latitude;
                var tourName = results.objectArray[i].Tour.tour_name;
                var tourGuide = results.objectArray[i].TourGuide.tourguide_name;
                var departure_date = results.objectArray[i].Tour.departure_date.split('T')[0];
                var return_date = results.objectArray[i].Tour.return_date.split('T')[0];
                var tourDetail = '/TourDetail/GetDetailTour/' + results.objectArray[i].Tour.tour_id;
                var currentLocation = new google.maps.LatLng(laitude, longitude);

                
               
                var contentString = '<div style="width: 300px;background: white;height: 150px;background: while;">' +
                    '<div style="height: 68%;">' +
                    '<div style="float: left; width: 35%; height: 100%; background: red;">' +
                    '<img style="width: 100%; height: 100%; background: red;" src="https://fbcdn-photos-b-a.akamaihd.net/hphotos-ak-xtp1/v/t1.0-0/p206x206/13620075_715125938628840_3925566649508034087_n.jpg?oh=3e2062e509862f4e898c8bfe3714c999&oe=58A211A1&__gda__=1487320374_107ddfdc334185bf70ca54924993fbd4"/>' +
                    '</div>' +
                    '<div style="float: right; width: 65%; height: 100%;">' +
                    '<div class="list-tour" style="height: 25px; margin-top: 0px; margin-left: 5px;" id="nav-accordion">' +
                    '<span style="color: black; font-size: 14px; margin-left: 10px; font-weight: bold;">' + tourName + '</span>' +
                    '</div>' +
                    '<div class="list-tour" style="height: 20px; margin-top: 0px; margin-left: 15px;" id="nav-accordion">' +
                    '<img src="/Content/Images/ic_tourguide.png" width="20" height="20"/>' +
                    '<span style="color: gray; font-size: 12px; margin-left: 20px;">' + tourGuide + '</span><span style="font-size: 16px;"></span>' +
                    '</div>' +
                    '<div class="list-tour" style="height: 20px; margin-top: 10px; margin-left: 15px;" id="nav-accordion">' +
                    '<img src="/Content/Images/ic_time.png" width="18" height="18" style="margin-top: -4px;"/>' +
                    '<span style="color: gray; font-size: 12px; margin-left: 20px;">' + departure_date + ' - ' + return_date + '</span>' +
                    '</div>' +
                    '<div class="list-tour" style="height: 20px; margin-top: 8px; margin-left: 15px;" id="nav-accordion">' +                 
                    '<span style="color: blue; font-size: 12px; margin-left: 20px; float: right;"><a href="'+tourDetail+'"  style="color: blue;">Xem chi tiết</a></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div style="height: 10%; border-bottom: 1px solid #e5e5e6;"> </div>'+
                    '<div style="height: 22%; margin-top: 10px;">' +
                    '<div style="float: right; height: 100%; width: 20%;">'+  
                    '<img src="/Content/Images/ic_message.png" width="30" height="30" style=" margin-left: 10px; float: right;"/>' +
                    '</div>'+
                    '<div style="float: right; height: 100%; width: 20%; ">' +
                    '<a href="#" data-toggle="modal" data-target="#myModal"><img src="/Content/Images/ic_warning.png" width="28" height="28" style=" margin-left: 20px; float: right;"/></a>' +
                    '</div>'+
                    '<div style="float: right; height: 100%; width: 20%;"></div>' +
                    '</div>'+
                    '</div>';

                var count = function () {
                    return 'some value';
                };


                createMarker(i, currentLocation, contentString);
               
            }

            //var results = $.parseJSON(result);
            //$(results).each(function (index, value) {
            //    var longitude = this['longitude'];
            //    var laitude = this['latitude'];
            //    var title = " " + this['tour_guide_id'];
            //    var currentLocation = new google.maps.LatLng(laitude, longitude);
            //    createMarker(index, currentLocation, title);
            //});
        },
        error: function (xhr) {
            alert('error');
        }
    });
}

function createMarker(index, latlng, title) {

   

    window.setTimeout(function () {
        listMarker[index] = new SlidingMarker({
            position: latlng,
            map: map,
            animation: google.maps.Animation.Gp,
            icon: pinImage,
            //shadow: pinShadow
        })
        listInforwindow[index] = new google.maps.InfoWindow({
            content: title
        });

        google.maps.event.addListener(listMarker[index], "click", function () {
            listInforwindow[index].open(map, listMarker[index]);
        });
    }, 200);
}

function MoverMarker(Latlng, index) {
    var duration = parseInt($('#durationOption').val());
    if (duration < 0) {
        duration = 1;
        $('#durationOption').val(duration);
    }
    listMarker[index].setDuration(duration);
    listMarker[index].setEasing($('#easingOption').val());
    listMarker[index].setPosition(Latlng);
}

function getMarkerSelected(id) {
    $.ajax({
        url: "/Home/GetLocationMarkerSelected/" + id,
        type: "GET",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        success: function (result) {
            var results = $.parseJSON(result);
            $(results).each(function (index, value) {

                var longitude = this['longitude'];
                var laitude = this['latitude'];
                var location = new google.maps.LatLng(laitude, longitude);
                panTo(laitude, longitude);
                map.setZoom(15);
                for (i = 0 ; i < listMarker.length; i++) {

                    listMarker[i].setIcon(pinImage);

                }
                listMarker[id - 1].setIcon(pinImageSelect);

            });
        },
        error: function (xhr) {
            alert('error');
        }
    });
}
var panPath = [];
var panQueue = [];
var STEPS = 50;
function panTo(newLat, newLng) {
    if (panPath.length > 0) {
        panQueue.push([newLat, newLng]);
    } else {

        panPath.push("LAZY SYNCRONIZED LOCK");
        var curLat = map.getCenter().lat();
        var curLng = map.getCenter().lng();
        var dLat = (newLat - curLat) / STEPS;
        var dLng = (newLng - curLng) / STEPS;

        for (var i = 0; i < STEPS; i++) {
            panPath.push([curLat + dLat * i, curLng + dLng * i]);
        }
        panPath.push([newLat, newLng]);
        panPath.shift();
        setTimeout(doPan, 20);
    }
}
function doPan() {
    var next = panPath.shift();
    if (next != null) {
        map.panTo(new google.maps.LatLng(next[0], next[1]));
        setTimeout(doPan, 20);
    } else {

        var queued = panQueue.shift();
        if (queued != null) {
            panTo(queued[0], queued[1]);
        }
    }
}
function messageClick() {
    var bla = 2;
    $.ajax({
        url: "/Home/RenderHomeOption/" + bla,
        type: "GET",
    })
    .done(function (partialViewResult) {
        $("#primary-div").html(partialViewResult);
    });

}
function mapClick() {
    var bla = 1;
    $.ajax({
        url: "/Home/RenderHomeOption/" + bla,
        type: "GET",
    })
    .done(function (partialViewResult) {
        $("#primary-div").html(partialViewResult);
    });
}

//// For POPUP
//var modal = document.getElementById('myModal');
//// Get the button that opens the modal
////var btn = document.getElementById("myBtn");
////// Get the <span> element that closes the modal
////var span = document.getElementsByClassName("closepopup")[0];

////// When the user clicks the button, open the modal
////btn.onclick = function () {
////    modal.style.display = "block";
////}
//// When the user clicks on <span> (x), close the modal
////span.onclick = function () {
////    modal.style.display = "none";
////}
//// When the user clicks anywhere outside of the modal, close it
//window.onclick = function (event) {
//    if (event.target == modal) {
//        modal.style.display = "none";
//    }
//}
//function showPopup() {
//    modal.style.display = "block";
//}
