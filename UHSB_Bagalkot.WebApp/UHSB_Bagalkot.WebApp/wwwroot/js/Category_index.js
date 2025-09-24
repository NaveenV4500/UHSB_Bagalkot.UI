$(document).ready(function () {
    loadCategories();

    // Convert uploaded file to Base64
    $("#ImageFile").change(function () {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $("#ImageUrl").val(e.target.result); // store base64 string
                $("#previewImg").attr("src", e.target.result).show();
            };
            reader.readAsDataURL(file);
        }
    });

    // Save form
    $("#categoryForm").submit(function (e) {
        e.preventDefault();

        var formData = $(this).serialize(); // bind form fields automatically

        $.ajax({
            url: '/Category/AddOrEdit',
            type: 'POST',
            data: formData,
            success: function () {
                resetForm();
                loadCategories();
            }
        });
    });

    // Back button
    $("#btnBack").click(function () {
        resetForm();
    });
});

// Load table
function loadCategories() {
    $.get("/Category/GetAll", function (data) {
        var rows = "";
        $.each(data, function (i, item) {
            var imgTag = item.imageUrl ? `<img src="${item.imageUrl}" style="max-height:50px;" />` : "";
            rows += `<tr>
                <td>${item.categoryId}</td>
                <td>${item.name}</td>
                <td>${imgTag}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editCategory(${item.categoryId},'${item.name}','${item.imageUrl || ""}')">Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteCategory(${item.categoryId})">Delete</button>
                </td>
            </tr>`;
        });
        $("#categoryTable tbody").html(rows);
    });
}

// Fill form for editing
function editCategory(id, name, imageUrl) {
    $("#CategoryId").val(id);
    $("#Name").val(name);
    $("#ImageUrl").val(imageUrl);
    if (imageUrl) {
        $("#previewImg").attr("src", imageUrl).show();
    } else {
        $("#previewImg").hide();
    }
    $('html, body').animate({ scrollTop: $("#categoryForm").offset().top }, 300);
}

// Delete
function deleteCategory(id) {
    if (confirm("Are you sure?")) {
        $.ajax({
            url: '/Category/Delete?id=' + id,
            type: 'DELETE',
            success: function () {
                loadCategories();
            }
        });
    }
}

// Reset form
function resetForm() {
    $("#categoryForm")[0].reset();
    $("#CategoryId").val("");
    $("#ImageUrl").val("");
    $("#previewImg").hide();
}
