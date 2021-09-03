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

function GetPieceByNotation(pos) {
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