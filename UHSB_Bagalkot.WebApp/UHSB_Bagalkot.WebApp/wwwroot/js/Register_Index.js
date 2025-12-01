$(document).ready(function () {

    var gridJson = null;
    var mode_edit = false;

    window.GridFilterViewUrl = "UserMaster/GetSearchView";
    window.orderByColumnId = "0";
    window.isDescendingFilter = false;

    var pageSizeKey = 'pagesize_usermaster';
    GetPageSizeFromStorage(pageSizeKey);

    // $('#BaseBranchId').prop("disabled", false);
    var roletype = $('#RoleType').val();
    if (roletype == 4 || roletype == 6 || roletype == 7) {
        $('#basebranchhide').hide();
    }
    else {
        $('#basebranchhide').show();
    }

    $('#IsActive').prop('checked', true);

    //#region Ajax Loader Code
    var loader = $("#ajaxLoader");
    function resetLoaderPosition() {

        var left = $('.table-responsive').scrollLeft();
        var width = $('.table-responsive').width();
        var total = left + (width / 2) - 40;
        loader.children("img").css({ 'left': total + 'px' });
    }

    $(window).on('resize', function () {
        resetLoaderPosition();
    });

    $('.table-responsive').on('scroll', function () {
        resetLoaderPosition();
    });

    function LoadAjaxAnimation(isLoadingData) {
        if (isLoadingData) {
            $("#ajaxLoader").removeClass("hidden");
        }
        else {
            $("#ajaxLoader").addClass("hidden");
        }
    }
    //#endregion

    //#region TheadContent

    var theadRow = $('#gridContent table tbody');

    //#endregion

    resetLoaderPosition();

    //#region Paging Click

    window.init = function () {

        LoadAjaxAnimation(true);
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
            url: GetRootPath(window.virtualPath) +'/UserMaster/GetGridContentV1',
            //url: GetRootPath(window.virtualPath) + 'UserMaster/GetGridContentV1',
            data: {
                "currentPage": currentPage,
                "pageSize": pageSize,
                "orderBy": window.orderByColumnId,
                "isDescending": window.isDescendingFilter,
                "filterDetails":""// window.filterCollection.length > 0 ? JSON.stringify(window.filterCollection) : ""

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

                        var rowContent = "";
                        var view_span = "";
                        if (response.CanEdit || response.CanDelete) {
                            var edit_span = '';
                            var delete_span = '';
                            var saperator = '|';


                            if (response.CanEdit) {
                                edit_span = '<span id=' + item.UserId + ' class="btn-link edit " style="cursor:pointer" >Edit</span>';
                            }
                            if (response.CanDelete) {
                                delete_span = '<span id=' + item.UserId + ' class="btn-link delete" style="cursor:pointer" >Delete</span></td>';
                            }
                            if (response.CanEdit && response.CanDelete) {
                                saperator = '|';
                            }
                            rowContent = "<td class='text-center'>" + view_span + edit_span + saperator + delete_span + "</td>";
                        }
                        if (!response.CanEdit && !response.CanDelete) {
                            delete_span = '<span id=' + item.id + ' class="btn-link delete" style="cursor:pointer" >Delete</span></td>';

                            view_span = '<span id=' + item.id + ' class="btn-link view" style="cursor:pointer" >View</span>';
                            rowContent = "<td class='text-center'>" + view_span + '|' +delete_span+ "</td>";
                        }
                        rowContent +=
                            '<td class="text-center">' + (parseInt((currentPage - 1) * pageSize) + parseInt(index + 1)) + '</td>' +
                            '<td>' + BindValueToGrid(item.userName, false, false) + '</td>' +
                            '<td>' + BindValueToGrid(item.roleTypeName || '', false, false) + '</td>' +

                            '<td>' + BindValueToGrid(item.districtsName || '', false, false) + '</td>' +
                            '<td>' + BindValueToGrid(item.village || '', false, false) + '</td>' +
                            '<td>' + BindValueToGrid(item.phoneNumber || '', false, false) + '</td>' +

                        '<td>' + BindValueToGrid(item.isActive, false, true) + '</td>' +

                        '<td>' + BindValueToGrid(item.createdDate, true, false) + '</td>' ;
                            //'<td>' + BindValueToGrid(item.isResetRequested || false, false, true) + '</td>';

                        tablebody.append('<tr>' + rowContent + '</tr>');
                    })
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
                LoadAjaxAnimation(false);
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
                LoadAjaxAnimation(false);
                loader.width(tablebody.width());
            }
        })

    }

    //#endregion

    window.init();




    //#region Edit
    $('body').on('click', 'span.edit,span.view', function (e) {
        debugger;
        if (mode_edit == true && !confirm("Do you want to cancel edit ?")) {
            return
        }
        ResetForm("#form1");
        mode_edit = true;
        var rowId = $(this).attr("id");
        var editBindId = "#";

        if (!gridJson || !gridJson.ItemDetails || !gridJson.ItemDetails.length) {
            alert("Please try to refresh the page and try again.")
            return
        }

        var selecteduserMasterDetails = $.grep(gridJson.ItemDetails, function (e) {
            return e.UserId == rowId;
        });
         

        if (!selecteduserMasterDetails.length) {
            alert("Please try to refresh the page and try again.")
            return
        }

        var selecteduserMaster = selecteduserMasterDetails[0];
        // ;
        if (selecteduserMaster.RoleTypeId == 4 || selecteduserMaster.RoleTypeId == 6 || selecteduserMaster.RoleTypeId == 7) {
            $('#basebranchhide').hide();
        }
        else {
            $('#basebranchhide').show();
        }

        $(editBindId + "UserId").val(selecteduserMaster.UserId);
        $(editBindId + "UserName").val(selecteduserMaster.UserName);
        $(editBindId + "FullName").val(selecteduserMaster.FullName);
        $(editBindId + "RoleType").val(selecteduserMaster.RoleTypeId);
        $(editBindId + "EmpId").val(selecteduserMaster.EmpId);
        $(editBindId + "Email").val(selecteduserMaster.Email);
        $(editBindId + "MobileNo").val(selecteduserMaster.MobileNo);
        $(editBindId + "BaseBranchId").val(selecteduserMaster.BaseBranchId);
        $(editBindId + "IsActive").prop('checked', (selecteduserMaster.IsActive));


        scrollTop();
        //$("#form1").submit(function () {

        //});

        //$('#BaseBranchId').prop("disabled", true);

        //$('#BaseBranchId option:not(selected)').attr('disabled', true);
        //$('#BaseBranchId').prop("readonly", true);
        //$('#BaseBranchId').css({"background-color":"#EBEBE4"});

        $("#BaseBranchId").attr("readonly", true);
        $('#BaseBranchId').css('background-color', '#eee');
        $('#Password').prop("disabled", true);
        $('#ConfirmPassword').prop("disabled", true);

        if (window.isDisableAddEdit) {
            DisableAllInputs();
        }

        return false;

    })
    //#endregion

    //region Disable basebranch Dropdown
    $('body').on('change', '#RoleType', function (e) {
        var BaseBranchId = $("#BaseBranchId").val();
        var roletype = $('#RoleType').val();
        if (roletype == 4 || roletype == 6 || roletype == 7) {
            $('#basebranchhide').hide();
        }
        else {
            debugger;
            $('#basebranchhide').show();

            if ((roletype == 1 || roletype == 2 || roletype == 3 || roletype == 5) && mode_edit == true && BaseBranchId != "0") {
                return false;
            }
            $("#BaseBranchId").attr("readonly", false);
            $('#BaseBranchId').css('background-color', '#FFFFFF');
            $.ajax({
                type: "post",
                url: GetRootPath(window.virtualPath) + "BranchMaster/GetROBranches",
                data: { roleType: roletype },
                datatype: "json",
                success: function (response, successStatusText, responseJqXhrObject) {
                    //window.LogAjaxRequestDetails(responseJqXhrObject);
                    var data = JSON.parse(response);
                    // alert(data);                 
                    debugger;
                    if (data) {
                        var ddlValue = $('#BaseBranchId').html('');
                        for (var key in data) {
                            ddlValue.append('<option value = "' + key + '" >' + data[key] + '</option>')
                        }
                    }


                }

            })

        }
    })
    //end region
    //#region Delete
    $('body').on('click', 'span.delete', function (e) {
        var rowId = $(this).attr("id");
        if (confirm('Are you sure to delete?')) {
            e.preventDefault();

            $.ajax({
                type: "POST",
                url: GetRootPath(window.virtualPath) + '/Account/DeleteUser',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(rowId),   // send only number
                dataType: "json",
                success: function (data) {
                    debugger
                    
                        alert(data.success);
                        setTimeout(() => location.reload(), 100);
                    
                },
                error: function (xhr) {
                    alert(xhr.statusText);
                }
            });
        }
    });

    //#endregion

    //#region AddNew

    $('#addNew').on('click', function (e) {

        if (!mode_edit) {
            if (!confirm("Do you want to clear form ?")) {
                return;
            }
            // ResetForm("#form1");
            $("#form1").find("input:text").val("");
            //$('#BaseBranchId').css('background-color', '#fff');
        }
        else {
            if (!confirm("Do you want to cancel edit ?")) {
                return;
            }
        }


        $('#form1')[0].reset();
        ResetForm("#form1");
        $("#form1").find("input:text").val("");
        url = GetRootPath(window.virtualPath) + 'UserMaster/Register';
        window.location.href = url;
        //$.get("~/UserMaster/Register", null, function (data) {
        //    alert("hi");
        //});
        scrollTop();
        mode_edit = false

        // $('#IsActive').prop('checked', true);
        // $('#BaseBranchId').prop("disabled", false);
    })


    //#endregion


    //#region preventOpeningDropdown

    $('#BaseBranchId').on('mousedown', function (e) {
        //var val = $('#BranchId').val();
        var isReadonly = $(this).is('[readonly]');
        if (!isReadonly) {
            return true;
        }
        e.preventDefault();
        this.blur();
        window.focus();
    });

    //#endregion







    // #region ResetPaswword
    $('body').on('click', '.resetpassword', function () {

        //alert("your Password request has been sent");

        var rowId = $(this).attr("id");

        if (!gridJson || !gridJson.ItemDetails || !gridJson.ItemDetails.length) {
            alert("Please try to refresh the page and try again.")
            return
        }

        var selecteduserMasterDetails = $.grep(gridJson.ItemDetails, function (e) {
            return e.UserId == rowId;
        });

        if (!selecteduserMasterDetails.length) {
            alert("Please try to refresh the page and try again.")
            return
        }
        var selecteduserMaster = selecteduserMasterDetails[0];

        $("#form2 #UserName").val(selecteduserMaster.UserName);

        $('#ResetPasswordModal').modal('show');
    })

    // #endregion

    //$('#btn_submit').on('click', function () {
    //    var form = $('#form1');
    //    if (mode_edit) {
    //        ;
    //        var url = $('#rootpath').text() + "UserMaster/Edit";
    //        form.attr('action', url);
    //    }
    //    else {
    //        ;
    //        var pasword = $('#Password').val();
    //        var confirmPassword = $('#ConfirmPassword').val();
    //        if (pasword == "" && confirmPassword == "") {
    //            alert("Pls enter the Password and Confirm Password");

    //            return false;
    //        }
    //    }


    //    form.submit();
    //})


    //#region Change PageSize

    $('#gridmemberCount').on('change', function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        SetPageSizeFromStorage(pageSizeKey);
        window.init();
    })

    //#endregion


    //#region Prevent Reload/Back In Edit Mode 

    window.onbeforeunload = function () {
        if (mode_edit && !window.forceRedirect && !window.IsSubmitting) {
            return "You work will be lost.Are you sure you want to leave?";
        }
    }

    window.onsubmit = function () {
        window.forceRedirect = true;
        window.IsSubmitting = true;
    }

    //#endregion

    function ToLocalFormat(input_dateformat, saperator) {

        if (!saperator) {
            saperator = "/";
        }
        if (!input_dateformat) {
            return ''
        }
        var date = new Date(parseInt(input_dateformat.substr(6)));
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

    //#region AjaxFormSubmission

    $('#btn_submit').click(function () {
        if (window.IsSubmittingForm == true) {
            return false;
        }

        var form = $('#form1');
        if (mode_edit) {
            var url = GetRootPath(window.virtualPath) + "UserMaster/Edit";
            form.attr('action', url);
        }
        else {
            var url = GetRootPath(window.virtualPath) + "UserMaster/Register";
            form.attr('action', url);
        }
    })

    window.onBegin = function () {
        if ($('#Identifier').val() && $('#Identifier').val() != "0") {
            if ($('#IsApproved').val() && $('#IsApproved').val() == "true") {
                if (!confirm("The record is already approved.If you edit,it will go to pending approval. Do you want to continue ?")) {
                    return false;
                }
            }
        }
        window.IsSubmittingForm = true;
    }

    window.onSuccess = function (response) {
        if (response == "true") {
            alert("User saved successfully.");
            window.location.href = window.location.href;
            return
        }
        else {
            $('#error_container').find('ul').html(response);
        }
        scrollTop();
        //Removed Con_sole logging
        window.IsSubmittingForm = false;
    }

    window.onFailure = function (error) {
        window.IsSubmittingForm = false;
        alert("failed!!! \n" + error.statusText);
    }

    //#endregion

    //#region ResetAllInputs

    function DisableAllInputs() {
        $('#form1').find(':input').not(':input.buttonforaction').prop('disabled', true);
    }

    if (window.isDisableAddEdit) {
        DisableAllInputs();
    }

    //#endregion

})