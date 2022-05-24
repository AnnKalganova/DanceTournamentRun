

$("#dialogAddReferee").on('show.bs.modal', function (e) {
    var groups = e.relatedTarget.dataset.groups;
    var groupsObj = JSON.parse(groups);
    //console.log(groupsObj);
    $('select').append('<option value="' + groupsObj[0].id + '">' + groupsObj[0].name.replace("_", " ") + '</option>');

    for (var i = 1; i < groupsObj.length; i++) {
        $('select').append('<option value="' + groupsObj[i].id + '">' + groupsObj[i].name.replace("_", " ") + '</option>');
    }
    $('select').selectpicker();
});


$('#submitAddReferee').on('click', function (e) {
    e.preventDefault();
   // console.log($('#addRefereeForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/AddReferee",
        data: $('#addRefereeForm').serialize(),
        success: function (data) {
            $('#refereesTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});

//edit referee
$("#dialogEditReferee").on('show.bs.modal', function (e) {
    var id = e.relatedTarget.dataset.id;
    var lastName = e.relatedTarget.dataset.lastname;
    var firstName = e.relatedTarget.dataset.firstname;
    var groups = e.relatedTarget.dataset.groups;
    var GroupsId = e.relatedTarget.dataset.refgroupsid;

    $(e.currentTarget).find('input[name="Id"]').val(id);
    $(e.currentTarget).find('input[name="LastName"]').val(lastName);
    $(e.currentTarget).find('input[name="FirstName"]').val(firstName);

    var groupsObj = JSON.parse(groups);
    var refGrId = JSON.parse(GroupsId);
 
    for (var i = 0; i < groupsObj.length; i++) {
        $('select').append('<option value="' + groupsObj[i].id + '">' + groupsObj[i].name.replace("_", " ") + '</option>');
    }
    $('select').selectpicker();

    var values = [];
    for (var k = 0; k < refGrId.length; k++) {
        values.push(refGrId[k]);
    }
    $('select').selectpicker('val', values);
   
});

$('#submitEditReferee').on('click', function (e) {
    e.preventDefault();
    console.log($('#editRefereeForm').serialize());

    $.ajax({
        type: "POST",
        url: "/Admin/EditReferee",
        data: $('#editRefereeForm').serialize(),
        success: function (data) {
            $('#refereesTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});

//delete referee 
var cancelDelGrp = document.getElementById('cancelDelReferee');

cancelDelGrp.addEventListener('click', function () {
    $("#delRefereeName").text("");
    $("#delRefereeId").val("");
});

$('#dialogDeleteReferee').on('show.bs.modal', function (event) {
    var reference_tag = $(event.relatedTarget);
    var id = reference_tag.data('id');
    var name = reference_tag.data('name');
    $("#delRefereeId").val(id);
    $("#delRefereeName").text(name);
})

$('#submitDelReferee').on('click', function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: "/Admin/DaleteReferee",
        data: $('#delRefereeForm').serialize(),
        success: function (data) {
            $('#refereesTable').html(data);
        },
        error: function () {
            alert('Error');
        }
    });
    $(".modal-backdrop").remove();
});