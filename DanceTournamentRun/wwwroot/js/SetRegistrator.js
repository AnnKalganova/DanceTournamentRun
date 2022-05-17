$('#submitAddReg').on('click', function (e) {
    e.preventDefault();
    console.log($('#addRegForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/AddRegistrator",
        data: $('#addRegForm').serialize(),
        success: function (data) {
            $('#registratorsTable').html(data);
        },
        error: function () {
            alert('Error');
        }

    });
    $(".modal-backdrop").remove();
});