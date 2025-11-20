$(document).ready(function () {
    var fileBaseUrl = $("#fileBaseUrl").val();
    $('.filter-controls-group').hide(); 
    $(".panel-body").hide();
    const FileTypes = {
        Category: 1,
        Crops: 2,
        Sections: 3,
        Items: 4,
        ItemContent: 5
    };

    // Set all toggle icons to "down" by default
    $(".toggle-panel-btn i").removeClass("bi-chevron-up").addClass("bi-chevron-down");


    if (!fileBaseUrl) {
        alert('No image available.');
    }
    fileBaseUrl = fileBaseUrl.replace("api", "");
    function callGetApi(actionName, params = {}, onSuccess, onError) {
        //showLoader();

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/CommonApi/ForwardApiResponse?action_name=' + actionName,
            type: 'GET',
            success: onSuccess,
            error: function (xhr) {
 
                hideLoader();
                console.error(xhr); if (onError) onError(xhr); }
        });
        //hideLoader();
    }

   

    // Load data
    showLoader();
    getCategories();
    getSections();
    getCrops(); 
    getItems(true);
    hideLoader();

    CategoryDD('.cls-CategoryDD', function (categories) {

        var $otherDropdown = $('.cls-CategoryDD'); // different dropdown
        $otherDropdown.empty().append('<option value="">Select Category</option>');

        $.each(categories, function (i, item) {
            $otherDropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
        });
    });

    $('#SearchItemBtn').click(function () {
        getItems();
    });
    $('#SearchItemBtnAll').click(function () {

        getItems(true);

    });

    $('#enableFilter').on('change', function () {
        if ($(this).is(':checked')) {
            $('.filter-controls-group').slideDown();
        } else {
            $('.filter-controls-group').slideUp();
        }
    });

    // ------------------ Render Tables ------------------
    function renderCategories(categories) {
        let html = '';
        $.each(categories, function (index, category) {
            html += `
        <tr data-cateid="${category.categoryId}"
            data-name="${category.name}" 
            data-description="${category.description || ''}" 
            data-images="${category.imageUrl || ''}"> 
            <td>${index + 1}</td>
            <td>${category.name}</td>
            <td>
                <img src="${fileBaseUrl}InwardsInvoices/TempFiles/Category/${category.imageUrl}" 
                     width="80" class="rounded" 
                     onerror="this.onerror=null;this.src='https://placehold.co/80x80/E9ECEF/6c757d?text=No+Image';" />
            </td>
            <td>
                <button class="btn btn-sm btn-outline-primary edit-btn-category" data-type="category" title="Edit">
    <i class="bi bi-pencil-square"></i>
</button>

<button class="btn btn-sm btn-outline-danger delete-category-btn" data-categoryid="${category.categoryId}" title="Delete">
    <i class="bi bi-trash"></i>
</button>

            </td>
        </tr>`;
        });
        $('#categoriesTable tbody').html(html);
    }
    function renderSection(sections) {
        let html = '';
        $.each(sections, function (index, section) {
            html += `
             <tr data-sectionId="${section.sectionId}"
                 data-name="${section.name}" 
                 data-description="${section.description || ''}" 
                 data-images="${section.imageUrl || ''}" >
                 
                 <td>${index + 1}</td>
                 <td>${section.name}</td>
                 <td>
                     <img src="${fileBaseUrl}InwardsInvoices/TempFiles/Sections/${section.imageUrl}" width="80" />
                 </td>
                 <td> 
                      <button class="btn btn-sm btn-outline-primary edit-btn-section" data-type="section" title="Edit">
                    <i class="bi bi-pencil-square"></i>
                </button>
                 </td>
             </tr>`;
             });
        $('#SectionTable tbody').html(html);
    }

    function renderCrops(crops) {
        console.log("crops >> ", JSON.stringify(crops));

        let html = '';
        $.each(crops, function (index, crop) {
            html += `
        <tr 
            data-cropid="${crop.cropId}" 
            data-categoryid="${crop.categoryId}"
            data-name="${crop.name}" 
            data-description="${crop.description || ''}" 
            data-images='${crop.imageUrl || ''}'>
            <td>${index + 1}</td>
            <td>${crop.name}</td>
            <td>${crop.categoryName}</td>
            <td>
                <img src="${fileBaseUrl}InwardsInvoices/TempFiles/Crops/${crop.imageUrl}" width="80" />
            </td>
            <td>
                 
                      <button class="btn btn-sm btn-outline-primary edit-btn" data-type="crop" title="Edit">
                    <i class="bi bi-pencil-square"></i>

                    <button class="btn btn-sm btn-outline-danger delete-crop-btn" data-categoryid="${crop.categoryId}" data-cropid="${crop.cropId}" title="Delete">
    <i class="bi bi-trash"></i>
</button>
            </td>
        </tr>`;
        });

        $('#cropTable tbody').html(html);
    }

    function renderItemDetails(ItemDetails) {
        console.log("ItemDetails >> ", JSON.stringify(ItemDetails));

        let html = '';
        $.each(ItemDetails, function (index, item) {
            html += `
        <tr 
            data-itemid="${item.itemId}"
            data-categoryid="${item.categoryId}"
            data-cropid="${item.cropId}"
            data-sectionid="${item.sectionId}"
            data-name="${item.name || ''}" 
            data-images="${item.imageUrl || ''}"> 
            <td>${index + 1}</td>
            <td>${item.categoryName || ''}</td>
            <td>${item.sectionName || ''}</td>
            <td>${item.cropName || ''}</td>
            <td>${item.name || ''}</td>
            <td><img src="${fileBaseUrl}InwardsInvoices/TempFiles/CropsItems/${item.imageUrl}" width="80" /></td>
            <td> 
                 <button class="btn btn-sm btn-outline-primary edit-btn-items" data-id="${item.itemId}" data-type="crop" title="Edit">
                    <i class="bi bi-pencil-square"></i>
                       <button class="btn btn-sm btn-outline-danger delete-itemdetails-btn" data-itemid="${item.itemId}" data-categoryid="${item.categoryId}" data-cropid="${item.cropId}" title="Delete">
    <i class="bi bi-trash"></i>
                </button>

            </td>
        </tr>`;
        });

        $('#ItemTable tbody').html(html);
    }

    // ------------------ Load Data ------------------
    function getSections() {
        callGetApi('HorticultureHandbook/GetgridContentSections', {}, renderSection);
    }

    function getCategories() {
        callGetApi('HorticultureHandbook/getgridContentcategories', {}, renderCategories);
    }

    //function getCrops() {
    //    callGetApi('HorticultureHandbook/getgridContentcrop', {}, renderCrops);
    //}

    function getCrops() {
        callGetApi(
            'HorticultureHandbook/getgridContentcrop',
            {},
            function (res) {
                initPagination("crop", res, renderCrops, 5);
            }
        );
    }


    function getItems(Isall = false) {
        var CategoryDD = 0;
        var CropsDD = 0;
        var SectionDD = 0;
        if (!Isall) {
            CategoryDD = $(".cls-CategoryDD").val();
            CropsDD = $(".cls-CropsDD").val();
            SectionDD = $(".cls-SectionDD").val();
        }
        var url = `HorticultureHandbook/GetgridContentItemDetails?categoryId=${CategoryDD}&cropId=${CropsDD}&sectionId=${SectionDD}`;
        //callGetApi(url, {}, renderItemDetails);
        callGetApi(
            url,
            {},
            function (res) {
                initPagination("items", res, renderItemDetails, 5);
            }
        );
    }
  

    function loadCategories(selectedValue = 0) {
        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Dashboard/GetCategories',
            type: 'GET',
            dataType: 'json',
            success: function (response) {

                var dropdown = $('#Categoriestype');
                dropdown.empty();
                dropdown.append('<option value="">--Select Category--</option>');

                $.each(response.data, function (index, item) {
                    dropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                });

                if (selectedValue) {
                    dropdown.val(selectedValue);
                }
            },
            error: function () {
                alert('Failed to load categories');
            }
        });
    }



    // ------------------ Add/Edit Modal ------------------
    $(document).on('click', '.edit-btn, .add-btn', function () {
        var categoryId = 0;

        const $row = $(this).closest('tr');
        const type = $(this).data('type');
        $('#itemType').val(type);
        $('#imagePreview').html('');
        if ($(this).hasClass('edit-btn')) {
            const id = $row.data('cropid');
            categoryId = $row.data('categoryid');
            const name = $row.data('name');
            const description = $row.data('description');
            const images = $row.data('images');
            debugger;
            loadCategories(categoryId);
            $('#itemName').val(name);
            $('#itemDescription').val(description);
            $('#Categoriestype').val(categoryId);
            $('#CropId').val(id); 
            $('#filename').val(images);

            // show popup modal
            $('#itemModal').modal('show');
            $('#imagePreview').html(`<img src = "${fileBaseUrl}InwardsInvoices/TempFiles/Crops/${images}"  class= "img-thumbnail" />`);

            // preview images if any

        } else {

            loadCategories(0);
            // clear for Add new
            $('#filename').val('');

            $('#itemId').val(0);
            $('#itemName').val('');
            $('#itemDescription').val('');
            $('#Categoriestype').val('');
            $('#imagePreview').html('');
            $('#itemModal').modal('show');
        }
    });
     

    // ------------------ Preview Images ------------------
    $('#itemImage').change(function () {
        $('#imagePreview').html('');
        const files = this.files;
        if (files) {
            Array.from(files).forEach(file => {
                const reader = new FileReader();
                reader.onload = function (e) {
                    $('#imagePreview').append(`<img src="${e.target.result}" width="100" class="me-2 mb-2" />`);
                };
                reader.readAsDataURL(file);
            });
        }
    });

    $('#itemDetailsImage').change(function () {
        $('#itemDetailsimagePreview').html('');
        const files = this.files;
        if (files) {
            Array.from(files).forEach(file => {
                const reader = new FileReader();
                reader.onload = function (e) {
                    $('#itemDetailsimagePreview').append(`<img src="${e.target.result}" width="100" class="me-2 mb-2" />`);
                };
                reader.readAsDataURL(file);
            });
        }
    });
    
    $('#SectionImage').change(function () {
        $('#SectionimagePreview').html('');
        const files = this.files;
        if (files) {
            Array.from(files).forEach(file => {
                const reader = new FileReader();
                reader.onload = function (e) {
                    $('#SectionimagePreview').append(`<img src="${e.target.result}" width="100" class="me-2 mb-2" />`);
                };
                reader.readAsDataURL(file);
            });
        }
    });

    $('#CategoryImage').change(function () {
        $('#CategoryimagePreview').html('');
        const files = this.files;
        if (files) {
            Array.from(files).forEach(file => {
                const reader = new FileReader();
                reader.onload = function (e) {
                    $('#CategoryimagePreview').append(`<img src="${e.target.result}" width="100" class="me-2 mb-2" />`);
                };
                reader.readAsDataURL(file);
            });
        }
    });


    // ------------------ Save Item ------------------
    $('#saveCropsBtn').click(function (e) {
       showLoader();
        e.preventDefault();
        debugger;
        var formData = new FormData();
        formData.append('CropId', $('#CropId').val() || 0);
        formData.append('Name', $('#itemName').val());
        formData.append('Description', $('#itemDescription').val());
        formData.append('CategoryId', $('#Categoriestype').val());
        formData.append('filename', $('#filename').val());
        
        var files = $('#itemImage')[0].files;
        if (files.length > 0) {
            for (var i = 0; i < files.length; i++) {
                formData.append('ImageFiles', files[i]);
            }
        }

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Horticulturehandbookweb/SaveOrEditCrop',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                alert('Saved successfully');
                $('#itemModal').modal('hide');
                getCategories();
                getCrops(); 
                hideLoader();

            },
            error: function () {
                alert('Save failed');
                hideLoader(); 
            }
        });
    });


    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }

    //regtion Items section

    $(document).on('click', '.toggle-panel-btn', function () {
        const $panel = $(this).closest('.card').find('.panel-body');
        const $icon = $(this).find('i');

        $panel.slideToggle(300); // smooth open/close animation

        // Toggle icon direction
        if ($icon.hasClass('bi-chevron-up')) {
            $icon.removeClass('bi-chevron-up').addClass('bi-chevron-down');
        } else {
            $icon.removeClass('bi-chevron-down').addClass('bi-chevron-up');
        }
    });


    CategoryDD('#CategoryDD', function (categories) {

        var $otherDropdown = $('#CategoryDD'); // different dropdown
        $otherDropdown.empty().append('<option value="">Select Category</option>');

        $.each(categories, function (i, item) {
            $otherDropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
        });
        //hideLoader();

    });


    function CategoryDD(idelement, callback) {
        //showLoader();
        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Dashboard/GetCategories',
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                if (data.success) {
                    callback(data.data);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error loading data:', error);
                hideLoader();
            },
            complete: function () {
                //hideLoader();
            }
        });
    }

     

    function SectionDD(categoryID = 0, cropsID = 0,idelemnt="") {
        //showLoader();
        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Dashboard/GetSections?catId=' + categoryID + '&cropId=' + cropsID,
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data.success) {
                    var $dropdown = $(idelemnt);
                    $dropdown.empty().append('<option value="">' + "Select Sction" + '</option>');
                    $.each(data.data, function (i, item) {
                        $dropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                } else {
                    window.location.href = data.redirectUrl;
                }
            },
            error: function (xhr, status, error) { hideLoader();  console.error('Error loading data:', error); },
            complete: function () { //hideLoader();
            }
        });
    }

    function CropsDD(categoryID = 0,idElement="") {
        //showLoader();
        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Dashboard/GetCrops?catId=' + categoryID,
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data.success) {
                    var $dropdown = $(idElement);
                    $dropdown.empty().append('<option value="">' + "Select Crops" + '</option>');
                    $.each(data.data, function (i, item) {
                        $dropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                } else {
                    window.location.href = data.redirectUrl;
                }
            },
            error: function (xhr, status, error) { hideLoader(); console.error('Error loading data:', error); },
            complete: function () {
                //hideLoader();
            }
        });
    }


    //Add Items
    $(document).on('click', '.add-btn-items,.edit-btn-items', function () {
        debugger;
     

        var categoryId = 0;
        const $row = $(this).closest('tr');
        const type = $(this).data('type');
        $('#itemType').val(type);
        $('#itemDetailsimagePreview').html('');
         

        if ($(this).hasClass('edit-btn-items')) {
            const itemid = $row.data('itemid');
            categoryId = $row.data('categoryid');
            var cropid = $row.data('cropid');
            var sectionid = $row.data('sectionid');
            const name = $row.data('name');
            const description = $row.data('description');
            const filename = $row.data('images');
            loadCategories(categoryId);
            CropsDD(categoryId,"#CropsDD");
            var $dropdown = $("#CropsDD");
            $dropdown.val(cropid).trigger('change');
            SectionDD(categoryId, cropid,"#SectionDD"); 
            
            $('#itemDetailsName').val(name);
            $('#ItemId').val(itemid);
            $('#CropId').val(cropid);
            $('#filename').val(filename);
            $("#CategoryDD").val(categoryId).trigger('change');
            $("#SectionDD").val(sectionid).trigger('change');
            $dropdown.val(cropid).trigger('change');

            // show popup modal
            $('#FinalitemModal').modal('show');
            $('#itemDetailsimagePreview').html(`<img src = "${fileBaseUrl}InwardsInvoices/TempFiles/CropsItems/${images}"  class= "img-thumbnail" />`);

            // preview images if any

        } else {


            CategoryDD('#CategoryDD', function (categories) {

                var $otherDropdown = $('#CategoryDD');
                $otherDropdown.empty().append('<option value="">Select Category</option>');

                $.each(categories, function (i, item) {
                    $otherDropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                });
                //hideLoader();

            });
            // clear for Add new
            $('#filename').val('');

            $('#itemId').val(0);
            $('#itemDetailsName').val('');
            $('#itemDescription').val('');
            $('#Categoriestype').val('');
            $('#imagePreview').html('');
            $('#FinalitemModal').modal('show');
        }
    });


    //change categoryDD
    $('#CategoryDD,.cls-CategoryDD').on('change', function () {
        debugger;
        debugger;
        resetDropdowns(['#CropsDD', '#SectionDD']);
        var categoryID = $(".cls-CategoryDD").val();
        var catID = $("#CategoryDD").val();

        if ($(this).hasClass('cls-CategoryDD')) {
            CropsDD(categoryID, ".cls-CropsDD");
        } else {
            CropsDD(catID, "#CropsDD");

        }
    });

    //change CropsDD
    $('#CropsDD,.cls-CropsDD').on('change', function () {
        debugger;
        resetDropdowns(['#SectionDD']);
        var CropsDD = $(this).val();
        var categoryID = $("#CategoryDD").val();
        

        if ($(this).hasClass('cls-CropsDD')) {
            SectionDD(categoryID, CropsDD, ".cls-SectionDD");
        } else {
            SectionDD(categoryID, CropsDD, "#SectionDD");

        }

    });


    function resetDropdowns(selectors) {
        selectors.forEach(function (sel) {
            $(sel).val('').html('<option value="">--Select--</option>');
        });
    }
    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }

    //save item
     
    $(document).on('click', '#saveItemBtn', function () {
        debugger;
        showLoader();
        const itemId = $('#ItemId').val() || 0;  
        //const cropId = $('#CropId').val() || 0; 
        const categoryId = $('#CategoryDD').val() || 0;
        const cropId = $('#CropsDD').val() || 0;
        const sectionId = $('#SectionDD').val() || 0;
        const name = $('#itemDetailsName').val();
        console.log($('#itemDetailsName').val());
        const description = $('#itemDescription').val();  
        const filename = $('#filename').val(); 

        const files = $('#itemDetailsImage')[0].files;

        const formData = new FormData();
        formData.append('ItemId', itemId);
        formData.append('CategoryId', categoryId);
        formData.append('CropId', cropId);
        formData.append('SectionId', sectionId);
        formData.append('Name', name);
        formData.append('Description', description);
        formData.append('filename', filename);

        if (files.length > 0) {
            Array.from(files).forEach(f => formData.append('ImageFiles', f));
        }
        if (formData == null) {
            alert("Error");
        }
        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Horticulturehandbookweb/SaveOrEditItem',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (res) {
                alert('Item saved successfully!');
                $('#FinalitemModal').modal('hide');
                getItems(true); // refresh grid after save
                hideLoader();
            },
            error: function (err) {
                console.error('Error:', err);
                alert('Failed to save item.');
                hideLoader();

            }
        });
    });


    //End items section




    //Section Start

    //Add Section
    $(document).on('click', '.add-btn-section,.edit-btn-section', function () {
        debugger;
        const $row = $(this).closest('tr');
        const type = $(this).data('type');
        $('#itemType').val(type);
        $('#itemDetailsimagePreview').html('');

        if ($(this).hasClass('edit-btn-section')) {
            var sectionid = $row.data('sectionid');
            const name = $row.data('name');
            const filename = $row.data('images');

            $('#Name').val(name);
            $("#SectionId").val(sectionid);
            $("#filename").val(filename);
            // show popup modal
            $('#SectionModal').modal('show');
            $('#SectionimagePreview').html(`<img src = "${fileBaseUrl}InwardsInvoices/TempFiles/Sections/${filename}"  class= "img-thumbnail" />`);

        } else {
            $("#filename").val(''); 
            $('#SectionId').val(0);
            $('#Name').val('');
            $('#itemDescription').val(''); 
            $('#SectionimagePreview').html('');
            $('#SectionModal').modal('show');
        }
    });



    //save Section
    $('#saveSectionBtn').click(function (e) {
        showLoader();
        debugger;
        e.preventDefault();
        debugger;

        var formData = new FormData();
          
        formData.append('SectionId', $('#SectionId').val() || 0);
        formData.append('Name', $('#Name').val());
        formData.append('Description', $('#itemDescription').val());
        formData.append('filename', $('#filename').val());
        

        var files = $('#SectionImage')[0].files;
        
        //if (files.length > 0) {
        //    for (var i = 0; i < files.length; i++) {
        //        formData.append('SectionImage', files[i]);
        //    }
        //}

        if (files.length > 0) {
            Array.from(files).forEach(f => formData.append('ImageFiles', f));
        }

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Horticulturehandbookweb/SaveOrEditSection',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                alert('Saved successfully');
                $('#SectionModal').modal('hide');
                getSections();
                hideLoader();

            },
            error: function () {
                alert('Save failed');
                hideLoader();
            }
        });
    });
    
    //Section End



    //Save Category

    $(document).on('click', '.add-btn-category,.edit-btn-category', function () {
        debugger;

        const $row = $(this).closest('tr');
        const type = $(this).data('type');
        $('#itemType').val(type);
        $('#itemDetailsimagePreview').html('');

        if ($(this).hasClass('edit-btn-category')) {
            var categoryId = $row.data('cateid');
            const name = $row.data('name');
            const images = $row.data('images');

            $('#filename').val(images);
            $('#CategoryName').val(name);
            $("#categoryId").val(categoryId);
            // show popup modal
            $('#CategoryModal').modal('show');
            $('#CategoryimagePreview').html(`<img src = "${fileBaseUrl}InwardsInvoices/TempFiles/Category/${images}"  class= "img-thumbnail" />`);

        } else {

            $('#CategoryId').val(0);
            $('#filename').val(''); 
            $('#CategoryName').val('');
            $('#itemDescription').val('');
            $('#CategoryimagePreview').html('');
            $('#CategoryModal').modal('show');
        }
    });



    $('#saveCategoryBtn').click(function (e) {
        showLoader();
        debugger;
        e.preventDefault();
        debugger;

        var formData = new FormData();

        formData.append('CategoryId', $('#categoryId').val() || 0);
        formData.append('Name', $('#CategoryName').val());
        formData.append('filename', $('#filename').val());
 
        var files = $('#CategoryImage')[0].files;

        //if (files.length > 0) {
        //    for (var i = 0; i < files.length; i++) {
        //        formData.append('SectionImage', files[i]);
        //    }
        //}

        if (files.length > 0) {
            Array.from(files).forEach(f => formData.append('ImageFiles', f));
        }

        $.ajax({
            url: GetRootPath(window.virtualPath) + '/Horticulturehandbookweb/SaveOrEditCategory',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                alert('Saved successfully');
                $('#SectionModal').modal('hide');
                getCategories();
                $('#CategoryModal').modal('hide');
                hideLoader();

            },
            error: function () {
                alert('Save failed');
                hideLoader();
            }
        });
    });


    //Category End



    //Delete Item
    
    $(document).on('click', '.delete-category-btn, .delete-crop-btn,.delete-itemdetails-btn', function () {
        showLoader();

        const categoryid = $(this).data('categoryid') || 0;
        const cropid = $(this).data('cropid') || 0;
        const itemdetailid = $(this).data('itemid') || 0;

        let pagetype = 0;

        if ($(this).hasClass('delete-category-btn')) {
            pagetype = 1;  
        } else if ($(this).hasClass('delete-crop-btn')) {
            pagetype = 2;  
        } else if ($(this).hasClass('delete-itemdetails-btn')) {
        pagetype = 4;
    }
        deleteItem(categoryid, cropid, itemdetailid, pagetype);
        hideLoader();
    });



    function deleteItem(categoryid, cropid, itemdetailid, pagetype) {
        if (!confirm("Are you sure you want to delete this item?")) return;
        debugger;
        $.ajax({
            url: GetRootPath(window.virtualPath) + "/Horticulturehandbookweb/DeleteItems",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                categoryId: categoryid,
                cropId: cropid,
                itemDetailId: itemdetailid,
                pageType: pagetype
            }),
            success: function (response) {
                let res = response;
                if (typeof response.result === "string") {
                    try {
                        res = JSON.parse(response.result);
                    } catch (e) {
                        console.error("Failed to parse response:", e);
                    }
                }

                alert(res.message || (res.success ? "Item deleted successfully." : "Failed to delete item."));
            },
            error: function () {
                alert("Error occurred while deleting.");
            }
        });
    }


    //End Delete
});
