var walletId;
var walletName;

const Application = PIXI.Application;
const Assets = PIXI.Assets;
const Sprite = PIXI.Sprite;
const Rectangle = PIXI.Rectangle;
const Container = PIXI.Container;
const Graphics = PIXI.Graphics;
const Spritesheet = PIXI.Spritesheet;
const Texture = PIXI.Texture;
const BitmapText = PIXI.BitmapText;

var app;

var heightLength;
var widthLength;
var ballRadius;
var rowCount;
var texture;

const payoutTable = [
    1000.,
    130.,
    26.,
    9.,
    4.,
    2.,
    0.2,
    0.2,
    0.2,
    0.2,
    0.2,
    2.,
    4.,
    9.,
    26.,
    130.,
    1000.
];

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

const staticBalls = [];
const rowsPosHeight = [];
const rowsStartIndex = [];
function getRndInteger(min, max) {
    return Math.floor(Math.random() * (max - min)) + min;
}
function convertPointsToLocalCurrency(value) {
    return +value;
}

function updateCurrencySpanText(element, currency) {
    element.innerText = "[" + currency + "]" + convertPointsToLocalCurrency(element.dataset.value).toLocaleString();
}
function updateCurrencyValueText(element, currency) {
    element.value = "[" + currency + "]" + convertPointsToLocalCurrency(element.dataset.value).toLocaleString();
}

function updateBetValue(cursor) {
    const element = document.getElementById('betValueSpan');
    let index = betsAllowedValues.findIndex((value) => value == element.dataset.value);
    if (index == -1) {
        return;
    }

    index = index + cursor;
    if (index < 0 || index >= betsAllowedValues.length) {
        return;
    }

    element.dataset.value = betsAllowedValues[index];
    updateCurrencySpanText(element, walletName);
}
function onBetIncrease() {
    updateBetValue(1);
}

function onBetDecrease() {
    updateBetValue(-1);
}

