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
    const defaultImg = fileBaseUrl + "InwardsInvoices/TempFiles/ProductswithVariets/no-image.jpg";
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
        const container = $("#productGrid");
        const skeleton = $("#productSkeleton");
        const districtid = $("#DistrictType").val();
        const centerid = $("#Centerstype").val();

        container.empty();
        skeleton.removeClass("d-none");

        $.ajax({
            type: "POST",
            dataType: "json",
            url: GetRootPath(window.virtualPath) + "/AvailabilityToolsWeb/GetGridContentV1",
            data: {
                currentPage: currentPage,
                pageSize: pageSize,
                orderBy: window.orderByColumnId,
                isDescending: window.isDescendingFilter,
                filterDetails: "",
                districtid: districtid,
                centerid: centerid
            },
            success: function (response) {
                container.empty();
                skeleton.addClass("d-none");

                const items = response?.data?.itemDetails || [];

                if (!items.length) {
                    container.html(`<div class="col-12 text-center py-5"><h5>No Products Found</h5></div>`);
                    return;
                }

                $.each(items, function (_, item) {
                 
                    const imgPath = item.filepath
                        ? `${fileBaseUrl}InwardsInvoices/TempFiles/Products/${item.filepath}`
                        : defaultImg;

                    // Use a fallback in case districtName is null
                    const district = BindValueToGrid(item.districtName) || "Karnataka";

                    $.each(items, function (_, item) {
                       
                        const imgPath = item.filepath
                            ? `${fileBaseUrl}InwardsInvoices/TempFiles/Products/${item.filepath}`
                            : defaultImg;

                        const card = `
                             <div class="col-6 col-md-4 col-lg-2-4 mb-3 d-flex align-items-stretch">
                                 <div class="zepto-card">
                                     <!--<div class="zepto-img-wrapper">
                                         <div class="status-badge">
                                             <span class="pulse-icon"></span> Available
                                         </div>
                                         <img src="${imgPath}" onerror="this.src='${defaultImg}'" loading="lazy" />
                                     </div>-->
                                     <div class="zepto-content">
                                         <div class="zepto-title" title="${item.productName_eng}">
                                             ${BindValueToGrid(item.productName_eng)}
                                         </div>
                                         <div class="zepto-subtitle">
                                             ${BindValueToGrid(item.productName_knd)}
                                         </div>
                                         
                                         <div class="location-footer">
                                             <div class="loc-item center-name">
                                                 <i class="bi bi-house-door"></i> ${BindValueToGrid(item.centername_eng)}
                                             </div>
                                             <div class="loc-item district-name">
                                                 <i class="bi bi-geo-alt"></i> ${BindValueToGrid(item.districtName)}
                                             </div>
                                         </div>

                                         <button class="view-details" id="btnviewproduct" productid="${item.productId}">
                                             View Details
                                         </button>
                                     </div>
                                 </div>
                             </div>`;
                    container.append(card);

                    });
                });
                totalRecords = response.data.totalCount;
                setupControls();
                pagingDetails();
            },
            error: function () {
                skeleton.addClass("d-none");
                console.error("Failed to fetch products.");
            }
        });
    };

    //#endregion

    window.init();

    //End grid
    $(document).on("click", "#griddata", function () {
        window.init();
    });

    
    $(document).on("click", "#mobilefilterbtn", function () {
        _init();
    });
    $("#resetFilter").on("click", function () {
        $("#DistrictType").val("").trigger("change");
        $("#Centerstype").val("").empty();
        $("#ProductSearch").val("");
        window.init();
    });

    $(document).on("click", ".edit", function () {

        var identifier = $(this).data("id");
        debugger;
        $.ajax({
            url: GetRootPath(window.virtualPath) + '/AvailabilityToolsWeb/GetbyIdProdect',
            type: 'GET',
            data: { identifier: identifier },
            headers: { 'RequestVerificationToken': token },

            success: function (response) {
                debugger;

                if (!response.success) {
                    alert(response.message);
                    return;
                }
                console.log("response " + response);
                // If API returns object
                var data = response.data;

                // ✅ Populate fields (change IDs as per your form)
                $("#Identifier").val(data.identifier);
                $("#ProductName").val(data.productName);
                $("#UnitId").val(data.unitId).change();
                $("#Price").val(data.price);
                $("#Quantity").val(data.quantity);

            },
            error: function (xhr) {
                alert("Something went wrong while fetching data.");
                console.log(xhr);
            }
        });

    });



    //#region Change PageSize

    $('#gridmemberCount').on('change', function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        SetPageSizeFromStorage(pageSizeKey);
        window.init();
    })




    // Variets Model
    $(document).on('click', '#btnviewproduct', function (e) {
        e.stopPropagation(); // avoid double fire
        debugger;
        const productId = $(this).attr("productid");

        if (!productId) return;
         
        const url = GetRootPath(window.virtualPath)
            + "/AvailabilityToolsWeb/VarietiesIndex?productId=" + productId;

        window.location.href = url;
    });

    $(document).on("click", ".zepto-btn", function () {
        debugger;
        const productId = $(this).data("id");

        $("#varietyContent").empty();
        $("#varietyLoader").removeClass("d-none");

        $("#productVarietyModal").modal("show");

        loadProductVarieties(productId);
    });

    function loadProductVarieties(productId) {
        const $container = $("#varietyContent");
        const $loader = $("#varietyLoader");

        // Show Loader
        $loader.removeClass("d-none");
        $container.empty();

        $.ajax({
            type: "POST",
            url: GetRootPath(window.virtualPath) + "/AvailabilityToolsWeb/GetProductVarieties",
            dataType: "json",
            data: {
                productid: productId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                $loader.addClass("d-none");
                const items = res?.data || [];

                if (!items.length) {
                    $container.html(`
                    <div class="text-center py-5">
                        <i class="bi bi-box-seam display-4 text-muted"></i>
                        <p class="mt-2 text-secondary">No varieties available for this product.</p>
                    </div>
                `);
                    return;
                }

                let html = `<div class="row g-4">`;

                $.each(items, function (_, v) {
                    // Image Logic
                   
                    const imageUrl = v.filepath && v.filepath.trim() !== ""
                        ? fileBaseUrl + "InwardsInvoices/TempFiles/ProductswithVariets/" + v.filepath
                        : "";

                    const isLowStock = v.stockQty <= v.minStockQty;

                    html += `
                <div class="col-12 col-md-6 col-lg-4">
                    <div class="card h-100 border-0 shadow-sm hover-shadow transition-all">
                        
                        <div class="position-relative">
                            <img src="${imageUrl}" 
                                 class="card-img-top" 
                                 alt="${v.varietyName_eng}"
                                 onerror="this.src='${defaultImg}'"
                                 style="height:200px; object-fit:cover;">
                            
                            <span class="position-absolute top-0 end-0 m-2 badge ${isLowStock ? 'bg-danger' : 'bg-success'} shadow-sm">
                                ${isLowStock ? 'Low Stock' : 'In Stock'}
                            </span>
                        </div>

                        <div class="card-body d-flex flex-column p-3">
                            <div class="mb-2">
                                <h6 class="card-title mb-0 fw-bold text-dark text-truncate">
                                    ${BindValueToGrid(v.varietyName_eng)}
                                </h6>
                                <small class="text-muted d-block italic">
                                    ${BindValueToGrid(v.varietyName_knd)}
                                </small>
                            </div>

                            <div class="d-flex align-items-baseline gap-2 mb-3">
                                <span class="fs-5 fw-bold text-primary">₹${BindValueToGrid(v.sellingPrice)}</span>
                                <span class="text-muted text-decoration-line-through small">
                                    ₹${BindValueToGrid(v.mrpPrice)}
                                </span>
                            </div>

                            <div class="p-2 bg-light rounded-2 mb-3 mt-auto">
                                <div class="d-flex justify-content-between align-items-center small">
                                    <span class="text-secondary">Available Stock:</span>
                                    <span class="fw-semibold ${isLowStock ? 'text-danger' : 'text-dark'}">
                                        ${BindValueToGrid(v.stockQty)} ${BindValueToGrid(v.unitName_eng)}
                                    </span>
                                </div>
                            </div>

                            ${isLowStock
                            ? `<button class="btn btn-outline-warning w-100 fw-bold request-stock" data-id="${v.varietiesId}">
                                     <i class="bi bi- bell me-1"></i> Request Stock
                                   </button>`
                            : `<button class="btn btn-primary w-100 fw-bold add-to-cart" data-id="${v.varietiesId}">
                                     <i class="bi bi-cart-plus me-1"></i> Add to Cart
                                   </button>`
                        }
                        </div>
                    </div>
                </div>`;
                });

                html += `</div>`;
                $container.html(html);
            },
            error: function () {
                $loader.addClass("d-none");
                $container.html(`
                <div class="alert alert-danger d-flex align-items-center" role="alert">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i>
                    <div>Failed to load varieties. Please try again later.</div>
                </div>
            `);
            }
        });
    }



    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }
});