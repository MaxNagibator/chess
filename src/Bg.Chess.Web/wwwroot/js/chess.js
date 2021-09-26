let checkEnemyStep = -1;
let checkEnemyStepInProcess = false;

let searchRequestInProcess = false;
function startSearch() {
    if (searchRequestInProcess) {
        return;
    }
    searchRequestInProcess = true;
    SendRequest({
        url: '/Chess/StartSearch',
        success: function (data) {
            let response = JSON.parse(data.responseText);
            if (response.error) {
                alert(response.message);
            } else {
                currentSearchId = 217;
                startTime = new Date();
                document.getElementById('searchBlock').classList.add('in-process');
            }
        },
        always: function (data) {
            searchRequestInProcess = false;
        }
    });
}

let currentSearchId = 999;
let startTime = null;

let checkSearchInProcess = false;

setInterval(function () {
    if (currentSearchId == -1) {
        return;
    }
    if (checkSearchInProcess) {
        return;
    }
    checkSearchInProcess = true;
    setTimeout(function () {
        SendRequest({
            url: '/Chess/CheckSearch',
            data: {
            },
            success: function (data) {
                let response = JSON.parse(data.responseText);
                let status = response.status;
                document.getElementById('searchSpan2').innerHTML = '';
                if (status == SearchStatuses.NeedConfirm) {
                    goConfirmAlert();
                } else {
                    if (status == SearchStatuses.NotFound) {
                        currentSearchId = -1;
                        document.getElementById('searchBlock').classList.remove('in-process');
                    } else if (status == SearchStatuses.InProcess) {
                        document.getElementById('searchSpan').innerHTML = document.getElementById('searchSpan').innerHTML + '.';
                    } else if (status == SearchStatuses.NeedConfirmOpponent) {
                        document.getElementById('searchSpan2').innerHTML = 'Ожидается подтверждение оппонента';
                    } else if (status == SearchStatuses.Finish) {
                        currentSearchId = -1;
                        document.getElementById('searchBlock').classList.remove('in-process');
                        goGame();
                    } else {
                        console.error('unrecognized status -> ' + status);
                    }
                    checkSearchInProcess = false;
                }
            },
            always: function (data) {
            }
        });
    }, 1000);
}, 1000);

//сделать обёртку для показа модалок, а то чёто много дублирования
let confirmStartGameModalDom = null;
let confirmStartGameModal = null;
function goConfirmAlert() {
    if (confirmStartGameModal == null) {
        confirmStartGameModalDom = document.getElementById('confirmStartGameModal');
        confirmStartGameModal = new bootstrap.Modal(confirmStartGameModalDom);
    }
    confirmStartGameModal.show();
}

function confirmStart(isOk) {
    if (isOk) {
        SendRequest({
            url: '/Chess/ConfirmSearch',
            data: {
            },
            success: function (data) {
                checkSearchInProcess = false;
                confirmStartGameModal.hide();
            },
            always: function (data) {
            }
        });
    } else {
        SendRequest({
            url: '/Chess/StopSearch',
            data: {
            },
            success: function (data) {
                checkSearchInProcess = false;
                confirmStartGameModal.hide();
                document.getElementById('searchBlock').classList.remove('in-process');
            },
            always: function (data) {
            }
        });
    }
}

function stopSearch() {
    SendRequest({
        url: '/Chess/StopSearch',
        data: {
        },
        success: function (data) {
            document.getElementById('searchBlock').classList.remove('in-process');
        },
        always: function (data) {
            checkSearchInProcess = false;
        }
    });
}

let game = {
    mySide: null,
    stepSide: null,
    status: null
};

let pawnTransformPieceModalSendRequest;
let pawnTransformPieceModalDom = null;
let pawnTransformPieceModal = null;
function move(pieceType, pieceSide, fromX, fromY, toX, toY) {
    if (pawnTransformPieceModal == null) {
        pawnTransformPieceModalDom = document.getElementById('pawnTransformPieceModal');
        pawnTransformPieceModal = new bootstrap.Modal(pawnTransformPieceModalDom);
    }
    let sendRequest = function (pawnTransformPiece) {
        SendRequest({
            url: '/Chess/Move',
            method: 'POST',
            body: {
                fromX: fromX,
                fromY: fromY,
                toX: toX,
                toY: toY,
                pawnTransformPiece: pawnTransformPiece
            },
            success: function (data) {
                if (pawnTransformPiece != null) {
                    pawnTransformPieceModal.hide();
                }
                let data2 = JSON.parse(data.responseText);
                initGame(data2);
            },
            error: function () {
                alert('shadow bolt');
            }
        });
    }

    if (pieceType == PieceTypes.Pawn) {
        if ((toY == 7 && pieceSide == Side.White) || (toY == 0 && pieceSide == Side.Black)) {
            pawnTransformPieceModalSendRequest = sendRequest;
            document.getElementsByClassName('pawn-transform-piece-select')[0].classList.add('select-' + pieceSide.toLowerCase());
            pawnTransformPieceModal.show();
            return;
        }
    }

    sendRequest(null);
}

