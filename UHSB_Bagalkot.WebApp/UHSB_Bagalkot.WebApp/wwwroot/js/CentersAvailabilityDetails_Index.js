$(document).ready(function () {
    var token = $('input[name="__RequestVerificationToken"]').val();

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
            $otherDropdown.append('<option value="' + item.centerId + '">' + item.centernameEng + ' - ' + item.centernameKnd + '</option>');
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

                        actionHtml += `<span class="btn-link edit" data-id="${item.productId}" style="cursor:pointer">Edit</span>`;

                        if (response.canDelete) {
                            if (response.canEdit) actionHtml += " | ";
                            actionHtml += `<span class="btn-link delete" data-id="${item.productId}" style="cursor:pointer">Delete</span>`;
                        }

                        let row = `
<tr>
    <td class="text-center">${actionHtml}</td>
    <td class="text-center">${((currentPage - 1) * pageSize) + (index + 1)}</td>

    <td>${BindValueToGrid(item.centername_eng)}</td>
    <td>${BindValueToGrid(item.recordHead_eng)}</td>
    <td>${BindValueToGrid(item.productName_eng)}</td>
     
    <td>${BindValueToGrid(item.remarks)}</td>
    <td>${BindValueToGrid(item.createdDate, true)}</td>
    <td>${BindValueToGrid(item.modifiedDate, true)}</td>
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

    //End grid
    //#region Change PageSize

    $('#gridmemberCount').on('change', function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        SetPageSizeFromStorage(pageSizeKey);
        window.init();
    })

    $(document).on("click", ".edit", function () {

        var productId = $(this).data("id");

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/AvailabilityToolsWeb/GetbyIdProdect',
            type: 'GET',
            data: { identifier: productId },
            headers: { 'RequestVerificationToken': token },

            success: function (response) {

                if (!response.success || !response.data) {
                    alert("No data found");
                    return;
                }

                const data = response.data;

                // ================= PRODUCT LEVEL =================
                $("#ProductId").val(data.productId);
                $("#Filepath").val(data.filepath ?? "");
                $("#ProductNameEng").val(data.productNameEng);
                $("#ProductNameKnd").val(data.productNameKnd);
                $("#Remarks").val(data.remarks ?? "");

                // ================= DROPDOWNS =================
                $("#DistrictType")
                    .val(data.districtId)
                    .trigger("change")
                    .css({
                        "pointer-events": "none",
                        "background-color": "rgb(255 253 190)"
                    });

                $("#Centerstype").val(data.centerId);
                $("#Centervariatestype").val(data.headId);

                // ================= VARIETIES =================
                $("#varietyBody").empty();

                const fileBaseUrl = $("#fileBaseUrl").val();

                if (data.productVarietyItems && data.productVarietyItems.length > 0) {

                    $.each(data.productVarietyItems, function (i, v) {

                        let row = `
<tr>
    <td>
        <input type="hidden" name="ProductVarietyItems[${i}].VarietiesId" value="${v.varietiesId}" />
        <input type="hidden" name="ProductVarietyItems[${i}].ProductId" value="${v.productId}" />

        <input type="text"
               name="ProductVarietyItems[${i}].VarietyNameEng"
               value="${v.varietyNameEng ?? ""}"
               class="form-control form-control-sm" />
    </td>

    <td>
        <input type="text"
               name="ProductVarietyItems[${i}].VarietyNameKnd"
               value="${v.varietyNameKnd ?? ""}"
               class="form-control form-control-sm" />
    </td>

    <td>
        <select name="ProductVarietyItems[${i}].UnitId"
                class="form-select form-select-sm unit-select"></select>
    </td>

    <td>
        <input type="number" step="0.01"
               name="ProductVarietyItems[${i}].Mrpprice"
               value="${v.mrpprice}"
               class="form-control form-control-sm" />
    </td>

    <td>
        <input type="number" step="0.01"
               name="ProductVarietyItems[${i}].SellingPrice"
               value="${v.sellingPrice}"
               class="form-control form-control-sm" />
    </td>

    <td>
        <input type="number"
               name="ProductVarietyItems[${i}].StockQty"
               value="${v.stockQty}"
               class="form-control form-control-sm" />
    </td>

    <td>
        <input type="number"
               name="ProductVarietyItems[${i}].MinStockQty"
               value="${v.minStockQty}"
               class="form-control form-control-sm" />
    </td>

    <td>
        <div style="display:flex;align-items:center;gap:6px;">
            <input type="file"
                   name="ProductVarietyItems[${i}].ImageFile"
                   class="form-control fileInput"
                   accept="image/*" />

            <button type="button"
                    class="btn btn-primary btn-sm viewImage"
                    data-path="${v.filepath ?? ""}"
                    style="${v.filepath ? "" : "display:none"}">
                View
            </button>

            <input type="hidden"
                   name="ProductVarietyItems[${i}].Filepath"
                   value="${v.filepath ?? ""}" />
        </div>
    </td>

    <td class="text-center">
        <input type="checkbox"
               name="ProductVarietyItems[${i}].IsDeleted"
               ${v.isDeleted ? "checked" : ""} />
    </td>
</tr>`;

                        $("#varietyBody").append(row);

                        // bind unit dropdown
                        populateUnitDropdown(i, v.unitId);

                        scrollToTop();
                    });
                }
            },
            error: function () {
                alert("Something went wrong while fetching data.");
            }
        });
    });

    function populateUnitDropdown(index, selectedUnitId) {

        const units = [
            { id: 1, text: "Kg" },
            { id: 2, text: "Gram" },
            { id: 3, text: "Nos" }
        ];

        const select = $(`select[name='ProductVarietyItems[${index}].UnitId']`);
        select.append(`<option value="">--Select Unit--</option>`);

        $.each(units, function (_, u) {
            select.append(
                `<option value="${u.id}" ${u.id === selectedUnitId ? "selected" : ""}>
                ${u.text}
             </option>`
            );
        });
    }


    //Add new record variets
    // Add new variety row
    $('#btnAddVariety').on('click', function () {
        addVarietyRow();
    });

    // Remove variety row (event delegation)
    $('#varietyTable').on('click', '.btnRemoveVariety', function () {
        $(this).closest('tr').remove();
        rearrangeIndexes();
    });

    // Remove variety row
    $('#varietyTable').on('click', '.btnRemoveVariety', function () {

        // Optional: prevent removing last row
        if ($('#varietyTable tbody tr').length === 1) {
            alert('At least one variety is required.');
            return;
        }

        $(this).closest('tr').remove();
        rearrangeIndexes();
    });

    function addVarietyRow() {
        var index = $('#varietyTable tbody tr').length;

        var row = `
        <tr>
            <td>
                <input name="ProductVarietyItems[${index}].VarietyNameEng"
                       class="form-control form-control-sm" data-val="true"
                       data-val-required="Variety Name (English) is required" />
            </td>
        
            <td>
                <input name="ProductVarietyItems[${index}].VarietyNameKnd"
                       class="form-control form-control-sm" data-val="true"
                       data-val-required="Variety Name (Kannada) is required" />
            </td>
        
            <td>
                <select name="ProductVarietyItems[${index}].UnitId"
                        class="form-select form-select-sm" data-val="true"
                        data-val-required="Unit is required">
                    <option value="">--Select Unit--</option>
                    <option value="1">Kg</option>
                    <option value="2">Gram</option>
                    <option value="3">Liter</option>
                    <option value="4">Ml</option>
                    <option value="5" selected>Nos</option>
                </select>
            </td>
        
          <!--   <td>
                <input name="ProductVarietyItems[${index}].Mrpprice"
                       class="form-control form-control-sm" type="number" step="0.01"
                       data-val="true" data-val-range="MRP must be greater than 0"
                       data-val-range-min="0.01" />
            </td> 
            -->
        
            <td>
                <input name="ProductVarietyItems[${index}].SellingPrice"
                       class="form-control form-control-sm" type="number" step="0.01"
                       data-val="true" data-val-range="Selling Price must be greater than 0"
                       data-val-range-min="0.01" />
            </td>
        
            <td>
                <input name="ProductVarietyItems[${index}].StockQty"
                       class="form-control form-control-sm" type="number" min="0"
                       data-val="true" data-val-range="Stock Qty must be greater than or equal to 0"
                       data-val-range-min="0" />
            </td>
        
            <!--   <td>
                <input name="ProductVarietyItems[${index}].MinStockQty"
                       class="form-control form-control-sm" type="number" min="1"
                       data-val="true" data-val-required="Min Stock Qty is required"
                       data-val-range="Min Stock Qty must be greater than 0"
                       data-val-range-min="1" />
            </td>

        <td>
                    <div style="display: flex; align-items: center; gap: 5px;">
                 <input type="file" name="ProductVarietyItems[${index}].Filepath" class="form-control fileInput" accept="image/*" style="flex: 1;" />
                    <button type="button" class="btn btn-primary btn-sm viewImage" style="display:none;">View</button>
                </div> 
        </td>
        -->


            <td class="text-center">
                <input type="checkbox" name="ProductVarietyItems[${index}].IsDeleted" class="form-check-input" />
            </td>
        </tr>`;

        $('#varietyTable tbody').append(row);

        $.validator.unobtrusive.parse('#varietyTable');
    }

    // Show "View" button and preview image
    $('#varietyTable').on('change', '.fileInput', function () {
        var file = this.files[0];
        var viewBtn = $(this).closest('td').find('.viewImage');
        if (file && file.type.startsWith('image/')) {
            viewBtn.show();
        } else {
            viewBtn.hide();
        }
    });

    // Preview image in modal
    $('#varietyTable').on('click', '.viewImage', function () {
        var fileInput = $(this).closest('td').find('.fileInput')[0];
        var file = fileInput.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                // You can use a modal or alert for preview
                var imgPreview = `<img src="${e.target.result}" style="max-width:500px; max-height:500px;" />`;
                // Simple inline preview using alert (or use modal)
                var previewWindow = window.open('', 'Image Preview', 'width=600,height=600');
                previewWindow.document.write(imgPreview);
            }
            reader.readAsDataURL(file);
        }
    });


    function rearrangeIndexes() {

        $('#varietyTable tbody tr').each(function (i) {

            $(this).find('input').each(function () {

                var name = $(this).attr('name');
                if (name) {
                    $(this).attr('name', name.replace(/\[\d+\]/, '[' + i + ']'));
                }
            });
        });
    }



    //Form submit using ajax

    $('body').on('click', '#btnsumbit', function (e) {
        e.preventDefault();

        var form = $(this).closest('form')[0];
        var formData = new FormData(form);

        $("#loader").show();
        $("#formMessages").html("");

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/AvailabilityToolsWeb/CentersAvailabilityDetails',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            success: function (response) {
                $("#loader").hide();

                if (response.success) {
                    alert(response.message);
                    location.reload();
                } else {
                    $("#formMessages").html(
                        '<div class="alert alert-danger">' +
                        (response.errors ? response.errors.join('<br/>') : response.message) +
                        '</div>'
                    );
                }
            },
            error: function () {
                $("#loader").hide();
                $("#formMessages").html(
                    '<div class="alert alert-danger">Something went wrong.</div>'
                );
            }
        });
    });

    function scrollToTop() {
        $('html, body').animate({ scrollTop: 0 }, 600);
    }


    //End Form submit



    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }
});