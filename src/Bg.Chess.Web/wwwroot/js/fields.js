function initField(fieldSelector, notation, availableMoves, mySide) {

    console.log(notation);

    //rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1

    let notationParts = notation.split(' ');

    // todo !!!!! запомнить положение фигур и если положение фигур не менялось. не перерисовывать
    let pieceLocation = notationParts[0];
    let pieceLocationLines = pieceLocation.split('/');
    let cellColorIndex = 0;
    let target = document.getElementById(fieldSelector);
    //let target = document.querySelector(fieldSelector);
    target.innerHTML = '';
    if (mySide == Side.Spectator) {
        target.classList.add('side-' + Side.White.toLowerCase());
    } else {
        target.classList.add('side-' + mySide.toLowerCase());
    }
    let draggables = [];
    let dropZones = [];

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
                label.innerHTML = Labels.Horizontal[i];
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
        label.innerHTML = Labels.Vertical[7 - rowIndex];
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
                if (mySide == piece.Side) {
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

function initHistory(historySelector, moves, mySide) {
    var historyBlock = document.getElementById(historySelector);
    historyBlock.innerHTML = '';
    for (let i = moves.length - 1; i >= 0; i--) {
        let move = moves[i];
        let moveDiv = document.createElement('div');
        moveDiv.classList.add('history-item');
        let from = Labels.Horizontal[move.from.x];
        from += Labels.Vertical[move.from.y];
        let to = Labels.Horizontal[move.to.x];
        to += Labels.Vertical[move.to.y];
        moveDiv.innerHTML = from + '->' + to;
        historyBlock.appendChild(moveDiv);

        asdasd
        if (mySide == game.stepSide && i == moves.length - 1) {
            let cells = document.getElementsByClassName('column');
            for (let moveIndex = 0; moveIndex < moves.length; moveIndex++) {
                for (let cellIndex = 0; cellIndex < cells.length; cellIndex++) {
                    if (cells[cellIndex].getAttribute('data-position-x') == move.from.x
                        && cells[cellIndex].getAttribute('data-position-y') == move.from.y) {
                        cells[cellIndex].classList.add('piece-is-last-move-from');
                    }
                    if (cells[cellIndex].getAttribute('data-position-x') == move.to.x
                        && cells[cellIndex].getAttribute('data-position-y') == move.to.y) {
                        cells[cellIndex].classList.add('piece-is-last-move-to');
                    }
                }
            }
        }
    }
}