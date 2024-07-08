
var $inner = $('.inner'),
    $data = $('.data'),
    $mask = $('.mask'),
    timer = 1500;

const red = [32, 19, 21, 25, 34, 27, 36, 30, 23, 5, 16, 1, 14, 9, 18, 7, 12, 3];
var selectedCoin;
var walletId;

var webSocketClient;
var keepAliveWebSocketInterval;

var nextSpinTime;
var totalClients;
var roomClients;
var spinNonce;
var myAccountId;
var myTwitchId;
var betClosed = false;

const rtf1 = new Intl.RelativeTimeFormat('en', { style: 'short' });

function updateSpanTimestamp(span, time) {
    const timeSpan = +span.dataset.time;
    const delta = Math.floor((time - timeSpan) / 1000);

    if (delta < 60) {
        span.innerText = rtf1.format(-delta, 'second');
    } else if (delta < 3600) {
        span.innerText = rtf1.format(-Math.round(delta / 60), 'minute');
    } else {
        span.innerText = rtf1.format(-Math.round(delta / 3600), 'hour');
    }
}

var currentRoundBets = {};
var lastRoundBets = {};

var otherPlayersBets = {};

const audioAssets = {
    ball: '../assets/roulette/ball.wav',
    win: '../assets/roulette/win.wav',
    click: '../assets/plinko/click.wav'
};

function playAudio(src, volume) {
    const audio = new Audio(src);
    if (volume) {
        audio.volume = volume;
    }
    audio.play();
}

function keepAliveWebSocket() {
    webSocketClient.send('#1');
}

function updateTooltipSpanByData(span, data) {
    const logins = Object.keys(data);
    const bets = [];

    for (let i = 0; i < logins.length; i++) {
        bets.push({
            login: logins[i],
            bet: data[logins[i]]
        });
    }

    bets.sort((a, b) => {
        if (a.bet > b.bet) {
            return -1;
        }

        if (a.bet < b.bet) {
            return 1;
        }
        return 0;
    });

    let text = '';
    for (let i = 0; i < bets.length; i++) {
        const bet = bets[i];
        if (i > 0)
            text += '\n';
        text += `${i + 1}. ${bet.login} ${bet.bet.toLocaleString()}`;
    }

    span.innerText = text;
}

function addLastResult(number) {
    const results = document.querySelector('.lastResults');
    const box = document.createElement('div');
    box.classList.add('spinResult', 'no-select');

    if (number == 0) {
        box.style.backgroundColor = 'green';
    } else {
        const found = red.find((element) => element == number);
        box.style.backgroundColor = found ? 'red' : 'black';
    }

    const span = document.createElement('span');
    span.innerText = number;
    box.appendChild(span);
    results.insertBefore(box, results.firstChild);
    if (results.children.length > 20) {
        results.removeChild(results.lastChild);
    }
}

function updateBalance(balance) {
    const element = document.getElementById('balanceSpan');
    element.dataset.value = balance;
    element.innerText = balance.toLocaleString();
}

function addToBalance(value) {
    const element = document.getElementById('balanceSpan');
    const balance = +value + +(element.dataset.value ?? 0);
    element.dataset.value = balance;
    element.innerText = balance.toLocaleString();

    const session = document.getElementById('currentSessionSpan');
    let sessionBalance = +(session.dataset.value ?? 0);

    sessionBalance += +value;

    if (sessionBalance != 0)
        session.style.color = sessionBalance > 0 ? 'green' : 'red';
    else
        session.style.color = 'white';

    session.dataset.value = sessionBalance;
    session.innerText = sessionBalance.toLocaleString();
}

function addMyBetToBoard(number, amount) {
    const e = document.querySelector('.horizontalBetBoard [data-bet="' + number + '"]');
    let playerCoin = e.querySelector('.coinOnNumber');
    if (!playerCoin) {
        playerCoin = document.createElement('div');
        playerCoin.classList.add('coin', 'coinOnNumber', 'red2', 'no-select');
        playerCoin.dataset.value = 0;

        e.appendChild(playerCoin);
    }
    playerCoin.dataset.value = +playerCoin.dataset.value + amount;
    playerCoin.innerText = playerCoin.dataset.value;

    if (!currentRoundBets[number]) {
        currentRoundBets[number] = 0;
    }

    currentRoundBets[number] += +amount;
}

