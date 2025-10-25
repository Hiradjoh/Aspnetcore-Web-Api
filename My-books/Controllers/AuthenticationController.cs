using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using My_books.Data.Models;
using My_books.Data.ViewModels.Authentication;
using Newtonsoft.Json.Converters;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace My_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;//manage the users
        private readonly RoleManager<IdentityRole> _roleManager;//manage roles
        private readonly ProjectDbContext _context;
        private readonly IConfiguration _configuration;//baraye dastresi be appsettings.json
        //RefreshTokens
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ProjectDbContext context,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("please , provide all required fields");
            }
            var userExists= await _userManager.FindByEmailAsync(payload.Email);
            if (userExists != null)
            {
                return BadRequest($"User {payload.Email} Already exists");
            }
            ApplicationUser newUser=new ApplicationUser()
            {
                Email=payload.Email,
                UserName=payload.UserName,
                SecurityStamp=Guid.NewGuid().ToString()// shenase amniati dar soorat daqir password ya log out estefade mishe 
            };
            var result=await _userManager.CreateAsync(newUser,payload.Password);//user misaze dar database va password be soorat hash save mikone 
            if (!result.Succeeded)
            {
                return BadRequest("User could not be created");
            }
            return Created(nameof(Register), $"User {payload.Email} Already Created");
        }

        [HttpPost("Login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("please , provide all required fields");
            }
            var user = await _userManager.FindByEmailAsync(payload.Email);// check kardan vojoode user ba email dade shode

            if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password)) //agar user vojoood dasht va password dorost bood
            {
             var tokenvalue=await GenerateJwtTokenAsync(user,"");   
            return Ok (tokenvalue);
            }

            return Unauthorized("Invalid Email or Password");
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]TokenRequestVM payload )
        {
            try
            { 
                var result = await VerifyAndGenerateTokenAsync(payload);// baraye verify kardan token va sakhte token jadid

                if (result == null) return BadRequest("Invalid tokens");

                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }





        private async Task<AuthResultVM> VerifyAndGenerateTokenAsync(TokenRequestVM payload)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();// baraye baresi kardan token ha 
            try
            {
                //Check 1-check Jwt token Format
                var tokenInVerification = jwtTokenHandler.ValidateToken(payload.Token, _tokenValidationParameters//check kardan secret key / audience , issuer 
                    , out var validatedToken);//agar token sahih bood token valid mishe



                //Check 2-Encryption Algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,//motmaen mishim algoritm JWt hamoon Hmac Sha256 hast 
                        StringComparison.InvariantCultureIgnoreCase);//barresi algoritm emza token
                    if (result == false) return null;
                }



                //check 3- validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type ==
                JwtRegisteredClaimNames.Exp).Value);// daryafte zaman monqazi shodan token az dakhel claim haye token

                var expiryDate = UnixTimeStampToDateTimeInUtc(utcExpiryDate);// tabdil zaman monqazi shode az format unix be DateTime
                if (expiryDate > DateTime.UtcNow) throw new Exception("Token has not expired yet!");// check kardan monqazi shodan token 



                //check 4 -refresh token exists in the DB
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);//check kardan inke refresh token voojood dare ya na 


                if (dbRefreshToken == null) throw new Exception("Refresh token does not exists in our Db ");// ag voojood nadashte bashe 

                else //ag vojood dashte bashe 
                {
                    //check 5--validateId
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;// daryafte jwtid az dakhel claim haye token
                    if (dbRefreshToken.JwtId != jti) throw new Exception("Token dos not match");// check kardan inke jwt id dakhel refresh token ba jti dakhel token barabar bashe


                    //check6--Refresh token  expiration
                    if (dbRefreshToken.DateExpire <= DateTime.UtcNow) throw new Exception("Your refresh token hast expired,please re-authenticate!");// check kardan monqazi shodan refresh token


                    //check 7-- Refresh token Revoked 
                    if (dbRefreshToken.IsRevoked) throw new Exception("Refresh token is revoke");// check kardan inke refresh token revoke ya monqazi  nashode bashe


                    //Generate new token (with existing refresh token)
                    var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);// daryafte etelaat user ba estefade az user id dakhel refresh token
                    var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);// sakhte token jadid ba estefade az refresh token ghabli
                    return await newTokenResponse;

                }
            }
            catch (SecurityTokenExpiredException)// age token monqazi shode bashe
            {
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);//check kardan inke refresh token voojood dare ya na
                //Generate new token (with existing refresh token)
                var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);// daryafte etelaat user ba estefade az user id dakhel refresh token
                var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);// sakhte token jadid ba estefade az refresh token ghabli
                return await newTokenResponse; ;
            }
            catch (Exception ex)// sayer error haye deg 
            {
                throw new Exception(ex.Message);
            }
            
           
        }

        private async Task <AuthResultVM> GenerateJwtTokenAsync(ApplicationUser user, string existingrefreshToken)
        {
            var authClaims = new List<Claim>() //etelaati ke dakhel jwt zakhire mishan 
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),//unique id baraye har token
            };
            //sabet mikone in token vaqan az server ma sakhte shode na kase deg
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
         
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],//kodom system ya server in sakhte 
                audience: _configuration["JWT:Audience"],//baraye che kasi in token sakhte shode
                                                         //masalan in token faqat baraye kaebaraye appliction mobile esteade mikonan sakhte shode 

                expires: DateTime.UtcNow.AddMinutes(1),//5-10mins
                claims: authClaims,//etelaat user dar token
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)//emza kardan token ba estefade az emza va algoritm emza
                );
            var jwtToken=new JwtSecurityTokenHandler().WriteToken(token);// tabdil token be string baraye ersal be client

            var refreshToken = new RefreshToken();
            
            if (string.IsNullOrEmpty(existingrefreshToken))
            {
                 refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),

                };
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }

            var response = new AuthResultVM()//modeli ke baraye javab dadan be client misazim
            {
                Token = jwtToken,
                RefreshToken =  (string.IsNullOrEmpty(existingrefreshToken))?   refreshToken.Token : existingrefreshToken,//agar refresh token ghabli vojood dasht hamoon ro bde
                ExpiresAt = token.ValidTo
            };
            return response;

        }
        private DateTime UnixTimeStampToDateTimeInUtc(long UnixTimeStamp)// tabdil zaman unix be DateTime
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal=dateTimeVal.AddSeconds(UnixTimeStamp);
            return dateTimeVal;
        }
    }
}
