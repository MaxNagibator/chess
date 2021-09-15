namespace Bg.Chess.Data.Repo
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;

    public interface IUserRepo
    {
        IdentityUser GetUser(string id);
    }

    public class UserRepo : IUserRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IdentityUser GetUser(string id)
        {
            var player = _unitOfWork.Context.Users
                .First(x => x.Id == id);
            return player;
        }
    }
}
