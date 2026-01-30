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
            url: GetRootPath(window.virtualPath) + '/UserMaster/GetGridContentV1',
            //url: GetRootPath(window.virtualPath) + 'UserMaster/GetGridContentV1',
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
                            rowContent = "<td class='text-center'>" + view_span + '|' + delete_span + "</td>";
                        }
                        rowContent +=
                            '<td class="text-center">' + (parseInt((currentPage - 1) * pageSize) + parseInt(index + 1)) + '</td>' +
                            '<td>' + BindValueToGrid(item.userName, false, false) + '</td>' +
                            '<td>' + BindValueToGrid(item.roleTypeName || '', false, false) + '</td>' +

                            '<td>' + BindValueToGrid(item.districtsName || '', false, false) + '</td>' +
                            '<td>' + BindValueToGrid(item.village || '', false, false) + '</td>' +
                            '<td>' + BindValueToGrid(item.phoneNumber || '', false, false) + '</td>' +

                            '<td>' + BindValueToGrid(item.isActive, false, true) + '</td>' +

                            '<td>' + BindValueToGrid(item.createdDate, true, false) + '</td>';
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

    //window.init();
    $(document).on("click", "#griddata", function () {
        window.init();
    });



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
            var url = GetRootPath(window.virtualPath) + "/UserMaster/Edit";
            form.attr('action', url);
        }
        else {
            var url = GetRootPath(window.virtualPath) + "/UserMaster/Register";
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






    function clearErrors() {
        $("span.text-danger").text("");
    }

    function validateForm() {
        clearErrors();
        let isValid = true;

        // Name
        if (!$("#UserName").val().trim()) {
            $("#errorUserName").text("Name is required");
            isValid = false;
        }

        // Phone (10 digits)
        const phone = $("#PhoneNumber").val().trim();
        if (!/^\d{10}$/.test(phone)) {
            $("#errorPhoneNumber").text("Enter valid 10-digit phone number");
            isValid = false;
        }

        // Role
        const role = parseInt($("#RoleType").val());
        if (!role || role === 0) {
            $("#errorRoleType").text("Please select a role");
            isValid = false;
        }

        // District
        const district = parseInt($("#DistrictsId").val());
        if (!district || district === 0) {
            $("#errorDistrictsId").text("Please select a district");
            isValid = false;
        }

        // Village
        if (!$("#Village").val().trim()) {
            $("#errorVillage").text("Village is required");
            isValid = false;
        }

        // Farmer validation
        //if (role === 2) {
        //    const landSize = parseFloat($("#LandSize").val());
        //    if (!landSize || landSize <= 0) {
        //        $("#errorLandSize").text("Enter valid land size");
        //        isValid = false;
        //    }
        //}

        // Scientist validation
        if (role === 3) {
            const empId = $("#EmployeeId").val().trim();
            const email = $("#EmailId").val().trim();
            if (!/^\d{5}$/.test(empId)) {
                $("#errorEmployeeId").text("Employee ID must be 5 digits");
                isValid = false;
            }
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                $("#errorEmailId").text("Valid email required");
                isValid = false;
            }
        }

        return isValid;
    }

    // Show/hide role-based fields
    $("#RoleType").on("change", function () {
        debugger;
        const role = parseInt($(this).val());
        $(".role-farmer, .role-scientist").addClass("d-none");

        if (role == 2) $(".role-farmer").removeClass("d-none");
        if (role == 3) $(".role-scientist").removeClass("d-none");

        clearErrors();
    });

    // AJAX submit
    var userroletype = 0;
    $("#btnRegister").click(function () {
        if (!validateForm()) return;

        const token = $('input[name="__RequestVerificationToken"]').val();
        userroletype = parseInt($("#RoleType").val());
        const data = {
            Id: parseInt($("#Id").val()) || 0,
            UserName: $("#UserName").val().trim(),
            PhoneNumber: $("#PhoneNumber").val().trim(),
            RoleType: parseInt($("#RoleType").val()),
            DistrictsId: parseInt($("#DistrictsId").val()),
            Village: $("#Village").val()?.trim() || null,
            Address: $("#Address").val()?.trim() || null,

            LandSize:  0,

            EmployeeId: $("#EmployeeId").val()
                ? parseInt($("#EmployeeId").val())
                : null,

            EmailId: $("#EmailId").val()?.trim() || null,

            IsActive: true,

            PasswordHash: "temp",

            CreatedDate: new Date().toISOString(),
            UpdatedDate: null,
            ModifiedDate: null,
            CreatedBy: null,
            ModifiedBy: null
        };


        $.ajax({
            url: GetRootPath(window.virtualPath) +'/Account/RegisterAutoLogin',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': token },
            data: JSON.stringify(data), 
            success: function (res) {

                if (typeof res === "string") {
                    res = JSON.parse(res);
                }

                if (res.success) {
                    alert(res.message || "Registered successfully");
                    autoLogin(
                        $("#UserName").val().trim(),
                        $("#PhoneNumber").val().trim()
                    );
                } else {
                    alert(res.message);
                }
            } ,
            error: function () {
                alert("Something went wrong");
            }
        });
});

    function autoLogin(userName, phoneNumber) {

        const token = $('input[name="__RequestVerificationToken"]').val();

        const loginData = {
            UserName: userName,
            PhoneNumber: phoneNumber,
            IsFromadmin: true
        };

        $.ajax({
            url: GetRootPath(window.virtualPath) +'/Account/Login',
            type: 'POST',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': token },
            data: JSON.stringify(loginData),
            success: function (res) {

                // 🔒 Safety: handle string JSON
                if (typeof res === "string") {
                    res = JSON.parse(res);
                }

                if (res.success) {
                    window.location.href = res.redirectUrl;
                } else {
                    if (userroletype == 3 || userroletype == "3") {
                        alert("Registered successfully !. Pending for admin approval.");
                        window.location.href = GetRootPath(window.virtualPath) +'/Account/Login'; 
                    }
                    alert(res.message || "Auto login failed. Please login manually.");
                    window.location.href = GetRootPath(window.virtualPath) +'/Account/Login'; // ✅ redirect on fail
                }
            },
            error: function () {
                alert("Auto login failed. Please login manually.");
                window.location.href = GetRootPath(window.virtualPath) +'/Account/Login'; // ✅ redirect on error
            }
        });
    }



});

