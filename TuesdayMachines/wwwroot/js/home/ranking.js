var isUpdating = false;

async function updateRanking() {
    if (isUpdating) {
        return;
    }

    isUpdating = true;

    const wallet = document.getElementById('walletId').value;
    if (!wallet) {
        isUpdating = false;
        return;
    }

    const result = await getApiPostResult(getAccountsRankingUrl, {
        id: wallet
    });

    if (result) {
        const table = document.getElementById('statsTable');
        const tbody = table.querySelector('tbody');
        tbody.innerHTML = '';

        const mappedUsers = {};

        for (let i = 0; i < result.accounts.length; i++) {
            const obj = result.accounts[i];
            mappedUsers[obj.id] = obj;
        }

        let index = 1;
        for (let i = 0; i < result.data.length; i++) {
            const data = result.data[i];
            const user = mappedUsers[data.twitchUserId];
            if (!user) {
                continue;
            }

            const tr = document.createElement('tr');
            const th = document.createElement('th');
            th.setAttribute('scope', 'row');
            th.innerText = index;

            const tdUser = document.createElement('td');
            const tdWallet = document.createElement('td');
            const tdValue = document.createElement('td');

            tdUser.innerText = user.twitchLogin;
            tdWallet.innerText = result.wallet.points + "[" + result.wallet.login + "]";
            tdValue.innerText = `${data.balance.toLocaleString()}`;

            tr.appendChild(th);
            tr.appendChild(tdUser);
            tr.appendChild(tdWallet);
            tr.appendChild(tdValue);
            tbody.appendChild(tr);

            index++;
        }
    }

    isUpdating = false;
}

(function () {
    updateRanking();
})();