
//#region BindValueToGridLogic
function isNumber(evt, element) {

    var charCode = (evt.which) ? evt.which : event.keyCode

    if (
        (charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
        (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
        (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function BindValueToGrid(value, isDate, isBool) {
    if (value == null || value == undefined) {
        if (isBool) {
            return "Not Applicable"
        }
        else {
            return ""
        }
    }
    else {
        if (isBool) {
            if (value == true) {
                return "Yes"
            }
            else {
                return "No"
            }
        }
        else if (isDate) {
            //Convert to date
            return ToLocalFormat(value, "-")
        }
        else {
            return value
        }
    }
}

function ToFixed2Digit(value) {
    if (!value) {
        value = 0;
    }
    value = parseFloat(value).toFixed(2);
    return value;
}

//#endregion

//#region ToLocalDate

function ToLocalFormat(input_dateformat, saperator) {

    if (!saperator) {
        saperator = "/";
    }
    if (!input_dateformat) {
        return ''
    }
    var date = new Date(parseInt(input_dateformat.substr(6)));
    if (date == "Thu Jan 01 1970 05:30:00 GMT+0530 (India Standard Time)")
        date = new Date(input_dateformat);
    var month = (date.getMonth() + 1); //months from 1-12
    if (parseInt(month) <= 9) {
        month = "0" + month;
    }

    var day = date.getDate();
    if (parseInt(day) <= 9) {
        day = "0" + day;
    }

    var year = date.getFullYear();

    return day + saperator + month + saperator + year;
}

function ToLocalFormat2(input_dateformat, saperator) {

    if (!saperator) {
        saperator = "/";
    }
    if (!input_dateformat) {
        return ''
    }

    input_dateformat = input_dateformat.replace(/-/g, '/');
    input_dateformat = input_dateformat.replace('T', ' ');
    input_dateformat = input_dateformat.replace(/(\+[0-9]{2})(\:)([0-9]{2}$)/, ' UTC\$1\$3');
    var date = new Date(input_dateformat);

    var month = (date.getMonth() + 1); //months from 1-12
    if (parseInt(month) <= 9) {
        month = "0" + month;
    }

    var day = date.getDate();
    if (parseInt(day) <= 9) {
        day = "0" + day;
    }

    var year = date.getFullYear();

    return day + saperator + month + saperator + year;
}

function ToLocalFormatWithTime(input_dateformat, saperator) {

    if (!saperator) {
        saperator = "/";
    }
    if (!input_dateformat) {
        return ''
    }
    var date = new Date(parseInt(input_dateformat.substr(6)));
    if (date == "Thu Jan 01 1970 05:30:00 GMT+0530 (India Standard Time)")
        date = new Date(input_dateformat);
    var month = (date.getMonth() + 1); //months from 1-12
    if (parseInt(month) <= 9) {
        month = "0" + month;
    }

    var day = date.getDate();
    if (parseInt(day) <= 9) {
        day = "0" + day;
    }

    var year = date.getFullYear();

    var hours = date.getHours();
    if (parseInt(hours) <= 9) {
        hours = "0" + hours;
    }

    var minutes = date.getMinutes();
    if (parseInt(minutes) <= 9) {
        minutes = "0" + minutes;
    }

    var seconds = date.getSeconds();
    if (parseInt(seconds) <= 9) {
        seconds = "0" + seconds;
    }

    return day + saperator + month + saperator + year + " " + hours + ":" + minutes + ":" + seconds;
}
//#endregion

//#region ToLocalDateYYYYMMDD

function To_YYYYMMDD(input_date) {

    if (!input_date) {
        return ''
    }
    var date = new Date(parseInt(input_date.substr(6)));
    var month = date.getMonth() + 1; //months from 1-12
    if (parseInt(month) < 9) {
        month = "0" + month;
    }
    var day = date.getDate();
    if (parseInt(day) < 9) {
        day = "0" + day;
    }
    var year = date.getFullYear();

    return day + "-" + month + "-" + year;
}

//#endregion

//#region DatePickerFormat

function JavascriptDateToDatePickerFormat(date, saperator) {

    if (!saperator) {
        saperator = "/";
    }
    if (!date) {
        return ''
    }
    var month = (date.getMonth() + 1); //months from 1-12
    if (parseInt(month) <= 9) {
        month = "0" + month;
    }

    var day = date.getDate();
    if (parseInt(day) <= 9) {
        day = "0" + day;
    }

    var year = date.getFullYear();

    return day + saperator + month + saperator + year;
}

//#endregion

//#region DatePickerFormat

function GetStartAndEndDateOfCurrentMonth() {
    var dateTimeNow = new Date();
    var startDateOfCurrentMonth = new Date();
    startDateOfCurrentMonth.setDate(1);
    var endDateOfCurrentMonth = new Date(dateTimeNow.getFullYear(), dateTimeNow.getMonth() + 1, 0);
    return {
        StartDate: startDateOfCurrentMonth,
        EndDate: endDateOfCurrentMonth
    }
}

function GetStartAndEndDateOfSelectedMonthYear(month, year) {
    var dateTimeStart = new Date(year, (month - 1), 1, 0, 0, 0, 0);
    var startDateOfCurrentMonth = dateTimeStart;
    startDateOfCurrentMonth.setDate(1);
    var endDateOfCurrentMonth = new Date(dateTimeStart.getFullYear(), dateTimeStart.getMonth() + 1, 0);
    return {
        StartDate: startDateOfCurrentMonth,
        EndDate: endDateOfCurrentMonth
    }
}

//#endregion

//#region ISO Format to Local dd MM yy

var IsoDateToLocal = function (date) {
    var date = date.match(/^([0-9]{4})-([0-9]{2})-([0-9]{2})T([0-9]{2}):([0-9]{2}):([0-9]{2})/);
    if (date == null) {
        return false;
    } else {
        var dateObj = {
            dateFormat1: date[3] + '-' + date[2] + '-' + date[1],
            //dateFormat2: date[1] + '-' + date[2] + '-' + date[3],
            //dateFormat3: date[2] + '/' + date[3] + '/' + date[1],
            //time: date[4] + ':' + date[5] + ':' + date[6],
        };
        if (dateObj.dateFormat1 == '01-01-0001') {
            return "";
        }
        else {
            return dateObj.dateFormat1;
        }

    }
};

//#endregion

//#region Reset Error

function ResetForm(formSelector) {

    $(formSelector + ' ' + '.validation-summary-errors li').remove();
    $(formSelector + ' ' + '.validation-summary-valid li').remove();
    $(formSelector + ' ' + '.field-validation-error').each(function () {
        $(this).html("");
    })
    //  $(formSelector + ' ' + '. validation-summary-valid').remove();

    $(formSelector)[0].reset();
}

//#endregion

//#region Calculate Sum For All Record In Current Page Table
function CalculateSumForAllRecordInCurrentPage(elements, isdecimal) {

    if (isdecimal == undefined) {
        isdecimal = false;
    }
    var total = 0;

    $.each(elements, function (index, item) {

        var itemVal = $(item).text();
        if (isdecimal) {
            itemVal = parseFloat(itemVal);
            //alert("itemVal is decimal: " + itemVal);
        }
        else {
            itemVal = parseInt(itemVal);
            //alert("itemVal is int: " + itemVal);
        }
        if (!isNaN(itemVal)) {
            total += itemVal;
            //alert("total: " + total);
        }
    })
    return total
}

//#endregion


//#region Storage of page size

function GetPageSizeFromStorage(name) {
    var pagesizeSession = localStorage.getItem(name);
    if (pagesizeSession && parseInt(pagesizeSession)) {
        $('#gridmemberCount').val(pagesizeSession);
        pageSize = parseInt(pagesizeSession);
    }
    else if (parseInt($('#gridmemberCount').val()) != pageSize) {
        pageSize = parseInt($('#gridmemberCount').val());
    }
}

function SetPageSizeFromStorage(name) {
    localStorage.setItem(name, $('#gridmemberCount').val());
}

//#endregion

//#region to add Item wise tax in CBS data
function SumOfTotalAmount(data, propertyName) {

    var total = 0;
    for (var i = 0; i < data.length; i++) {
        total += parseFloat((data[i][propertyName] ? data[i][propertyName] : 0));
    }
    return total.toFixed(2);
}
//#endregion

//#region ErrorLog

function ErrorLog(errorData, errorType, comment) {
    if (typeof errorData === 'string' || errorData instanceof String) {
    }
    else
        errorData = JSON.stringify(errorData);

    var model = {
        Error: errorData,
        ErrorType: errorType,
        Comment: comment
    }

    $.ajax({
        url: $('#rootpath').text() + 'Error/Log_Error',
        type: "POST",
        data: JSON.stringify({ model: model }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response, successStatusText, responseJqXhrObject) {
            window.LogAjaxRequestDetails(responseJqXhrObject);

        },
        error: function (error) {

            //Removed Con_sole logging
        }
    })

}

//#endregion

//#region Check_IsObject

function isTypeOfObject(value) {
    return value && typeof value === 'object' && value.constructor === Object;
};

//#endregion

//#region Download File

function DownloadJsonFile(filePathDetails) {

    for (var i = 0; i < filePathDetails.length; i++) {
        window.open($('#rootpath').text() + "GSTR1General/DownloadJsonFileFromPath?relativePath=" + filePathDetails[i], '_blank');
    }
}

function DownloadJsonFileAsZip(filePathDetails) {
    debugger;
    $('#customForm').remove();
    $form = $('<form id="customForm" style="display:none;" method="post" action="' + $('#rootpath').text() + 'GSTR1General/DownloadJsonFileAsZip' + '"></form>');

    var count = 0;
    for (var i = 0; i < filePathDetails.length; i++) {
        for (var j = 0; j < filePathDetails[i].FilePaths.length; j++) {
            $form.append('<input type="text" name="[' + count + '].StateId" value="' + filePathDetails[i].StateId + '">');
            $form.append('<input type="text" name="[' + count + '].FilePath" value="' + filePathDetails[i].FilePaths[j] + '">');
            count++;
        }
    }

    $form.attr('target', "_blank");
    $('body').append($form);
    $form.submit();
    return false;
}
function DownloadJsonFileAsZipGstanx1(filePathDetails) {
    debugger;
    $('#customForm').remove();
    $form = $('<form id="customForm" style="display:none;" method="post" action="' + $('#rootpath').text() + 'GSTANX1General/DownloadJsonFileAsZip' + '"></form>');

    var count = 0;
    for (var i = 0; i < filePathDetails.length; i++) {
        for (var j = 0; j < filePathDetails[i].FilePaths.length; j++) {
            $form.append('<input type="text" name="[' + count + '].StateId" value="' + filePathDetails[i].StateId + '">');
            $form.append('<input type="text" name="[' + count + '].FilePath" value="' + filePathDetails[i].FilePaths[j] + '">');
            count++;
        }
    }

    $form.attr('target', "_blank");
    $('body').append($form);
    $form.submit();
    return false;
}
//#endregion
 

