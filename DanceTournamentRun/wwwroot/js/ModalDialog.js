var showModal = document.getElementById('showModalAddGroup');
/*var ok = document.getElementById('ok');*/
var cancel = document.getElementById('cancel');
var dialog = document.getElementById('dialogAddGroup');
var groupName = document.getElementById('groupName');
var message = document.getElementById('message');

showModal.addEventListener('click', function () {
    if (!dialog.open) {
        dialog.showModal();
    }
    else {
        message.innerHTML = 'Dialog is already open!';
    }
});


var addGroup = function (tournId) {
    dialog.close();
    $.ajax({
        url: "/Admin/AddGroup",
        data: { "tournId": tournId, "Name": groupName.value },
        type: "POST",
        success: function (data) {
            $('#groupsTable').html(data);
        },
        error: function () {
            message.innerHTML = 'ERROR';
        }
    });
    groupName.value = "";

}


if (cancel) {
    cancel.addEventListener('click', function () {
        message.innerHTML = 'You cancelled the dialog';
        dialog.close();
    });
}
