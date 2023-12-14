
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSTART_Hiring_Task.Data;
using MSTART_Hiring_Task.Models.Enum;
using MSTART_Hiring_Task.Models;
using Microsoft.EntityFrameworkCore;
using MSTART_Hiring_Task.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace MSTART_Hiring_Task.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IdentityDbContext<AppUser> _context;
        private readonly UserManager<AppUser> _userManager;


        public UsersController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        #region List of Users
        public async Task<IActionResult> Index(string searchTerm)
        {
            var usersQuery = _context.Users.AsQueryable();
            if (!String.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(c => c.UserName.Contains(searchTerm)
                                                || c.Email.Contains(searchTerm)
                                                || c.Id.Contains(searchTerm));
            }
            var usersData = await usersQuery.Select(c => new AppUser
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                First_Name = c.First_Name,
                Last_Name = c.Last_Name,
                Status = c.Status,
                Gender = c.Gender,
                Date_Of_Birth = c.Date_Of_Birth,
                PhoneNumber = c.PhoneNumber,
            }).ToListAsync();

            return View(usersData);
        }
        #endregion

        #region Create User 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsersCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var existingUsername = await _context.Users.Where(u => u.UserName == user.UserName).FirstOrDefaultAsync();
                var existingEmail = await _context.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
                if (existingUsername != null)
                {
                    ModelState.AddModelError("Username", "This Username already exists.");
                    return View(user);
                }
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "This Email already exists.");
                    return View(user);
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // User created successfully
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If ModelState is not valid or user creation fails, return to the view with the model
            return View(model);
        }

        #endregion


        #region Edit of user 
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    First_Name = user.First_Name,
                    Last_Name = user.Last_Name,
                    Status = user.Status,
                    Gender = user.Gender,
                    Date_Of_Birth = user.Date_Of_Birth,
                };
                return View(userViewModel);
            }
            return RedirectToAction("UserNotFound");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model, string action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.Id);

                    if (user != null)
                    {
                        user.UserName = model.UserName;
                        user.Email = model.Email;
                        user.First_Name = model.First_Name;
                        user.Last_Name = model.Last_Name;
                        user.Status = model.Status;
                        user.PhoneNumber = model.PhoneNumber;
                        user.Date_Of_Birth = (DateTime)model.Date_Of_Birth;
                        user.Gender = model.Gender;

                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            if (action == "continue")
                            {
                                TempData["SuccessMessage"] = "User Update successfully!";
                                return RedirectToAction("Edit", new { id = user.Id });
                            }

                            TempData["SuccessMessage"] = "User Update successfully!";
                            return RedirectToAction("ViewUser", new { id = user.Id });
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the department.");
            }

            // If ModelState is not valid or user update fails, return to the view with the model
            return View(model);
        }

        #endregion

        #region User not found

        [HttpGet]
        public IActionResult UserNotFound()
        {
            return View();
        }
        #endregion

        #region View User 
        [HttpGet]
        public IActionResult ViewUser(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var user = _context.Users.Find(id);
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("UserNotFound");
        }
        #endregion


        #region Delete Selected Users
        [HttpPost]
        public async Task<IActionResult> DeleteSelectedUsers(List<string> userIds)
        {
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).Include(u => u.Accounts).ToListAsync();

            foreach (var user in users)
            {
                if (user.Status == UserStatus.Deleted)
                {
                    TempData["DangerMessage"] = "Users already Deleted.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Selected users were marked as deleted!";
                }
                user.Status = UserStatus.Deleted;
                foreach (var account in user.Accounts)
                {
                    account.Status = AccountStatus.Deleted;  // Assuming you have a similar enum for AccountStatus
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


    }
}