function updateBalance(balance) {
    const currentBalanceText = document.getElementById('currentBalanceSpan');
    currentBalanceText.dataset.value = balance;
    updateCurrencyValueText(currentBalanceText, walletName);
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

function progressColorPara(progress) {
    const hue = 48 * (1 - progress);
    const lightness = 71 - 11 * progress;
    return `hsl(${hue},100%,${lightness}%)`;
}

const glowHitTime = 20;

class StaticBall {
    constructor(x, y) {
        this.sprite = new Sprite(texture);
        this.sprite.position.set(x, y);

        this.glowSprite = new Sprite(texture);
        this.glowSprite.position.set(x, y);

        this.x = x;
        this.y = y;

        this.playingHitAnim = false;
        this.lastHit = 0;
        app.stage.addChild(this.sprite);
    }

    hitted() {
        if (this.playingHitAnim) {
            return;
        }

        app.stage.addChild(this.glowSprite);
        this.playingHitAnim = true;
        this.lastHit = 0;
    }

    tick(deltaTime) {
        if (!this.playingHitAnim) {
            return;
        }

        this.lastHit += deltaTime;
        if (this.lastHit >= glowHitTime) {
            app.stage.removeChild(this.glowSprite);
            this.playingHitAnim = false;
            return;
        }

        let percent = 0;
        const half = glowHitTime / 2;
        if (this.lastHit < half) {
            percent = this.lastHit / half;
        } else {
            percent = 1 - ((this.lastHit - half) / half);
        }

        this.glowSprite.alpha = 0.8 * percent;
        this.glowSprite.scale.set(1 + percent, 1 + percent);
        this.glowSprite.position.set(this.x - (ballRadius * percent), this.y - (percent * ballRadius));
    }
}

var lastBallUpdatedBalanceTime = 0;

class Ball {
    constructor(data) {
        const middleBall = staticBalls[1];

        this.balance = data.currentBalance;
        this.creationTime = Date.now();
        this.index = 0;
        this.speed = getRndInteger(4, 8);
        this.bounceSpeed = getRndInteger(2, 4);
        this.data = data.path;
        this.payoutIndex = this.data.reduce((partialSum, a) => partialSum + a, 0);
        this.sprite = new Sprite(texture);
        this.sprite.tint = 0xff0000;
        this.sprite.position.set(middleBall.x, 0);
        this.currentBallTarget = 1;
        this.posXBounce = 0;

        app.stage.addChild(this.sprite);
    }

    tick(deltaTime) {
        if (this.posXBounce > 0) {
            const bounceMove = deltaTime * this.bounceSpeed;
            const direction = this.data[this.index - 1];

            this.sprite.position.y += bounceMove;

            if (direction) {
                this.sprite.position.x += bounceMove;
                if (this.sprite.position.x >= this.posXBounce) {
                    this.sprite.position.x = this.posXBounce;
                    this.posXBounce = 0;
                }
            } else {
                this.sprite.position.x -= bounceMove;
                if (this.sprite.position.x <= this.posXBounce) {
                    this.sprite.position.x = this.posXBounce;
                    this.posXBounce = 0;
                }
            }

            return false;
        }

        if (this.index >= rowsPosHeight.length) {
            const box = document.querySelector('[data-box-number="' + this.payoutIndex + '"]');
            box.classList.add('winnerBoxAnim');

            if (this.creationTime > lastBallUpdatedBalanceTime) {
                lastBallUpdatedBalanceTime = this.creationTime;

                updateBalance(this.balance);
            }

            return true;
        }

        const move = deltaTime * this.speed;
        const rowHeight = rowsPosHeight[this.index] - ballRadius;
        this.sprite.position.y += move;
        if (this.sprite.position.y >= rowHeight) {
            const direction = this.data[this.index];
            const nextBallHitIndex = this.currentBallTarget + 3 + this.index + direction;

            staticBalls[this.currentBallTarget].hitted();

            if (nextBallHitIndex < staticBalls.length) {
                this.posXBounce = staticBalls[nextBallHitIndex].x;
            } else {
                if (direction)
                    this.posXBounce = (widthLength / 2);
                else
                    this.posXBounce = -(widthLength / 2);

                this.posXBounce += this.sprite.position.x;
            }

            this.currentBallTarget = nextBallHitIndex;
            this.index++;
        }

        return false;
    }
}

const balls = [];

function changeRows(count) {
    if (count < 8) {
        count = 8;
    }
    if (count > 16) {
        count = 16;
    }

    rowCount = count;

    app.stage.removeChildren();

    for (let i = 0; i < staticBalls.length; i++) {
        staticBalls[i].sprite.destroy();
        staticBalls[i].glowSprite.destroy();
    }
    staticBalls.length = 0;
    rowsPosHeight.length = 0;
    rowsStartIndex.length = 0;

    const height = app.canvas.height;
    const width = app.canvas.width;

    heightLength = height / (count + 1);
    widthLength = (width * 0.8) / (count + 2);
    ballRadius = Math.floor(widthLength / 6);

    {
        const gr = new Graphics();
        gr.beginFill(0xFFFFFF);
        gr.lineStyle(0);
        gr.drawCircle(0, 0, ballRadius);
        gr.endFill();

        if (texture) {
            texture.destroy();
        }

        texture = app.renderer.generateTexture(gr);
    }

    const positionStartX = width * 0.1;

    for (let i = 1; i <= count; i++) {
        const posX = positionStartX + ((count - i) * widthLength / 2);
        const y = heightLength * i;

        rowsStartIndex.push(staticBalls.length);
        rowsPosHeight.push(y);
        for (let j = 0; j < (i + 2); j++) {
            const x = posX + (j * widthLength);

            staticBalls.push(new StaticBall(x, y));
        }
    }

    const winBoxes = document.getElementById('gameWinBoxes');
    winBoxes.innerHTML = '';
    winBoxes.style.width = `${Math.floor(width * 0.8) - ballRadius}px`;
    for (let i = 0; i <= count; i++) {
        const box = document.createElement('div');
        const span = document.createElement('span');
        const color = progressColorPara(Math.abs(i - count / 2) / (count / 2));
        box.classList.add('winnerBox');
        box.style.setProperty('--shadow-color', color);
        box.style.width = `${widthLength - ballRadius * 1.5}px`;
        box.style.height = box.style.width;
        box.style.background = color;
        box.dataset.boxNumber = i;
        box.addEventListener('webkitAnimationEnd', function () {
            box.classList.remove('winnerBoxAnim');
        }, false);
        span.innerText = `${payoutTable[i]}` + (payoutTable[i] < 100 ? '\xD7' : '');
        box.appendChild(span);
        winBoxes.appendChild(box);
    }
}

var isPlaying = false;
async function playGame() {
    if (isPlaying) {
        return;
    }

    isPlaying = true;

    const element = document.getElementById('betValueSpan');
    let index = betsAllowedValues.findIndex((value) => value == element.dataset.value);
    if (index == -1) {
        isPlaying = false;
        return;
    }

    const betValue = betsAllowedValues[index];
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
            alert(json.error);
            console.error(json.error);
            isPlaying = false;
            return;
        }

        const currentBalanceText = document.getElementById('currentBalanceSpan');
        currentBalanceText.dataset.value = +currentBalanceText.dataset.value - betValue;
        updateCurrencyValueText(currentBalanceText, walletName);

        balls.push(new Ball(json));

        isPlaying = false;
    } catch (error) {
        isPlaying = false;
        console.error(error);
    }

    isPlaying = false;
}

window.addEventListener('keydown', function (e) {

    if (e.keyCode == 32) {
        playGame();
        e.preventDefault();
    }
});

(async function () {
    const searchParams = new URLSearchParams(window.location.search);
    walletId = searchParams.get('wallet');

    var walletInfo = await getCurrentBalance(walletId);
    if (!walletInfo)
        return;

    walletName = walletInfo.name;

    updateBalance(walletInfo.balance);
    updateCurrencySpanText(document.getElementById('betValueSpan'), walletName);

    app = new Application();
    await app.init({ backgroundAlpha: 0, width: 700, height: 500, antialias: true });

    changeRows(16);

    document.getElementById('gameContainer').appendChild(app.canvas);

    app.ticker.add((ticker) => {
        let i = balls.length;
        while (i--) {
            const ball = balls[i];
            if (ball.tick(ticker.deltaTime)) {
                app.stage.removeChild(ball.sprite);
                ball.sprite.destroy();
                balls.splice(i, 1);
            }
        }

        for (let i = 0; i < staticBalls.length; i++) {
            staticBalls[i].tick(ticker.deltaTime);
        }
    });

})();