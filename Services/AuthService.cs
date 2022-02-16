using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FoosballApi.Data;
using FoosballApi.Helpers;
using FoosballApi.Models;
using FoosballApi.Models.Accounts;
using Microsoft.IdentityModel.Tokens;

namespace FoosballApi.Services
{
    public interface IAuthService
    {
        User Authenticate(string username, string password);
        void CreateUser(User user);
        void VerifyEmail(string token);
        VerificationModel ForgotPassword(ForgotPasswordRequest model, string origin);
        bool SaveChanges();
        VerificationModel AddVerificationInfo(User user, string origin);
        void ResetPassword(ResetPasswordRequest model);
        string CreateToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        private readonly Secrets _secrets;

        public AuthService(DataContext context, Secrets secrets)
        {
            _context = context;
            _secrets = secrets;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == email);

            // check if username exists
            if (user == null)
                return null;

            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!verified)
                return null;

            return user;
        }

        public void CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            // get random number
            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 99999);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            DateTime now = DateTime.Now;
            User tmpUser = new User();
            tmpUser.Email = user.Email;
            tmpUser.Password = passwordHash;
            tmpUser.FirstName = user.FirstName;
            tmpUser.LastName = user.LastName;
            tmpUser.Created_at = now;
            tmpUser.PhotoUrl = "https://avatars.dicebear.com/api/personas/:" + randomNumber + ".png";
            _context.Users.Add(tmpUser);
            _context.SaveChanges();
        }

        public VerificationModel ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Users.SingleOrDefault(x => x.Email == model.Email);

            VerificationModel vModel = _context.Verifications.SingleOrDefault(x => x.UserId == account.Id);

            // always return ok response to prevent email enumeration
            if (account == null) return null;

            // create reset token that expires after 1 day
            vModel.PasswordResetToken = RandomTokenString();
            vModel.PasswordResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Verifications.Update(vModel);
            _context.SaveChanges();

            return vModel;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void VerifyEmail(string token)
        {
            var account = _context.Verifications.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            account.HasVerified = true;
            account.VerificationToken = null;

            _context.Verifications.Update(account);
            _context.SaveChanges();
        }

        public VerificationModel AddVerificationInfo(User user, string origin)
        {
            VerificationModel vModel = new VerificationModel();
            vModel.UserId = user.Id;
            vModel.VerificationToken = RandomTokenString();
            vModel.HasVerified = false;
            _context.Verifications.Add(vModel);
            _context.SaveChanges();

            return vModel;
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            VerificationModel vModel = _context.Verifications.SingleOrDefault(x =>
                x.PasswordResetToken == model.Token &&
                x.PasswordResetTokenExpires > DateTime.UtcNow);

            if (vModel == null)
                throw new AppException("Invalid token");

            User user = _context.Users.SingleOrDefault(x => x.Id == vModel.UserId);

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // update password and remove reset token
            user.Password = passwordHash;
            vModel.PasswordResetTokenExpires = DateTime.UtcNow;
            vModel.PasswordResetToken = null;

            _context.Verifications.Update(vModel);
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secrets.JWTSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim("CurrentOrganisationId", user.CurrentOrganisationId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

    }
}