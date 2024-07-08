const reelsData = {
    baseGame: [
        [1, 1, 1, 5, 5, 6, 6, 4, 4, 8, 8, 3, 3, 2, 2, 2, 8, 7, 7, 7, 1, 9, 9, 9, 10, 10, 1, 5, 5, 5, 4, 0, 4, 4, 2, 2, 8, 1, 4, 4, 4, 7, 9, 10, 3, 3, 3, 6, 6, 6, 9, 9, 9, 1, 2, 3, 4, 0, 5, 6, 7, 8, 9, 10, 1, 5, 2, 8, 7, 3, 5, 6],
        [10, 2, 2, 2, 4, 7, 7, 2, 8, 8, 6, 6, 2, 3, 8, 8, 0, 9, 9, 9, 3, 3, 3, 1, 2, 0, 7, 7, 7, 7, 5, 2, 9, 10, 10, 7, 4, 4, 1, 1, 1, 9, 5, 5, 1, 5, 5, 8, 8, 8, 2, 2, 2, 5, 5, 2, 9, 9, 10, 7, 7, 7, 2, 2, 2, 7, 7, 7, 1, 1, 1],
        [7, 8, 8, 2, 0, 2, 2, 6, 3, 1, 9, 10, 10, 10, 6, 7, 7, 7, 5, 5, 5, 9, 1, 1, 8, 4, 2, 2, 7, 7, 1, 1, 4, 9, 4, 10, 8, 1, 1, 3, 9, 9, 9, 1, 1, 1, 0, 6, 6, 8, 8, 10, 10, 10, 6, 6, 6, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
        [2, 2, 5, 5, 4, 4, 4, 7, 2, 1, 1, 5, 7, 7, 9, 10, 6, 6, 6, 7, 7, 0, 7, 7, 4, 4, 8, 10, 10, 10, 2, 1, 9, 5, 10, 8, 2, 5, 5, 8, 4, 8, 8, 4, 9, 9, 9, 3, 1, 2, 2, 2, 8, 8, 6, 6, 6, 1, 1, 1],
        [6, 6, 1, 1, 1, 0, 8, 8, 8, 10, 10, 9, 4, 4, 4, 7, 7, 7, 1, 9, 2, 2, 6, 4, 10, 10, 10, 4, 2, 3, 3, 8, 1, 6, 6, 7, 9, 9, 9, 2, 2, 5, 5, 10, 10, 9, 9, 9, 5, 5, 5, 7, 7, 7, 1, 2, 0, 3, 4, 5, 6, 7, 8, 9, 10]
    ],
    trainBonus: [
        [1, 1, 1, 6, 6, 4, 4, 8, 3, 7, 1, 2, 10, 8, 3, 3, 2, 2, 8, 7, 1, 9, 9, 9, 10, 1, 4, 4, 4, 5, 10, 5, 5, 4, 8, 4, 4, 2, 2, 8, 1, 7, 9, 10, 3, 6, 6, 6, 9, 9, 9, 7, 1, 5, 2, 6],
        [10, 2, 4, 7, 7, 2, 8, 8, 6, 6, 2, 3, 7, 1, 8, 9, 3, 3, 3, 1, 2, 6, 6, 7, 5, 2, 9, 10, 8, 8, 8, 7, 4, 4, 1, 1, 1, 9, 5, 7, 5, 1, 8, 2, 2, 2, 5, 5, 2, 9, 9, 10, 7, 2, 2, 2, 7, 7, 7, 1, 1, 1, 10, 2, 4, 7],
        [7, 7, 2, 6, 3, 1, 9, 10, 6, 7, 7, 7, 5, 5, 9, 9, 9, 1, 1, 8, 4, 2, 2, 7, 7, 1, 1, 4, 9, 4, 10, 8, 1, 1, 3, 9, 9, 7, 4, 5, 9, 1, 1, 1, 6, 6, 8, 8, 10, 6, 6, 6],
        [2, 2, 5, 8, 5, 4, 9, 9, 9, 4, 8, 7, 2, 1, 1, 5, 7, 7, 9, 10, 6, 3, 5, 1, 6, 4, 4, 8, 10, 2, 1, 9, 5, 10, 8, 2, 6, 5, 5, 8, 4, 8, 8, 8, 4, 9, 9, 3, 1, 2, 2, 10, 2, 8, 8, 8, 6, 6, 1, 1, 1],
        [10, 6, 6, 1, 8, 8, 8, 10, 9, 4, 4, 4, 7, 7, 1, 9, 2, 2, 6, 4, 10, 4, 2, 3, 3, 8, 1, 6, 6, 7, 9, 9, 9, 2, 2, 5, 5, 10, 9, 9, 9, 5, 4, 1, 3, 5, 7, 1, 6, 3, 4, 7]
    ],
    duelBonus: [
        [5, 5, 5, 5, 5, 4, 9, 8, 8, 7, 7, 7, 6, 1, 2, 10, 2, 2, 2, 7, 7, 7, 7, 1, 1, 4, 3, 9, 9, 4, 8, 1, 5, 7, 6, 4, 4, 2, 2, 1, 1, 1, 7, 7, 7, 8, 8, 8, 1, 3, 9, 10, 2, 2, 1, 5, 4, 8, 7, 6, 9, 9, 2],
        [1, 1, 1, 1, 2, 10, 9, 2, 8, 3, 7, 7, 7, 0, 6, 4, 4, 4, 4, 8, 8, 8, 8, 8, 8, 3, 3, 1, 5, 9, 2, 6, 6, 6, 4, 7, 2, 8, 1, 5, 9, 9, 4, 5, 6, 7, 0, 3, 3, 3, 9, 10, 3, 3, 2, 1, 10, 7, 7, 4, 3, 3, 3, 6, 6, 3, 6, 6, 2, 10],
        [6, 6, 6, 6, 6, 2, 3, 7, 1, 1, 5, 5, 0, 5, 10, 9, 7, 7, 6, 10, 6, 9, 9, 4, 4, 8, 7, 10, 6, 7, 9, 3, 3, 2, 1, 2, 4, 4, 4, 9, 8, 5, 7, 3, 3, 3, 8, 3, 6, 7, 4, 4, 4, 9, 10, 2, 2, 2, 2, 5],
        [7, 7, 7, 7, 7, 9, 5, 5, 9, 3, 3, 8, 8, 7, 7, 9, 10, 1, 1, 1, 0, 3, 3, 3, 9, 10, 10, 6, 6, 6, 4, 4, 4, 3, 3, 3, 2, 0, 1, 5, 5, 8, 6, 2, 2, 4, 10, 2, 8, 1, 7, 6, 6, 3, 9, 9, 9, 9, 9, 6, 6, 6, 6, 6, 2, 5, 4, 3, 2, 1],
        [10, 10, 5, 2, 7, 10, 10, 7, 0, 0, 8, 5, 5, 5, 5, 5, 5, 8, 6, 6, 1, 1, 1, 1, 1, 3, 2, 2, 2, 2, 2, 8, 8, 8, 8, 8, 6, 6, 1, 7, 5, 4, 4, 4, 4, 9, 9, 9, 9, 9, 7, 2, 1, 9, 5, 8, 6, 6, 6, 3, 2, 9, 10, 8, 3, 4, 3, 4]
    ]
};

const gameRect = {
    width: 1792,
    height: 1024
};

const gameBoardBasePosition = {
    x: 512,
    y: 50
};

const gameSymbolsBoardBasePosition = {
    x: gameBoardBasePosition.x + 60,
    y: gameBoardBasePosition.y + 75
};

const gameSymbolsBoardSize = {
    x: 660,
    y: 660
};

const gameSymbolSize = {
    x: 132,
    y: 132
};

const reelSpeed = 20;
const nextReelStopTime = 10;

const Application = PIXI.Application;
const Assets = PIXI.Assets;
const Sprite = PIXI.Sprite;
const Rectangle = PIXI.Rectangle;
const Container = PIXI.Container;
const Graphics = PIXI.Graphics;
const Spritesheet = PIXI.Spritesheet;
const Texture = PIXI.Texture;
const BitmapText = PIXI.BitmapText;

const gameAssets = {
    background: {
        path: '../assets/3xduel/background.webp'
    },
    board: {
        path: '../assets/3xduel/board.webp'
    },
    wild: {
        path: '../assets/3xduel/wild.webp'
    },
    ten: {
        path: '../assets/3xduel/ten.webp'
    },
    j: {
        path: '../assets/3xduel/j.webp'
    },
    q: {
        path: '../assets/3xduel/q.webp'
    },
    k: {
        path: '../assets/3xduel/k.webp'
    },
    a: {
        path: '../assets/3xduel/a.webp'
    },
    bull: {
        path: '../assets/3xduel/bull.webp'
    },
    bandit: {
        path: '../assets/3xduel/bandit.webp'
    },
    skull: {
        path: '../assets/3xduel/skull.webp'
    },
    whisky: {
        path: '../assets/3xduel/whisky.webp'
    },
    revolver: {
        path: '../assets/3xduel/revolver.webp'
    },
    vs: {
        path: '../assets/3xduel/vs.webp'
    },
    duel: {
        path: '../assets/3xduel/duel.webp'
    },
    train: {
        path: '../assets/3xduel/train.webp'
    }
};

const symbolsTextureArray = [
    gameAssets.wild,
    gameAssets.ten,
    gameAssets.j,
    gameAssets.q,
    gameAssets.k,
    gameAssets.a,
    gameAssets.bull,
    gameAssets.bandit,
    gameAssets.skull,
    gameAssets.whisky,
    gameAssets.revolver,
    gameAssets.vs,
    gameAssets.duel,
    gameAssets.train
];

const gameBasePosition = {
    x: 0,
    y: 0
};

var isPlaying;
var isFastSpinEnabled = false;
var isFastSpinOnce = false;

var replayData;

var walletId;
var walletName;

function isFastSpins() {
    if (isFastSpinEnabled) {
        return true;
    }

    if (isFastSpinOnce) {
        isFastSpinOnce = false;
        return true;
    }

    return false;
}

class ReelInfo {
    constructor(reelData, index) {
        this.index = index;
        this.startIndex = 0;
        this.currentIndex = 0;
        this.replacements = [];
        this.hasSetReel = false;
        this.isMoving = false;
        this.data = reelData;
    }

    setStopIndex(index, replacements = null) {
        this.startIndex = index;
        this.currentIndex = this.startIndex;
        this.isMoving = true;
        this.hasSetReel = true;

        if (replacements) {
            this.replacements = replacements;
        }
    }

    startMoving() {
        this.isMoving = true;
        this.hasSetReel = false;
        this.currentIndex = 0;
        this.startIndex = 0;
    }

    stopMoving() {
        this.isMoving = false;
    }

    getTextureRealIndex(index, position = -1) {
        while (index >= this.data.length || index < 0) {
            if (index >= this.data.length) {
                index = index - this.data.length;
            } else if (index < 0) {
                index = this.data.length + index;
            }
        }

        if (position >= 0) {
            for (let i = 0; i < this.replacements.length; i++) {
                const replacement = this.replacements[i];
                if (replacement.index == position) {
                    return replacement.symbol;
                }
            }
        }

        return this.data[index];
    }

    getNextTextureIndex() {
        this.currentIndex++;
        if (this.currentIndex >= this.data.length) {
            this.currentIndex = 0;
        }

        return this.data[this.currentIndex];
    }
}

class SpinGameInfo {
    constructor() {
        this.spinData = null;
        this.spinDataIndex = 0;
        this.currentSpinStage = 0;
        this.currentStopReel = 0;
        this.lastStopReel = 0;
        this.elapsedCountTimeSpinWin = 0;
        this.lastUpdateWinText = 0;
    }

    allReelsStopped() {
        if (this.currentStopReel < 5) {
            return false;
        }

        for (let i = 0; i < 5; i++) {
            if (boardReelsInfo[i].isMoving) {
                return false;
            }
        }

        return true;
    }

    stopAllReels() {
        if (this.currentStopReel >= 5) {
            return 5;
        }

        const spin = this.spinData.spins[this.spinDataIndex];

        for (let i = this.currentStopReel; i < 5; i++) {
            boardReelsInfo[i].setStopIndex(spin.stops[i], spin.replacements);
        }

        let backup = this.currentStopReel;
        this.currentStopReel = 5;
        return backup;
    }

    tryStopNextReel(deltaTime) {
        if (this.currentStopReel >= 5) {
            return false;
        }

        this.lastStopReel += deltaTime;
        if (this.lastStopReel > nextReelStopTime) {
            const spin = this.spinData.spins[this.spinDataIndex];

            boardReelsInfo[this.currentStopReel].setStopIndex(spin.stops[this.currentStopReel], spin.replacements);

            this.currentStopReel++;
            this.lastStopReel = 0;

            return true;
        }

        return false;
    }

    nextSpin() {
        this.lastStopReel = 0;
        this.currentStopReel = 0;
        this.elapsedCountTimeSpinWin = 0;
        this.lastUpdateWinText = 0;
        this.spinDataIndex++;
        isFastSpinOnce = false;
    }
};

const gameSpinInfo = new SpinGameInfo();
const boardReelsInfo = [];
const boardGems = [];
const objectsToScale = [];
var boardContainer;

for (let i = 0; i < 5; i++) {
    boardReelsInfo.push(new ReelInfo(reelsData.baseGame[i], i));
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

function disableAutoPlay() {

}

function canStartNewGame() {
    if (replayData) {
        return false;
    }

    if (isPlaying) {
        return false;
    }

    if (gameSpinInfo.spinData && gameSpinInfo.spinDataIndex < gameSpinInfo.spinData.spins.length) {
        return false;
    }

    if (gameSpinInfo.currentSpinStage > 0) {
        return false;
    }

    return true;
}

async function playGame() {
    if (!canStartNewGame()) {
        return;
    }

    isPlaying = true;

    for (let i = 0; i < boardReelsInfo.length; i++) {
        const boardReel = boardReelsInfo[i];
        boardReel.data = reelsData.baseGame[i];
        boardReel.startMoving();
    }

    //const element = document.getElementById('betValueSpan');
    //let index = betsAllowedValues.findIndex((value) => value == element.dataset.value);
    //if (index == -1) {
    //    isPlaying = false;
    //    return;
    //}
    //
    //const betValue = betsAllowedValues[index];

    const betValue = 20;

    try {
        const response = await fetch(playGameUrl, {
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
                bet: betValue,
                wallet: walletId
            })
        });

        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        const json = await response.json();

        if (json.error) {
            for (let i = 0; i < boardReelsInfo.length; i++) {
                const boardReel = boardReelsInfo[i];
                boardReel.data = reelsData.baseGame[i];
                boardReel.setStopIndex(0);
            }
            disableAutoPlay();
            alert(json.error);
            console.error(json.error);
            isPlaying = false;
            return;
        }

        gameSpinInfo.spinData = json;
        gameSpinInfo.spinDataIndex = 0;
        isPlaying = false;
    } catch (error) {
        isPlaying = false;
        console.error(error);
    }

}

