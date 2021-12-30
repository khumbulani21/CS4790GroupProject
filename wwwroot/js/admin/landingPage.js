function processCheckIn(elem, enrollmentID) {
    var attendanceInfo = $(elem).closest(".attendanceInfo");
    var mealInfo = $(attendanceInfo).siblings('.mealInfo');
    var previousHtml = $(attendanceInfo).html();
    $(attendanceInfo).html("<strong>Processing....</strong>");
    $.ajax({
        type: "POST",
        url: "/api/Attendance/ProcessCheckIn/" + enrollmentID,
        dataType: "json",
        success: function (data) {
            var attendanceHtml = `<button class='button color-2' onclick='processCheckOut(this, ${data.attendanceID})'>Check Out</button><br/><span class='tiny'><strong>Checked In:</strong> ${data.checkedIn}</span> <i class="fi-x-circle text-danger large pointer" onclick="VerifyCancel('CheckIn' ,this, ${data.attendanceID})">‌</i>`;
            $(attendanceInfo).html(attendanceHtml);
            var mealHtml = `<div class='row'><div class='col-4'><strong>B</strong><br/><input type='checkbox' onclick='UpdateMeal(this)' data-attendance-id='${data.attendanceID}' data-meal='Breakfast'/></div><div class='col-4'><strong>L</strong><br/><input type='checkbox' onclick='UpdateMeal(this)' data-attendance-id='${data.attendanceID}' data-meal='Lunch'/></div><div class='col-4'><strong>S</strong><br/><input type='checkbox' onclick='UpdateMeal(this)' data-attendance-id='${data.attendanceID}' data-meal='Snack'/></div></div>`;
            $(mealInfo).html(mealHtml);
        },
        error: function (data) {
            alert("Processing the request failed, please try again.");
            $(attendanceInfo).html(previousHtml);
        }
    });
}

function processCheckOut(elem, attendanceID) {
    var attendanceInfo = $(elem).closest(".attendanceInfo");
    var previousHtml = $(attendanceInfo).html();
    $(attendanceInfo).html("<strong>Processing....</strong>");
    $.ajax({
        type: "POST",
        url: "/api/Attendance/ProcessCheckOut/" + attendanceID,
        dataType: "json",
        success: function (data) {
            var newHtml = `<span class='tiny'><strong>Checked In:</strong> ${data.checkedIn}</span><br/><span><strong>Checked Out:</strong> ${data.checkedOut}</span> <i class="fi-x-circle text-danger large pointer" onclick="VerifyCancel('CheckOut', this, ${data.attendanceID})">‌</i>`;
            $(attendanceInfo).html(newHtml);
        },
        error: function (data) {
            alert("Processing the request failed, please try again.");
            $(attendanceInfo).html(previousHtml);
        }
    });
}

function cancelCheckIn(elem, attendanceID) {
    var attendanceInfo = $(elem).closest(".attendanceInfo");
    var previousHtml = $(attendanceInfo).html();
    var mealInfo = $(attendanceInfo).siblings('.mealInfo');
    $(attendanceInfo).html("<strong>Processing....</strong>");
    $.ajax({
        type: "POST",
        url: "/api/Attendance/CancelCheckIn/" + attendanceID,
        dataType: "json",
        success: function (data) {
            var newHtml = `<button class="button color-12" onclick="processCheckIn(this, ${data.enrollmentID})">Check In</button>`;
            $(attendanceInfo).html(newHtml);
            $(mealInfo).html('');
        },
        error: function (data) {
            alert("Processing the request failed, please try again.");
            $(attendanceInfo).html(previousHtml);
        }
    });
}

function cancelCheckOut(elem, attendanceID) {
    var attendanceInfo = $(elem).closest(".attendanceInfo");
    var previousHtml = $(attendanceInfo).html();
    $(attendanceInfo).html("<strong>Processing....</strong>");
    $.ajax({
        type: "POST",
        url: "/api/Attendance/CancelCheckOut/" + attendanceID,
        dataType: "json",
        success: function (data) {
            var newHtml = `<button class='button color-2' onclick='processCheckOut(this, ${data.attendanceID})'>Check Out</button><br/><span class='tiny'><strong>Checked In:</strong> ${data.checkedIn}</span> <i class="fi-x-circle text-danger large pointer" onclick="VerifyCancel('CheckIn', this, ${data.attendanceID})">‌</i>`;
            $(attendanceInfo).html(newHtml);
        },
        error: function (data) {
            alert("Processing the request failed, please try again.");
            $(attendanceInfo).html(previousHtml);
        }
    });
}

function VerifyCancel(chosenAction, elem, attendanceID) {

    swal({
        title: "Are you sure?",
        text: "Removing an attendance time cannot be undone and will not maintain the previous time if resubmitted.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            console.log(chosenAction);
            if (chosenAction === "CheckIn") {
                cancelCheckIn(elem, attendanceID);
            }
            else {
                cancelCheckOut(elem, attendanceID);
            }
        }
    });
}

function UpdateMeal(elem, attendanceID) {
    var meal = $(elem).data('meal');
    var attendanceID = $(elem).data("attendance-id");
    $.ajax({
        type: "POST",
        url: "/api/Attendance/UpdateMeal/" + attendanceID + "/" + meal,
        dataType: "json",
        success: function (data) {
            toastr.success(meal + " updated successfully!");
        },
        error: function (data) {
            toastr.error(meal + " failed to be updated.");
        }
    });
}