function processMessagePacket(packet) {
    const chatWindow = document.querySelector('section.chat-window');
    if (chatWindow.children.length > 30) {
        chatWindow.children[0].remove();
    }

    const message = document.createElement('div');
    const messageText = document.createElement('p');
    messageText.classList.add('msg');
    messageText.innerText = packet.msg;
    message.appendChild(messageText);

    let messageAdded = false;
    const isMyMessage = packet.twitchId == myTwitchId;
    if (chatWindow.children.length > 0) {
        const lastMessage = chatWindow.children[chatWindow.children.length - 1];
        const messages = lastMessage.querySelector('.messages[data-id="' + packet.twitchId + '"]');
        const posttimeSpan = lastMessage.querySelector('.flr span[data-time]');

        if (messages && ((packet.time - posttimeSpan.dataset.time) / 1000) < 5) {
            messages.appendChild(message);
            messageAdded = true;

            const posttimeSpan = lastMessage.querySelector('.flr span[data-time]');
            posttimeSpan.dataset.time = packet.time;
            posttimeSpan.innerText = 'Now';
        }
    }

    if (!messageAdded) {
        const sec = document.createElement('article');
        sec.classList.add('msg-container');
        sec.classList.add(isMyMessage ? 'msg-self' : 'msg-remote');

        const div = document.createElement('div');
        div.classList.add('msg-box');

        const div2 = document.createElement('div');
        div2.classList.add('flr');

        const div3 = document.createElement('div');
        div3.dataset.id = packet.twitchId;
        div3.classList.add('messages');

        const timestamp = document.createElement('span');
        timestamp.classList.add('timestamp');

        const username = document.createElement('span');
        username.classList.add('username');
        username.innerText = packet.login;

        const bull = document.createTextNode('\u2022');
        const posttime = document.createElement('span');
        posttime.classList.add('posttime');
        posttime.dataset.time = packet.time;
        posttime.innerText = 'Now';

        timestamp.appendChild(username);
        timestamp.appendChild(bull);
        timestamp.appendChild(posttime);

        div3.appendChild(message);
        div2.appendChild(div3);
        div2.appendChild(timestamp);
        div.appendChild(div2);

        sec.appendChild(div);
        chatWindow.append(sec);
    }
}

function scrollToEndChat() {
    const chatWindow = document.querySelector('section.chat-window');

    var limit = Math.max(document.body.scrollHeight, document.body.offsetHeight,
        chatWindow.clientHeight, chatWindow.scrollHeight, chatWindow.offsetHeight);

    chatWindow.scrollTo(0, limit);
}

