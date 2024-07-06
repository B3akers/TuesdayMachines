const betsAllowedValues = [
    25,
    50,
    75,
    100,
    125,
    250,
    500,
    750,
    1000,
    1250,
    2500,
    5000,
    10000,
    20000
];

const replecmentsGems = [
    4,
    5,
    6,
    7,
    1,
    0
];

const reelSpeed = 45;
const coverSpeed = 10;

const lockedSymbolWinAnimationTime = 55;
const newSymbolFrameRestoreTime = 45;
const nextReelStopTime = 15;

const winTextUpdateInterval = 2;
const winTextStayInterval = 60;

var autoPlayInterval = null;

const gameRect = {
    width: 1792,
    height: 1024
};

const reelsData = {
    baseGame: [
        [-3, -4, -5, -3, -4, -5, -3, -6, -5, -3, -2, -5, -3, -5, -6, -3, -5, -6, -3, -5, -4, -5, -3, -4, -5, -3, -3, -5, -4, -3, -5, -5, -3, -2, -5, -3, -6, -5, -3, -6, -5, -3, -6, -5, -3, -6, -5, -3, -3, -5, -4, -3, -5, -6, -3, -5, -4, -3, -5, -6, -3, -5, -3, -6, -5, -3, -4, -5, -3, -3, -5, -2, -3, -5, -2, -3, -5, -4, -3, -5, -4, -3, -5, -6, -3, -5, -2, -3, -5, -6, -3, -5, -4, -3, -5, -4, -3, -5, -6, -3, -5, -5, -3, -2, -5, -3, -4, -5, -3, -2, -5, -3, -2, -5, -3, -4, -5, -3, -2, -5, -3, -3, -5, -5, -3, -5, -2, -3, -5, -6, -3, -5, -2, -3, -5, -4, -3, -5, -2, -3, -5, -6, -3, -5, -2, -3, -5, -4, -3, -5, -2, -3, -5, -2, -3, -5, -5, -3, -6, -5, -3, -6, -5, -3, -4, -5, -3, -4, -5, -3, -2, -5, -3, -5, -6, -3, -5, -6, -3, -5, -4, -3, -2, -5, -3, -2, -5, -3, -3, -5, -2, -3, -6, -5, -3, -2, -5, -3, -4, -5],
        [3, 4, 1, 5, 0, 6, 2, 5, 1, 7, 5, 2, 2, 5, 6, 1, 7, 2, 6, 0, 5, 1, 1, 6, 2, 7, 7, 1, 6, 2, 2, 5, 6, 1, 4, 2, 5, 1, 1, 7, 2, 5, 0, 0, 0, 4, 2, 2, 5, 0, 6, 1, 1, 1, 6, 6, 2, 5, 3, 4, 1, 6, 2, 4, 1, 6, 6, 2, 2, 2, 7, 3, 3, 7, 7, 7, 2, 2, 2, 5, 1, 6, 2, 2, 5, 0, 6, 2, 5, 1, 1, 6, 2, 4, 4, 4, 1, 5, 2, 2, 4, 3, 7, 6, 2, 5, 1, 7, 0, 6, 6, 6, 2, 5, 1, 6, 2, 4, 7, 5, 1, 6, 2, 5, 1, 6, 7, 2, 5, 6, 1, 1, 6, 6, 4, 2, 2, 6, 3, 3, 3, 4, 1, 6, 3, 5, 2, 6, 1, 4, 2, 5, 1, 6, 0, 0, 6, 3, 4, 2, 5, 5, 5, 2, 7, 6, 1, 5, 2, 6, 1, 5, 2, 6, 0, 5, 1, 1, 7, 2, 4, 3, 5, 2, 2, 5, 1, 6, 0, 0, 5, 3, 3, 7, 5, 5, 1, 7, 2, 5],
        [7, 7, 7, 1, 0, 2, 2, 2, 1, 4, 0, 0, 5, 1, 1, 1, 4, 0, 0, 0, 4, 1, 3, 3, 4, 1, 0, 0, 1, 7, 2, 0, 3, 4, 0, 1, 1, 5, 0, 0, 1, 3, 4, 0, 1, 4, 0, 3, 5, 1, 2, 0, 7, 1, 1, 3, 0, 0, 0, 4, 1, 0, 2, 4, 0, 3, 1, 0, 2, 3, 3, 4, 0, 3, 1, 0, 3, 3, 2, 0, 1, 6, 6, 6, 0, 1, 1, 0, 4, 3, 0, 0, 3, 4, 1, 0, 3, 1, 1, 5, 0, 1, 5, 5, 5, 0, 0, 4, 1, 1, 0, 3, 4, 2, 0, 4, 1, 0, 3, 3, 3, 0, 1, 5, 0, 0, 1, 1, 1, 3, 0, 6, 1, 1, 4, 0, 1, 3, 0, 4, 4, 4, 1, 0, 2, 2, 1, 1, 2, 2, 5, 0, 3, 4, 0, 0, 3, 7, 1, 0, 3, 4, 1, 0, 5, 3, 0, 1, 5, 0, 0, 5, 3, 3, 5, 0, 3, 3, 4, 0, 1, 3, 4, 0, 0, 1, 1, 5, 0, 0, 0, 1, 4, 4, 0, 0, 1, 4, 3, 0],
        [2, 5, 3, 3, 3, 7, 2, 0, 6, 2, 2, 3, 7, 2, 3, 5, 5, 5, 3, 0, 6, 3, 2, 7, 7, 3, 0, 1, 3, 3, 6, 2, 3, 7, 2, 2, 4, 3, 7, 0, 2, 6, 3, 0, 7, 2, 2, 7, 3, 0, 6, 6, 2, 2, 2, 7, 3, 6, 6, 3, 1, 7, 3, 3, 7, 2, 3, 6, 0, 0, 3, 7, 2, 3, 6, 6, 2, 2, 7, 3, 6, 2, 2, 6, 3, 0, 7, 2, 3, 6, 1, 7, 2, 6, 3, 3, 3, 7, 2, 6, 3, 7, 0, 2, 6, 3, 3, 7, 0, 0, 3, 7, 7, 3, 2, 6, 1, 1, 1, 7, 3, 2, 6, 6, 6, 3, 3, 7, 7, 3, 2, 6, 3, 7, 2, 2, 7, 0, 0, 7, 3, 1, 6, 3, 2, 7, 3, 0, 6, 2, 3, 7, 2, 0, 6, 3, 3, 4, 4, 4, 3, 3, 7, 0, 1, 6, 3, 3, 7, 2, 1, 7, 3, 3, 6, 1, 1, 7, 3, 6, 2, 7, 0, 0, 0, 7, 2, 0, 6, 3, 0, 7, 7, 7, 3, 3, 7, 2, 3, 6],
        [-4, -3, -6, -2, -3, -4, -6, -2, -3, -4, -2, -6, -5, -4, -2, -6, -3, -5, -2, -4, -6, -2, -5, -4, -6, -2, -4, -5, -6, -2, -4, -3, -6, -2, -4, -3, -6, -4, -5, -2, -6, -4, -5, -6, -2, -4, -6, -2, -3, -6, -4, -2, -6, -5, -4, -2, -6, -4, -3, -2, -5, -4, -6, -2, -4, -6, -3, -2, -4, -6, -2, -4, -3, -2, -6, -4, -2, -3, -4, -2, -6, -5, -4, -2, -6, -4, -3, -6, -2, -4, -6, -2, -4, -6, -5, -2, -6, -4, -3, -2, -6, -4, -2, -6, -4, -2, -3, -4, -6, -5, -2, -4, -6, -2, -4, -6, -2, -4, -5, -6, -4, -2, -6, -4, -3, -2, -6, -4, -5, -6, -2, -3, -6, -4, -2, -6, -5, -4, -2, -6, -4, -3, -6, -2, -4, -6, -2, -4, -5, -6, -2, -4, -3, -6, -2, -4, -5, -2, -6, -4, -2, -6, -4, -5, -6, -2, -4, -6, -2, -4, -6, -3, -5, -2, -4, -6, -2, -4, -3, -6, -2, -3, -5, -4, -2, -5, -6, -4, -2, -6, -5, -4, -2, -6, -3, -2, -4, -6, -2, -5]
    ],
    respinA: [
        [-4, 5, 1, -5, 2, 7, -2, 4, 2, -5, 7, -2, 0, 5, -5, 7, -3, -2, -5, 5, -2, 7, 2, -5, -2, -3, -5, -6, -2, -3, -5, -4, -2, -5, 0, 5, -5, 0, 0, -2, -5, -3, -6, -2, -5, -4, -2, -5, -1, -2, -3, -5, -2, -4, -5, -6, -2, -5, -4, -6, -2, -5, -4, -3, -2, -5, -4, -2, 7, 1, -5, -2, -4, -5, -2, 4, -1, 5, 2, -2, 1, 6, -5, 2, -1, -2, 1, 6, -5, 7, 1, -2, -5, -3, -2, -5, 5, 3, -6, -2, -5, -4, -2, -5, 6, 0, -2, 7, 2, -5, 6, 1, -2, 2, 2, -5, 5, 1, -2, -6, -5, -1, -2, -5, -6, -2, -4, -5, -2, -6, -5, -2, -3, -5, -2, 3, 3, -5, -2, -4, -5, -2, -4, -5, -2, -3, -6, -5, -2, -3, -5, -2, 5, 1, -2, 5, 1, -5, 3, 6, -2, -5, -3, -2, -5, -4, -2, -6, -5, -2, -6, -5, -3, -2, -5, -1, -2, -5, -6, -2, -5, -3, -2, -1, -5, -2, -1, 7, 2, -5, 1, 1, -5, 5, 2, -2, -1, -6, -5, -2],
        [2, 6, -4, 2, 7, -4, 1, 7, -4, 5, 3, -4, 6, -4, -3, -4, -4, -2, -3, -4, -4, -6, -3, -4, -5, -2, -4, -4, -6, -4, -4, -5, -1, -4, 0, 0, -4, -2, -4, -5, -4, -4, -2, 5, -4, 3, 3, -4, 2, 5, -4, -4, -2, 5, -4, 6, 1, -4, 2, 5, -4, 2, 6, -4, 4, 0, -4, 6, 3, -4, 2, 5, -4, -3, -4, 5, 2, -4, 6, -5, -4, -4, -6, -2, -4, -4, -5, -4, -3, 6, -4, 1, 7, -4, -2, -1, -4, -5, -4, -6, -4, -2, 5, -4, -1, 7, -4, 4, 2, -4, 0, 7, -4, 6, 1, -4, 2, 2, -4, 6, -3, -4, 1, 6, -4, 5, -4, 2, 7, -4, 1, 1, -4, 2, 7, -4, 6, 3, -4, 5, 1, -4, 4, -2, -4, -4, 6, -4, 1, 6, -4, 5, 2, -4, 7, 0, -4, 6, 3, -4, 5, 0, -4, 1, 6, -4, 5, 1, -4, 0, -5, -4, -4, -2, -1, -4, -4, -3, -4, 2, 7, -4, -4, -6, -4, -4, -6, -2, -4, -6, -4, -3, 6, -4, 5, 3, -4, 1, 5, -4],
        [-3, -4, -1, -6, -3, 1, 4, -6, 7, 3, -3, -6, -5, -3, -6, -5, -3, -6, 1, -3, 0, -6, 2, -3, 0, 7, -6, 1, -3, 0, -6, -3, 6, 3, -3, 0, -6, -3, -1, -6, -3, 1, 1, -3, -6, -1, -3, -4, -6, -5, -3, -6, 0, 4, -6, -3, 2, 7, -6, -3, 2, -6, 1, 7, -3, 0, -6, -3, 3, 6, -6, -3, 6, 1, -6, 3, -3, -6, 0, 0, -3, 4, 0, -6, 3, 7, -6, 1, 4, -6, -3, -2, -6, -3, 2, 2, -6, 3, -3, 2, -6, 0, 0, -3, -6, -4, -3, -5, -6, -2, -3, -6, 0, 6, -3, 0, -6, 1, -3, 2, 5, -6, 0, -3, 4, 0, -6, 1, -3, -6, -2, -3, -6, 0, -3, 5, 3, -6, -3, -1, -6, -4, -3, -6, 4, 3, -3, -6, 0, 6, -3, 0, -6, 3, -3, -2, -6, -3, 5, 1, -6, 3, 3, -3, 0, -6, -4, -3, 3, -6, 7, 2, -3, -6, 6, 1, -3, 2, -6, 0, 5, -3, -6, 1, -3, 7, -6, -3, -5, -1, -6, -3, 0, 5, -3, -6, -2, -3, -1, -6],
        [-5, -5, -1, 3, -5, 7, 2, -5, 0, 0, -5, 6, 0, -5, 7, 2, -5, 3, -4, -5, -5, -3, 1, -5, 4, 3, -5, 4, 2, -5, 3, 7, -5, 6, 3, -5, 2, -2, -5, -5, -4, -5, -5, -2, -5, -5, -3, -5, -5, 7, 3, -5, -6, -5, -5, -2, -4, -5, -5, -4, -1, -5, -6, -5, -2, -5, -5, -1, -3, -5, -5, -3, -5, -5, 3, 5, -5, 2, 5, -5, 3, 6, -5, -5, -4, -5, -5, -2, -3, -5, -5, -3, -5, -5, -6, 2, -5, 3, 7, -5, 3, 3, -5, 0, 7, -5, 4, 3, -5, 2, 2, -5, 0, 6, -5, 1, -5, -6, -5, -5, -4, -5, -2, -5, -5, -6, 3, -5, 0, 6, -5, 2, 5, -5, 7, 1, -5, 3, 6, -5, 7, 2, -5, 1, -5, 0, -2, -5, -5, -2, -6, -5, -5, 1, 1, -5, 0, 7, -5, 2, 5, -5, 6, 3, -5, 7, -2, -5, -5, -1, -5, -5, -3, -2, -5, -5, -6, -5, -5, 3, -5, 7, 1, -5, 2, 6, -5, 7, 3, -5, 1, 6, -5, 3, 3, -5, 0, -5, -6, -4],
        [-3, -2, -4, -3, -2, -5, -4, -2, 7, -4, -2, -1, -4, -6, -2, -4, 0, -1, -4, -2, -3, -4, -6, -2, -5, -4, -3, -2, -4, -1, -2, -4, -5, -2, -4, -5, -2, -6, -4, -2, 3, 6, -4, 3, -1, -2, -4, 1, -2, 4, -4, -2, 1, -4, 6, 3, -4, -2, 2, -4, 0, 5, -2, -4, 0, 0, -4, -2, 2, 7, -4, -2, -6, -4, -2, -3, -4, -2, 3, 7, -4, -2, 5, 1, -4, 0, -2, -4, -1, -2, -4, -6, -2, -4, 7, 0, -4, -2, 2, 2, -2, -4, -5, -2, -4, -3, -2, -4, -3, -2, -4, -6, -2, 7, -5, -2, -4, -3, -2, -4, -6, -2, -4, 0, -2, 7, -4, -2, 1, -4, 7, -2, -4, 6, -2, 2, -4, 5, -2, 3, -4, -2, -6, -4, -2, -5, -4, -2, -1, -4, -2, -1, -5, -4, -2, 0, -4, -2, 7, 2, -2, 3, -4, 0, 6, -2, -4, -5, -2, 3, 3, -4, -2, 1, 1, -4, 0, 4, -2, -4, 1, 7, -2, -4, -3, -2, -6, -4, -2, 0, 7, -2, -6, -4, -2, -3, -4, -5, -2, -4]
    ],
    respinB: [
        [-2, 4, 3, -5, -2, -4, -5, -2, -3, -5, -4, -2, -5, -1, -2, -5, 0, 5, -5, -2, 0, -5, -2, -3, -5, 5, -2, -5, 1, -2, -3, -5, -6, -2, -3, -5, -2, -3, -5, 1, -2, 7, -5, 3, -2, 7, 0, -5, -2, 3, 3, -5, -2, -4, -5, -2, -4, -5, -6, -2, -5, -4, -6, -2, -5, 0, -2, 4, -5, -2, 0, 5, -5, -2, 3, 7, -5, -2, 1, -5, -2, -4, -1, -5, -2, -6, -5, -2, -4, -5, -2, -6, -5, -2, 0, 5, -5, -2, -4, -5, -2, -6, -5, -2, 1, 1, -3, -2, -5, -3, -2, -5, 0, 0, -5, -2, 5, -5, -3, -2, -5, 1, -2, 5, -5, 2, -2, 3, -5, 7, -2, -3, -5, -2, -6, -5, -4, -2, -5, -1, -2, -5, -3, -6, -2, -5, 2, 5, -2, -5, 0, 7, -2, -5, -3, -2, -4, -5, 5, -2, 2, -5, -2, 6, 3, -2, -5, 6, -2, 3, -5, -2, -6, -1, -5, -2, -6, -5, -2, -3, -5, -4, -2, -5, 3, 6, -2, -5, 2, 2, -2, -5, 7, -2, 0, -5, -2, -6, -4, -5],
        [-4, -4, -2, -4, -4, -5, -4, -4, -3, -1, -4, -2, -4, -4, -3, -4, -4, -5, -4, -4, -5, -1, -4, -4, 2, 5, -4, 1, 5, -4, 3, 6, -4, 0, 6, -4, 4, 1, -4, 7, 1, -4, 0, 0, -4, 7, 2, -4, 5, 2, -4, 6, -6, -4, -4, 3, 6, -4, 3, 6, -4, 5, 3, -4, 1, 5, -4, 3, 3, -4, 5, 0, -4, 2, 6, -4, 0, 6, -4, 1, 5, -4, 1, 1, -4, 2, 5, -4, 1, 6, -4, 5, 0, -4, 7, 2, -4, -5, -4, -4, -2, -4, -4, -6, -4, -2, -4, -4, -1, -4, -4, -2, -4, -4, -3, -4, -4, -2, -4, -4, -6, -4, -4, -5, -4, -2, -4, -4, -6, -4, -4, -2, -4, -4, -3, -4, -4, -6, -4, -4, -5, -4, -2, -4, -4, -3, -4, -4, -2, -4, -4, 1, 6, -4, 2, 4, -4, 7, -4, -2, -4, -4, -3, 5, -4, 7, 0, -4, 6, 1, -4, 2, 7, -4, 3, 6, -4, 2, 5, -4, 2, 7, -4, 3, 5, -4, 2, 2, -4, 6, 2, -4, 0, 7, -4, -4, -1, -4, -4, -6],
        [-6, 4, 0, -3, 3, -6, -3, 0, 4, -6, 3, 6, -6, -3, 3, -6, 1, -3, 6, -6, -3, -4, -6, -3, -1, 0, -6, -3, 3, -6, -3, 1, 1, -3, -6, 6, 3, -3, -6, 2, 7, -3, -6, 3, -3, -4, -6, -3, 1, -6, 2, 2, -6, -3, 0, 4, -3, -6, 0, -3, 7, -6, 0, -3, 2, 4, -6, 5, -3, -6, 7, 0, -3, 2, -6, -3, -2, -6, -3, 6, 0, -3, -6, 7, -3, -6, 3, -3, 5, -6, -3, -5, -6, -3, -5, -6, 1, -3, -6, -2, -3, -6, -4, -3, -6, 0, 6, -6, -3, -2, -6, 1, -3, 3, 3, -3, -6, 0, -3, -1, -6, 2, -3, 1, -6, 6, 1, -3, -6, 0, -3, 3, -1, -6, -3, -5, -6, -3, 0, -6, -3, -5, -6, -3, 7, -6, 3, -3, -6, -5, -3, -6, 1, -4, -3, -6, 2, -3, -6, -1, -3, -2, -6, -3, 0, 7, -3, -6, 4, 1, -6, 2, -3, 5, 2, -6, -3, 0, 0, -6, 1, 5, -3, 3, -6, 1, 5, -3, -6, -2, -3, -6, -4, -3, -6, 7, 2, -6, -3, 0],
        [-5, 3, -5, 6, 1, -5, 3, -5, 0, 4, -5, 1, 7, -5, 2, 7, -5, 3, 7, -5, 3, 6, -5, 2, 2, -5, 1, 5, -5, 7, 0, -5, 7, 3, -5, 2, 6, -5, 0, 7, -5, 4, 3, -5, -5, -3, -5, -5, -2, -5, -1, -4, -5, -5, -6, -5, -5, -2, -5, -5, -3, -5, -5, -3, -5, -5, -2, -5, -5, -4, -5, -5, -2, -5, -5, 3, -5, 1, 1, -5, 0, 5, -5, 3, -5, 0, 6, -5, 2, 6, -5, 3, 3, -5, 1, -5, -2, -5, -5, -3, -5, -5, -4, -5, -5, -2, -6, -5, -5, -6, -5, -5, -4, -5, 3, -5, 0, 0, -5, 2, 5, -5, 2, 6, -5, 5, 0, -5, 1, -5, 3, -5, 0, 7, -5, 3, 3, -5, -5, -2, -5, 3, -5, 7, 1, -5, 7, 2, -5, -5, -2, -5, -5, -3, -5, -5, -1, -5, -5, -6, -5, -5, -2, -5, -5, -6, -5, -5, -1, -5, -3, -5, -5, -6, -5, -5, -4, -5, -5, -1, -5, -5, -2, -5, -5, 3, -5, 2, 7, -5, 2, 6, -5, 3, -5, 2, -5, -5, -4, -5],
        [3, -4, 1, -2, 2, -4, -2, 3, 3, -4, -2, 6, 2, -4, -2, -5, -1, -2, -3, -4, -2, 5, -4, 1, -2, 4, -4, -2, -6, -4, -2, -3, -4, -2, -5, -2, -4, 0, 0, -4, -2, 7, -4, -2, -3, -4, -2, -3, -4, -2, 0, -4, 7, -2, -4, 1, 7, -2, -4, 2, -2, 7, -4, -2, -5, -4, -1, -2, -4, -5, -2, -4, 2, 2, -2, -4, -5, -2, -4, -6, -2, -3, -4, -2, 7, -4, 1, -2, 0, -4, 2, -2, -4, -3, -2, -4, 1, -2, 3, -4, -2, -5, -4, -2, -6, -2, -4, -3, -2, -4, -3, -2, -1, -4, -5, -2, -4, -6, -2, -4, -6, -2, -4, 1, -2, -4, 1, 1, -4, -2, -6, -4, -2, -6, -4, -2, -5, -4, -3, -2, -4, 2, -2, -4, -6, -2, -3, -4, -2, -6, -4, -2, -1, -4, -2, -3, -4, 4, -2, -5, -4, -2, 2, -4, -5, -2, 7, -4, -2, 7, -4, 1, -2, 0, -4, 6, -2, -4, 7, -2, -6, -4, 5, -2, 1, -4, -2, 3, -4, 0, -2, 6, -4, -2, -6, -4, -2, -5, -4, -2]
    ],
    respinC: [
        [-5, -2, -6, -5, -2, -4, -5, -2, -2, -5, -3, -2, -5, 2, -2, 0, -5, 5, -2, -5, 2, -2, 7, -5, -2, -6, -5, -2, 7, -5, 3, -2, -5, -3, -2, -5, -4, -2, -5, -3, -2, -5, 0, 0, -2, -5, -2, 3, -5, -2, -4, -5, -2, -6, -5, -2, -6, -5, -2, 4, -5, -2, 2, 2, -5, -2, -2, -5, -4, -2, -5, 6, -2, 1, -5, -2, -3, -5, -2, 6, -5, 0, -2, -5, 3, -2, -5, 0, -2, -5, 5, -2, -5, -6, -2, -4, -5, -2, -3, -5, -2, -3, -5, -2, 2, -5, 7, -2, -5, -3, -2, -5, 1, 1, -2, -5, 2, -2, -5, -5, -2, -2, -5, -2, -1, -5, -2, -6, -5, -2, -6, -5, -2, -4, -5, -2, -4, -5, -2, 1, -5, 4, -2, -5, 2, -2, -5, -4, -2, -5, -5, -2, -6, -5, -2, -4, -5, -2, -3, -5, -2, -6, -5, -2, 3, -5, -2, -5, -6, -2, -5, 3, 3, -5, -2, -3, -5, 6, -2, 0, -5, 1, -2, -5, 0, -2, 7, -5, 3, -2, 7, -5, -2, -4, -5, -2, 3, -5, -2, -3],
        [-4, -4, -2, -4, -4, -6, -4, -4, -5, -4, -4, 0, -4, -4, -2, -4, -4, 7, -4, -4, 0, -4, -4, 2, 2, -4, -4, 4, 1, -4, -4, 0, -4, -4, -3, -4, -4, 0, 0, -4, -4, -2, -4, -4, -6, -4, -4, -3, -4, -4, 3, 3, -4, -4, 2, -4, -4, 5, -4, -4, 2, -4, -4, 2, 7, -4, -4, 5, -4, 3, -4, -4, 1, -4, -4, 7, -4, -4, 6, 0, -4, -4, 3, -4, -4, 7, -4, -4, 1, -4, -4, 0, -4, -4, 3, -4, -4, -2, -4, -4, -2, -4, -4, 7, 3, -4, -4, -6, -4, -4, 6, -4, -4, 1, -4, 3, -4, 6, 2, -4, -4, 1, 5, -4, -4, -5, -4, -4, 4, -4, -4, -5, -4, -4, 0, -4, -4, 1, 1, -4, 5, -4, -4, 3, -4, -4, 3, -4, -4, -2, -4, -4, -5, -4, -2, -4, -5, -4, -4, -6, -4, -4, -2, -4, -4, 2, -4, -4, 2, -4, -4, 7, 2, -4, -4, -2, -4, -4, -3, -4, -4, -3, -4, -4, 3, -4, -4, -3, -4, -4, -6, -4, -4, -2, -4, -4, 0, -4, -4, -2],
        [-3, 2, -6, 0, -3, -6, -5, -3, -6, -2, -3, -6, 2, 2, -3, -6, -6, -3, -5, -6, -3, 7, 0, -6, -3, 3, -6, -3, -4, -6, -3, 6, -6, -3, -5, -6, -3, 7, -6, -3, 2, -6, 3, -3, -6, 7, -3, -6, 3, -3, 6, -6, -3, 0, 7, -6, -3, 7, -6, -3, 1, 1, -6, -3, 5, -6, -3, -2, -6, -4, -3, -6, 0, -3, 5, -6, 0, -3, 5, -6, 0, -3, -6, -2, -3, -6, 2, -3, -6, 7, -3, 2, -6, 3, -3, -6, -4, -3, -6, 1, -3, 3, -6, -3, 5, -6, 0, -3, -6, 2, -3, -6, 1, -3, 4, -6, -3, 2, -6, -3, 2, -6, -3, 1, -6, -3, 6, -6, 3, -3, -6, 6, -3, 1, -6, -3, -5, -6, -3, 2, -6, -3, -5, -6, -3, 5, -6, -3, 3, 6, -6, -3, -6, -3, 0, 0, -6, -3, 6, -6, -3, 3, 3, -6, -3, 2, -6, 7, -3, 3, -6, -3, -4, -6, -3, -4, -6, -3, -2, -6, -3, 0, -6, 1, -3, -6, 1, -3, -6, -2, -3, -6, 3, -3, 1, -6, 3, -3, 4, -6],
        [-5, -5, 2, -5, 3, -5, -5, 6, -5, 1, -5, -5, -4, -5, -5, 2, -5, -5, -4, -5, -5, -6, -5, -5, 3, -5, -5, 0, -5, -5, 6, -5, -5, -2, -5, -5, -2, -5, -5, -6, -5, -5, 0, -5, -5, -3, -5, -5, -2, -5, -5, -2, -5, -5, -4, -5, -5, 3, -5, -5, 5, -5, -5, 3, 3, -5, -5, 5, -5, 2, -5, 3, -5, -5, 1, -5, -5, 2, -5, -5, -2, -5, -5, 0, -5, -5, 7, -5, -5, 1, -5, -5, 7, -5, -5, -6, -5, -5, -4, -5, -5, -2, -5, -5, 2, -5, -5, 0, -5, -5, 5, -5, -5, -3, -5, -5, 0, -5, -5, 3, -5, 1, -5, -5, 7, -5, 1, -5, -5, 2, 2, -5, -5, 0, 7, -5, -5, 3, -5, 4, -5, -5, -2, -5, -5, 2, -5, -5, 3, -5, -5, 5, -5, -5, 7, -5, 2, 7, -5, -5, 1, 1, -5, -5, 0, 0, -5, -5, 3, 4, -5, -5, -2, -5, -5, -6, -5, -5, -3, -5, -5, -2, -5, -5, -6, -5, -5, -4, -5, -5, -2, -5, -5, -3, -5, -5, -3, -5, -5, 6],
        [0, -4, -2, -3, -4, -2, -5, -4, -2, -5, -4, -2, -3, -4, -2, -6, -4, -2, -4, 5, -2, -4, -3, -2, -4, 3, -2, -4, 7, -2, -4, 2, -2, -4, -5, -2, -4, 7, -2, 1, -4, -2, 2, -4, -2, 7, -4, -6, -2, -4, 2, -2, 4, -4, 0, -2, -4, -5, -2, -4, -6, -2, -4, 2, -2, 4, -4, -2, 2, 2, -4, -2, 0, -4, -2, -6, -4, -2, -3, -4, -2, -3, -4, -2, 1, -4, -2, 1, 1, -4, 6, -2, -4, -3, -2, -4, -6, -2, -4, 7, -2, -4, 6, -2, 3, -4, -2, -3, -4, -2, -5, -4, -2, -3, -4, -2, -1, -4, -2, 0, 0, -2, -4, -5, -2, -4, -6, -2, -4, 7, -2, 3, -4, -2, -4, -3, -2, -4, -6, -2, -4, -5, -2, -4, 0, -2, 3, -4, -2, -5, -4, -2, -5, -4, -2, -6, -4, -2, -6, -4, -2, -5, -4, -2, -3, -4, -2, -5, -4, -2, -3, -4, -2, 3, -4, -2, -4, 3, 3, -4, 1, -2, -4, -6, -2, -4, -6, -2, -4, 5, -2, -4, 6, -2, 2, -4, -2, -4, 3, -2]
    ]
};

