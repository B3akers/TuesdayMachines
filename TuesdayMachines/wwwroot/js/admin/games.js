var gamesTableDatatable = $('#gamesTable').DataTable({
    "pageLength": 25,
    "ajax": getGamesUrl,
    "columns": [
        { "data": "name" },
        { "data": "code" },
        {
            name: "color",
            data: "color",
            render: function (data, type) {
                if (type === "display") {
                    return `<div style="height: 1.5rem; background-color: ${data};"></div>`;
                }
                return data;
            },
            orderable: false
        },
        {
            "data": "id", orderable: false, render: function (data, type, row) {
                if (type === "display") {
                    return `<i role="button" data-id="${data}"onclick="editGameClick(this)" class="fas fa-edit m-1"></i><i role="button" style="color: #ff0000;" data-id="${data}" onclick="deleteGameClick(this)" class="fas fa-times m-1"></i>`;
                }
                return data;
            }
        },
    ]
});

function getDataById(id) {
    const data = gamesTableDatatable.rows().data();
    for (let i = 0; i < data.length; i++) {
        const obj = data[i];
        if (obj && obj.id == id)
            return obj;
    }
    return null;
}

function deleteGameClick(e) {
    document.getElementById('confirmModalButtonYes').dataset.id = e.dataset.id;
    document.getElementById('confirmModalButtonYes').dataset.url = deleteGameUrl;
    document.getElementById('confirmModalButtonYes').dataset.datatable = 'gamesTableDatatable';
    document.getElementById('confirmModalButtonYes').dataset.callback = 'confirmObjectDelete';

    $('#confirmModal').modal('show');
}

function addGameClick() {
    document.getElementById('gameModal').querySelector('[data-form-name="id"]').value = '';
    $('#gameModal').modal('show');
}

function editGameClick(e) {
    const data = getDataById(e.dataset.id);
    if (!data) {
        return;
    }

    const modal = document.getElementById('gameModal');
    modal.querySelector('[data-form-name="id"]').value = data.id;

    modal.querySelector('[data-form-name="name"]').value = data.name;
    modal.querySelector('[data-form-name="code"]').value = data.code;
    modal.querySelector('[data-form-name="color"]').value = data.color;
    modal.querySelector('[data-form-name="logo"]').value = data.logo;
    modal.querySelector('[data-form-name="metadata"]').value = data.metadata.join('\n');

    $('#gameModal').modal('show');
}

function addGameResponse(json) {
    $('#gameModal').modal('hide');
    toastr.success(translateCode(json.success));
    if (gamesTableDatatable)
        gamesTableDatatable.ajax.reload();
}
