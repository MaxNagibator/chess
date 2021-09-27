const SearchStatuses = {
    NotFound: 'NotFound',
    InProcess: 'InProcess',
    NeedConfirm: 'NeedConfirm',
    NeedConfirmOpponent: 'NeedConfirmOpponent',
    Finish: 'Finish'
}

const FinishReason = {
    Mate: 'Mate',
    Surrender: 'Surrender',
    TimeOver: 'TimeOver',
    Draw: 'Draw',
    DrawByAgreement: 'DrawByAgreement',
    DrawByTime: 'DrawByTime',
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
    Black: 'Black',
    Spectator: 'Spectator'
}

const Labels = {
    Vertical: "12345678",
    Horizontal: "ABCDEFGH"
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

// чтоб модалка не закрывалась, если кликнуть мимо или ESC
$.fn.modal.prototype.constructor.Constructor.Default.keyboard = false;
$.fn.modal.prototype.constructor.Constructor.Default.backdrop = 'static';