const audioAssets = {
    pop: '../assets/mayan/pop.mp3',
    main: '../assets/mayan/main.mp3',
    win: '../assets/mayan/wingeneral.wav',
    winLarge: '../assets/mayan/winlarge.wav',
    winEpicStart: '../assets/mayan/epicwinstart.wav',
    winEpicEnd: '../assets/mayan/epicwinend.wav'
};

const gameAssets = {
    background: {
        path: '../assets/mayan/background.webp'
    },
    greenGem: {
        path: '../assets/mayan/green.webp'
    },
    purpleGem: {
        path: '../assets/mayan/purple.webp'
    },
    blueGem: {
        path: '../assets/mayan/blue.webp'
    },
    redGem: {
        path: '../assets/mayan/red.webp'
    },
    mayaGem: {
        path: '../assets/mayan/mayana.webp'
    },
    lionGem: {
        path: '../assets/mayan/lion.webp'
    },
    owlGem: {
        path: '../assets/mayan/owl.webp'
    },
    frogGem: {
        path: '../assets/mayan/frog.webp'
    },
    stone: {
        path: '../assets/mayan/stone.webp'
    },
    locked: {
        path: '../assets/mayan/locked.webp'
    },
    frame: {
        path: '../assets/mayan/frame.webp'
    },
    winAnim: {
        path: '../assets/mayan/win.webp'
    }
};

