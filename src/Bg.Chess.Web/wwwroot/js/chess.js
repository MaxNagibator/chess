const SearchStatuses = {
    NotFound: 'NotFound',
    InProcess: 'InProcess',
    NeedConfirm: 'NeedConfirm',
    NeedConfirmOpponent: 'NeedConfirmOpponent',
    Finish: 'Finish'
}

const GameStatus = {
    Draw: 'Draw',
    InProgress: 'InProgress',
    WaitStart: 'WaitStart',
    WinBlack: 'WinBlack',
    WinWhite: 'WinWhite'
}

const PieceTypes = {
    Bishop: 'Bishop',
    King: 'King',
    Knight: 'Knight',
    Pawn: 'Pawn',
    Queen: 'Queen',
    Rook: 'Rook'
}

const Side = {
    White: 'White',
    Black: 'Black'
}

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
            var response = JSON.parse(data.responseText);
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
    var isOk = confirm("Игра найдена! Играем?");
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

function move(fromX, fromY, toX, toY) {
    SendRequest({
        url: '/Chess/Move',
        method: 'POST',
        body: {
            fromX: fromX,
            fromY: fromY,
            toX: toX,
            toY: toY,
        },
        success: function (data) {
            var data2 = JSON.parse(data.responseText);
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
            var data2 = JSON.parse(data.responseText);
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

    if (game.status == GameStatus.InProgress) {
        initField(data2.notation, data2.availableMoves);
        if (game.mySide == game.stepSide) {
            checkEnemyStep = -1;
        } else {
            checkEnemyStep = 217;
        }
    } else {
        checkEnemyStep = -1;
        if (game.status == GameStatus.WaitStart) {
            console.log('wow, bad status ' + game.status);
        } else if (game.status == GameStatus.WinWhite) {
            if (game.mySide == Side.White) {
                alert('you won!');
            } else {
                alert('you lose!');
            }
        } else if (game.status == GameStatus.WinBlack) {
            if (game.mySide == Side.Black) {
                alert('you won!');
            } else {
                alert('you lose!');
            }
        } else if (game.status == GameStatus.Draw) {
            alert('draw!');
        }
    }
}

function initField(notation, availableMoves) {

    function getPieceByNotation(pos) {
        function getTypeByChar(char) {
            switch (char) {
                case 'R':
                    return PieceTypes.Rook;
                case 'N':
                    return PieceTypes.Knight;
                case 'B':
                    return PieceTypes.Bishop;
                case 'Q':
                    return PieceTypes.Queen;
                case 'K':
                    return PieceTypes.King;
                case 'P':
                    return PieceTypes.Pawn;
                default:
                    console.error('type not recognized: ' + char);
                    return null;
            }
        }

        var toUpper = pos.toUpperCase();

        var piece = {
            Side: pos === toUpper ? Side.White : Side.Black,
            Type: getTypeByChar(toUpper),
        };

        return piece;
    }
    console.log(notation);

    //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1

    let notationParts = notation.split(' ');
    let pieceLocation = notationParts[0];
    let pieceLocationLines = pieceLocation.split('/');
    let cellColorIndex = 0;
    var target = document.querySelector("#field");
    target.innerHTML = '';
    target.classList.add('side-' + game.mySide.toLowerCase());
    var draggables = [];
    var dropZones = [];

    for (let i = 0; i < 8; i++) {
        let line = pieceLocationLines[i];
        var divLine = document.createElement('div');
        divLine.classList.add('line');
        target.appendChild(divLine);
        var posX = 0;
        for (let posIndex = 0; posIndex < line.length; posIndex++) {
            var posY = 7 - i;
            let pos = line[posIndex];
            var emptyFields = pos * 1;
            if (Number.isInteger(emptyFields)) {
                for (let cellsCount = emptyFields; cellsCount > 0; cellsCount--) {
                    // todo обобщить с дублированием снизу
                    var div = document.createElement('div');
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
                var div = document.createElement('div');
                divLine.appendChild(div);
                div.classList.add('column');
                if (cellColorIndex % 2 === 0) {
                    div.classList.add('position-white');
                } else {
                    div.classList.add('position-black');
                }
                div.setAttribute('data-position-x', posX);
                div.setAttribute('data-position-y', posY);

                divLine.appendChild(div);
                cellColorIndex++;
                let piece = getPieceByNotation(pos);
                var img = document.createElement('img');
                var imgSrcName = piece.Type + "-" + piece.Side + '.png';
                img.src = '/Content/Images/Piece/' + imgSrcName;
                div.appendChild(img);
                dropZones.push(div);
                if (game.mySide == piece.Side) {
                    draggables.push(img);
                }
                posX++;
            }
        }

        cellColorIndex++;
        var target = document.querySelector("#field");
    }

    let dnd_successful;
    for (let i = 0; i < draggables.length; i++) {
        draggables[i].addEventListener('dragstart', function (event) {
            event.target.classList.add('piece-select');

            let posX = event.target.parentElement.getAttribute('data-position-x');
            let posY = event.target.parentElement.getAttribute('data-position-y');
            var moves = getMoves(availableMoves, posX, posY);
            var cells = document.getElementsByClassName('column');
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
                let fromX = event.target.parentElement.getAttribute('data-position-x');
                let fromY = event.target.parentElement.getAttribute('data-position-y');
                let toX = placeForDropPiece.getAttribute('data-position-x');
                let toY = placeForDropPiece.getAttribute('data-position-y');
                move(fromX, fromY, toX, toY);
            }
            else {
            }
            event.target.classList.remove('piece-select');

            var cells = document.getElementsByClassName('column');
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

            //todo переименовать column в cell/position/...
            if ((event.target.classList.contains('column')
                && event.target.classList.contains('piece-move-target-good'))

                || (event.target.parentElement.classList.contains('column')
                    && event.target.parentElement.classList.contains('piece-move-target-good'))
            ) {
                //event.target.classList.remove('piece-move-target-good');
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

setInterval(function () {
    if (checkEnemyStep == -1) {
        // заполнить статус лейбл, ваш ход
        return;
    } else {
        // заполнить статус лейбл "ход оппонента"
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