var isUpdating = false;
async function updateStatistics() {
    if (isUpdating) {
        return;
    }

    isUpdating = true;

    const timeValues = [
        (Date.now() - (3600 * 24 * 1000)) / 1000,
        (Date.now() - (7 * (3600 * 24 * 1000))) / 1000,
        (Date.now() - (30 * (3600 * 24 * 1000))) / 1000,
    ];

    const game = document.getElementById('gameCodeId').value;
    const time = document.getElementById('timeRange').value;
    const wallet = document.getElementById('walletId').value;
    const showX = document.getElementById('showType').value == '1';

    const result = await getApiPostResult(getSpinsStatsUrl, {
        game: game,
        wallet: wallet,
        time: Math.floor(timeValues[time]),
        sortByXWin: showX
    });


    if (result) {
        const table = document.getElementById('statsTable');
        const tbody = table.querySelector('tbody');
        tbody.innerHTML = '';

        const mappedUsers = {};
        const mappedBroadcasters = {};

        for (let i = 0; i < result.accounts.length; i++) {
            const obj = result.accounts[i];
            mappedUsers[obj.id] = obj;
        }

        for (let i = 0; i < result.wallets.length; i++) {
            const obj = result.wallets[i];
            mappedBroadcasters[obj.id] = obj;
        }

        for (let i = 0; i < result.data.length; i++) {
            const data = result.data[i];

            const tr = document.createElement('tr');
            const th = document.createElement('th');
            th.setAttribute('scope', 'row');
            th.innerText = (i + 1);

            const tdUser = document.createElement('td');
            const tdWallet = document.createElement('td');
            const tdDate = document.createElement('td');
            const tdValue = document.createElement('td');

            const date = new Date(data.datetime * 1000);

            tdUser.innerText = mappedUsers[data.accountId].twitchLogin;
            tdWallet.innerText = mappedBroadcasters[data.wallet].points + "[" + mappedBroadcasters[data.wallet].login + "]";
            tdDate.innerText = date.toLocaleDateString() + " " + date.toLocaleTimeString();
            tdValue.innerText = `${data.win.toLocaleString()} (${data.winX.toLocaleString()}X)`;

            tr.appendChild(th);
            tr.appendChild(tdUser);
            tr.appendChild(tdWallet);
            tr.appendChild(tdDate);
            tr.appendChild(tdValue);
            tbody.appendChild(tr);
        }
    }
    isUpdating = false;
}

(function () {
    updateStatistics();
})();