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
    Rook: 'Rook',
    Dragon: 'Dragon',
    Soldier: 'Soldier',
    Hydra: 'Hydra',
}

const Side = {
    White: 'White',
    Black: 'Black',
    Spectator: 'Spectator'
}

const Labels = {
    Vertical: "12345678",
    Horizontal: "ABCDEFGHIJ"
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
            case 'D':
                return PieceTypes.Dragon;
            case 'S':
                return PieceTypes.Soldier;
            case 'H':
                return PieceTypes.Hydra;
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