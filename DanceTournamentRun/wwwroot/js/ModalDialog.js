

var groupName = document.getElementById('groupName');
var cancel = document.getElementById('cancelAddGrp');
var close = document.getElementById('closeAddGrp');

cancel.addEventListener('click', function () {
    groupName.value = "";
});

close.addEventListener('click', function () {
    groupName.value = "";
});

$('#submitAddGrp').on('click', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: "/Admin/AddGroup",
        data: $('#addGrForm').serialize(),
        success: function (data) {
            $('#groupsTable').html(data);
        },
        error: function () {
            message.innerHTML = 'ERROR';
            alert('Error');
        }

    });
    console.log($('#addGrForm').serialize());
});



$(document).on('click', '.btn-add', function (event) {
    event.preventDefault();
    var controlForm = $('.controls');
    var currentEntry = $(this).parents('.entry:first');
    var newEntry = $(currentEntry.clone()).appendTo(controlForm);
    newEntry.find('input').val('');
    controlForm.find('.entry:not(:last) .btn-add')
        .removeClass('btn-add').addClass('btn-remove')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="oi oi-minus" title="icon name" aria-hidden="true"></span>');

    var inputs = $('.controls .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});

$(document).on('click', '.btn-remove', function (event) {
    event.preventDefault();
    $(this).parents('.entry:first').remove();
    var inputs = $('.controls .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});