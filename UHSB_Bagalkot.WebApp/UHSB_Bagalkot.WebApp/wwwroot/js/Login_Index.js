
//$(document).ready(function () {
//    // Example: set userName and phone number (you can set this from server if needed)
//    if (!localStorage.getItem("UserName")) {
//        var username = $("UserName").val();
//        localStorage.setItem("userName", username);
//    }
//    if (!localStorage.getItem("phoneNumber")) {
//        var phoneNumber = $("phoneNumber").val();
//        localStorage.setItem("phoneNumber", phoneNumber);
//    }

//    // Display the values
//    $('#UserName').text(localStorage.getItem("userName"));
//    $('#PhoneNumber').text(localStorage.getItem("phoneNumber"));

//    // Optional: update values via input or button
//    $('#saveUserBtn').click(function () {
//        var newUserName = $('#userNameInput').val();
//        var newPhone = $('#phoneInput').val();
//        localStorage.setItem("userName", newUserName);
//        localStorage.setItem("phoneNumber", newPhone);

//        $('#UserName').text(newUserName);
//        $('#PhoneNumber').text(newPhone);
//    });
//});


$(document).ready(function () {
    var RoleypeId = 0;
    // Restore localstorage
    if (localStorage.getItem("UserName")) {
        $('#UserName').val(localStorage.getItem("UserName"));
        $('#PhoneNumber').val(localStorage.getItem("PhoneNumber"));
        $('#rememberMe').prop('checked', true);
    }

    var OtpEnable = $("#OtpEnable").length
        ? ($("#OtpEnable").val() || "").toString().toLowerCase()
        : "false";


    // Toggle OTP vs Phone-only login based on role
    function toggleLoginFields() {
        var role = $('#ddlRole').val();
        roleTypeId = parseInt(role, 10);

        if (role === "1" || role === "3") {   // Admin & Scientist → OTP
            $('#divUserName').show();
            $('#divPhoneNumber').show();
            if (OtpEnable == "true") {
                $('#sendOtpBtn').show();
                $('#verifyOtpBtn').hide();
                $('#sendwithoutOtpBtn').hide();
                $('#otpContainer,#otpInfo').hide();
            } else {
                $('#sendwithoutOtpBtn').show();

            }
           

        } else if (role != "0" || role != 0) {           // Other roles → No OTP
            $('#divUserName').hide();
            $('#divPhoneNumber').show();
            $('#sendOtpBtn,#verifyOtpBtn').hide();
            $('#sendwithoutOtpBtn').show();
            $('#otpContainer,#otpInfo').hide();

        } else {                              // No role selected
            $('#divUserName,#divPhoneNumber').hide();
            $('#sendOtpBtn,#verifyOtpBtn,#sendwithoutOtpBtn').hide();
            $('#otpContainer,#otpInfo').hide();
        }
    }

    $('#ddlRole').change(toggleLoginFields);
    $('#ddlRole').trigger('change');

    // Timer function for OTP
    let timerInterval;
    const startTimer = () => {
        let time = 180;
        $('#resendOtp').off('click').hide();
        $('#countdown').show();
        if (timerInterval) clearInterval(timerInterval);
        timerInterval = setInterval(() => {
            $('#countdown').text("Resend in " + time + " sec");
            time--;
            if (time < 0) {
                clearInterval(timerInterval);
                $('#countdown').hide();
                $('#resendOtp').show().off('click').on('click', function (e) {
                    e.preventDefault(); sendOtpRequest('/UHSBWeb/Account/SendOtp', true);
                });
            }
        }, 1000);
    };

    // Mask email
    const maskEmail = email => {
        if (!email) return "";
        let [name, domain] = email.split("@");
        const maskedName = name.length > 3 ? name.charAt(0) + '***' + name.charAt(name.length - 1) : '***';
        return maskedName + "@" + domain;
    };

    // AJAX send OTP or phone login
    const sendOtpRequest = (url, isOtp) => {
        let obj = {
            UserName: $('#UserName').val(),
            PhoneNumber: $('#PhoneNumber').val(),
            IsFromadmin: true,
            RoleypeId: roleTypeId
        };
        if (isOtp) {
            $("#sendOtpBtn").prop("disabled", true).html(`<span class='spinner-border spinner-border-sm'></span> Sending...`);
            $('#otpError').text("");
            $('#otpInfo').hide();
        } else {
            $("#sendwithoutOtpBtn").prop("disabled", true).html(`<span class='spinner-border spinner-border-sm'></span> Processing...`);
        }

        $.ajax({
            url: url,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(obj),
            success: function (res) {
                if (isOtp) {
                    $("#sendOtpBtn").prop("disabled", false).text("Send OTP & Login");
                    if (!res.success) {
                        $('#otpError').text(res.message || "Login failed.");
                        $('#toastBody').text(res.message);

                        var toastEl = document.getElementById('errorToast');
                        var toast = new bootstrap.Toast(toastEl, { delay: 3000 });
                        toast.show();
                        return;
                    }

                    if (res.redirectUrl) { window.location.href = res.redirectUrl; return; }

                    $("#otpContainer").slideDown();
                    $("#otpInfo").html(`<i class="bi bi-check-circle me-1"></i> OTP sent to: ${maskEmail(res.email)}`).show();
                    $("#verifyOtpBtn").show();
                    $("#sendOtpBtn").hide();
                    $("#verifyOtpBtn").data("userId", res.userId);
                    startTimer();
                } else {
                    if (res.success) {
                        window.location.href = res.redirectUrl;
                    } else {
                        $('#toastBody').text(res.message);

                        var toastEl = document.getElementById('errorToast');
                        var toast = new bootstrap.Toast(toastEl, { delay: 3000 });
                        toast.show();

                        $("#sendwithoutOtpBtn").prop("disabled", false).text("Login");
                    }
                }
            },
            error: () => { if (isOtp) { $("#sendOtpBtn").prop("disabled", false).text("Send OTP & Login"); } else { $("#sendwithoutOtpBtn").prop("disabled", false).text("Login"); } }
        });
    };

    // Click handlers
    $('#sendOtpBtn,#sendwithoutOtpBtn,#resendOtp').click(function (e) {
        e.preventDefault();
        if ($('#rememberMe').is(':checked')) {
            localStorage.setItem("UserName", $('#UserName').val());
            localStorage.setItem("PhoneNumber", $('#PhoneNumber').val());
        }
        else {
            localStorage.clear();
        }

        let url = ""; let isOtp = false;
        if (this.id === "sendOtpBtn" || this.id === "resendOtp") {
            url = '/UHSBWeb/Account/SendOtp'; isOtp = true;
        }
        else if (this.id === "sendwithoutOtpBtn") {
            url = '/UHSBWeb/Account/Login';
        }
        if (url) sendOtpRequest(url, isOtp);
    });

    $('#Otp').on("input", () => $('#otpError').text(""));

    $('#verifyOtpBtn').click(function () {
        let otp = $('#Otp').val();
        let userId = $(this).data('userId');
        if (!otp) return $('#otpError').text("Please enter the received OTP.");
        if (!userId) { console.error("User session lost. Please try again."); return; }
        $("#verifyOtpBtn").prop("disabled", true).html(`<span class='spinner-border spinner-border-sm'></span> Verifying...`);
        let obj = { UserId: userId, Otp: otp, UserName: $('#UserName').val(), PhoneNumber: $('#PhoneNumber').val(), IsFromadmin: true };
        $.ajax({
            url: "/UHSBWeb/Account/VerifyOtpPost",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(obj),
            success: (res) => { $("#verifyOtpBtn").prop("disabled", false).text("Verify OTP & Login"); if (res.success) { window.location.href = res.redirectUrl; } else { $('#otpError').text(res.message || "OTP verification failed."); } },
            error: () => { $("#verifyOtpBtn").prop("disabled", false).text("Verify OTP & Login"); }
        });
    });

});