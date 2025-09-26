$(document).ready(function () {
    $('#ItemImagesBody').find('input, select, textarea, button').prop('disabled', true);
    $('#addRow').prop('disabled', true);

    var currentDescriptionInput;
    var editorInstance;

    _init();

    function _init() {
        loadDropdown('/Dashboard/GetCategories', '#Categoriestype', '--Select Category--');
    }

    function loadDropdown(url, dropdownSelector, defaultText) {
        showLoader();
        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                var $dropdown = $(dropdownSelector);
                $dropdown.empty().append('<option value="">' + defaultText + '</option>');
                $.each(data, function (i, item) {
                    $dropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                });
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
        loadDropdown('/Dashboard/GetCrops?catId=' + $(this).val(), '#Cropstype', '--Select Crop--');
    });

    $('#Cropstype').on('change', function () {
        resetDropdowns(['#Sectionstype', '#SubSectionstype']);
        loadDropdown('/Dashboard/GetSections?cropId=' + $(this).val(), '#Sectionstype', '--Select Section--');
    });

    $('#Sectionstype').on('change', function () {
        resetDropdowns(['#SubSectionstype']);
        loadDropdown('/Dashboard/GetSubSections?sectId=' + $(this).val(), '#SubSectionstype', '--Select Subsection--');
    });

    function resetDropdowns(selectors) {
        selectors.forEach(function (sel) {
            $(sel).val('').html('<option value="">--Select--</option>');
        });
    }

    // SubSection change → populate table
    $('#SubSectionstype').on('change', function () {
        var subSectId = $(this).val();
        $('#ItemImagesBody tr:gt(1)').remove();

        if (!subSectId) return;

        $('#ItemImagesBody').find('input, select, textarea, button').prop('disabled', false);
        $('#addRow').prop('disabled', false);
        loadDropdown('/Dashboard/GetItems?subSectId=' + $(this).val(), '#ItemsIdtype', '--Select Item--');
        loadItems(subSectId);
    });

    var roletypeNo = parseInt($('#userTypeNo').val() || 0); // Example role type
    var currentPage = 1;
    var pageSize = 10;

    function loadItems(subSectId) {

        var tbody = $('#ItemImagesTable tbody');
        tbody.empty(); // clear existing rows
        tbody.append('<tr><td colspan="5" class="text-center">Loading...</td></tr>');

        $.ajax({
            url: '/Dashboard/GetgridItems?subSectId=' + subSectId,
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                tbody.empty();
                var items = response.data || [];

                if (items.length === 0) {
                    tbody.append('<tr><td colspan="5" class="text-center">No items found</td></tr>');
                    return;
                }

                items.forEach(function (item, index) {
                    var fileLink = item.filePath
                        ? '<a href="' + $.trim($('#rootpath').text()) + 'Dashboard/downloadFile?FilePath=' + item.filePath + '" target="_blank">View</a>'
                        : '';


                    var actionCell = '';
                    if (true) {
                        //actionCell = `<span class="btn-link View" style="cursor:pointer" onclick="ViewItemimage(${item.itemId})">View image</span> | ${fileLink}`;
                        var actionCell = `<span class="btn-link View" style="cursor:pointer" data-filepath="${item.imageUrl || ''}">View Image</span> | ${fileLink}`;

                    } else {
                        actionCell = fileLink;
                    }

                    var row = `
                    <tr>
                        <td class="text-center">${(currentPage - 1) * pageSize + (index + 1)}</td>
                        <td class="text-center">${item.itemId}</td>
                        <td class="text-justify">${item.description}</td> 
                        <td class="text-center">${actionCell}</td>
                    </tr>
                `;
                    tbody.append(row);
                });
            },
            error: function (xhr, status, error) {
                tbody.empty();
                tbody.append('<tr><td colspan="5" class="text-center">Error loading data</td></tr>');
                console.error('AJAX error:', error, xhr.responseText);
            }
        });
    }

    // Delete function
    function deleteItem(itemId) {
        if (confirm('Are you sure to delete Item ID: ' + itemId + '?')) {
            console.log('Delete logic for ID:', itemId);
            // Add AJAX delete call here
        }
    }
    //View Image

    // Put this once in your JS, after table exists
    $('#ItemImagesTable').on('click', '.View', function () {
        var filePath = $(this).data('filepath');
        if (!filePath) {
            alert('No image available.');
            return;
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
                <img src="${"http://10.255.136.158:15815/" + filePath}" style="max-width:100%; height:auto;" />
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

        if (editorInstance) {
            editorInstance.destroy().then(() => initEditor(currentDescriptionInput.val()));
        } else {
            initEditor(currentDescriptionInput.val());
        }
    });

    function initEditorOld(data) {
        ClassicEditor.create(document.querySelector('#editorDescription'), {
            fontFamily: { options: ['Noto Sans Kannada, sans-serif', 'Arial, sans-serif'] }
        }).then(editor => {
            editorInstance = editor;
            editor.setData(data);
        });
    }
    function initEditor(data) {
        ClassicEditor.create(document.querySelector('#editorDescription'), {
            fontFamily: {
                options: [
                    'Noto Sans Kannada, sans-serif',
                    'Arial, sans-serif'
                ]
            },
            table: {
                contentToolbar: [
                    'tableColumn', 'tableRow', 'mergeTableCells'
                    // 'tableProperties', 'tableCellProperties' → needs custom build
                ]
            }
        }).then(editor => {
            editorInstance = editor;
            editor.setData(data);

            // ✅ Add border styling for tables in editor content
            const style = document.createElement('style');
            style.innerHTML = `
            .ck-content table, 
            .ck-content th, 
            .ck-content td {
                border: 1px solid #dee2e6;
                border-collapse: collapse;
                padding: 6px;
            }
        `;
            document.head.appendChild(style);
        });
    }

    $('#saveDescription').click(function () {
        if (editorInstance && currentDescriptionInput) {
            currentDescriptionInput.val(editorInstance.getData());
            $('#descriptionModal').modal('hide');
        }
    });


    // Add new row
    $("#addRow").click(function () {
        // Get the current row count from the correct table body
        var rowCount = $("#ItemImagesBody tr").length;

        // The HTML for the new row. We create it first.
        var newRowHtml = `
        <tr>
            <td>
                <select name="ItemImages[${rowCount}].ItemId" class="form-select itemDropdown">
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

        // 1. Append the new row and get a reference to it
        var $newRow = $(newRowHtml).appendTo('#ItemImagesBody');

        // 2. Find the specific dropdown within the new row you just added
        var $newDropdown = $newRow.find('.itemDropdown');

        // 3. Get the selected Sub Section ID from the main filter dropdown
        var subSectionId = $('#SubSectionstype').val();

        // 4. If a sub-section is selected, call loadDropdown for the new dropdown
        if (subSectionId) {
            // Assuming 'loadDropdown' is a function you've defined elsewhere
            loadDropdown('/Dashboard/GetItems?subSectId=' + subSectionId, $newDropdown, '--Select Item--');
        }
    });

    // Remove row - This can remain the same, but ensure it's inside doc ready
    $(document).on("click", ".removeRow", function () {
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
        var subSectId = $(this).val();
        debugger;
        return {
            "currentPage": currentPage,
            "pageSize": pageSize,
            "orderBy": window.orderByColumnId,
            "isDescending": window.isDescendingFilter,
            "filterDetails": window.filterCollection.length > 0 ? JSON.stringify(window.filterCollection) : "",
            "externalFilter": window.getExternalFilter,
            "subSectId": subSectId
        }
    }
});

//    window.init = function () {

//        LoadAjaxAnimation(true);
//        var tablebody = $('#gridContent table tbody');

//        if (!tablebody.children("tr").length) {
//            tablebody.append(theadRow);
//        }
//        if (tablebody.children().children("td").length < 2) {
//            tablebody.append('<tr><td colspan="27"><div style="min-height:120px;"></div></td><tr>');
//        }

//        loader.width(tablebody.width());
//        debugger;
//        $.ajax({

//            cache: false,
//            type: "POST",
//            dataType: 'json',
//            url: GetRootPath(window.virtualPath) + '/Dashboard/GetgridItems',
//            data: GetAjaxOptionsDataAndParameters(),
//            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
//            success: function (response, successStatusText, responseJqXhrObject) {
//                window.LogAjaxRequestDetails(responseJqXhrObject);
//                gridJson = response;
//                tablebody.find('tr:gt(0)').remove();
//                var filetext = "Download";

//                if (response.ItemDetails && response.ItemDetails.length) {
//                    if (!response.CanEdit && !response.CanViewMultiple && !response.CanDelete) {
//                        var f_child = tablebody.children('tr:first').children("th:first");
//                        f_child.remove();
//                    }
//                    console.log("Data >>" + JSON.stringify(response.ItemDetails));
//                    $.each(response.ItemDetails, function (index, item) {
//                        var rowContent = "";
//                        if (response.CanEdit || response.CanViewMultiple || response.CanDelete) {
//                            var edit_span = '';
//                            var delete_span = '';
//                            var view_attach_span = "";
//                            var saperator = '|';
//                            /*var popupedit = 1;*/
//                            //var popupdelete = 2;
//                            //var popupdirectdelete = 3;
//                            if (roletypeNo == 6) {
//                                delete_span = '<span id=' + item.Identifier + ' class="btn-link delete" style="cursor:pointer" >Delete</span>';
//                            }

//                            if (item.FilePath != "" && item.FilePath != null) {
//                                view_attach_span = '<a href="' + ($.trim($('#rootpath').text()) + 'FTPDocuments/downloadFile?FilePath=' + item.FilePath) +
//                                    '" target="_blank">' + filetext + '</a>';
//                            }
//                        }

//                        rowContent =
//                            '<td class="text-center">' + ((currentPage - 1) * pageSize + (index + 1)) + '</td>' +
//                            '<td class="text-center">' + BindValueToGrid(item.UserName, false, false) + '</td>' +
//                            '<td class="text-center">' + BindValueToGrid(item.BranchName, false, false) + '</td>' +
//                            '<td class="text-center">' + BindValueToGrid(item.DepartmentName, false, false) + '</td>' +
//                            '<td class="text-center">' + BindValueToGrid(item.Description, false, false) + '</td>';

//                        if (roletypeNo == 6) {
//                            rowContent +=
//                                '<td class="text-center">' +
//                                '<textarea style="width:350px; height:100px;" class="form-control" id="textarea_' + item.Identifier + '" ' +
//                                'data-user="' + item.UserId + '" rows="3"></textarea>' +
//                                '</td>';
//                        }



//                        //'<td class="text-center">' + BindValueToGrid(item.FilePath, false, false) + '</td>';

//                        if (roletypeNo == 6) {
//                            rowContent += "<td class='text-center'>" + delete_span + saperator + view_attach_span + "</td>";
//                        } else {
//                            rowContent += "<td class='text-center'>" + view_attach_span + "</td>";
//                        }


//                        tablebody.append('<tr>' + rowContent + '</tr>');
//                    })
//                }
//                else {
//                    tablebody.append('<tr><td colspan="27"><h3 style="width:400px;min-height:120px;">No Data Found!!!</h3></td><tr>');
//                }
//                // $('#btnBack').hide();
//                totalRecords = response.TotalCount;
//                debugger;
//                setupControls();
//                pagingDetails();
//                var elem = $('#pagingButtons li a');
//                $('#pagingButtons li a').removeClass('active');
//                $('#pagingButtons li a:contains(' + currentPage + ')').parent('li').addClass("active")
//                $('#pagingButtons li a:contains(' + currentPage + ')').css("cursor", "not-allowed")
//                LoadAjaxAnimation(false);
//                loader.width(tablebody.width());
//            },
//            error: function (error) {

//                if (error.status == 302) {
//                    //alert(302);
//                    return
//                }
//                alert(error.statusText + "\n" + error.responseText);
//                //Removed Con_sole logging
//                LoadAjaxAnimation(false);
//                loader.width(tablebody.width());
//            }
//        })

     

//});