function initWebsocketConnection() {
    if (!walletId) {
        return;
    }

    webSocketClient = new WebSocket(playGameUrl);

    webSocketClient.onopen = function (event) {
        keepAliveWebSocketInterval = setInterval(keepAliveWebSocket, 5000);
        webSocketClient.send(walletId);
    };

    webSocketClient.onmessage = function (event) {
        if (event.data == '#2')
            return;

        const packet = JSON.parse(event.data);
        if (packet.id == 'game_state') {
            myAccountId = packet.accountId;
            myTwitchId = packet.twitchId;

            const lastResults = document.querySelector('.lastResults');
            lastResults.innerHTML = '';

            updateBalance(packet.balance);
            clearBets();
            clearWinners();

            currentRoundBets = {};

            if (packet.myBets) {
                const keys = Object.keys(packet.myBets);
                for (let i = 0; i < keys.length; i++) {
                    const number = keys[i];
                    addMyBetToBoard(number, packet.myBets[number]);
                }
            }

            for (let i = 0; i < packet.lastResults.length; i++) {
                const item = packet.lastResults[i];
                addLastResult(item);
            }

            const chatWindow = document.querySelector('section.chat-window');
            chatWindow.innerHTML = '';

            for (let i = 0; i < packet.messages.length; i++) {
                processMessagePacket(packet.messages[i]);
            }

            if (packet.messages.length > 0) {
                scrollToEndChat();
            }
            
            betClosed = packet.betClosed;
            totalClients = packet.clientsCount;
            roomClients = packet.roomClientsCount;
            nextSpinTime = packet.nextSpinTime;
            spinNonce = packet.nonce;

        } else if (packet.id == 'game_result') {
            clearBets();
            rouletteSpin(packet.number);
            betClosed = false;
            totalClients = packet.clientsCount;
            nextSpinTime = packet.nextSpinTime;
            spinNonce = packet.nonce;

            lastRoundBets = currentRoundBets;
            currentRoundBets = {};
        } else if (packet.id == 'bet_closed') {
            betClosed = packet.betClosed;
        } else if (packet.id == 'game_bet') {
            if (packet.accountId == myAccountId) {
                addMyBetToBoard(packet.number, packet.amount);
            } else {
                const e = document.querySelector('.horizontalBetBoard [data-bet="' + packet.number + '"]');
                let span = e.querySelector('span.tooltiptext');
                if (!span) {
                    span = document.createElement('span');
                    span.classList.add('tooltiptext');
                    e.appendChild(span);
                }

                if (!otherPlayersBets[packet.number]) {
                    otherPlayersBets[packet.number] = {};
                }

                const otherPlayerOnNumber = otherPlayersBets[packet.number];
                if (!otherPlayerOnNumber[packet.login]) {
                    otherPlayerOnNumber[packet.login] = 0;
                }

                otherPlayerOnNumber[packet.login] += +packet.amount;

                updateTooltipSpanByData(span, otherPlayerOnNumber);
            }
        } else if (packet.id == 'game_result_winners') {
            roomClients = packet.roomClientsCount;

            packet.winners.sort((a, b) => {
                if (a.win > b.win) {
                    return -1;
                }

                if (a.win < b.win) {
                    return 1;
                }
                return 0;
            });

            setTimeout(function () {
                const winners = document.querySelector('.currentWinners');

                for (let i = 0; i < packet.winners.length; i++) {
                    const winner = packet.winners[i];

                    if (winner.twitchId == myTwitchId) {
                        addToBalance(winner.win);

                        playAudio(audioAssets.win);

                        document.getElementById('mainDisplay').classList.add('grayscale-full');
                        document.querySelector('.myWin').classList.remove('d-none');
                        document.querySelector('.myWin span').innerText = winner.win.toLocaleString();

                        setTimeout(function () {
                            document.getElementById('mainDisplay').classList.remove('grayscale-full');
                            document.querySelector('.myWin').classList.add('d-none');
                        }, 2500);
                    }

                    const div = document.createElement('div');
                    div.innerText = `${i + 1}. ${winner.login} ${winner.win.toLocaleString()}`;

                    winners.appendChild(div);
                }

                setTimeout(clearWinners, 10000);
            }, timer);
        } else if (packet.id == 'game_update_balance') {
            updateBalance(packet.balance);
        } else if (packet.id == 'game_chat_msg') {
            processMessagePacket(packet);
            scrollToEndChat();
        }
    };

    webSocketClient.onclose = function (event) {
        clearInterval(keepAliveWebSocketInterval);

        if (event.code == 1006) {
            initWebsocketConnection();
            return;
        }

        if (event.code == 3001) {
            document.getElementById('mainDisplay').classList.add('d-none');
            const error = document.getElementById('errorDisplay');
            error.classList.remove('d-none');
            error.querySelector('h1').innerText = 'Detected new connection';
        }
    };
}

function rouletteSpin(randomNumber) {
    color = null;
    $inner.attr('data-spinto', randomNumber).find('li:nth-child(' + randomNumber + ') input').prop('checked', 'checked');

    $('.placeholder').remove();

    playAudio(audioAssets.ball);

    setTimeout(function () {
        if ($.inArray(randomNumber, red) !== -1) { color = 'red'; } else { color = 'black'; };
        if (randomNumber == 0) { color = 'green'; };

        $('.result-number').text(randomNumber);
        $('.result-color').text(color);
        $('.result').css({ 'background-color': '' + color + '' });
        $data.addClass('reveal');
        $inner.addClass('rest');

        addLastResult(randomNumber);

        setTimeout(reset, 1000);
    }, timer);
}

function reset() {
    $inner.attr('data-spinto', '').removeClass('rest');
    $data.removeClass('reveal');
}

function updateTime() {
    const total = document.getElementById('totalClientsSpan');
    const room = document.getElementById('yourClientsSpan');

    total.innerText = totalClients;
    room.innerText = roomClients;

    const nonceSpan = document.getElementById('spinNonceSpan');
    nonceSpan.innerText = spinNonce ? spinNonce : '-';

    const currentTime = Date.now();

    const element = document.getElementById('spinTimeData');
    const delta = nextSpinTime - (currentTime / 1000);
    if (delta < 0) {
        element.innerText = 'Wait for next round';
    } else {
        element.innerText = `Place your bets: ${Math.round(delta)} seconds`;
    }

    document.querySelectorAll('span[data-time]').forEach(x => {
        updateSpanTimestamp(x, currentTime);
    });
}

function clearWinners() {
    document.querySelector('.currentWinners').innerHTML = '';
}

