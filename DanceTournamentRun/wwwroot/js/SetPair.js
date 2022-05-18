$("#dialogAddPair").on('show.bs.modal', function (e) {
    var groups = e.relatedTarget.dataset.groups;
    var groupsObj = JSON.parse(groups);
    console.log(groupsObj);
    $('#groupSelect').append('<option value="' + groupsObj[0].id + '">' + groupsObj[0].name + '</option>');

    for (var i = 1; i < groupsObj.length; i++) {
        $('#groupSelect').append('<option value="' + groupsObj[i].id + '">' + groupsObj[i].name + '</option>');
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

