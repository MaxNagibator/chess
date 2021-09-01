using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bg.Chess.Web.Data
{
    [Table("ChessGame")]
    public class ChessGame
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int WhitePlayerId { get; set; }

        public int BlackPlayerId { get; set; }

        public string Data { get; set; }
    }
}
