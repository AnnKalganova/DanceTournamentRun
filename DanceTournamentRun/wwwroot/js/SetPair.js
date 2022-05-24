$("#dialogAddPair").on('show.bs.modal', function (e) {
    var groups = e.relatedTarget.dataset.groups;
    var groupsObj = JSON.parse(groups);
    console.log(groupsObj);
    $('#groupSelect').append('<option value="' + groupsObj[0].id + '">' + groupsObj[0].name.replace("_", " ") + '</option>');

    for (var i = 1; i < groupsObj.length; i++) {
        $('#groupSelect').append('<option value="' + groupsObj[i].id + '">' + groupsObj[i].name.replace("_", " ") + '</option>');
    }
});

$('#submitAddPair').on('click', function (e) {
    e.preventDefault();
   // console.log($('#addPairForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/AddPair",
        data: $('#addPairForm').serialize(),
        success: function (data) {
            $('#pairsTable').html(data);
        },
        error: function () {
            alert('Error');
        }

    });
    $(".modal-backdrop").remove();
});

//edit pair 

$("#dialogEditPair").on('show.bs.modal', function (e) {

    var pairId = e.relatedTarget.dataset.id;
    var p1LastN = e.relatedTarget.dataset.p1lastname;
    var p1FirstN = e.relatedTarget.dataset.p1firstname;
    var p2LastN = e.relatedTarget.dataset.p2lastname;
    var p2FirstN = e.relatedTarget.dataset.p2firstname;

    //populate the controls with values        
    $(e.currentTarget).find('input[name="Id"]').val(pairId);
    $(e.currentTarget).find('input[name="Partner1LastName"]').val(p1LastN);
    $(e.currentTarget).find('input[name="Partner1FirstName"]').val(p1FirstN);
    $(e.currentTarget).find('input[name="Partner2LastName"]').val(p2LastN);
    $(e.currentTarget).find('input[name="Partner2FirstName"]').val(p2FirstN);

});

$('#submitEditPair').on('click', function (e) {
    e.preventDefault();
    //console.log($('#editPairForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/EditPair",
        data: $('#editPairForm').serialize(),
        success: function (data) {
            $('#pairsTable').html(data);
        },
        error: function () {
            alert('Error');
        }

    });
    $(".modal-backdrop").remove();
});

//delete pair 
var cancelDelGrp = document.getElementById('cancelDelPair');

cancelDelGrp.addEventListener('click', function () {
    $("#delPairName").text("");
    $("#delPairId").val("");
});

$('#dialogDeletePair').on('show.bs.modal', function (event) {
    var reference_tag = $(event.relatedTarget);
    var id = reference_tag.data('id');
    var name = reference_tag.data('name');
    $("#delPairId").val(id);
    $("#delPairName").text(name);
})

$('#submitDelPair').on('click', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: "/Admin/DaletePair",
        data: $('#delPairForm').serialize(),
        success: function (data) {
            $('#pairsTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});

//change plus on minus
$(document).on('click', '.btn-open', function (event) {
    event.preventDefault();
    var controlForm = $('.controls-open');
    controlForm.find('.entry-add:not(:last) .btn-add')
        .removeClass('btn-add').addClass('btn-remove')
        .removeClass('btn-success').addClass('btn-danger')
        .html('<span class="oi oi-minus" title="icon name" aria-hidden="true"></span>');

    var inputs = $('.controls-add .form-control-add');
    $.each(inputs, function (index, item) {
        item.name = 'Dances[' + index + ']';
    });
});