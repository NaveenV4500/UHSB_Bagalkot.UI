 
    $(document).ready(function() {
        // Example: set userName and phone number (you can set this from server if needed)
        if (!localStorage.getItem("UserName")) {
            var username = $("UserName").val();
            localStorage.setItem("userName", username);
        }
        if (!localStorage.getItem("phoneNumber")) {
            var phoneNumber = $("phoneNumber").val();
            localStorage.setItem("phoneNumber", phoneNumber);
        }

        // Display the values
        $('#UserName').text(localStorage.getItem("userName"));
        $('#PhoneNumber').text(localStorage.getItem("phoneNumber"));

        // Optional: update values via input or button
        $('#saveUserBtn').click(function() {
            var newUserName = $('#userNameInput').val();
            var newPhone = $('#phoneInput').val();
            localStorage.setItem("userName", newUserName);
            localStorage.setItem("phoneNumber", newPhone);

            $('#UserName').text(newUserName);
            $('#PhoneNumber').text(newPhone);
        });
    });
 
