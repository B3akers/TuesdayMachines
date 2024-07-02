var channelsTableDatatable = $('#broadcastersTable').DataTable({
    "pageLength": 25,
    "ajax": getBroadcastersUrl,
    "columns": [
        {
            "data": "twitchId", orderable: false, render: function (data, type, row) {
                if (type === "display") {
                    return `<a href="/broadcaster/index/${row['accountId']}">${data}</a>`;
                }
                return data;
            }
        },
        { "data": "login" },
        { "data": "points" },
        {
            "data": "id", orderable: false, render: function (data, type, row) {
                if (type === "display") {
                    return `<i role="button" style="color: #ff0000;" data-id="${data}" onclick="deleteBroadcasterClick(this)" class="fas fa-times m-1"></i>`;
                }
                return data;
            }
        },
    ]
});

function deleteBroadcasterClick(e) {
    document.getElementById('confirmModalButtonYes').dataset.id = e.dataset.id;
    document.getElementById('confirmModalButtonYes').dataset.url = deleteBroadcasterUrl;
    document.getElementById('confirmModalButtonYes').dataset.datatable = 'channelsTableDatatable';
    document.getElementById('confirmModalButtonYes').dataset.callback = 'confirmObjectDelete';

    $('#confirmModal').modal('show');
}

function addBroadcasterResponse(json) {
    $('#broadcasterModal').modal('hide');
    toastr.success(translateCode(json.success));
    if (channelsTableDatatable)
        channelsTableDatatable.ajax.reload();
}
