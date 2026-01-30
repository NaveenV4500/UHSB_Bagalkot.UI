$(document).ready(function () {
    $('#ItemImagesBody').find('input, select, textarea, button').prop('disabled', true);
    $('#addRow').prop('disabled', true);
    var fileBaseUrl = $("#fileBaseUrl").val();
    fileBaseUrl = fileBaseUrl.replace("api", "");
    var currentDescriptionInput;
    var editorInstance;
    window.alertMessageAfterSavePayment = "";
    window.GridFilterViewUrl = "FTPDocuments/GetSearchView";
    window.orderByColumnId = "0";
    window.isDescendingFilter = false;
    var pageSizeKey = 'pagesize_taxableinward';
    var loader = $("#ajaxLoader");

    GetPageSizeFromStorage(pageSizeKey);
    _init();

    function _init() {
        showLoader();
        getDistrictDD();
        getrecordheadtypeDD();
        hideLoader();
    }

    function getDistrictDD() {
        callGetApi('WeatherCast/DistrictDD', {}, renderDistrictDD);
    }


    function getrecordheadtypeDD() {
        callGetApi('AvailabilityTools/recordheadtypeDD', {}, renderplantingcenters);
    }


    $(document).on('click', '.district-btn', function () {
        debugger;
        var districtId = $(this).data('districtid');
         
        $('.district-btn').removeClass('active');
        $(this).addClass('active');
        callGetApi('WeatherCast/Weekly?districtId=' + districtId, {}, renderWeeklyData);
         
    });
    function formatDateDDMMYYYY(dateStr) {
        if (!dateStr) return '';

        var date = new Date(dateStr);
        var day = String(date.getDate()).padStart(2, '0');
        var month = String(date.getMonth() + 1).padStart(2, '0');
        var year = date.getFullYear();

        return `${day}-${month}-${year}`;
    }

    function renderDistrictDD(CentersData) {

        var $container = $('#DistrictButtons');
        $container.empty();

        if (!CentersData || CentersData.length === 0) {
            $container.append(
                '<div class="col-12 text-center">' +
                '<div class="alert alert-warning">ಜಿಲ್ಲೆಗಳ ಮಾಹಿತಿ ಲಭ್ಯವಿಲ್ಲ</div>' +
                '</div>'
            );
            return;
        }

        $.each(CentersData, function (i, item) {

            var buttonHtml =
                '<div class="col-6 col-md-4 col-lg-3 mb-4">' +
                '<button type="button" ' +
                'class="district-btn btn btn-outline-primary w-100" ' +
                'data-districtid="' + item.id + '">' +
                item.text +
                '</button>' +
                '</div>';

            $container.append(buttonHtml);
        });
    }


    function renderplantingcenters(CentersData) {
        var $otherDropdown = $('#Centervariatestype');
        $otherDropdown.empty().append('<option value="">Please Select</option>');

        $.each(CentersData, function (i, item) {
            $otherDropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
        });
    }

    function renderrecordheadtype(CentersData) {
        var $otherDropdown = $('#Centerstype');
        $otherDropdown.empty().append('<option value="">Please Select</option>');

        $.each(CentersData, function (i, item) {
            $otherDropdown.append('<option value="' + item.centerId + '">' + item.centernameEng + ' - ' + item.centernameKnd + '</option>');
        });
    }
    function renderWeeklyData(CentersData) {
        var html = '';
        debugger;
        if (!CentersData || CentersData.length === 0 || CentersData == "No reports found for the given district.") {
            html = '<div class="alert alert-warning">No weekly data available <br> ವಾರದ ಮಾಹಿತಿಯು ಲಭ್ಯವಿಲ್ಲ</div>';
            $('#weeklyWeatherBody').html(html);
            $('#weeklyWeatherModal').modal('show');
            return;
        }

        $.each(CentersData, function (i, item) {

            var fileUrl = fileBaseUrl +  item.filePath;
            var startDate = formatDateDDMMYYYY(item.weekStartDate);
            var endDate = formatDateDDMMYYYY(item.weekEndDate);

            html += `
            <div class="border rounded p-3 mb-3">
                <div class="fw-bold mb-2">
                    ${item.weekType}
                </div>

                <div class="small text-muted mb-2">
                    ${startDate} to ${endDate}
                </div>

                <button type="button"
                        class="btn btn-sm btn-primary open-week-file"
                        data-fileurl="${fileUrl}">
                    View Report
                </button>
            </div>
        `;
        });

        $('#weeklyWeatherBody').html(html);
        $('#weeklyWeatherModal').modal('show');
    }


    $(document).on('click', '.open-week-file', function () {
        var fileUrl = $(this).data('fileurl'); 

        if (fileUrl) {
            window.open(fileUrl, '_blank');    
        }

    });



    //change Centerstype
    //$('#Centerstype').on('change', function () {
    //    debugger;
    //    debugger;
    //    resetDropdowns(['#CropsDD', '#SectionDD']);
    //    var CenterstypeId = $("Centerstype").val();
    //    callGetApi('AvailabilityTools/plantingcentersDD', {}, renderplantingcenters);
    //});

    //change DD
    $('#DistrictType').on('change', function () {
        showLoader();

        resetDropdowns(['#Centerstype']);
        var districtid = $(this).val();
        callGetApi('AvailabilityTools/getcenterbydistrict?districtid=' + districtid, {}, renderrecordheadtype);
        hideLoader();
    });

    function callGetApi(actionName, params = {}, onSuccess, onError) {
        //showLoader();

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/CommonApi/ForwardApiResponse?action_name=' + actionName,
            type: 'GET',
            success: onSuccess,
            error: function (xhr) {

                hideLoader();
                console.error(xhr); if (onError) onError(xhr);
            }
        });
        //hideLoader();
    }

    // Cascading dropdowns
    $('#Categoriestype').on('change', function () {
        resetDropdowns(['#Cropstype', '#Sectionstype', '#SubSectionstype']);
        loadDropdown(GetRootPath(window.virtualPath) + '/Dashboard/GetCrops?catId=' + $(this).val(), '#Cropstype', '--Select Crop--');
    });

    function resetDropdowns(selectors) {
        selectors.forEach(function (sel) {
            $(sel).val('').html('<option value="">--Select--</option>');
        });
    }

    //gride
    //#region Paging Click

    window.init = function () {

        //LoadAjaxAnimation(true);
        var tablebody = $('#gridContent table tbody');
        if (!tablebody.children().children("td").length < 2) {
            //tablebody.append('<tr><td colspan="27"><div style="min-height:120px;"></div></td><tr>');
        }
        debugger;
        loader.width(tablebody.width());

        $.ajax({
            cache: false,
            type: "POST",
            dataType: 'json',
            url: GetRootPath(window.virtualPath) + '/AvailabilityToolsWeb/GetGridContentV1',
            data: {
                "currentPage": currentPage,
                "pageSize": pageSize,
                "orderBy": window.orderByColumnId,
                "isDescending": window.isDescendingFilter,
                "filterDetails": ""// window.filterCollection.length > 0 ? JSON.stringify(window.filterCollection) : ""

            },
            success: function (response, successStatusText, responseJqXhrObject) {
                //window.LogAjaxRequestDetails(responseJqXhrObject);
                debugger;
                debugger;
                gridJson = response;
                console.log(JSON.stringify(gridJson));

                tablebody.find('tr:gt(0)').remove();

                if (response.data.itemDetails && response.data.itemDetails.length) {

                    $.each(response.data.itemDetails, function (index, item) {

                        let actionHtml = "";
                        if (response.CanEdit) {
                            actionHtml += `<span class="btn-link edit" data-id="${item.identifier}" style="cursor:pointer">Edit</span>`;
                        }
                        if (response.CanDelete) {
                            if (response.CanEdit) actionHtml += " | ";
                            actionHtml += `<span class="btn-link delete" data-id="${item.identifier}" style="cursor:pointer">Delete</span>`;
                        }
                        if (!response.CanEdit && !response.CanDelete) {
                            actionHtml = `<span class="btn-link view" data-id="${item.identifier}" style="cursor:pointer">View</span>`;
                        }

                        let row =
                            `<tr>
            <td class="text-center">${((currentPage - 1) * pageSize) + (index + 1)}</td>
                        <td>${BindValueToGrid(item.centername_eng)}</td>

            <td>${BindValueToGrid(item.recordHead_eng)}</td>
            <td>${BindValueToGrid(item.availToolname_eng)}</td>
            <td>${BindValueToGrid(item.quantity)}</td>
            <td>${BindValueToGrid(item.unit)}</td>
            <td>${BindValueToGrid(item.price)}</td>
            <td>${BindValueToGrid(item.availabilityDate, true)}</td>
            <td>${BindValueToGrid(item.remarks)}</td>
            <td class="text-center">${actionHtml}</td>
        </tr>`;

                        $("#itemGrid tbody").append(row);
                    });


                }
                else {
                    tablebody.append('<tr><td colspan="27"><h3 style="width:400px;min-height:120px;">No Data Found!!!</h3></td><tr>');
                }
                totalRecords = response.data.totalCount;
                setupControls();
                pagingDetails();
                var elem = $('#pagingButtons li a');
                $('#pagingButtons li a').removeClass('active');
                $('#pagingButtons li a:contains(' + currentPage + ')').parent('li').addClass("active")
                $('#pagingButtons li a:contains(' + currentPage + ')').css("cursor", "not-allowed")
                //LoadAjaxAnimation(false);
                loader.width(tablebody.width());
            },
            error: function (error) {
                if (error.status != 200) {
                    alert("You are not authorized to access the page.");
                    return false;
                }
                if (error.status == 302) {
                    //alert(302);
                    return
                }
                alert(error.statusText);
                //Removed Con_sole logging
                //LoadAjaxAnimation(false);
                loader.width(tablebody.width());
            }
        })

    }

    //#endregion

    window.init();

    //End gride
    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }
});