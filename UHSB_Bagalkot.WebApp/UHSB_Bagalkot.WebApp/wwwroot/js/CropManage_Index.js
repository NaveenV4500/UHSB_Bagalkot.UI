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
        loadDropdown(GetRootPath(window.virtualPath) + '/Dashboard/GetCategories', '#Categoriestype', '--Select Category--');
    }

    function loadDropdown(url, dropdownSelector, defaultText) {
        showLoader();
        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data.success) {
                    var $dropdown = $(dropdownSelector);
                    $dropdown.empty().append('<option value="">' + defaultText + '</option>');
                    $.each(data.data, function (i, item) {
                        $dropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                    });
                } else {
                    window.location.href = data.redirectUrl;
                }
            },
            error: function (xhr, status, error) { console.error('Error loading data:', error); },
            complete: function () { hideLoader(); }
        });
    }

    function showLoader() { $("#loader").fadeIn(200); }
    function hideLoader() { $("#loader").fadeOut(200); }

    // Cascading dropdowns
    $('#Categoriestype').on('change', function () {
        resetDropdowns(['#Cropstype', '#Sectionstype', '#SubSectionstype']);
        loadDropdown(GetRootPath(window.virtualPath) + '/Dashboard/GetCrops?catId=' + $(this).val(), '#Cropstype', '--Select Crop--');
    });

    $('#Cropstype').on('change', function () {
        resetDropdowns(['#Sectionstype', '#SubSectionstype']);
        loadDropdown(GetRootPath(window.virtualPath) + '/Dashboard/GetSections?cropId=' + $(this).val(), '#Sectionstype', '--Select Section--');
    });

    //$('#Sectionstype').on('change', function () {
    //    resetDropdowns(['#SubSectionstype']);
    //    loadDropdown(GetRootPath(window.virtualPath) +'/Dashboard/GetSubSections?sectId=' + $(this).val(), '#SubSectionstype', '--Select Subsection--');
    //});

    function resetDropdowns(selectors) {
        selectors.forEach(function (sel) {
            $(sel).val('').html('<option value="">--Select--</option>');
        });
    }

    // SubSection change → populate table
    $('#Sectionstype').on('change', function () {
        var subSectId = $(this).val();
        var Cropstype = $("#Cropstype").val();
        $('#ItemImagesBody tr:gt(1)').remove();

        if (!subSectId) return;

        $('#ItemImagesBody').find('input, select, textarea, button').prop('disabled', false);
        $('#addRow').prop('disabled', false);
        loadDropdown(GetRootPath(window.virtualPath) + '/Dashboard/GetItems?sectionId=' + $(this).val() + '&cropId=' + Cropstype, '#ItemsIdtype', '--Select Item--');
        //loadItems(subSectId);
        //window.init();
    });

    var roletypeNo = parseInt($('#userTypeNo').val() || 0); // Example role type
    var currentPage = 1;
    var pageSize = 10;

 
    // Delete function
    function deleteItem(itemId) {
        if (confirm('Are you sure to delete Item ID: ' + itemId + '?')) {
            console.log('Delete logic for ID:', itemId);
            // Add AJAX delete call here
        }
    }

    //Edit Data 
    $('#gridContent').on('click', '.Editdata', function () {
        var itemId = $(this).data('itemid');
        alert('Edit functionality for Item ID: ' + itemId);
    });
    //View Image

    // Put this once in your JS, after table exists
    $('#gridContent').on('click', '.View', function () {

        var filePath = $(this).data('filepath');
        if (!filePath) {
            alert('No image available.');
            
        }
      
        if (!fileBaseUrl) {
            alert('No image available.');
           
        }
       

        var modalHtml = `
        <div id="imageModal" class="modal fade" tabindex="-1" role="dialog">
          <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">View Image</h5>
                <button type="button" class="close" data-dismiss="modal">&times;</button>
              </div>
              <div class="modal-body text-center">
                <img src="${fileBaseUrl}InwardsInvoices/TempFiles/Content/${filePath}" style="max-width:100%; height:auto;" />
         

              </div>
            </div>
          </div>
        </div>
    `;

        $('body').append(modalHtml);
        $('#imageModal').modal('show');

        $('#imageModal').on('hidden.bs.modal', function () {
            $(this).remove();
        });
    });



    // CKEditor modal
    $(document).on('click', '.editDescription', function () {
        var index = $(this).data('index');
        currentDescriptionInput = $('.descriptionHidden').eq(index);
        $('#descriptionModal').modal('show');

        setTimeout(function () {
            if (editorInstance && CKEDITOR.instances.editorDescription) {
                CKEDITOR.instances.editorDescription.destroy();
            }
            initEditor(currentDescriptionInput.val());
        }, 200);
    });

 
    function initEditor(data) {
        setTimeout(function () {
            if (editorInstance && CKEDITOR.instances['editorDescription']) {
                CKEDITOR.instances['editorDescription'].destroy();
            }

            editorInstance = CKEDITOR.replace('editorDescription', {
                extraPlugins: 'table,tabletools,tableresize,pastefromword',
                allowedContent: true,
                contentsCss: [
                    'https://fonts.googleapis.com/css2?family=Noto+Sans+Kannada:wght@400;700&display=swap'
                ],
                font_defaultLabel: 'Noto Sans Kannada',
                font_names: 'Noto Sans Kannada/Noto Sans Kannada, sans-serif;' + CKEDITOR.config.font_names
            });

            editorInstance.setData(data);
        }, 200);
    }




    $('#saveDescription').click(function () {
        if (editorInstance && currentDescriptionInput) {
            currentDescriptionInput.val(editorInstance.getData());
            $('#descriptionModal').modal('hide');

            // ✅ Add green check mark after description is saved
            var $row = currentDescriptionInput.closest('td');
            if ($row.find('.descriptionSaved').length === 0) {
                $row.append('<i class="bi bi-check-circle-fill text-success ms-2 descriptionSaved"></i>');
            }
        }
    });


    // Add new row
    $("#addRow").click(function () {
        debugger;
        var rowCount = $("#ItemImagesBody tr").length - 1;

        var newRowHtml = `
                  <tr>
                      <td>
                          <select name="ItemImages[${rowCount}].ItemId" id="ItemsIdtype${rowCount}" class="form-select itemDropdown">
                              <option value=''>--Select Item--</option>
                          </select>
                      </td>
                      <td>
                          <button type="button" class="btn btn-outline-secondary btn-sm editDescription" data-index="${rowCount}">
                              <i class="bi bi-pencil me-1"></i> Edit Description
                          </button>
                          <input type="hidden" name="ItemImages[${rowCount}].Description" class="descriptionHidden" />
                      </td>
                      <td>
                          <input type="file" name="ItemImages[${rowCount}].ImageFile" class="form-control form-control-file" />
                      </td>
                      <td class="text-center">
                          <button type="button" class="btn btn-outline-danger btn-sm removeRow">
                              <i class="bi bi-trash"></i> Remove
                          </button>
                      </td>
                  </tr>`;

        var $newRow = $(newRowHtml).appendTo('#ItemImagesBody');
        var $newDropdown = $newRow.find('.itemDropdown');

        var SectionId = $('#Sectionstype').val();
        var cropId = $("#Cropstype").val();

        loadDropdown(GetRootPath(window.virtualPath) + '/Dashboard/GetItems?sectionId=' + SectionId + '&cropId=' + cropId, '#ItemsIdtype' + rowCount, '--Select Item--');

    });

    //image preview
    // Image preview event
    $(document).on('change', 'input[type="file"]', function (event) {
        var input = this;
        var file = input.files && input.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                // Remove existing preview if any
                $(input).closest('td').find('.img-preview').remove();

                // Create preview image element
                var imgPreview = $('<img>')
                    .attr('src', e.target.result)
                    .addClass('img-preview mt-2 rounded')
                    .css({
                        width: '80px',
                        height: '80px',
                        objectFit: 'cover',
                        border: '1px solid #ccc'
                    });

                // Append preview below file input
                $(input).after(imgPreview);
            };
            reader.readAsDataURL(file);
        }
    });



    $(document).on("click", "#griddata", function () {
        window.init();
    });

    // Remove row - This can remain the same, but ensure it's inside doc ready
    $(document).on("click", ".removeRow", function () {
        $(this).closest("tr").remove();
        resetIndexes();
        $(this).closest("tr").remove();
        resetIndexes();
    });

    // Re-index names for model binding
    function resetIndexes() {
        // Target the correct table body to find the rows
        $("#ItemImagesBody tr").each(function (i, row) {
            // Update the name and data-index attributes for each input in the row
            $(row).find(".itemDropdown").attr('name', `ItemImages[${i}].ItemId`);
            $(row).find(".descriptionHidden").attr('name', `ItemImages[${i}].Description`);
            $(row).find('input[type="file"]').attr('name', `ItemImages[${i}].ImageFile`);
            $(row).find(".editDescription").attr('data-index', i);
        });
    }

    //grid content with pagination

    $('#gridmemberCount').on('change', function () {
        var pagevalue = $(this).val();
        pageSize = pagevalue;
        currentPage = 1;
        SetPageSizeFromStorage(pageSizeKey);
        window.init();
    })


    function GetAjaxOptionsDataAndParameters() {
        var subSectId = $("#Sectionstype").val() || 0;
        var cropid = $("#Cropstype").val() || 0;

        return {
            currentPage: currentPage,                          // 1
            pageSize: pageSize,                                // 2
            orderBy: window.orderByColumnId,                   // 3
            isDescending: window.isDescendingFilter,           // 4
            filterDetails: window.filterCollection.length > 0
                ? JSON.stringify(window.filterCollection)
                : null,                            // 5
            externalFilter: window.getExternalFilter || null,  // 6
            subSectId: subSectId,
            cropid: cropid,
            categoryid: $("#Categoriestype").val() || 0
        };
    }




    window.init = function () {
        debugger;
        //LoadAjaxAnimation(true);
        var tablebody = $('#gridContent table tbody');

        if (!tablebody.children("tr").length) {
            tablebody.append(tablebody);
        }
        if (tablebody.children().children("td").length < 1) {
            tablebody.append('<tr><td colspan="27"><div style="min-height:120px;"></div></td><tr>');
        }

        //loader.width(tablebody.width());
        debugger;
        $.ajax({

            cache: false,
            type: "POST",
            dataType: 'json',
            url: GetRootPath(window.virtualPath) + '/Dashboard/GetGridContentV1',
            data: GetAjaxOptionsDataAndParameters(),
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (response, successStatusText, responseJqXhrObject) {
                //window.LogAjaxRequestDetails(responseJqXhrObject);
                gridJson = response;
                tablebody.find('tr:gt(0)').remove();
                var filetext = "Download";
                debugger;
                

                if (response.data.itemDetails && response.data.itemDetails.length) {
                    //if (!response.CanEdit && !response.CanViewMultiple && !response.CanDelete) {
                    //    var f_child = tablebody.children('tr:first').children("th:first");
                    //f_child.remove();

                    tablebody.children("tr").remove();

                    // }

                    $.each(response.data.itemDetails, function (index, item) {
                        debugger;

                        var rowContent = "";
                        var actionCell = `<button type="button" class="btn btn-sm btn-primary View" data-filepath="${item.imageUrl}">View</button>`;
                        var actionedit = `<button type="button" class="btn btn-sm btn-warning btn-edit" data-id="${item.imageId || ''}">Edit</button>`;

                        rowContent =
                            '<td class="text-center">' + ((currentPage - 1) * pageSize + (index + 1)) + '</td>' +
                            '<td class="text-center">' + BindValueToGrid(item.categoryName, false, false) + '</td>' +
                            '<td class="text-center">' + BindValueToGrid(item.cropName, false, false) + '</td>' +
                            '<td class="text-center">' + BindValueToGrid(item.sectionName, false, false) + '</td>' +
                            '<td class="text-center">' + BindValueToGrid(item.itemName, false, false) + '</td>' +
                            '<td class="text-justify">' + BindValueToGrid(item.description, false, false) + '</td>' +
                            '<td class="text-center">' + actionCell  + actionedit + '</td>';
                        tablebody.append('<tr>' + rowContent + '</tr>');
                    })

                }
                else {
                    tablebody.append('<tr><td colspan="27"><h3 style="width:400px;min-height:120px;">No Data Found!!!</h3></td><tr>');
                }
                totalRecords = response.data.totalCount;
                debugger;
                setupControls();
                pagingDetails();
                var elem = $('#pagingButtons li a');
                $('#pagingButtons li a').removeClass('active');
                $('#pagingButtons li a:contains(' + currentPage + ')').parent('li').addClass("active")
                $('#pagingButtons li a:contains(' + currentPage + ')').css("cursor", "not-allowed")
                //LoadAjaxAnimation(false);
                //loader.width(tablebody.width());
            },
            error: function (error) {

                if (error.status == 302) {
                    //alert(302);
                    return
                }
                alert(error.statusText + "\n" + error.responseText);
                //Removed Con_sole logging
                //LoadAjaxAnimation(false);
                //loader.width(tablebody.width());
            }
        })
    };


    $('body').on('click', '.btn-edit', function () {
        var imagecontentid = $(this).data('id');

        if (!imagecontentid || imagecontentid == "undefined") {
            alert("Internal server error!");
            return false;
        }

        var actionurl = `Dashboard/GetByIdImageConentDetails/${imagecontentid}`;

        $.ajax({
            cache: false,
            url: GetRootPath(window.virtualPath) + '/CommonApi/ForwardApiResponse?action_name=' + actionurl,
            type: "GET",
            responseType: "application/json",
            success: function (response) {
                if (!response || !response.success || !response.data) {
                    alert("Invalid response received.");
                    return;
                }

                var data = response.data;
                ClearForm();

                // Dropdown binding
                $("#Categoriestype").val(data.categoryId).trigger('change');

                // Load dependent dropdowns with delays to ensure data loads in correct order
                setTimeout(function () {
                    $("#Cropstype").val(data.cropId).trigger('change');
                }, 500);

                setTimeout(function () {
                    $("#Sectionstype").val(data.sectionId).trigger('change');
                }, 1000);

                // Load item list and select the correct item
                setTimeout(function () {
                    var sectionId = data.sectionId;
                    var cropId = data.cropId;
                    loadDropdown(
                        GetRootPath(window.virtualPath) + '/Dashboard/GetItems?sectionId=' + sectionId + '&cropId=' + cropId,
                        '#ItemsIdtype',
                        '--Select Item--'
                    );
                }, 100);
           
                setTimeout(function () {
                    $("#ItemsIdtype").val(data.itemId).trigger('change');
                }, 2000);
                 // Description
                $("input[name='ItemImages[0].Description']").val(data.description || '');

                // Image preview
                $("#filename").val(data.imageUrl || "");
                $("#ImageId").val(data.imageId || "");  // <-- use correct field from API
             

                if (data.imageUrl) {
                     const fullUrl = fileBaseUrl +"InwardsInvoices/TempFiles/Content/" + data.imageUrl;

                    // Remove old preview if any
                    $(".img-preview").remove();

                    // Create new preview
                    var imgPreview = $('<img>')
                        .attr('src', fullUrl)
                        .addClass('img-preview mt-2 rounded')
                        .css({
                            width: '100px',
                            height: '100px',
                            objectFit: 'cover',
                            border: '1px solid #ccc'
                        });

                    // Append preview below file input
                    $("input[type='file']").after(imgPreview);
                }
            },
            error: function () {
                alert("Failed to fetch details!");
            }
        });
    });



    function ClearForm() {

        // Reset all dropdowns
        $("#Categoriestype").val("").trigger('change');
        $("#Cropstype").val("").trigger('change');
        $("#Sectionstype").val("").trigger('change');
        $("#ItemsIdtype").empty().append('<option value="">--Select Item--</option>');

        // Clear text fields
        $("input[type='text']").val("");

        // Clear description
        $("input[name='ItemImages[0].Description']").val("");

        // Clear file input
        $("input[type='file']").val("");

        // Remove all previous image previews
        $(".img-preview").remove();

        // Clear hidden fields
        $("#CropId").val("");
        $("#ItemId").val("");
        $("#SectionId").val("");
        $("#CategoryId").val("");
        $("#itemType").val("");
        $("#filename").val("");
        $(".img-preview").val("");

    }


});