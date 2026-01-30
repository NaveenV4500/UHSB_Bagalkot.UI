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

    // Get productId from query string
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('productId');

    if (!productId) {
        $('#varietyContent').html(`
            <div class="alert alert-warning text-center">Invalid Product.</div>
        `);
        return;
    }

    loadProductVarieties(productId);
    function cartItemTemplate(item) {
        return `
    <div class="card mb-3 shadow-sm cart-item" data-id="${item.cartId}">
        <div class="row g-0 align-items-center">

            <div class="col-md-3 text-center p-2">
                <img src="${item.imageUrl}" class="img-fluid"
                     style="height:120px; object-fit:contain">
            </div>

            <div class="col-md-6">
                <div class="card-body">
                    <h6 class="fw-bold mb-1">${item.productName}</h6>
                    <small class="text-muted">${item.varietyName}</small>

                    <div class="mt-2">
                        <span class="fw-bold text-success">₹${item.price}</span>
                    </div>
                </div>
            </div>

            <div class="col-md-3 text-center">
                <div class="input-group input-group-sm px-3">
                    <button class="btn btn-outline-secondary qty-minus">−</button>
                    <input type="text" class="form-control text-center qty"
                           value="${item.quantity}">
                    <button class="btn btn-outline-secondary qty-plus">+</button>
                </div>

                <button class="btn btn-link text-danger mt-2 remove-item">
                    Remove
                </button>
            </div>

        </div>
    </div>`;
    }

    function loadProductVarieties(productId) {
        const $container = $("#varietyContent");
        const $loader = $("#varietyLoader");
        debugger;    
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

                let html = '';
                $.each(items, function (_, v) {
                    const defaultImg = fileBaseUrl + "InwardsInvoices/TempFiles/ProductswithVariets/no-image.jpg";
                    const imageUrl = v.filepath && v.filepath.trim() !== ""
                        ? fileBaseUrl + "InwardsInvoices/TempFiles/ProductswithVariets/" + v.filepath
                        : defaultImg;

                    const isLowStock = v.stockQty <= v.minStockQty;

                    html += `
<div class="col-12 col-md-6 col-lg-4">
    <div class="card h-100 shadow-sm border-0">

        <!-- Image -->
        <img src="${imageUrl}"
             class="card-img-top"
             style="height:200px; object-fit:cover"
             alt="${v.varietyName_eng}">
         

        <div class="card-body p-3">

            <!-- Product -->
            <div class="mb-2">
                <h6 class="fw-bold mb-0">${v.productName_eng}</h6>
                <small class="text-muted">${v.productName_knd}</small>
            </div>

            <!-- Variety -->
            <div class="mb-2">
                <strong>Variety:</strong> ${v.varietyName_eng}<br>
                <small class="text-muted">${v.varietyName_knd}</small>
            </div>

            <!-- Center -->
            <div class="mb-2 small">
                <strong>Center:</strong> ${v.centername_eng}<br>
                <strong>District:</strong> ${v.districtName}
            </div>

            <!-- Stock -->
            <div class="mb-2 small">
                <strong>Stock:</strong> ${v.stockQty}
                <span class="ms-2 text-muted">(Min: ${v.minStockQty})</span>
            </div>

            <!-- Unit / SKU -->
            <div class="mb-2 small">
                <strong>Unit:</strong> ${v.unitName_eng || '-'}<br>
                <strong>SKU:</strong> ${v.stock_Keeping_Unit || '-'}<br>
                <strong>Barcode:</strong> ${v.barcode || '-'}
            </div>

            <!-- Price -->
            <div class="mb-3">
                <span class="fw-bold text-success fs-5 price">₹${v.sellingPrice}</span>
                <span class="text-muted text-decoration-line-through ms-2">₹${v.mrpPrice}</span>
            </div>

            <!-- Quantity -->
            <div class="mb-2">
                <label class="form-label fw-bold">Quantity</label>
                <input type="number" class="form-control quantity" min="0" value="0">
            </div>

            <!-- Total -->
            <p class="mb-3">
                <strong>Total:</strong> ₹<span class="totalPrice">0.00</span>
            </p>

            <!-- Stock Status -->
            <span class="badge ${v.stockQty <= v.minStockQty ? 'bg-danger' : 'bg-success'} mb-2">
                ${v.stockQty <= v.minStockQty ? 'Low Stock' : 'In Stock'}
            </span>

            <!-- Action -->
            <button class="btn btn-primary w-100 add-to-cart"
                    data-id="${v.varietiesId}">
                Add to Cart
            </button>

        </div>
    </div>
</div>`;

                });

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

    // Calculate total price on quantity change
    $(document).on('input', '.quantity', function () {
        const card = $(this).closest('.card');
        const price = parseFloat(card.find('.price').text().replace('₹', '')) || 0;
        const qty = parseInt($(this).val()) || 0;
        card.find('.totalPrice').text((price * qty).toFixed(2));
    });

    // Add to cart using $.ajax POST
    $(document).on('click', '.add-to-cart', function () {
        const card = $(this).closest('.card');
        const productId = $(this).data('id');
        const qty = parseInt(card.find('.quantity').val()) || 0;
        const total = parseFloat(card.find('.totalPrice').text()) || 0;

        if (qty <= 0) {
            alert("Enter a valid quantity!");
            return;
        }

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: {
                productId: productId,
                quantity: qty,
                totalPrice: total,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() // MVC AntiForgery
            },
            success: function (response) {
                if (response.success) {
                    alert("Added to cart!");
                } else {
                    alert("Failed to add to cart. Try again.");
                }
            },
            error: function (xhr, status, error) {
                console.error(error);
                alert("An error occurred while adding to cart.");
            }
        });
    });

});