function clearBets() {
    otherPlayersBets = {};

    document.querySelectorAll('.coinOnNumber').forEach(x => {
        x.remove();
    });

    document.querySelectorAll('span.tooltiptext').forEach(x => {
        x.remove();
    })
}

function tryPlaceBet(betNumber, betValue) {
    const balance = +(document.getElementById('balanceSpan').dataset.value ?? 0);

    if (betValue > balance) {
        return false;
    }

    const e = document.querySelector('.horizontalBetBoard [data-bet="' + betNumber + '"]');
    const currentCoin = e.querySelector('.coinOnNumber');
    if (currentCoin) {
        const final = +currentCoin.dataset.value + betValue;
        if (betNumber != 'black' && betNumber != 'red' && final > 10000) {
            return false;
        }

        if ((betNumber == 'black' || betNumber == 'red') && final > 100000) {
            return false;
        }
    }

    let totalBets = betValue;
    document.querySelectorAll('.coinOnNumber').forEach(x => {
        totalBets = totalBets + +x.dataset.value;
    });

    if (totalBets > 200000) {
        return false;
    }

    addToBalance(-betValue);

    webSocketClient.send(JSON.stringify({
        $type: 'placeBet',
        number: betNumber,
        amount: betValue
    }));

    return true;
}

function onPlaceBet(e) {
    if (!selectedCoin) {
        return;
    }

    if (betClosed) {
        return;
    }

    const betNumber = e.dataset.bet;
    const betValue = +selectedCoin.dataset.bet;

    if (tryPlaceBet(betNumber, betValue)) {
        playAudio(audioAssets.click, 0.2);
    }
}
function repeatBets() {
    const keys = Object.keys(lastRoundBets);
    if (keys.length > 0) {
        playAudio(audioAssets.click);
    }
    for (let i = 0; i < keys.length; i++) {
        const betNumber = keys[i];
        const betValue = lastRoundBets[betNumber];

        tryPlaceBet(betNumber, betValue);
    }
    lastRoundBets = {};
}

function onChipChange(e) {
    const oldChip = document.querySelector('.coin.selected');
    if (oldChip) {
        oldChip.classList.remove('selected');
    }

    e.classList.add('selected');
    selectedCoin = e;
}

function sendChatMessage() {
    if (!webSocketClient) {
        return;
    }

    const input = document.querySelector('.chat-input input');
    const message = input.value;
    input.value = '';

    webSocketClient.send(JSON.stringify({
        $type: 'chatMsg',
        msg: message
    }));
}

async function getCurrentBalance() {
    try {
        const response = await fetch(getWalletUrl, {
            method: 'POST',
            mode: 'cors',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json',
                'X-Csrf-Token-Value': csrfToken
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
            body: JSON.stringify({
                id: walletId
            })
        });

        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();

        if (!json.error) {
            return json;
        } else {
            console.error(json.error);
        }
    } catch (error) {
        console.error(error);
    }

    return null;
}

(async function () {
    const searchParams = new URLSearchParams(window.location.search);
    walletId = searchParams.get('wallet');

    var walletInfo = await getCurrentBalance(walletId);
    if (!walletInfo)
        return;

    const horizontal = document.querySelector('.horizontalBetBoard');
    for (let i = 0; i < 37; i++) {
        const div = document.createElement('div');
        div.innerText = `${i}`;
        div.style.gridArea = `n${i}`;
        if (i == 0) {
            div.style.backgroundColor = 'green';
            div.style.height = '150px';
        } else {
            const found = red.find((element) => element == i);
            div.style.backgroundColor = found ? 'red' : 'black';
        }
        div.dataset.bet = i;
        div.classList.add('horizontalBetNumber', 'no-select');
        div.addEventListener('click', function () {
            onPlaceBet(div);
        });
        horizontal.appendChild(div);
    }

    const special = ['red', 'black'];
    for (let i = 0; i < special.length; i++) {
        const type = special[i];

        const div = document.createElement('div');
        div.innerText = type.toUpperCase();
        div.style.gridArea = type;
        div.classList.add('horizontalBetNumber', 'no-select');
        div.style.height = '70px';
        div.style.backgroundColor = type;
        div.style.gridArea = type;
        div.dataset.bet = type;

        div.addEventListener('click', function () {
            onPlaceBet(div);
        });
        horizontal.appendChild(div);
    }

    document.querySelectorAll('.coin').forEach(x => {
        x.addEventListener('click', function () {
            onChipChange(x);
        });
    });

    setInterval(updateTime, 250);

    initWebsocketConnection();
})();