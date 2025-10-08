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

    });

});