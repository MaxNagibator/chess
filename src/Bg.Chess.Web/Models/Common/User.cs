namespace Bg.Chess.Web.Models.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Пользователь.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Почта подтверждена.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
    }
}
