var accountsTableDatatable;

async function editAccountClick(e) {
    const id = e.dataset.id;
    const twitchId = e.dataset.twitchId;
    const result = await getApiPostResult(getAccountWalletUrl, { twitchId: twitchId });

    const modal = document.getElementById('editAccountModal');

    modal.querySelector('[data-form-name="twitchId"]').value = result.twitchId;
    modal.querySelector('[data-form-name="points"]').value = result.balance;

    $('#editAccountModal').modal('show');
}

(function () {
    accountsTableDatatable = $('#accountsTable').DataTable({
        pageLength: 25,
        serverSide: true,
        order: [],
        ajax: {
            url: getAccountsUrl,
            contentType: "application/json",
            type: "POST",
            data: function (d) {
                return JSON.stringify(d);
            }
        },
        columns: [
            { data: "twitchId", orderable: false },
            { data: "twitchLogin", orderable: false },
            {
                data: "creationTime", orderable: false, render: function (data, type) {
                    if (type === "display") {
                        return new Date(data * 1000).toLocaleString();
                    }
                    return data;
                }
            },
            {
                data: "id", orderable: false, render: function (data, type, row) {
                    if (type === "display") {
                        return `<i role="button" data-twitch-id="${row['twitchId']}" data-id="${data}"onclick="editAccountClick(this)" class="fas fa-edit m-1"></i>`;
                    }
                    return data;
                }
            },
        ]
    });
})();