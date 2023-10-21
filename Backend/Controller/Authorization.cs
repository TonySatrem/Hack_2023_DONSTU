using Backend.DataBaseController;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Backend.Controller
{
    public static class Authorization
    {
        public static byte[] PrivateKey()
        {
            return Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        }

        private static bool TokenComparer(string tokenString1, string tokenString2)
        {
            if (tokenString1 == null || tokenString2 == null)
                return false;

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken1 = jwtTokenHandler.ReadJwtToken(tokenString1);
            var jwtToken2 = jwtTokenHandler.ReadJwtToken(tokenString2);

            var uniqueName1 = jwtToken1.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
            var uniqueName2 = jwtToken2.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            return uniqueName1 == uniqueName2;
        }

        public static AccountUser? Authorizate(int id, ApplicationContext db, HttpContext context, byte[] key)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null || !TokenComparer(user.token, token))
                return null;

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}
