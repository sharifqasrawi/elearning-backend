using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Learning.Dtos.Users;
using E_Learning.Helpers;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using E_Learning.Emails;

namespace E_Learning.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppSettings _appSettings;
        private IMapper _mapper;
        private readonly ITranslator _translator;


        public AccountController(UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    IOptions<AppSettings> appSettings,
                                    IMapper mapper,
                                    ITranslator translator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _translator = translator;
        }

     

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegUserDto userDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(userDto.FirstName))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.FIRSTNAME_REQUIRED", lang));

            if (string.IsNullOrEmpty(userDto.LastName))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.LASTNAME_REQUIRED", lang));

            if (string.IsNullOrEmpty(userDto.Country))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.COUNTRY_REQUIRED", lang));

            if (string.IsNullOrEmpty(userDto.Gender))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.GENDER_REQUIRED", lang));

            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.EMAIL_REQUIRED", lang));

            if (string.IsNullOrEmpty(userDto.Password))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORD_REQUIRED", lang));

            if (userDto.Password != userDto.ConfirmPassword)
            {
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORDS_MATCH", lang));

                return BadRequest(new { errors = errorMessages });
            }

            if(errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            try
            {
                bool roleExists = await _roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    var role = new IdentityRole();
                    role.Name = "User";
                    await _roleManager.CreateAsync(role);
                }

                var user = new ApplicationUser
                {
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Country = userDto.Country,
                    Gender = userDto.Gender,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // save 
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");

                    if (addToRoleResult.Succeeded)
                    {
                        try
                        {
                            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                            var confirmationLink = "https://qasrawi.fr/security/email-confirmation?userId=" + user.Id + "&token=" + verificationToken.ToString();
                            string To = user.Email;
                            string Subject = _translator.GetTranslation("ACCOUNT.REGISTER_EMAIL_SUBJECT", lang);
                            string Body = _translator.GetTranslation("ACCOUNT.REGISTER_EMAIL_MESSAGE", lang) + " : " + $"<br><a href=\"{confirmationLink}\"> {confirmationLink}</a>";
                            Email email = new Email(To, Subject, Body);
                            email.Send();
                        }
                        catch { }
                        return Ok(new { status = "User Created" });
                    }


                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    

                    return BadRequest(new { errors = errorMessages });
                }

                errorMessages.Add(_translator.GetTranslation("ERROR", lang));

                return BadRequest(new { errors = errorMessages });
            }
            catch 
            {
                // return error message if there was an exception

                //errorMessages.Add(ex.Message);
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));

                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            var errorMessages = new List<string>();
            var lang = Request.Headers["language"].ToString();

            if (string.IsNullOrEmpty(userDto.Email))
            {
                errorMessages.Add(_translator.GetTranslation("VALIDATION.EMAIL_REQUIRED", lang));
            }

            if (string.IsNullOrEmpty(userDto.Password))
            {
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORD_REQUIRED", lang));
            }

            if(errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });


            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if(user != null && !user.IsActive)
            {
                errorMessages.Add(_translator.GetTranslation("ACCOUNT.DEACTIVATED", lang));
               
                return BadRequest(new { errors = errorMessages });
            }

            if (user != null && (await _userManager.CheckPasswordAsync(user, userDto.Password)) && !user.EmailConfirmed)
            {
                errorMessages.Add(_translator.GetTranslation("ACCOUNT.EMAIL_NOT_CONFIRMED", lang));
                
                return BadRequest(new { errors = errorMessages });
            }
            

            var result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, false, false);

            if (!result.Succeeded)
            {
                errorMessages.Add(_translator.GetTranslation("ACCOUNT.INVALID_USERNAME_PASSWORD", lang));
               

                return BadRequest(new { errors = errorMessages });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var role = "User";
            if (user.IsAdmin)
                role = "Admin";
            else if (user.IsAuthor)
                role = "Author";
            else
                role = "User";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                id = user.Id,
                username = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                isAdmin = user.IsAdmin,
                isAuther = user.IsAuthor,
                token = tokenString,
                expiresIn = tokenDescriptor.Expires,
            });
        }


        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailDto confirmEmailDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            if (confirmEmailDto.userId == null || confirmEmailDto.token == null)
            {
                errorMessages.Add(_translator.GetTranslation("ACCOUNT.USER_NOT_FOUND", lang));
                return BadRequest(new { errors = errorMessages });
            }

            var user = await _userManager.FindByIdAsync(confirmEmailDto.userId);

            if (user == null)
            {
                errorMessages.Add(_translator.GetTranslation("ACCOUNT.USER_NOT_FOUND", lang));
                return BadRequest(new { errors = errorMessages });
            }

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.token);

            if (result.Succeeded)
            {
                return Ok(true);
            }

            errorMessages.Add(_translator.GetTranslation("ERROR", lang));

            return BadRequest(new { errors = errorMessages });
        }

        [AllowAnonymous]
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationLink([FromBody] UserDto userDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.EMAIL_REQUIRED", lang));

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);

                var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);


                var confirmationLink = "https://qasrawi.fr/security/email-confirmation?userId=" + user.Id + "&token=" + verificationToken.ToString();
                string To = user.Email;
                string Subject = _translator.GetTranslation("ACCOUNT.REGISTER_EMAIL_SUBJECT", lang);
                string Body = _translator.GetTranslation("ACCOUNT.REGISTER_EMAIL_MESSAGE", lang) + " : " + $"<br><a href=\"{confirmationLink}\"> {confirmationLink}</a>";
                Email email = new Email(To, Subject, Body);
                email.Send();

                return Ok(true);

            }
            catch
            {
                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserDto userDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();
            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.EMAIL_REQUIRED", lang));

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });


            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                try
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPwdLink = "https://qasrawi.fr/security/reset-password?email=" + user.Email + "&token=" + resetToken.ToString();
                    string To = user.Email;
                    string Subject = _translator.GetTranslation("ACCOUNT.RESET_PASSWORD_EMAIL_SUBJECT", lang);
                    string Body = _translator.GetTranslation("ACCOUNT.RESET_PASSWORD_EMAIL_MESSAGE", lang) + " : " + $"<br><a href=\"{resetPwdLink}\"> {resetPwdLink}</a>";
                    Email email = new Email(To, Subject, Body);
                    email.Send();

                    return Ok(true);

                }
                catch
                {
                    errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                    return BadRequest(new { errors = errorMessages });
                }

            }

            errorMessages.Add(_translator.GetTranslation("ACCOUNT.CANNOT_RESET_PASSWORD", lang));
            return BadRequest(new { errors = errorMessages });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(resetPasswordDto.Password))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORD_REQUIRED", lang));

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });


            if (resetPasswordDto.Password != resetPasswordDto.ConfirmPassword)
            {
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORDS_MATCH", lang));

                return BadRequest(new { errors = errorMessages });
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if(user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
                if(result.Succeeded)
                {
                    return Ok(true);
                }


                errorMessages.Add(_translator.GetTranslation("ERROR", lang));
                return BadRequest(new { errors = errorMessages });
            }

            errorMessages.Add(_translator.GetTranslation("ERROR", lang));
            return BadRequest(new { errors = errorMessages });
        }

        [Authorize(Roles = "Admin, Author, User")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var lang = Request.Headers["language"].ToString();
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(changePasswordDto.CurrentPassword))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORD_REQUIRED", lang));

            if (string.IsNullOrEmpty(changePasswordDto.NewPassword))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORD_REQUIRED", lang));

            if (string.IsNullOrEmpty(changePasswordDto.ConfirmPassword))
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORD_REQUIRED", lang));

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                errorMessages.Add(_translator.GetTranslation("VALIDATION.PASSWORDS_MATCH", lang));

                return BadRequest(new { errors = errorMessages });
            }

            var user = await _userManager.FindByIdAsync(changePasswordDto.UserId);

            if (!await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword))
            {

                errorMessages.Add(_translator.GetTranslation("ACCOUNT.CURRENT_PASSWORD_INCORRECT", lang));

                return BadRequest(new { errors = errorMessages });
            }

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new { result = true });
                }

                errorMessages.Add(_translator.GetTranslation("ERROR", lang));

                return BadRequest(new { errors = errorMessages });
            }

            errorMessages.Add(_translator.GetTranslation("ERROR", lang));
            return BadRequest(new { errors = errorMessages });
        }
    }
}