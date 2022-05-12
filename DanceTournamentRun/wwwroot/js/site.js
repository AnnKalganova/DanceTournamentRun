// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var viewGroups = function (tournId) {
    $.ajax({
        url: "/Admin/ViewGroups",
        data: { "tournId": tournId},
        type: "GET",
        success: function (data) {
            $('#groupsTable').html(data);
        },
        error: function () {
            $("#groupsTable").html("ERROR");
        }
    });
};

var viewPairs = function (tournId) {
    $.ajax({
        url: "/Admin/ViewPairs",
        data: { "tournId": tournId },
        type: "GET",
        success: function (data) {
            $('#pairsTable').html(data);
        },
        error: function () {
            $("#pairsTable").html("ERROR");
        }
    });
};

var viewReferees = function (tournId) {
    $.ajax({
        url: "/Admin/ViewReferees",
        data: { "tournId": tournId },
        type: "GET",
        success: function (data) {
            $('#refereesTable').html(data);
        },
        error: function () {
            $("#refereesTable").html("ERROR");
        }
    });
};

var viewRegistrators = function (tournId) {
    $.ajax({
        url: "/Admin/ViewRegistrators",
        data: { "tournId": tournId },
        type: "GET",
        success: function (data) {
            $('#registratorsTable').html(data);
        },
        error: function () {
            $("#registratorsTable").html("ERROR");
        }
    });
};

$('#runTournament').click(function () {
    var buttonData = $(this).attr('data');
    console.log(buttonData);
    $.ajax({
        type: "GET",
        url: "/Admin/RunTourn",
        data: { "tournId": buttonData },
        success: function (data) {
            alert('турнир начался');
            $('#refereesTable').html(data);
        },
        error: function () {
            alert('Error');
        }

    });
});

