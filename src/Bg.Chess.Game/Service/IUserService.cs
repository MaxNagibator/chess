namespace Bg.Chess.Game
{
    using Bg.Chess.Data.Repo;

    using Microsoft.AspNetCore.Identity;

    public interface IUserService
    {
        User GetUser(string id);
    }

    public class UserService : IUserService
    {
        private IUserRepo _userRepo;

        public UserService(IUserRepo playerRepo)
        {
            _userRepo = playerRepo;
        }

        public User GetUser(string id)
        {
            var dbUser = _userRepo.GetUser(id);
            return FillUserDto(dbUser);
        }

        private User FillUserDto(IdentityUser dbUser)
        {
            var userDto = new User();
            userDto.Id = dbUser.Id;
            userDto.IsEmailConfirmed = dbUser.EmailConfirmed;
            return userDto;
        }
    }
}
