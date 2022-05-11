

$("#dialogAddReferee").on('show.bs.modal', function (e) {
    var groups = e.relatedTarget.dataset.groups;
    var groupsObj = JSON.parse(groups);
    console.log(groupsObj);
    $('select').append('<option value="' + groupsObj[0].id + '">' + groupsObj[0].name + '</option>');

    for (var i = 1; i < groupsObj.length; i++) {
        $('select').append('<option value="' + groupsObj[i].id + '">' + groupsObj[i].name + '</option>');
    }

    $('select').selectpicker();
});


$('#submitAddReferee').on('click', function (e) {
    e.preventDefault();
    console.log($('#addRefereeForm').serialize());

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
});