const gemsTextureArray = [
    gameAssets.greenGem, //F7 - 0
    gameAssets.purpleGem, //F6 - 1
    gameAssets.blueGem, //F8 - 2
    gameAssets.redGem, //F9 - 3
    gameAssets.mayaGem, //M1 - 4
    gameAssets.lionGem, //M2 - 5
    gameAssets.owlGem,  //M3 - 6
    gameAssets.frogGem  //M4 - 7
];

const gameBoardBasePosition = {
    x: 256,
    y: -192
};

const gameGemSize = {
    x: 256,
    y: 256
};

const Application = PIXI.Application;
const Assets = PIXI.Assets;
const Sprite = PIXI.Sprite;
const Rectangle = PIXI.Rectangle;
const Container = PIXI.Container;
const Graphics = PIXI.Graphics;
const Spritesheet = PIXI.Spritesheet;
const Texture = PIXI.Texture;
const BitmapText = PIXI.BitmapText;

var walletId;
var walletName;

var boardContainer;
var isFastSpinEnabled = false;
var isFastSpinOnce = false;

var audioWinPlayer;
var audioVolume = 1;

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

function convertPointsToLocalCurrency(value) {
    return +value;
}

function updateCurrencySpanText(element, currency) {
    element.innerText = "[" + currency + "]" + convertPointsToLocalCurrency(element.dataset.value).toLocaleString();
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

var mainMusicAudioPlayer = new Audio(audioAssets.main);

function onAudioVolumeChange(volume) {
    audioVolume = volume / 100;
    mainMusicAudioPlayer.volume = audioVolume / 2;
}

function playAudio(audio) {
    const a = new Audio(audio);
    a.volume = audioVolume;
    a.play();
}

const currentWinBitmapTest = new BitmapText({
    text: '0.00',
    style: {
        fontFamily: 'Desyrel',
        fontSize: 150,
        align: 'center',
    },
});

class StoneCover {
    constructor(sprite) {
        this.sprite = sprite;
        this.isMoving = false;
        this.isCovering = false;
    }

    setPositions(startPosX, startPoxY, endPosX, endPosY) {
        this.startPos = { x: startPosX, y: startPoxY };
        this.endPos = { x: endPosX, y: endPosY };
        this.position = { x: startPosX, y: startPoxY };
        this.sprite.position.set(startPosX, startPoxY);
    }

    startCover() {
        this.isMoving = true;
        this.isCovering = true;
    }

    startUnConver() {
        this.isMoving = true;
        this.isCovering = false;
    }

    tick(deltaTime) {
        if (!this.isMoving) {
            return;
        }

        const realSpeed = coverSpeed * deltaTime;
        const isLeft = this.startPos.x < this.endPos.x;

        if ((isLeft && this.isCovering) || (!isLeft && !this.isCovering)) {
            const endMovingX = isLeft ? this.endPos.x : this.startPos.x;
            this.position.x += realSpeed;

            if (this.position.x >= endMovingX) {
                this.position.x = endMovingX;
                this.isMoving = false;
            }
        } else if ((isLeft && !this.isCovering) || (!isLeft && this.isCovering)) {
            const endMovingX = isLeft ? this.startPos.x : this.endPos.x;
            this.position.x -= realSpeed;

            if (this.position.x <= endMovingX) {
                this.position.x = endMovingX;
                this.isMoving = false;
            }
        }

        this.sprite.position.set(this.position.x, this.position.y);
    }
}

class LockedSymbol {
    constructor(sprite) {
        this.sprite = sprite;
        this.animations = [];
        this.lockedSprite = null;

        boardContainer.addChild(this.sprite);
    }

    isIdle() {
        return this.animations == 0;
    }

    createLockedSprite() {
        if (this.lockedSprite) {
            return;
        }

        this.lockedSprite = new Sprite(gameAssets.locked.texture);
        this.lockedSprite.width = gameGemSize.x;
        this.lockedSprite.height = gameGemSize.y;
        this.lockedSprite.position.set(this.sprite.position.x, this.sprite.position.y);

        boardContainer.addChild(this.lockedSprite);
    }

    startFoundAnimation() {
        const sprite = new Sprite(gameAssets.frame.texture);

        sprite.width = gameGemSize.x;
        sprite.height = gameGemSize.y;
        sprite.position.set(this.sprite.position.x, this.sprite.position.y);

        const animation = {
            sprite: sprite,
            time: 0,
            endTime: newSymbolFrameRestoreTime,
            update: function (e, deltaTime) {
                this.time += deltaTime;
                const animationEnded = this.time >= this.endTime;
                if (animationEnded) {
                    boardContainer.removeChild(this.sprite);
                    this.sprite.destroy();
                    return true;
                }

                const animationPercent = Math.min(1, this.time / this.endTime);

                this.sprite.alpha = 1 * (1 - animationPercent);

                return false;
            }
        };

        boardContainer.addChild(sprite);
        this.animations.push(animation);
    }

    startWinAnimation() {
        const texture = new Texture(gameAssets.winAnim.texture.baseTexture);
        texture.frame = new Rectangle(0, 0, 256, 256);
        texture.updateUvs();

        const sprite = new Sprite(texture);
        sprite.width = gameGemSize.x;
        sprite.height = gameGemSize.y;
        sprite.position.set(this.sprite.position.x, this.sprite.position.y);

        const animation = {
            sprite: sprite,
            time: 0,
            currentAnimIndex: 0,
            animationsLength: 14,
            endTime: lockedSymbolWinAnimationTime,
            update: function (e, deltaTime) {
                this.time += deltaTime;

                const animationEnded = this.time >= this.endTime;
                if (animationEnded) {
                    boardContainer.removeChild(this.sprite);
                    this.sprite.destroy();
                    return true;
                }

                const animationPercent = Math.min(1, this.time / this.endTime);
                const animIndex = Math.floor(this.animationsLength * animationPercent);
                if (animIndex != this.currentAnimIndex) {

                    const texture = new Texture(gameAssets.winAnim.texture.baseTexture);
                    texture.frame = new Rectangle(256 * animIndex, 0, 256, 256);
                    texture.updateUvs();

                    if (this.sprite.texture) {
                        this.sprite.texture.destroy();
                    }

                    this.sprite.texture = texture;

                    this.currentAnimIndex = animIndex;
                }

                return false;
            }
        };


        boardContainer.addChild(sprite);
        this.animations.push(animation);
    }

    tick(deltaTime) {
        let i = this.animations.length;
        while (i--) {
            const animation = this.animations[i];
            if (animation.update(this, deltaTime)) {
                this.animations.splice(i, 1);
            }
        }
    }

    remove() {
        boardContainer.removeChild(this.sprite);
        this.sprite.destroy();

        if (this.lockedSprite) {
            boardContainer.removeChild(this.lockedSprite);
            this.lockedSprite.destroy();
        }
    }
}

class ReelInfo {
    constructor(reelData, index) {
        this.index = index;
        this.startIndex = 0;
        this.currentIndex = 0;
        this.hasSetReel = false;
        this.isMoving = false;
        this.data = reelData;
    }

    setStopIndex(index) {
        this.startIndex = index;
        this.currentIndex = this.startIndex;
        this.isMoving = true;
        this.hasSetReel = true;
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

    getIndexFromData(dataIndex) {
        if (dataIndex < 0) {
            return replecmentsGems[dataIndex + replecmentsGems.length];
        }

        return dataIndex;
    }

    getTextureRealIndex(index) {
        while (index >= this.data.length || index < 0) {
            if (index >= this.data.length) {
                index = index - this.data.length;
            } else if (index < 0) {
                index = this.data.length + index;
            }
        }

        return this.getIndexFromData(this.data[index]);
    }

    getNextTextureIndex() {
        this.currentIndex++;
        if (this.currentIndex >= this.data.length) {
            this.currentIndex = 0;
        }

        return this.getIndexFromData(this.data[this.currentIndex]);
    }
}

const objectsToScale = [];

//Current game info
//

var isPlaying = false;

const boardStoneCovers = [];
const boardGems = [];
const boardReelsInfo = [];
const boardLockedSymbols = [];

for (let i = 0; i < 5; i++) {
    boardReelsInfo.push(new ReelInfo(reelsData.baseGame[i], i));
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
            boardReelsInfo[i].setStopIndex(spin.stops[i]);
        }

        let backup = this.currentStopReel;
        this.currentStopReel = 5;
        return backup;
    }

    tryStopNextReel(deltaTime) {
        if (this.currentStopReel >= 5) {
            return false;
        }

        const isInActiveRow = (this.spinDataIndex == 0 && (this.currentStopReel == 0 || this.currentStopReel == 4));

        this.lastStopReel += deltaTime;
        if (this.lastStopReel > nextReelStopTime || isInActiveRow) {
            const spin = this.spinData.spins[this.spinDataIndex];

            boardReelsInfo[this.currentStopReel].setStopIndex(spin.stops[this.currentStopReel]);

            this.currentStopReel++;
            this.lastStopReel = 0;

            if (isInActiveRow) {
                return false;
            }

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

const gameBasePosition = {
    x: 0,
    y: 0
};

function rescaleWinBitmapText() {
    const ratio = gameRect.width / gameRect.height;
    let width = window.innerWidth;
    let height = window.innerWidth / ratio;
    if (height > window.innerHeight) {
        height = window.innerHeight;
        width = window.innerHeight * ratio;
    }

    const scaleY = height / gameRect.height;

    const bounds = currentWinBitmapTest.getLocalBounds();

    currentWinBitmapTest.x = gameBasePosition.x + width / 2 - bounds.maxX / 2;
    currentWinBitmapTest.y = gameBasePosition.y + ((gameBoardBasePosition.y + (gameGemSize.y * 2.5)) * scaleY) - (bounds.maxY / 1.2);
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

    rescaleWinBitmapText();

    for (let i = 0; i < objectsToScale.length; i++) {
        const obj = objectsToScale[i];
        obj.sprite.position.set(gameBasePosition.x + (obj.basePosition.x * scaleX), gameBasePosition.y + (obj.basePosition.y * scaleY));
        obj.sprite.scale.set(scaleX, scaleY);
    }

    boardContainer.mask = new Graphics()
        .rect(gameBasePosition.x + ((gameBoardBasePosition.x - gameGemSize.x) * scaleX), gameBasePosition.y + ((gameBoardBasePosition.y + gameGemSize.y) * scaleY), (7 * gameGemSize.x) * scaleX, (3 * gameGemSize.y) * scaleY)
        .fill(0xffffff);

    const gameUi = document.getElementById('game-ui');

    gameUi.classList.remove('d-none');

    gameUi.style.width = `${(gameGemSize.x * 5) * scaleX}px`;
    gameUi.style.height = `${(gameGemSize.y * 0.7) * scaleY}px`;

    gameUi.style.top = `${(gameBasePosition.y + ((gameGemSize.x * 3.30) * scaleY))}px`;
    gameUi.style.left = `${(gameBasePosition.x + (gameGemSize.x * scaleX))}px`;
}

function canStartNewGame() {
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

function isLockedSymbolsIdle() {
    for (let i = 0; i < boardLockedSymbols.length; i++) {
        if (!boardLockedSymbols[i].isIdle()) {
            return false;
        }
    }

    return true;
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

async function playGame() {
    if (!canStartNewGame()) {
        return;
    }

    isPlaying = true;

    for (let i = 0; i < boardLockedSymbols.length; i++) {
        boardLockedSymbols[i].remove();
    }
    boardLockedSymbols.length = 0;

    for (let i = 0; i < boardStoneCovers.length; i++) {
        const cover = boardStoneCovers[i];
        cover.startCover();
    }

    for (let i = 0; i < boardReelsInfo.length; i++) {
        const boardReel = boardReelsInfo[i];
        boardReel.data = reelsData.baseGame[i];
        boardReel.startMoving();
    }

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

        const currentBalanceText = document.getElementById('currentBalanceSpan');
        currentBalanceText.dataset.value = +currentBalanceText.dataset.value - betValue;
        updateCurrencySpanText(currentBalanceText, walletName);

        gameSpinInfo.spinData = json;
        gameSpinInfo.spinDataIndex = 0;
        isPlaying = false;
    } catch (error) {
        isPlaying = false;
        console.error(error);
    }

}

function playGameOrFastSpinOnce() {
    if (canStartNewGame()) {
        playGame();
    } else {
        isFastSpinOnce = true;
    }
}

function onFastSpinSwitch(e) {
    isFastSpinEnabled = !isFastSpinEnabled;
    if (isFastSpinEnabled) {
        e.classList.add('btn-selected');
    } else {
        e.classList.remove('btn-selected');
    }
}

function disableAutoPlay() {
    const e = document.getElementById('autoPlayButton');
    e.classList.remove('btn-selected');
    clearInterval(autoPlayInterval);
    autoPlayInterval = null;
}

function enableAutoPlay() {
    const e = document.getElementById('autoPlayButton');
    e.classList.add('btn-selected');
    autoPlayInterval = setInterval(function () {
        if (canStartNewGame()) {
            playGame();
        }
    }, 400);
}

function onAutoPlaySwitch() {
    if (autoPlayInterval) {
        disableAutoPlay();
    } else {
        enableAutoPlay();
    }
}

window.addEventListener('resize', function () {
    rescaleObjects();
});

(async function () {
    const searchParams = new URLSearchParams(window.location.search);
    walletId = searchParams.get('wallet');

    var walletInfo = await getCurrentBalance(walletId);
    if (!walletInfo)
        return;

    walletName = walletInfo.name;

    //Update initial balance
    //
    const currentBalanceText = document.getElementById('currentBalanceSpan');
    currentBalanceText.dataset.value = walletInfo.balance;
    updateCurrencySpanText(currentBalanceText, walletName);

    const app = new Application();
    await app.init({ resizeTo: window });

    app.canvas.style.position = 'absolute';

    document.body.appendChild(app.canvas);

    //Load assests
    //
    {
        await Assets.load('https://pixijs.com/assets/bitmap-font/desyrel.xml');

        const assets = Object.keys(gameAssets);
        for (let i = 0; i < assets.length; i++) {
            const asset = gameAssets[assets[i]];
            const texture = await Assets.load(asset.path);

            asset.texture = texture;
        }
    }

    //Create static objects
    //
    {
        //Create background
        //
        {
            const background = new Sprite(gameAssets.background.texture);

            objectsToScale.push({ sprite: background, basePosition: { x: 0, y: 0 } });
            app.stage.addChild(background);
        }

        //Create other ui elements
        //
        {

        }

        //Create gems board
        //
        {
            boardContainer = new Container();
            boardContainer.position.set(gameBoardBasePosition.x, gameBoardBasePosition.y);
            boardContainer.mask = new Graphics()
                .rect(0, 0, 0, 0)
                .fill(0xffffff);

            for (let i = 0; i < 5; i++) {
                for (let j = 0; j < 4; j++) {
                    const gem = new Sprite(gemsTextureArray[boardReelsInfo[i].getTextureRealIndex(j)].texture);
                    gem.width = gameGemSize.x;
                    gem.height = gameGemSize.y;
                    gem.position.set(i * gameGemSize.x, j * gameGemSize.y);

                    boardContainer.addChild(gem);
                    boardGems.push({
                        sprite: gem
                    });
                }
            }

            //Create stone covers
            //

            for (let i = 0; i < 2; i++) {
                for (let j = 0; j < 3; j++) {
                    const stoneConverSprite = new Sprite(gameAssets.stone.texture);
                    stoneConverSprite.width = gameGemSize.x;
                    stoneConverSprite.height = gameGemSize.y;

                    const stoneConver = new StoneCover(stoneConverSprite);

                    const posX = i > 0 ? gameGemSize.x * 5 : -gameGemSize.x;
                    const posY = (j + 1) * gameGemSize.y;
                    stoneConver.setPositions(posX, posY, posX + (i > 0 ? -gameGemSize.x : gameGemSize.x), posY);
                    stoneConver.startCover();

                    boardStoneCovers.push(stoneConver);
                    boardContainer.addChild(stoneConverSprite);
                }
            }

            objectsToScale.push({ sprite: boardContainer, basePosition: { x: gameBoardBasePosition.x, y: gameBoardBasePosition.y } });

            app.stage.addChild(boardContainer);
        }
    }

    //Rescale objects
    //
    {
        rescaleObjects();
        onAudioVolumeChange(100);
        updateCurrencySpanText(document.getElementById('betValueSpan'), walletName);
        updateCurrencySpanText(document.getElementById('currentBalanceSpan'), walletName);
    }

    //Play background music
    //
    mainMusicAudioPlayer.loop = true;

    window.addEventListener('mouseup', function (e) {
        if (!e.target || e.target.type != 'button') {
            if (e.which == 1 && !canStartNewGame()) {
                isFastSpinOnce = true;
            }
        }

        if (mainMusicAudioPlayer.paused) {
            mainMusicAudioPlayer.play();
        }
    });

    window.addEventListener('keydown', function (e) {

        if (mainMusicAudioPlayer.paused) {
            mainMusicAudioPlayer.play();
        }

        if (e.keyCode == 32) {
            playGameOrFastSpinOnce();
            e.preventDefault();
        }
    });

    app.ticker.add((ticker) => {
        //Process spin
        //
        if (gameSpinInfo.spinData && (gameSpinInfo.spinDataIndex < gameSpinInfo.spinData.spins.length || gameSpinInfo.currentSpinStage == 7)) {
            const spin = gameSpinInfo.spinData.spins[gameSpinInfo.spinDataIndex];

            //Set replacements
            //
            if (gameSpinInfo.currentSpinStage == 0) {
                replecmentsGems.length = 0;
                for (let i = 0; i < spin.replacements.length; i++) {
                    replecmentsGems.push(spin.replacements[i]);
                }

                gameSpinInfo.currentSpinStage = 1;
                gameSpinInfo.reelsSpinning = [];

                if (gameSpinInfo.spinDataIndex == 0) {
                    const winSpan = document.getElementById('totalWinSpan');

                    winSpan.dataset.value = 0;
                    winSpan.innerText = '';
                }
            }
            //Stop reels
            //
            else if (gameSpinInfo.currentSpinStage == 1) {
                //Start animation for new found locked symbols
                //
                let i = gameSpinInfo.reelsSpinning.length
                while (i--) {
                    const reel = gameSpinInfo.reelsSpinning[i];
                    if (!reel.isMoving) {
                        const newLocks = spin.newLockedSymbols;
                        const reelStartIndex = reel.index * 3;
                        const reelEndIndex = reelStartIndex + 3;

                        for (let j = 0; j < newLocks.length; j++) {
                            const lockPosition = newLocks[j];
                            if (lockPosition >= reelEndIndex) {
                                continue;
                            }

                            if (lockPosition < reelStartIndex) {
                                continue;
                            }

                            const maxGemIndex = (reel.index + 1) * 4;
                            for (let k = reel.index * 4; k < maxGemIndex; k++) {
                                const gem = boardGems[k];
                                if (gem.position != lockPosition) {
                                    continue;
                                }

                                const sprite = new Sprite(gemsTextureArray[gem.textureIndex].texture);
                                sprite.position.set(gem.sprite.position.x, gem.sprite.position.y);
                                sprite.width = gameGemSize.x;
                                sprite.height = gameGemSize.y;

                                const lockedSymbol = new LockedSymbol(sprite);

                                if (gameSpinInfo.spinDataIndex > 0) {
                                    playAudio(audioAssets.pop);

                                    lockedSymbol.startFoundAnimation();
                                }

                                boardLockedSymbols.push(lockedSymbol);
                                break;
                            }
                        }

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
            } else if (gameSpinInfo.currentSpinStage == 2) { //Payout

                if (gameSpinInfo.spinData.totalWin > 0) {
                    app.stage.addChild(currentWinBitmapTest);

                    const multiWin = spin.spinWin / gameSpinInfo.spinData.bet;

                    if (!audioWinPlayer) {
                        audioWinPlayer = new Audio();
                    }

                    audioWinPlayer.volume = audioVolume;

                    if (multiWin >= 100) {
                        audioWinPlayer.src = audioAssets.winEpicStart;
                    } else if (multiWin >= 20) {
                        audioWinPlayer.src = audioAssets.winLarge;
                    } else {
                        audioWinPlayer.src = audioAssets.win;
                    }

                    audioWinPlayer.play();

                    for (let i = 0; i < boardLockedSymbols.length; i++) {
                        boardLockedSymbols[i].startWinAnimation();
                    }

                    for (let i = 0; i < boardGems.length; i++) {
                        boardGems[i].sprite.tint = 0xb3b0a0;
                    }

                    gameSpinInfo.currentSpinStage = 3;
                } else {
                    gameSpinInfo.currentSpinStage = 6;
                }
            } else if (gameSpinInfo.currentSpinStage == 3) { //Wait for animations end
                const multiWin = spin.spinWin / gameSpinInfo.spinData.bet;

                let countedSpinWin = 0;

                if (isFastSpins()) {
                    countedSpinWin = spin.spinWin;
                    gameSpinInfo.elapsedCountTimeSpinWin = 50000;
                } else {
                    let totalCountSeconds = 1500;

                    if (multiWin >= 100) {
                        totalCountSeconds = 11000;
                    } else if (multiWin >= 20) {
                        totalCountSeconds = 3000;
                    }

                    gameSpinInfo.elapsedCountTimeSpinWin += ticker.deltaMS;

                    countedSpinWin = (gameSpinInfo.elapsedCountTimeSpinWin / totalCountSeconds) * spin.spinWin;
                    if (countedSpinWin > spin.spinWin) {
                        countedSpinWin = spin.spinWin;
                    }
                }

                gameSpinInfo.lastUpdateWinText += ticker.deltaTime;

                if (countedSpinWin == spin.spinWin || gameSpinInfo.lastUpdateWinText >= winTextUpdateInterval) {
                    currentWinBitmapTest.text = (Math.round(convertPointsToLocalCurrency(countedSpinWin) * 100) / 100).toLocaleString();
                    rescaleWinBitmapText();

                    gameSpinInfo.lastUpdateWinText = 0;
                }

                if (countedSpinWin >= spin.spinWin && isLockedSymbolsIdle()) {
                    audioWinPlayer.pause();

                    if (multiWin >= 100) {
                        audioWinPlayer.src = audioAssets.winEpicEnd;
                        audioWinPlayer.play();
                    }

                    const winSpan = document.getElementById('totalWinSpan');

                    winSpan.dataset.value = +spin.spinWin + +winSpan.dataset.value;
                    updateCurrencySpanText(winSpan, walletName);

                    gameSpinInfo.currentSpinStage = 4;
                }

            } else if (gameSpinInfo.currentSpinStage == 4) {
                gameSpinInfo.lastUpdateWinText += ticker.deltaTime;

                if (gameSpinInfo.lastUpdateWinText >= winTextStayInterval) {
                    gameSpinInfo.currentSpinStage = 5;
                }
            } else if (gameSpinInfo.currentSpinStage == 5) { //Wait for locked animation

                app.stage.removeChild(currentWinBitmapTest);

                for (let i = 0; i < boardGems.length; i++) {
                    boardGems[i].sprite.tint = 0xffffff;
                }

                for (let i = 0; i < boardLockedSymbols.length; i++) {
                    boardLockedSymbols[i].createLockedSprite();
                }

                gameSpinInfo.currentSpinStage = 6;
            } else if (gameSpinInfo.currentSpinStage == 6) { //Next spin
                gameSpinInfo.nextSpin();

                if (gameSpinInfo.spinDataIndex < gameSpinInfo.spinData.spins.length) {
                    for (let i = 0; i < boardReelsInfo.length; i++) {
                        const boardReel = boardReelsInfo[i];

                        if (gameSpinInfo.spinDataIndex >= 4) {
                            boardReel.data = reelsData.respinC[i];
                        } else if (gameSpinInfo.spinDataIndex >= 2) {
                            boardReel.data = reelsData.respinB[i];
                        } else if (gameSpinInfo.spinDataIndex >= 1) {
                            boardReel.data = reelsData.respinA[i];
                        }

                        boardReel.startMoving();
                    }

                    const spin = gameSpinInfo.spinData.spins[gameSpinInfo.spinDataIndex];

                    for (let i = 0; i < boardStoneCovers.length; i++) {
                        let realIndex = i > 2 ? i - 3 : i;
                        if ((3 - realIndex) > gameSpinInfo.spinDataIndex) {
                            continue;
                        }

                        const cover = boardStoneCovers[i];
                        cover.startUnConver();
                    }

                    gameSpinInfo.currentSpinStage = 0;
                } else {
                    const currentBalanceText = document.getElementById('currentBalanceSpan');
                    currentBalanceText.dataset.value = gameSpinInfo.spinData.currentBalance;
                    updateCurrencySpanText(currentBalanceText, walletName);

                    const finalWin = gameSpinInfo.spinData.totalWin / gameSpinInfo.spinData.bet;
                    if (finalWin >= 5) {

                        for (let i = 0; i < boardGems.length; i++) {
                            boardGems[i].sprite.tint = 0xb3b0a0;
                        }

                        for (let i = 0; i < boardLockedSymbols.length; i++) {
                            boardLockedSymbols[i].sprite.tint = 0xb3b0a0;
                        }

                        currentWinBitmapTest.text = `Total win:\n${(Math.round(convertPointsToLocalCurrency(gameSpinInfo.spinData.totalWin) * 100) / 100).toLocaleString()}`;
                        rescaleWinBitmapText();
                        app.stage.addChild(currentWinBitmapTest);

                        gameSpinInfo.lastUpdateWinText = 0;
                        gameSpinInfo.currentSpinStage = 7;
                    } else {
                        gameSpinInfo.currentSpinStage = 0;
                    }
                }
            } else if (gameSpinInfo.currentSpinStage == 7) {
                gameSpinInfo.lastUpdateWinText += ticker.deltaTime;

                if (gameSpinInfo.lastUpdateWinText >= (winTextStayInterval * 2) || isFastSpinOnce) {
                    app.stage.removeChild(currentWinBitmapTest);

                    isFastSpinOnce = false;

                    for (let i = 0; i < boardGems.length; i++) {
                        boardGems[i].sprite.tint = 0xffffff;
                    }

                    for (let i = 0; i < boardLockedSymbols.length; i++) {
                        boardLockedSymbols[i].sprite.tint = 0xffffff;
                    }

                    gameSpinInfo.currentSpinStage = 0;
                }
            }
        }

        for (let i = 0; i < boardStoneCovers.length; i++) {
            boardStoneCovers[i].tick(ticker.deltaTime);
        }

        for (let i = 0; i < boardLockedSymbols.length; i++) {
            boardLockedSymbols[i].tick(ticker.deltaTime);
        }

        const rowSize = gameGemSize.y * 4
        const moveValue = reelSpeed * ticker.deltaTime;
        for (let j = 0; j < 5; j++) {
            const rowIndex = j * 4;
            const reelInfo = boardReelsInfo[j];
            if (!reelInfo.isMoving) {
                continue;
            }

            for (let i = 0; i < 4; i++) {
                if (!reelInfo.isMoving) {
                    continue;
                }

                const gem = boardGems[rowIndex + i];
                const sprite = gem.sprite;

                sprite.position.y += moveValue;

                if (sprite.position.y >= rowSize) {
                    sprite.position.y -= rowSize;
                    if (reelInfo.hasSetReel) {
                        const gemReelIndex = (2 - (reelInfo.currentIndex - reelInfo.startIndex));
                        const gemTextureIndex = reelInfo.getTextureRealIndex(reelInfo.startIndex + gemReelIndex);
                        if (gemReelIndex >= 0) {
                            gem.position = j * 3 + gemReelIndex;
                        } else {
                            gem.position = -1;
                        }

                        gem.textureIndex = gemTextureIndex;

                        sprite.texture = gemsTextureArray[gemTextureIndex].texture;
                        reelInfo.currentIndex++;

                        //Check if we already set reel
                        //
                        if (reelInfo.currentIndex - reelInfo.startIndex >= 4) {
                            //Fix position for reels
                            //
                            for (let y = 0; y < 4; y++) {
                                let realIndex = i + y;
                                if (realIndex >= 4) {
                                    realIndex = realIndex - 4;
                                }
                                const nextGem = boardGems[rowIndex + realIndex];
                                nextGem.sprite.position.set(j * gameGemSize.x, y * gameGemSize.y);
                            }
                            reelInfo.stopMoving();
                        }
                    } else {
                        gem.position = -1;
                        sprite.texture = gemsTextureArray[reelInfo.getNextTextureIndex()].texture;
                    }
                }
            }
        }

    });


})();
