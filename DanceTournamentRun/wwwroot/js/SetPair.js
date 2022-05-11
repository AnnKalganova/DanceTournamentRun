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
    console.log($('#addPairForm').serialize());

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
});

