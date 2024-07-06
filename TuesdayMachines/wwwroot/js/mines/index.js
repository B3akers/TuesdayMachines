var gameStarted = false;
var openedSafeSquares = 0;
var betAmount = 0;
var walletId;
var walletName;
var currentTimeoutWinnings;
var gameStarting = false;

const betsAllowedValues = [
    5,
    10,
    25,
    50,
    100,
    150,
    250,
    500,
    1000,
    1500,
    2000,
    5000,
    10000,
    15000,
    20000
];

function modifyBetAmount(cursor) {
    const element = document.getElementById('bet-amount');
    let index = betsAllowedValues.findIndex((value) => value == element.value);
    if (index == -1) {
        return;
    }

    index = index + cursor;
    if (index < 0 || index >= betsAllowedValues.length) {
        return;
    }

    element.value = betsAllowedValues[index];
    betAmount = +element.value;
}

function getPostOptions(data) {
    return {
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
        body: JSON.stringify(data)
    };
}

async function showImage(event) {
    const square = event.target;
    const index = Array.from(square.parentNode.children).indexOf(square);
    const clickSound = document.getElementById('click-sound');
    const bombSound = document.getElementById('bomb-sound');

    try {
        const response = await fetch(playGameUrl, getPostOptions({
            $type: 'reveal_tile',
            index: index
        }));
        const data = await response.json();

        if (data.error) {
            throw new Error(data.error);
        }

        if (data.isMine) {
            revealAll(data.mines, data.picked);
            square.style.backgroundImage = "url('../assets/mines/bomb.png')"; // Bomb
            bombSound.volume = 0.1;
            bombSound.play();
            displayResult('lost');
            document.getElementById('bet-button').innerText = 'Bet';
            document.getElementById('bet-button').onclick = startGame;
            gameStarted = false;
        } else {
            square.style.backgroundImage = "url('../assets/mines/coin.png')"; // Safe
            clickSound.volume = 0.2;
            clickSound.play();
            openedSafeSquares++;
            square.classList.add('clicked');
            square.removeEventListener('click', showImage);

            checkAllSafeSquaresOpened(data);
        }

        square.style.backgroundSize = 'cover';
    } catch (error) {
        console.error('Error checking square:', error);
    }
}

function createGrid() {
    const grid = document.getElementById('grid');
    grid.innerHTML = '';

    for (let i = 0; i < 25; i++) {
        const square = document.createElement('div');
        square.classList.add('grid-square');
        square.addEventListener('click', showImage);
        grid.appendChild(square);
    }
}

