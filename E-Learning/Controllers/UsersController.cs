using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Learning.Dtos.Users;
using E_Learning.Helpers;
using E_Learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace E_Learning.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppSettings _appSettings;

        public UsersController(UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Country,
                    u.Gender,
                    u.EmailConfirmed,
                    u.IsAdmin,
                    u.IsAuthor ,
                    u.CreatedAt,
                    u.IsActive
                })
                .OrderBy(u => u.FirstName);
            return Ok(new { users = users });
        }


        [HttpGet("search")]
        public IActionResult SearchUsers([FromQuery] string searchKey)
        {
            var users = _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Country,
                    u.Gender,
                    u.EmailConfirmed,
                    u.IsAdmin,
                    u.IsAuthor,
                    u.CreatedAt,
                    u.IsActive
                })
                .Where(u => u.FirstName.Contains(searchKey)
                           || u.LastName.Contains(searchKey)
                           || u.Email.Contains(searchKey)
                           || string.IsNullOrEmpty(searchKey))
                .OrderBy(u => u.FirstName);

            return Ok(new { users = users });
        }


        [AllowAnonymous]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser([FromQuery] string id)
        {
            var errorMessages = new List<string>();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                errorMessages.Add("User not found");
                return BadRequest(new { errors = errorMessages });
            }

            var userData = new
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                username = user.UserName,
                country = user.Country,
                gender = user.Gender,
                isAdmin = user.IsAdmin,
                isAuthor = user.IsAuthor,
                createdAt = user.CreatedAt,
                isActive = user.IsActive,
                emailConfirmed = user.EmailConfirmed
            };

            return Ok(new { user = userData });
        }

        [AllowAnonymous]
        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles([FromQuery] string id)
        {
            var errorMessages = new List<string>();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                errorMessages.Add("User not found");
                return BadRequest(new { errors = errorMessages });
            }

            var userData = new
            {
                isAdmin = user.IsAdmin,
                isAuthor = user.IsAuthor
            };

            return Ok(new { userRoles = userData });
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody]RegUserDto userDto)
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

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            try
            {
               bool roleAdminExists = await _roleManager.RoleExistsAsync("Admin");
                if (!roleAdminExists)
                {
                    var roleAdmin = new IdentityRole();
                    roleAdmin.Name = "Admin";
                    await _roleManager.CreateAsync(roleAdmin);
                }

                bool roleAuthorExists = await _roleManager.RoleExistsAsync("Author");
                if (!roleAdminExists)
                {
                    var roleAuthor = new IdentityRole();
                    roleAuthor.Name = "Author";
                    await _roleManager.CreateAsync(roleAuthor);
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
                    IsAdmin = userDto.IsAdmin.Value,
                    IsAuthor = userDto.IsAuthor.Value,
                    EmailConfirmed = userDto.EmailConfirmed.Value,
                    IsActive=true
                };

                // save 
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    if(user.IsAdmin)
                    {
                        var addToAdminResult = await _userManager.AddToRoleAsync(user, "Admin");
                        if(!addToAdminResult.Succeeded)
                        {
                            foreach (var error in addToAdminResult.Errors)
                            {
                                errorMessages.Add(error.Description);
                            }

                            return BadRequest(new { errors = errorMessages });
                        }
                    }
                    if (user.IsAuthor)
                    {
                        var addToAuthorResult = await _userManager.AddToRoleAsync(user, "Author");
                        if (!addToAuthorResult.Succeeded)
                        {
                            foreach (var error in addToAuthorResult.Errors)
                            {
                                errorMessages.Add(error.Description);
                            }

                            return BadRequest(new { errors = errorMessages });
                        }
                    }


                    var responseData = new
                    {
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        country=user.Country,
                        gender = user.Gender,
                        isAdmin = user.IsAdmin,
                        isAuthor = user.IsAuthor,
                        id = user.Id,
                        emailConfirmed = user.EmailConfirmed,
                        createdAt = user.CreatedAt,
                        isActive = user.IsActive
                    };
                    return Ok(new { user = responseData });
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

        [HttpPut("act-deact")]
        public async Task<IActionResult> ActivateDeactivateUser([FromBody] UserActivatedDeactivateDto userActivatedDeactivateDto)
        {
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(userActivatedDeactivateDto.UserId);
                if(user == null)
                {
                    return NotFound();
                }

                if (userActivatedDeactivateDto.Option == "activate")
                {
                    user.IsActive = true;
                }
                else if (userActivatedDeactivateDto.Option == "deactivate")
                {
                    user.IsActive = false;
                }

                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    return Ok(new { userId = user.Id, isActive = user.IsActive });
                }

                foreach(var error in result.Errors)
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

        [HttpPut("update-user")]
        public  async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto userDto)
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

            if (errorMessages.Count > 0)
                return BadRequest(new { errors = errorMessages });

            try
            {
                var user = await _userManager.FindByIdAsync(userDto.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.EmailConfirmed = userDto.EmailConfirmed.Value;
                user.Country = userDto.Country;
                user.Gender = userDto.Gender;
                user.IsAdmin = userDto.IsAdmin.Value;
                user.IsAuthor = userDto.IsAuthor.Value;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var response = new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        Country = user.Country,
                        Gender = user.Gender,
                        IsAdmin = user.IsAdmin,
                        IsAuthor = user.IsAuthor,
                        IsActive = user.IsActive
                    };

                    return Ok(new { user = response });
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

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)
        {
            var errorMessages = new List<string>();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { userId = user.Id });
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
    }
}