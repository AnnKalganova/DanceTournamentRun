$("#dialogAddPair").on('show.bs.modal', function (e) {
    var groups = e.relatedTarget.dataset.groups;
    var groupsObj = JSON.parse(groups);
    console.log(groupsObj)
    $('#groupSelect').append('<option value="' + groupsObj.Id + '">' + groupsObj.Name + '</option>');

    //for (var i = 1; i < groupsObj.length; i++) {
    //    $('#groupSelect').append('<option value="' + groupsObj[i].Id + '">' + groupsObj[i].Name + '</option>');
    //}
});

$('#submitAddPair').on('click', function (e) {
    e.preventDefault();
    console.log($('#addPairForm').serialize());

    //$.ajax({
    //    type: "POST",
    //    url: "/Admin/EditGroup",
    //    data: $('#editGrForm').serialize(),
    //    success: function (data) {
    //        $('#groupsTable').html(data);
    //    },
    //    error: function () {
    //        alert('Error');
    //    }

    //});
});
