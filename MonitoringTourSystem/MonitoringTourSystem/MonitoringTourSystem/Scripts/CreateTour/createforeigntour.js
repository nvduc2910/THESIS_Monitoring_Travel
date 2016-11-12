$(document).ready(function () {

    var max_fields = 100; //maximum input boxes allowed
    var wrapper = $(".schedule-form"); //Fields wrapper
    var add_button = $(".add_field_button"); //Add button ID
    var province_button = $(".country");
    var x = 1; //initlal text box count

    $('#datetimepicker2').datetimepicker();
    $('#datetimepicker1').datetimepicker();
    $('#datetimepicker3').datetimepicker();
    var listPlace = [];
    var listTourGuide = [];
    var listProvince = [];

    $("#datetimepicker1").on("dp.change", function (e) {
        getTourGuideAvailable();
    });


    $("#datetimepicker3").on("dp.change", function (e) {

        getTourGuideAvailable();

    });

    function sendFile(file) {
        var formData = new FormData();
        formData.append('file', $('#f_UploadImage')[0].files[0]);
        $.ajax({
            type: 'post',
            url: '/CreateTour/FileUpload',
            data: formData,
            success: function (result) {

                var pathImage = result.PathImage;
                var success = result.Success;
                if (success == true) {
                    console.log(pathImage);
                    $("#myUploadedImg").attr("src", "http://localhost:20261/Content/Images/" + pathImage);
                }
            },
            processData: false,
            contentType: false,
            error: function () {
                alert("Whoops something went wrong!");
            }
        });
    }
    var _URL = window.URL || window.webkitURL;
    $("#f_UploadImage").on('change', function () {

        var file, img;
        if ((file = this.files[0])) {
            img = new Image();
            img.onload = function () {
                sendFile(file);
            };
            img.onerror = function () {
                alert("Not a valid file:" + file.type);
            };
            img.src = _URL.createObjectURL(file);
        }
    });

    $.ajax({
        url: "/CreateTour/GetCountry",
        type: "GET",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        success: function (result) {
            var results = $.parseJSON(result);
            $(results).each(function (index, value) {
                var countryName = this['country_name'];
                var countryID = this['country_id'];
                $(".country").append(new Option(countryName, countryID))
            });
            $(".country").select2({

            });
        },
        error: function (xhr) {
            alert('error');
        }
    });
    //$.ajax({
    //    url: "/CreateTour/GetListTourGuide",
    //    type: "GET",
    //    dataType: 'json',
    //    contentType: 'application/json; charset=utf-8',
    //    async: true,
    //    processData: false,
    //    cache: false,
    //    success: function (result) {
    //        var results = $.parseJSON(result);
    //        $(results).each(function (index, value) {
    //            var tourGuideID = this['tourguide_id'];
    //            var tourGuideName = this['tourguide_name'];
    //            $("#tourguide").append(new Option(tourGuideName, tourGuideID))
    //        });
    //        $("#tourguide").select2({

    //        });
    //    },
    //    error: function (xhr) {
    //        alert('error');
    //    }
    //});

    $(document).on('change', '.schedule-form .tour-travel .country', function () {

        var parent = $(this).parent();
        parent.closest('.tour-travel').find('.placeschedule:last').children().remove();
        var vehecial = ["Xe máy", "Xe ô-tô", "Máy bay"];
        var id = $(this).val();
        $.ajax({
            url: "/CreateTour/GetListPlaceForCountry/" + id,
            type: "GET",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            async: true,
            processData: false,
            cache: false,
            success: function (result) {
                var results = $.parseJSON(result);
                $(results).each(function (index, value) {

                    var placeName = this['place_name'];
                    var placeId = this['place_id'];
                    parent.closest('.tour-travel').find('.placeschedule:last').append(new Option(placeName, placeId))
                });
                parent.closest('.tour-travel').children().find('.placeschedule:last').select2({
                });
            },
            error: function (xhr) {
                alert('error');
            }
        });


        parent.closest('.tour-travel').children().find('.vehicalschedule:last').select2({
            data: vehecial


        });
    });

    function yourfunction() {
        $('.datepicker_init').datetimepicker({

        });
        $('.datepicker_end').datetimepicker({

        });

    }
    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();
        var parent = $(this).parent();
        if (x < max_fields) { //max input box allowed
            x++; //text box increment
            $(wrapper).append('<div class="tour-travel form-group">\
                    <div class="row" style="width: 100%; margin-left: 0px;">\
                        <div class="row">\
                            <label for="exampleInputEmail1" style="margin-left: 15px;">Nhập hành trình tour</label>\
                        </div>\
                        <div class="row">\
                            <div class="col-sm-4">\
                                 <div class="input_fields_wrap">\
                                    <div class="input-group date datepicker_end">\
                                        <input class="form-control" placeholder="Chọn thời gian"  type="text" name="mytext[]">\
                                        <span class="input-group-addon">\
                                            <span class="glyphicon glyphicon-calendar"></span>\
                                        </span>\
                                    </div>\
                                </div>\
                            </div>\
                            <div class="col-sm-3">\
                                <select class="country" style="width: 100%; height: 34px;">\
                                    <option selected disabled value="0">Chọn đất nước</option>\
                                </select>\
                            </div>\
                            <div class="col-sm-3">\
                                <select class="placeschedule" style="width: 100%; height: 34px; border-radius: 5px;">\
                                     <option selected disabled value="0">Chọn địa điểm</option>\
                                </select>\
                            </div>\
                            <div class="col-sm-2">\
                                <select class="vehicalschedule" style="width: 100%; height: 34px; border-radius: 5px;">\
                                <option selected disabled value="0">PT</option>\
                                </select>\
                            </div>\
                        </div>\
                        <div class="row" style="margin-top: 20px;">\
                            <textarea  style="resize: none; border-radius: 5px; padding-left: 12px; padding-top: 12px; margin-left: 15px; width: 96%; margin-right: 15px;" rows="4" class="descriptionTour" placeholder="Nhập mô tả về tour..."></textarea>\
                        </div>\
                    </div>\
                    <button href="#" style="margin-left: 0px; margin-top: 10px;" class="btn remove_field btn-warning">Xóa</button>\
                </div>\
                ');
            var vehecial = ["Xe máy", "Xe ô-tô", "Máy bay"];
            $.ajax({
                url: "/CreateTour/GetCountry",
                type: "GET",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                async: true,
                processData: false,
                cache: false,
                success: function (result) {
                    var results = $.parseJSON(result);
                    $(results).each(function (index, value) {
                        var countryName = this['country_name'];
                        var countryID = this['country_id'];
                        parent.children().find('.country:last').append(new Option(countryName, countryID))
                    });
                    parent.children().find('.country:last').select2({

                    });
                },
                error: function (xhr) {
                    alert('error');
                }
            });
            parent.children().find('.vehicalschedule:last').select2({
                data: vehecial
            });
        }
        yourfunction();
    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault(); $(this).parent('div').remove(); x--;
    })
    yourfunction();

});

