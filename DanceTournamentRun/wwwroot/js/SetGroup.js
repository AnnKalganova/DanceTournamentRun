//add group
var groupName = document.getElementById('groupName');
var cancelAddGrp = document.getElementById('cancelAddGrp');
var close = document.getElementById('closeAddGrp');

cancelAddGrp.addEventListener('click', function () {
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
            alert('Error');
        }

    });
});

$(document).on('click', '.btn-add.addGr', function (event) {
    event.preventDefault();
    var controlForm = $('.controls.addGr');
    var currentEntry = $(this).parents('.entry:first');
    var newEntry = $(currentEntry.clone()).appendTo(controlForm);
    newEntry.find('input').val('');
    controlForm.find('.entry:not(:last) .btn-add.addGr')
        .removeClass('btn-add').addClass('btn-remove')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="oi oi-minus" title="icon name" aria-hidden="true"></span>');

    var inputs = $('.controls.addGr .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});

$(document).on('click', '.btn-remove.addGr', function (event) {
    event.preventDefault();
    $(this).parents('.entry:first').remove();
    var inputs = $('.controls.addGr .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});


//delete group
var cancelDelGrp = document.getElementById('cancelDelGrp');

cancelDelGrp.addEventListener('click', function () {
    $("#delGrName").text("");
    $("#delGrId").val("");
});

$('#dialogDeleteGroup').on('show.bs.modal', function (event) {
    var reference_tag = $(event.relatedTarget);
    var id = reference_tag.data('id')
    var name = reference_tag.data('name');
    $("#delGrId").val(id);
    $("#delGrName").text(name);
})

$('#submitDelGrp').on('click', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: "/Admin/DeleteGroup",
        data: $('#delGrForm').serialize(),
        success: function (data) {
            $('#groupsTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});


//edit group
$("#dialogEditGroup").on('show.bs.modal', function (e) {

    var grId = e.relatedTarget.dataset.id;
    var grName = e.relatedTarget.dataset.name;
    var grNumber = e.relatedTarget.dataset.number;
    var dances = e.relatedTarget.dataset.dances;

    var dancesObj = JSON.parse(dances);
    //populate the controls with values        
    $(e.currentTarget).find('input[name="GroupId"]').val(grId);
    $(e.currentTarget).find('input[name="Name"]').val(grName);
    $(e.currentTarget).find('input[name="Number"]').val(grNumber);

    $(e.currentTarget).find('input[name="Dances[0]"]').val(dancesObj[0].Name);

    if (dancesObj.length >= 2) {
        for (var i = 1; i < dancesObj.length; i++) {
            var controlForm = $('.controls.editGr');
            var currentEntry = $('.form-control.editGr').parents('.entry:first');
            var newEntry = $(currentEntry.clone()).appendTo(controlForm);
            newEntry.find('input').val(dancesObj[i].Name);
            controlForm.find('.entry:not(:last) .btn-add.editGr')
                .removeClass('btn-add').addClass('btn-remove')
                .removeClass('btn-success').addClass('btn-danger')
                .html('<span class="oi oi-minus" title="icon name" aria-hidden="true"></span>');
            var inputs = $('.controls.editGr .form-control');
            $.each(inputs, function (index, item) {
                item.name = 'Dances[' + index + ']';
            });
        }
 
    }
   // $(e.currentTarget).find('input[name="ViewModel.PropertyB"]').val();
});

$("#dialogEditGroup").on('hidden.bs.modal', function (e) {
    $(".entry:not(:last)").each(function () {
        $(this).remove();
    });

    var inputs = $('.controls.editGr .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});

$('#submitEditGrp').on('click', function (e) {
    e.preventDefault();
    //console.log($('#editGrForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/EditGroup",
        data: $('#editGrForm').serialize(),
        success: function (data) {
            $('#groupsTable').html(data);
        },
        error: function () {
            alert('Error');
        }

    });
});

$(document).on('click', '.btn-add.editGr', function (event) {
    event.preventDefault();
    var controlForm = $('.controls.editGr');
    var currentEntry = $(this).parents('.entry:first');
    var newEntry = $(currentEntry.clone()).appendTo(controlForm);
    newEntry.find('input').val('');
    controlForm.find('.entry:not(:last) .btn-add.editGr')
        .removeClass('btn-add').addClass('btn-remove')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="oi oi-minus" title="icon name" aria-hidden="true"></span>');

    var inputs = $('.controls.editGr .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});

$(document).on('click', '.btn-remove.editGr', function (event) {
    event.preventDefault();
    $(this).parents('.entry:first').remove();
    var inputs = $('.controls.editGr .form-control');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});
    
    