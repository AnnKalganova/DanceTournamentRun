

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
   /* e.preventDefault();*/
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
});