function getTourGuideAvailable() {

    var startday = $("#startday").val();
    var endday = $("#endday").val();


    if (startday == null || startday == null || endday == null || endday == null) {
        $('#warning').text("Vui lòng chọn ngày bắt đầu và kết thúc");
        return;
    }
    if (startday > endday) {
        $('#warning').text("Vui lòng chọn ngày bắt đầu nhỏ kết thúc");
        return;
    }

    $("#processgettourguide").html("<img src='/Content/process_small.gif' width='20' height='20' style='margin-left: 20px;' /> ");
    $.ajax({
        url: "/CreateTour/GetTourGuideAvailable",
        type: "POST",
        data: {
            departuredate: startday,
            returndate: endday
        },
        success: function (result) {
            var results = $.parseJSON(result);
            $('#tourguide').empty();
            $(results).each(function (index, value) {

                var tourGuideID = this['tourguide_id'];
                var tourGuideName = this['tourguide_name'];
                $("#tourguide:last").append(new Option(tourGuideName, tourGuideID))
            });
            $("#tourguide:last").select2({

            });
            $('#warning').text("");
            $("#processgettourguide").html("");
        },
        error: function (xhr) {

        }
    });
}

function addNewTour() {
    var isValidSchedule;
    var listSchedule = [];
    var index = 0;
    var tourcode = $("#tourcode").val();
    var nameTour = $("#nametour").val();
    var tourguideID = $("#tourguide").val();
    var numberoftourist = $("#numberoftourist").val();
    var day = $("#day").val();
    var startday = $("#startday").val();
    var endday = $("#endday").val();
    var description = $("#descriptionTour").val();
    var cover_photo = "photo";
    if (tourcode == null || tourcode == "") {
        swal("Vui lòng nhập mã tour!")
        return;
    }

    if (nameTour == null || nameTour == "") {

        swal("Vui lòng nhập tên tour!")
        return;
    }
    if (tourguide == null || tourguide == "") {
        swal("Chọn hướng dẫn viên cho tour");
        return;
    }
    if (numberoftourist == null || numberoftourist == "") {
        swal("Nhập số lượng du khách");
        return;
    }
    if (day == null || day == "") {
        swal("Nhập số ngày của tour");
        return;
    }
    if (startday == null || startday == "") {
        swal("Nhập ngày khởi hành tour");
        return;
    }
    if (endday == null || endday == "") {
        swal("Nhập ngày kết thúc tour");
        return;
    }
    else if (description == null || description == "") {
        swal("Nhập mô tả về tour");
        return;
    }
    else if (cover_photo == null || cover_photo == "") {
        swal("cover photo");
        return;
    }

    if (startday > endday) {
        swal("Thời gian kết thúc tour phải lớn hơn thời gian bắt đầu");
        return;
    }


    $('div.tour-travel').each(function () {

        var inps = document.getElementsByName('mytext[]');
        var time = inps[index].value;
        var placeId = $(this).children().find('.placeschedule option:selected').val();
        var placename = $(this).children().find('.placeschedule option:selected').text();
        var placenameVal = $(this).children().find('.placeschedule option:selected').val();
        var vehicalschedule = $(this).children().find('.vehicalschedule option:selected').text();
        var vehicalscheduleVal = $(this).children().find('.vehicalschedule option:selected').val();
        var descriptionTour = $(this).children().find('.descriptionTour').val();
        var countryName = $(this).children().find('.country option:selected').text();
        if (time == null || time == "") {
            swal("Nhập thời gian hành trình tour!");
            isValidSchedule = false;
            return;
        }

            //else if(time < startday || time > endday)
            //{
            //    swal("Thời gian " + time + " phải nằm trong khoảng thời gian bắt đầu và kết thúc tour");
            //    isValidSchedule = false;
            //}
        else if (placenameVal == "0") {
            swal("Nhập địa điểm tour!");
            isValidSchedule = false;
            return;
        }

        else if (vehicalscheduleVal == "0") {
            swal("Nhập phương tiện!");
            isValidSchedule = false;
            return;
        }
        else if (descriptionTour == "" || descriptionTour == null) {
            swal("Nhập mô tả chi tiết về hành trình tour!");
            isValidSchedule = false;
            return;
        }
        alert(placename);
        listSchedule[index] =
            {
                tour_schedule1: null,
                tour_id: null,
                place_id: placeId,
                place_name: placename,
                vehicle: vehicalschedule,
                time: time,
                description: descriptionTour,
                nameProvince: countryName
            };
        index++;
    });

    if (isValidSchedule == false) {
        listSchedule = [];
        return;
    }
    $.ajax({
        url: "/CreateTour/AddNewTour",
        type: "POST",
        data: {

            tour_code: tourcode,
            manager_id: "123",
            tourguide_id: tourguideID,
            tour_name: nameTour,
            status: "Opening",
            departure_date: startday,
            return_date: endday,
            tourist_quantity: numberoftourist,
            description: description,
            day: day,
            cover_photo: cover_photo,
            ListTourSchedule: listSchedule
        },
        success: function (result) {
            swal("Thêm Tour!", "Bạn đã thêm tour thành công!", "success")
            location.reload();
        },
        error: function (xhr) {
            sweetAlert("Oops...", "Thêm tour thất bại, vui lòng thử lại", "error");
        }
    });
}