function rescaleObjects() {
    const ratio = gameRect.width / gameRect.height;

    //Try scale with width
    //

    let width = window.innerWidth;
    let height = window.innerWidth / ratio;
    if (height > window.innerHeight) { //We cant scale with width, try with height
        height = window.innerHeight;
        width = window.innerHeight * ratio;
    }

    gameBasePosition.x = (window.innerWidth - width) / 2;
    gameBasePosition.y = (window.innerHeight - height) / 2;

    const scaleX = width / gameRect.width;
    const scaleY = height / gameRect.height;

    for (let i = 0; i < objectsToScale.length; i++) {
        const obj = objectsToScale[i];
        obj.sprite.position.set(gameBasePosition.x + (obj.basePosition.x * scaleX), gameBasePosition.y + (obj.basePosition.y * scaleY));
        obj.sprite.scale.set(scaleX, scaleY);
    }

    boardContainer.mask = new Graphics()
        .rect(gameBasePosition.x + (gameSymbolsBoardBasePosition.x * scaleX), gameBasePosition.y + (gameSymbolsBoardBasePosition.y * scaleY), gameSymbolsBoardSize.x * scaleX, gameSymbolsBoardSize.y * scaleY)
        .fill(0xffffff);
}

window.addEventListener('resize', function () {
    rescaleObjects();
});