function selectPawnTransformPiece(pieceType) {
    pawnTransformPieceModalSendRequest(pieceType);
}

let surrenderModalDom = null;
let surrenderModal = null;

function surrenderInitial() {
    if (surrenderModal == null) {
        surrenderModalDom = document.getElementById('surrenderModal');
        surrenderModal = new bootstrap.Modal(surrenderModalDom);
    }
    surrenderModal.show();
}

function surrender() {
    SendRequest({
        url: '/Chess/Surrender',
        method: 'POST',
        body: {
        },
        success: function (data) {
            surrenderModal.hide();

            let data2 = JSON.parse(data.responseText);
            initGame(data2);
        },
        error: function () {
            alert('shadow bolt');
        }
    });
}

function goGame(alwaysCallback) {
    SendRequest({
        url: '/Chess/GetGame',
        method: 'POST',
        success: function (data) {
            let data2 = JSON.parse(data.responseText);
            initGame(data2);
        },
        always: function () {
            if (alwaysCallback != undefined) {
                alwaysCallback();
            }
        }
    });
}

function initGame(data2) {
    game.id = data2.id;
    game.mySide = data2.side;
    game.stepSide = data2.stepSide;
    game.status = data2.status;
    game.isFinish = data2.isFinish;
    game.finishReason = data2.finishReason;
    game.winSide = data2.winSide;
    game.availableMoves = data2.availableMoves;
    game.notation = data2.notation;
    game.historyMoves = data2.historyMoves;

    let myWinLabel = document.getElementsByClassName('game-my-win')[0];
    let notMyWinLabel = document.getElementsByClassName('game-not-my-win')[0];
    let drawLabel = document.getElementsByClassName('game-draw')[0];
    let finishReasonLabel = document.getElementsByClassName('game-finish-reason')[0];
    myWinLabel.classList.add('hidden');
    notMyWinLabel.classList.add('hidden');
    drawLabel.classList.add('hidden');
    finishReasonLabel.classList.add('hidden');

    let notMyStepLabel = document.getElementsByClassName('game-not-my-step')[0];
    let myStepLabel = document.getElementsByClassName('game-my-step')[0];
    notMyStepLabel.classList.add('hidden');
    myStepLabel.classList.add('hidden');


    let gameLink = document.getElementById('gameLink');
    gameLink.classList.remove('hidden');
    gameLink.href = '/History/' + game.id;

    if (game.isFinish == false) {
        document.getElementById('gameBlock').classList.add('game-status-process');
        initField("field", game.notation, game.availableMoves, game.mySide);
        if (game.mySide == game.stepSide) {
            checkEnemyStep = -1;
            myStepLabel.classList.remove('hidden');
        } else {
            checkEnemyStep = 217;
            notMyStepLabel.classList.remove('hidden');
        }
    } else {
        document.getElementById('gameBlock').classList.remove('game-status-process');
        checkEnemyStep = -1;

        initField("field", game.notation, null, game.mySide);

        if (game.winSide == null) {
            drawLabel.classList.remove('hidden');
        } else {
            if (game.winSide == game.mySide) {
                myWinLabel.classList.remove('hidden');
            } else {
                notMyWinLabel.classList.remove('hidden');
            }

            finishReasonLabel.classList.remove('hidden');
            if (game.finishReason == FinishReason.Mate) {
                finishReasonLabel.innerHTML = 'Поставлен мат';
            } else if (game.finishReason == FinishReason.Surrender) {
                finishReasonLabel.innerHTML = 'Признанное поражение';
            } else if (game.finishReason == FinishReason.TimeOver) {
                finishReasonLabel.innerHTML = 'Время закончилось';
            } else {
                finishReasonLabel.innerHTML = game.finishReason;
            }
        }
    }

    initHistory('historyBlock', game.historyMoves);
}

setInterval(function () {
    if (checkEnemyStep == -1) {
        return;
    }

    if (checkEnemyStepInProcess) {
        return;
    }

    checkEnemyStepInProcess = true;

    goGame(function () {
        if (game.mySide == game.stepSide) {
            checkEnemyStep = -1;
        }
        checkEnemyStepInProcess = false;
    });
}, 1000);

function getMoves(availableMoves, x, y) {
    for (let i = 0; i < availableMoves.length; i++) {
        if (availableMoves[i].from.x == x
            && availableMoves[i].from.y == y) {
            return availableMoves[i].to;
        }
    }
    console.error('not found moves for piece in ' + x + '/' + y);
}