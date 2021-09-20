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

function goConfirmAlert() {
    let isOk = confirm("Игра найдена! Играем?");
    if (isOk) {
        SendRequest({
            url: '/Chess/ConfirmSearch',
            data: {
            },
            success: function (data) {
            },
            always: function (data) {
                checkSearchInProcess = false;
            }
        });
    } else {
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
    game.mySide = data2.side;
    game.stepSide = data2.stepSide;
    game.status = data2.status;
    game.isFinish = data2.isFinish;
    game.finishReason = data2.finishReason;
    game.winSide = data2.winSide;

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

    if (game.isFinish == false) {
        document.getElementById('gameBlock').classList.add('game-status-process');
        initField(data2.notation, data2.availableMoves);
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

        initField(data2.notation);

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
}

function initField(notation, availableMoves) {

    console.log(notation);

    //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1

    let notationParts = notation.split(' ');

    // todo !!!!! запомнить положение фигур и если положение фигур не менялось. не перерисовывать
    let pieceLocation = notationParts[0];
    let pieceLocationLines = pieceLocation.split('/');
    let cellColorIndex = 0;
    let target = document.querySelector("#field");
    target.innerHTML = '';
    target.classList.add('side-' + game.mySide.toLowerCase());
    let draggables = [];
    let dropZones = [];
    let labels = {
        vertical: "12345678",
        horizontal: "ABCDEFGH"
    }

    function horizontalLabel(borderPos) {
        let divNumber1Line = document.createElement('div');
        divNumber1Line.classList.add('line');
        target.appendChild(divNumber1Line);
        for (let i = -1; i <= 8; i++) {
            let div = document.createElement('div');
            div.classList.add('column');
            div.classList.add('column-label');

            let label = document.createElement('label');
            label.classList.add('field-label');
            if (i != -1 && i != 8) {
                label.innerHTML = labels.horizontal[i];
                if (borderPos) {
                    div.classList.add('field-border-bottom');
                } else {
                    div.classList.add('field-border-top');
                }
            }
            div.appendChild(label);
            divNumber1Line.appendChild(div);
        }
    }

    function verticalLabel(divLine, rowIndex, borderPos) {
        let div = document.createElement('div');
        div.classList.add('column');
        div.classList.add('column-label');
        if (borderPos) {
            div.classList.add('field-border-left');
        } else {
            div.classList.add('field-border-right');
        }
        let label = document.createElement('label');
        label.classList.add('field-label');
        label.innerHTML = labels.vertical[7 - rowIndex];
        div.appendChild(label);
        divLine.appendChild(div);
    }

    horizontalLabel(true);

    for (let i = 0; i < 8; i++) {
        let line = pieceLocationLines[i];
        let divLine = document.createElement('div');
        divLine.classList.add('line');
        target.appendChild(divLine);
        let posX = 0;

        verticalLabel(divLine, i, false);

        for (let posIndex = 0; posIndex < line.length; posIndex++) {
            let posY = 7 - i;
            let pos = line[posIndex];
            let emptyFields = pos * 1;
            if (Number.isInteger(emptyFields)) {
                for (let cellsCount = emptyFields; cellsCount > 0; cellsCount--) {
                    // todo обобщить с дублированием снизу
                    let div = document.createElement('div');
                    div.classList.add('column');
                    if (cellColorIndex % 2 === 0) {
                        div.classList.add('position-white');
                    } else {
                        div.classList.add('position-black');
                    }
                    div.setAttribute('data-position-x', posX);
                    div.setAttribute('data-position-y', posY);

                    divLine.appendChild(div);
                    dropZones.push(div);
                    cellColorIndex++;
                    posX++;
                }

            } else {
                let div = document.createElement('div');
                divLine.appendChild(div);
                div.classList.add('column');
                if (cellColorIndex % 2 === 0) {
                    div.classList.add('position-white');
                } else {
                    div.classList.add('position-black');
                }

                let piece = GetPieceByNotation(pos);
                div.setAttribute('data-piece-type', piece.Type);
                div.setAttribute('data-piece-side', piece.Side);
                div.setAttribute('data-position-x', posX);
                div.setAttribute('data-position-y', posY);

                divLine.appendChild(div);
                cellColorIndex++;
                let img = document.createElement('img');
                let imgSrcName = piece.Type + "-" + piece.Side + '.png';
                img.src = '/Content/Images/Piece/' + imgSrcName;
                div.appendChild(img);
                dropZones.push(div);
                if (game.mySide == piece.Side) {
                    draggables.push(img);
                }
                posX++;
            }
        }

        verticalLabel(divLine, i, true);

        cellColorIndex++;
    }

    horizontalLabel(false);

    if (availableMoves != undefined) {
        let dnd_successful;
        for (let i = 0; i < draggables.length; i++) {
            draggables[i].addEventListener('dragstart', function (event) {
                event.target.classList.add('piece-select');

                let posX = event.target.parentElement.getAttribute('data-position-x');
                let posY = event.target.parentElement.getAttribute('data-position-y');
                let moves = getMoves(availableMoves, posX, posY);
                let cells = document.getElementsByClassName('column');
                for (let moveIndex = 0; moveIndex < moves.length; moveIndex++) {
                    for (let cellIndex = 0; cellIndex < cells.length; cellIndex++) {
                        if (cells[cellIndex].getAttribute('data-position-x') == moves[moveIndex].x
                            && cells[cellIndex].getAttribute('data-position-y') == moves[moveIndex].y) {
                            cells[cellIndex].classList.add('piece-move-target-good');
                        }
                    }
                }
                event.dataTransfer.effectAllowed = "move";
                dnd_successful = false;
            });

            draggables[i].addEventListener('dragend', function (event) {
                if (dnd_successful) {
                    let pieceType = event.target.parentElement.getAttribute('data-piece-type');
                    let pieceSide = event.target.parentElement.getAttribute('data-piece-side');
                    let fromX = event.target.parentElement.getAttribute('data-position-x');
                    let fromY = event.target.parentElement.getAttribute('data-position-y');
                    let toX = placeForDropPiece.getAttribute('data-position-x');
                    let toY = placeForDropPiece.getAttribute('data-position-y');
                    move(pieceType, pieceSide, fromX, fromY, toX, toY);
                }
                else {
                }
                event.target.classList.remove('piece-select');

                let cells = document.getElementsByClassName('column');
                for (let cellIndex = 0; cellIndex < cells.length; cellIndex++) {
                    cells[cellIndex].classList.remove('piece-move-target-good');
                    cells[cellIndex].classList.remove('piece-move-target-bad');
                }
            });
        }

        let placeForDropPiece;
        for (let i = 0; i < dropZones.length; i++) {
            dropZones[i].addEventListener('dragenter', function (event) {
                if (event.target.classList.contains('column') &&
                    !event.target.classList.contains('piece-move-target-good')) {
                    event.target.classList.add('piece-move-target-bad');
                }
                event.preventDefault();
            });

            dropZones[i].addEventListener('dragleave', function (event) {
                if (event.target.classList.contains('column') &&
                    !event.target.classList.contains('piece-move-target-good')) {
                    event.target.classList.remove('piece-move-target-bad');
                }
            });

            dropZones[i].addEventListener('dragover', function (event) {
                event.dataTransfer.dropEffect = "move";
                event.preventDefault();
                return false;
            });

            dropZones[i].addEventListener('drop', function (event) {

                if ((event.target.classList.contains('column')
                    && event.target.classList.contains('piece-move-target-good'))

                    || (event.target.parentElement.classList.contains('column')
                        && event.target.parentElement.classList.contains('piece-move-target-good'))
                ) {
                    placeForDropPiece = event.target;
                    if (!placeForDropPiece.classList.contains('column')) {
                        placeForDropPiece = placeForDropPiece.parentElement;
                    }
                    dnd_successful = true;
                    event.preventDefault();
                }
            });
        }
    }
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