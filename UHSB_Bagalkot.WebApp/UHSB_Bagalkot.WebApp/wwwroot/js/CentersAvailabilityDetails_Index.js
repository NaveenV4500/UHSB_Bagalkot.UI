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
    
    
    function renderDistrictDD(CentersData) {
        var $otherDropdown = $('#DistrictType');
        $otherDropdown.empty().append('<option value="">Please Select</option>');

        $.each(CentersData, function (i, item) {
            $otherDropdown.append('<option value="' + item.id + '">' + item.text + '</option>');
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
            $otherDropdown.append('<option value="' + item.centerId + '">' + item.centernameEng + ' - ' + item.centernameKnd+ '</option>');
        });
    }

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
    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }
});