(async function () {
    const searchParams = new URLSearchParams(window.location.search);
    walletId = searchParams.get('wallet');
    const replayId = searchParams.get('replayId');

    if (replayId) {
        walletName = "";
        walletInfo = {
            name: '',
            balance: 0
        }

        replayData = await getReplaySpinData(replayId);
        if (!replayData) {
            return;
        }
    } else {
        if (!walletId) {
            return;
        }

        var walletInfo = await getCurrentBalance(walletId);
        if (!walletInfo)
            return;

        walletName = walletInfo.name;
    }

    const app = new Application();
    await app.init({ resizeTo: window });

    app.canvas.style.position = 'absolute';

    document.body.appendChild(app.canvas);

    {
        const assets = Object.keys(gameAssets);
        for (let i = 0; i < assets.length; i++) {
            const asset = gameAssets[assets[i]];
            const texture = await Assets.load(asset.path);

            asset.texture = texture;
        }
    }

    {
        const background = new Sprite(gameAssets.background.texture);

        objectsToScale.push({ sprite: background, basePosition: { x: 0, y: 0 } });
        app.stage.addChild(background);
    }

    {
        const board = new Sprite(gameAssets.board.texture);

        objectsToScale.push({ sprite: board, basePosition: gameBoardBasePosition });
        app.stage.addChild(board);
    }

    {
        boardContainer = new Container();
        boardContainer.position.set(gameSymbolsBoardBasePosition.x, gameSymbolsBoardBasePosition.y);
        boardContainer.mask = new Graphics()
            .rect(0, 0, 0, 0)
            .fill(0xffffff);

        for (let i = 0; i < 5; i++) {
            for (let j = 0; j < 6; j++) {
                const symbol = new Sprite(symbolsTextureArray[boardReelsInfo[i].getTextureRealIndex(j)].texture);
                symbol.width = gameSymbolSize.x;
                symbol.height = gameSymbolSize.y;
                symbol.position.set(i * gameSymbolSize.x, j * gameSymbolSize.y - gameSymbolSize.y);

                boardContainer.addChild(symbol);
                boardGems.push({
                    sprite: symbol
                });
            }
        }

        objectsToScale.push({ sprite: boardContainer, basePosition: { x: gameSymbolsBoardBasePosition.x, y: gameSymbolsBoardBasePosition.y } });
        app.stage.addChild(boardContainer);
    }

    rescaleObjects();

    app.ticker.add((ticker) => {

        if (gameSpinInfo.spinData && gameSpinInfo.spinDataIndex < gameSpinInfo.spinData.spins.length) {
            const event = gameSpinInfo.spinData.spins[gameSpinInfo.spinDataIndex];

            if (gameSpinInfo.currentSpinStage == 0) {

                gameSpinInfo.currentSpinStage = 1;
                gameSpinInfo.reelsSpinning = [];

            } else if (gameSpinInfo.currentSpinStage == 1) {
                let i = gameSpinInfo.reelsSpinning.length;
                while (i--) {
                    const reel = gameSpinInfo.reelsSpinning[i];
                    if (!reel.isMoving) {
                        gameSpinInfo.reelsSpinning.splice(i, 1);
                    }
                }

                if (gameSpinInfo.allReelsStopped()) {
                    if (gameSpinInfo.reelsSpinning.length == 0) {
                        gameSpinInfo.currentSpinStage = 2;
                    }
                } else {
                    if (isFastSpins()) {
                        const lastReel = gameSpinInfo.stopAllReels();
                        for (let i = lastReel; i < 5; i++) {
                            gameSpinInfo.reelsSpinning.push(boardReelsInfo[i]);
                        }
                    } else {
                        if (gameSpinInfo.tryStopNextReel(ticker.deltaTime)) {
                            gameSpinInfo.reelsSpinning.push(boardReelsInfo[gameSpinInfo.currentStopReel - 1]);
                        }
                    }
                }
            } else if (gameSpinInfo.currentSpinStage == 2) {
                gameSpinInfo.nextSpin();
                gameSpinInfo.currentSpinStage = 0;
            }
        }

        const rowSize = gameSymbolSize.y * 5
        const moveValue = reelSpeed * ticker.deltaTime;
        for (let j = 0; j < 5; j++) {
            const rowIndex = j * 6;
            const reelInfo = boardReelsInfo[j];
            if (!reelInfo.isMoving) {
                continue;
            }

            for (let i = 0; i < 6; i++) {
                if (!reelInfo.isMoving) {
                    continue;
                }

                const gem = boardGems[rowIndex + i];
                const sprite = gem.sprite;

                sprite.position.y += moveValue;

                if (sprite.position.y >= rowSize) {
                    sprite.position.y -= (rowSize + gameSymbolSize.y);
                    if (reelInfo.hasSetReel) {
                        const gemReelIndex = (4 - (reelInfo.currentIndex - reelInfo.startIndex));
                        if (gemReelIndex >= 0) {
                            gem.position = j * 5 + gemReelIndex;
                        } else {
                            gem.position = -1;
                        }
                        const gemTextureIndex = reelInfo.getTextureRealIndex(reelInfo.startIndex + gemReelIndex, gem.position);

                        gem.textureIndex = gemTextureIndex;

                        sprite.texture = symbolsTextureArray[gemTextureIndex].texture;
                        reelInfo.currentIndex++;

                        //Check if we already set reel
                        //
                        if (reelInfo.currentIndex - reelInfo.startIndex >= 6) {
                            //Fix position for reels
                            //
                            for (let y = 0; y < 6; y++) {
                                let realIndex = i + y;
                                if (realIndex >= 6) {
                                    realIndex = realIndex - 6;
                                }
                                const nextGem = boardGems[rowIndex + realIndex];
                                nextGem.sprite.position.set(j * gameSymbolSize.x, y * gameSymbolSize.y - gameSymbolSize.y);
                            }
                            reelInfo.stopMoving();
                        }
                    } else {
                        gem.position = -1;
                        sprite.texture = symbolsTextureArray[reelInfo.getNextTextureIndex()].texture;
                    }
                }
            }
        }
    });
})();