const userWallets = [];

function getSelectGame() {
    const selectedCategory = document.querySelector('.category-selection.show');
    if (!selectedCategory) {
        return undefined;
    }

    return selectedCategory.dataset.categoryId;
}

function playClick() {
    const game = getSelectGame();
    if (!game) {
        return;
    }

    document.querySelectorAll('[data-form-name="gameId"]').forEach(x => {
        x.value = game;
    });

    $('#selectWalletModal').modal('show');
}

function makeid(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
}

var currentSeed = null;

function updateFairPlaySeeds() {
    const modal = document.getElementById('fairPlayModal');

    modal.querySelector('input[name="activeClientSeed"]').value = currentSeed.client;
    modal.querySelector('input[name="activeServerSeed"]').value = currentSeed.server;
    modal.querySelector('input[name="nonceNumber"]').value = currentSeed.nonce;
    modal.querySelector('input[name="nextServerSeed"]').value = currentSeed.nextServer;

    modal.querySelector('input[data-form-name="clientSeed"]').value = makeid(16);
}

async function fairPlayClick() {
    if (!currentSeed) {
        currentSeed = await getApiGetResult(getCurrentSeedUrl);
    }

    updateFairPlaySeeds();

    $('#fairPlayModal').modal('show');
}

function decryptServerResponse(json) {
    const modal = document.getElementById('decryptServerModal');
    modal.querySelector('input[name="decryptedServerSeed"]').value = json.seed;
}

function onDecryptServerClick() {
    $('#fairPlayModal').modal('hide');
    $('#decryptServerModal').modal('show');
}

function changeSeedResponse(json) {
    currentSeed = json;
    updateFairPlaySeeds();
}

function playGameResponse(json) {
    window.location.href = json.redirect;
}

(async function () {
    showMessagesFromUrl();

    const walletsData = await getApiGetResult(getWalletsUrl);
    if (walletsData) {
        const mapped = {};
        for (let i = 0; i < walletsData.broadcasters.length; i++) {
            const broadcaster = walletsData.broadcasters[i];
            mapped[broadcaster.accountId] = broadcaster;
        }

        for (let i = 0; i < walletsData.wallets.length; i++) {
            const wallet = walletsData.wallets[i];
            const mappedBroadcaster = mapped[wallet.broadcasterAccountId];
            if (!mappedBroadcaster) {
                continue;
            }

            userWallets.push({
                id: mappedBroadcaster.accountId,
                name: mappedBroadcaster.points,
                owner: mappedBroadcaster.login,
                balance: wallet.balance
            });
        }

        const walletsSelect = document.getElementById('playWalletId');
        for (let i = 0; i < userWallets.length; i++) {
            const wallet = userWallets[i];
            const option = document.createElement('option');
            option.value = wallet.id;
            option.innerText = `${wallet.name}[${wallet.balance.toLocaleString()}] (${wallet.owner})`;
            walletsSelect.appendChild(option);
        }
    }
})();