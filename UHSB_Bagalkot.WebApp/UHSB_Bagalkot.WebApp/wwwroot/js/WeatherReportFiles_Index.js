$(document).ready(function () {

    $("#uploadForm").on("submit", function (e) {
        e.preventDefault();

        var district = $("#DistrictId").val();
        var desc = $("#Description").val().trim();
        var startDate = $("#StartDate").val();
        var endDate = $("#EndDate").val();
        var file = $("#file").val();

        // Basic validations
        if (!district) {
            alert("Please select a district.");
            return;
        }
        if (desc === "") {
            alert("Please enter a description.");
            return;
        }
        if (!startDate) {
            alert("Please select a start date.");
            return;
        }
        if (!endDate) {
            alert("Please select an end date.");
            return;
        }
        if (new Date(endDate) < new Date(startDate)) {
            alert("End Date cannot be earlier than Start Date.");
            return;
        }
        if (!file) {
            alert("Please upload a file.");
            return;
        }
        this.submit();

    });


    function formatDate(date) {
        var d = new Date(date);
        var month = (d.getMonth() + 1).toString().padStart(2, '0');
        var day = d.getDate().toString().padStart(2, '0');
        var year = d.getFullYear();
        return year + '-' + month + '-' + day;
    }

    var today = new Date();

    // Monday of current week
    var firstDayOfWeek = new Date(today);
    firstDayOfWeek.setDate(today.getDate() - today.getDay() + 1);

    // Sunday of current week
    var lastDayOfWeek = new Date(firstDayOfWeek);
    lastDayOfWeek.setDate(firstDayOfWeek.getDate() + 6);

    var minDate = "2000-01-01"; // any past date
    var maxDate = formatDate(lastDayOfWeek);

    // Apply limits to date inputs
    $("#StartDate, #EndDate").attr("min", minDate);
    $("#StartDate, #EndDate").attr("max", maxDate);

    // When user selects a date
    //$("#StartDate, #EndDate").on("change", function () {
    //    var selectedDate = $(this).val();
    //    alert("You selected: " + selectedDate);
    //});

});