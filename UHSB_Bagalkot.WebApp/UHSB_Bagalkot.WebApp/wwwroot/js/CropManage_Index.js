$(document).ready(function () {
     
    $('#ItemImagesTable').find('input, select, textarea, button').prop('disabled', true);
    $('#addRow').prop('disabled', true);
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
                debugger;
                var $dropdown = $(dropdownSelector);
                $dropdown.empty().append('<option value="">' + defaultText + '</option>');
                $.each(data, function (i, item) {
                    $dropdown.append('<option value="' + item.id + '">' + item.name + '</option>');
                });
            },
            error: function (xhr, status, error) {
                hideLoader();
                console.error('Error loading data:', error);
            }
        });
        hideLoader();
    }

    // change events
    $('#Categoriestype').on('change', function () { 
        var catId = $(this).val();
        
        $('#Cropstype, #Sectionstype, #SubSectionstype, #ItemsIdtype').val(0).html('<option value="0">--Select--</option>');

        loadDropdown('/Dashboard/GetCrops?catId=' + catId, '#Cropstype', '--Select Crop--');
    });



    $('#Cropstype').on('change', function () {
        var cropId = $(this).val();
        $("#Sectionstype").val(0);
        $("#SubSectionstype").val(0);
        $("#ItemsIdtype").val(0); 
 
        loadDropdown('/Dashboard/GetSections?cropId=' + cropId, '#Sectionstype', '--Select Section--');
    });



    $('#Sectionstype').on('change', function () {
        var sectId = $(this).val();
        $("#SubSectionstype").val(0);
        $("#ItemsIdtype").val(0); 
        loadDropdown('/Dashboard/GetSubSections?sectId=' + sectId, '#SubSectionstype', '--Select Subsection--');
    });

    $('#SubSectionstype').on('change', function () {
        var subsectId = $(this).val();
        debugger; 
        $('#ItemImagesTable').find('input, select, textarea, button').prop('disabled', false);
        $('#addRow').prop('disabled', false);

        loadDropdown('/Dashboard/GetItems?subSectId=' + subsectId, '#ItemsIdtype', '--Select Subsection--');
    });

    $('#ItemsIdtype').on('change', function () {
        var sectId = $(this).val();
        //loadDropdown('/Dashboard/GetItems?sectId=' + sectId, '#ItemsIdtype', '--Select Items--');
    });
    
   
    function showLoader() {
        $("#loader").fadeIn(200);
    }
     
    function hideLoader() {
        $("#loader").fadeOut(200);
    }
 

    // Add new row
    $("#addRow").click(function () {
        var rowCount = $("#ItemImagesTable tbody tr").length;
        var newRow = `
        <tr>
            <td>
                <select name="ItemImages[${rowCount}].ItemId" class="form-control">
                    <option value="">--Select Item--</option>
                    ${$("#ItemsIdtype:first").html()} 
                </select>
            </td>
            <td>
                <input type="text" name="ItemImages[${rowCount}].Description" class="form-control" />
            </td>
            <td class="text-center">
                <label class="btn btn-outline-primary btn-sm mb-0">
                    <i class="fa fa-upload"></i> Upload
                    <input type="file" name="ItemImages[${rowCount}].ImageFile" class="d-none" />
                </label>
            </td>
            <td>
                <button type="button" class="btn btn-danger btn-sm removeRow">Remove</button>
            </td>
        </tr>`;

        $("#ItemImagesTable tbody").append(newRow);
        $('#SubSectionstype').trigger();
    });


    // Remove row
    $(document).on("click", ".removeRow", function () {
        $(this).closest("tr").remove();
        resetIndexes();
    });

    // Re-index names so model binding works
    function resetIndexes() {
        $("#ItemImagesTable tbody tr").each(function (i, row) {
            $(row).find("select, input").each(function () {
                var name = $(this).attr("name");
                if (name) {
                    var newName = name.replace(/\[\d+\]/, "[" + i + "]");
                    $(this).attr("name", newName);
                }
            });
        });
    }
});
