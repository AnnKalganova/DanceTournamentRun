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


//edit registrator
$("#dialogEditRegistrator").on('show.bs.modal', function (e) {
    var id = e.relatedTarget.dataset.id;
    var login = e.relatedTarget.dataset.login;
    var lastName = e.relatedTarget.dataset.lastname;
    var firstName = e.relatedTarget.dataset.firstname;

    $(e.currentTarget).find('input[name="Id"]').val(id);
    $(e.currentTarget).find('input[name="Login"]').val(login);
    $(e.currentTarget).find('input[name="LastName"]').val(lastName);
    $(e.currentTarget).find('input[name="FirstName"]').val(firstName);

});

$('#submitEditReg').on('click', function (e) {
    e.preventDefault();
    console.log($('#editRegForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/EditRegistrator",
        data: $('#editRegForm').serialize(),
        success: function (data) {
            $('#registratorsTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});

//delete referee 
var cancelDelReg = document.getElementById('cancelDelReg');

cancelDelReg.addEventListener('click', function () {
    $("#delRegName").text("");
    $("#delRegId").val("");
});

$('#dialogDeleteReg').on('show.bs.modal', function (event) {
    var reference_tag = $(event.relatedTarget);
    var id = reference_tag.data('id');
    var name = reference_tag.data('name');
    $("#delRegId").val(id);
    $("#delRegName").text(name);
})

$('#submitDelReg').on('click', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: "/Admin/DaleteRegistrator",
        data: $('#delRegForm').serialize(),
        success: function (data) {
            $('#registratorsTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});