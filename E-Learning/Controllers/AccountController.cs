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

        public AccountController(UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    IOptions<AppSettings> appSettings,
                                    IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

     

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegUserDto userDto)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(userDto.FirstName))
                errorMessages.Add("First name is required");

            if (string.IsNullOrEmpty(userDto.LastName))
                errorMessages.Add("Last name is required");

            if (string.IsNullOrEmpty(userDto.Country))
                errorMessages.Add("Country is required");

            if (string.IsNullOrEmpty(userDto.Gender))
                errorMessages.Add("Gender is required");

            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add("Email is required");

            if (string.IsNullOrEmpty(userDto.Password))
                errorMessages.Add("Password is required");

            if (userDto.Password != userDto.ConfirmPassword)
            {
                errorMessages.Add("Passwords do not match");

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

                            var confirmationLink = "http://localhost:4200/email-confirmation?userId=" + user.Id + "&token=" + verificationToken.ToString();
                            string To = user.Email;
                            string Subject = "Confirm your email";
                            string Body = "Please click on the link below to confirm your email: " + confirmationLink;
                            Email email = new Email(To, Subject, Body);
                            email.Send();
                        }
                        catch { }
                        return Ok(new { status = "User Created" });
                    }

                    foreach (var error in addToRoleResult.Errors)
                    {
                        errorMessages.Add(error.Description);
                    }

                    return BadRequest(new { errors = errorMessages });
                }

                foreach (var error in result.Errors)
                {
                    errorMessages.Add(error.Description);
                }

                return BadRequest(new { errors = errorMessages });
            }
            catch (Exception ex)
            {
                // return error message if there was an exception

                errorMessages.Add(ex.Message);

                return BadRequest(new { errors = errorMessages });
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add("Email is required");

            if (string.IsNullOrEmpty(userDto.Password))
                errorMessages.Add("Email is required");

            if(errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });


            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if(user != null && !user.IsActive)
            {
                errorMessages.Add("Your account is deactivated. Please contact website administrator");
                return BadRequest(new { errors = errorMessages });
            }

            if (user != null && (await _userManager.CheckPasswordAsync(user, userDto.Password)) && !user.EmailConfirmed)
            {

                errorMessages.Add("Email not confirmed.");
                return BadRequest(new { errors = errorMessages });
            }


            var result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, false, false);

            if (!result.Succeeded)
            {

                errorMessages.Add("Invalid username or password");
                return BadRequest(new { errors = errorMessages });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var role = "";
            if (user.IsAdmin)
                role = "Admin";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                }),
                Expires = DateTime.Now.AddHours(3),
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
            var errorMessages = new List<string>();

            if (confirmEmailDto.userId == null || confirmEmailDto.token == null)
            {
                errorMessages.Add("User not found");
                return BadRequest(new { errors = errorMessages });
            }

            var user = await _userManager.FindByIdAsync(confirmEmailDto.userId);

            if (user == null)
            {
                errorMessages.Add("User not found");
                return BadRequest(new { errors = errorMessages });
            }

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.token);

            if (result.Succeeded)
            {
                return Ok(true);
            }

            foreach (var error in result.Errors)
            {
                errorMessages.Add(error.Description);
            }

            return BadRequest(new { errors = errorMessages });
        }

        [AllowAnonymous]
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationLink([FromBody] UserDto userDto)
        {
            var errorMessages = new List<string>();
            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add("Email is required");

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            var user = await _userManager.FindByEmailAsync(userDto.Email);

            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);


            var confirmationLink = "http://localhost:4200/security/email-confirmation?userId=" + user.Id + "&token=" + verificationToken.ToString();
            string To = user.Email;
            string Subject = "Confirm your email";
            string Body = "Please click on the link below to confirm your email: " + confirmationLink;
            Email email = new Email(To, Subject, Body);
            email.Send();

            return Ok(true);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserDto userDto)
        {
            var errorMessages = new List<string>();
            if (string.IsNullOrEmpty(userDto.Email))
                errorMessages.Add("Email is required");

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });


            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if(user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPwdLink = "http://localhost:4200/security/reset-password?email=" + user.Email + "&token=" + resetToken.ToString();
                string To = user.Email;
                string Subject = "Reset Password";
                string Body = "Please click on the link below to reset your password: " + resetPwdLink;
                Email email = new Email(To, Subject, Body);
                email.Send();

                return Ok(true);

            }

            errorMessages.Add("Cannot reset password. please confirm your email address.");
            return BadRequest(new { errors = errorMessages });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(resetPasswordDto.Password))
                errorMessages.Add("Password is required");

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });


            if (resetPasswordDto.Password != resetPasswordDto.ConfirmPassword)
            {
                errorMessages.Add("Passwords do not match");

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

                
                foreach(var error in result.Errors)
                {
                    errorMessages.Add(error.Description);
                }
                return BadRequest(new { errors = errorMessages });
            }

            errorMessages.Add("Cannot reset password. Please try again.");
            return BadRequest(new { errors = errorMessages });
        }
    }
}