function revealAll(mines, picked) {
    try {
        const squares = document.querySelectorAll('.grid-square');
        squares.forEach((square, index) => {
            if (!picked.includes(index)) {
                if (mines.includes(index)) {
                    square.style.backgroundImage = "url('../assets/mines/bomb.png')"; // Bomb
                } else {
                    square.style.backgroundImage = "url('../assets/mines/coin.png')"; // Safe
                    square.classList.add('darker');
                }
                square.style.backgroundSize = 'cover';
                square.removeEventListener('click', showImage);
            }
        });
    } catch (error) {
        console.error('Error fetching game result:', error);
    }
}
async function getCurrentBalance() {
    try {
        const response = await fetch(getWalletUrl, getPostOptions({
            id: walletId
        }));

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

async function startGame() {
    if (gameStarting) {
        return;
    }

    gameStarting = true;

    if (!gameStarted) {
        try {
            const betAmount = parseFloat(document.getElementById('bet-amount').value);
            const mines = parseInt(document.getElementById('mines').value);
            const response = await fetch(playGameUrl, getPostOptions({
                $type: 'start_game',
                wallet: walletId,
                bet: betAmount,
                mines: mines
            }));

            const data = await response.json();
            if (data.error) {
                alert(data.error);
                throw new Error(data.error);
            }

            addToBalance(-betAmount);
            setBalance(data.balance);

            gameStarted = true;
            openedSafeSquares = 0;
            const result = document.getElementById('result');
            result.style.display = 'none';
            document.getElementById('bet-button').innerText = 'Cashout';
            document.getElementById('bet-button').onclick = cashout;
            document.getElementById('bet-button').disabled = false;
            document.getElementById('message').style.display = 'none';
            createGrid();
        } catch (error) {
            console.error('Error starting game:', error);
        }
    }

    gameStarting = false;
}

window.addEventListener('keydown', function (e) {
    if (e.keyCode == 32) {
        startGame();
        e.preventDefault();
    }
});

async function cashout() {
    if (gameStarting || openedSafeSquares == 0) {
        return;
    }

    gameStarting = true;

    if (gameStarted) {
        try {
            const response = await fetch(playGameUrl, getPostOptions({
                $type: 'cashout_game'
            }));

            const data = await response.json();
            if (data.error) {
                throw new Error(data.error);
            }

            const winnings = data.winnings;
            const multiplier = data.multiplier;

            displayResult('cashout', winnings, multiplier, data.balance);
            document.getElementById('bet-button').innerText = 'Bet';
            document.getElementById('bet-button').onclick = startGame;
            revealAll(data.mines, data.picked);

            gameStarted = false;
        } catch (error) {
            console.error('Error cashing out:', error);
        }
    }

    gameStarting = false;
}

function setBalance(value) {
    const balance = document.getElementById('currentBalacne');
    balance.dataset.value = value;
    balance.innerText = value.toLocaleString();
}
function addToBalance(value) {
    const balance = document.getElementById('currentBalacne');
    const newValue = +(balance.dataset.value ?? 0) + +value;
    balance.dataset.value = newValue;
    balance.innerText = newValue.toLocaleString();

    const session = document.getElementById('currentSession');
    let sessionValue = +(session.dataset.value ?? 0);
    sessionValue = sessionValue + +value;

    if (sessionValue == 0) {
        session.style.color = 'white';
    } else {
        session.style.color = sessionValue > 0 ? 'green' : 'red';
    }

    session.dataset.value = sessionValue;
    session.innerText = sessionValue.toLocaleString();
}

async function loadPage() {
    const grid = document.getElementById('grid');
    grid.innerHTML = '';

    for (let i = 0; i < 25; i++) {
        const square = document.createElement('div');
        square.classList.add('grid-square');
        grid.appendChild(square);
    }

    const searchParams = new URLSearchParams(window.location.search);
    walletId = searchParams.get('wallet');

    var walletInfo = await getCurrentBalance(walletId);
    if (!walletInfo)
        return;

    walletName = walletInfo.name;
    setBalance(walletInfo.balance);

    document.querySelectorAll('.pointsName').forEach(x => {
        x.innerText = `[${walletName}]`;
    });

    await fetchGameData();
}

function displayResult(type, winnings = 0, multiplier = 0, balance = 0) {
    const result = document.getElementById('result');
    if (type === 'lost') {
        result.innerText = 'You lost!';
    } else if (type === 'cashout') {
        addToBalance(winnings);
        setBalance(balance);
        result.innerText = `Cashout! Multiplier: ${multiplier.toFixed(2)}x, Winnings: ${winnings}`;
    }
    result.style.display = 'block';
    if (currentTimeoutWinnings) {
        clearTimeout(currentTimeoutWinnings);
    }

    currentTimeoutWinnings = setTimeout(() => {
        result.style.display = 'none';
    }, 3000);
}

function checkAllSafeSquaresOpened(data) {
    const squares = document.querySelectorAll('.grid-square');
    let totalSafeSquares = 0;
    let clickedSafeSquares = 0;

    squares.forEach(square => {
        if (!square.classList.contains('mine')) {
            totalSafeSquares++;
            if (square.classList.contains('clicked')) {
                clickedSafeSquares++;
            }
        }
    });

    if (clickedSafeSquares === totalSafeSquares || data.winnings) {
        const message = document.getElementById('message');
        message.style.display = 'block';
        gameStarted = false;
        document.getElementById('bet-button').innerText = 'Bet';
        document.getElementById('bet-button').onclick = startGame;

        displayResult('cashout', data.winnings, data.multiplier, data.balance);
    }
}

async function fetchGameData() {
    try {
        const response = await fetch(playGameUrl, getPostOptions({
            $type: 'base'
        }));
        const data = await response.json();

        if (data.game) {
            const { mines, bet, picked } = data.game;
            betAmount = bet;

            document.getElementById('mines').value = mines;
            document.getElementById('bet-amount').value = bet;

            openedSafeSquares = picked.length;
            multiplier = data.multiplier;
            gameStarted = true;
            document.getElementById('bet-button').innerText = 'Cashout';
            document.getElementById('bet-button').onclick = cashout;
            document.getElementById('bet-button').disabled = false;

            const result = document.getElementById('result');
            result.style.display = 'none';
            document.getElementById('message').style.display = 'none';
            createGrid();

            const squares = document.querySelectorAll('.grid-square');

            picked.forEach(position => {
                squares[position].classList.add('clicked');
                squares[position].style.backgroundImage = "url('../assets/mines/coin.png')";
                squares[position].style.backgroundSize = 'cover';
                squares[position].removeEventListener('click', showImage);
            });
        }
    } catch (error) {
        console.error('Error fetching game data:', error);
    }
}

window.onload = loadPage;