using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MSTART_Hiring_Task.Data;
using MSTART_Hiring_Task.Models;
using MSTART_Hiring_Task.Models.ViewModel;
using MSTART_Hiring_Task.Servies;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace MSTART_Hiring_Task.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Setting Info
        private readonly ILogger<HomeController> _logger;
        private readonly LanguagesServices _localization;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly  IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger,LanguagesServices localization,UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager,AppDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _localization = localization;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Change language 
        [AllowAnonymous]
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(2) });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        #endregion

        #region  Open  Accounting and login ,logout

        #region Login Controller
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.WelcomeMassage = _localization.Getkey("str-login-masseage").Value;
            ViewBag.emailMassage = _localization.Getkey("str-email").Value;
            ViewBag.passwordMasseage = _localization.Getkey("str-password").Value;
            ViewBag.alreadyMasseage = _localization.Getkey("str-already").Value;
            ViewBag.signMasseage = _localization.Getkey("str-sigup").Value;
            ViewBag.signMasseage = _localization.Getkey("str-rememberme").Value;
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                ViewBag.loginMassage = _localization.Getkey("str-login-succf").Value;
                ViewBag.loginattemptMassage = _localization.Getkey("str-attempt-login").Value;
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "The login is successful!";
                        return RedirectToAction("Dashboard", "Home");
                    }

                    if (result.IsLockedOut)
                    {
                        // Handle account lockout
                        ModelState.AddModelError(string.Empty, "Account locked out. Try again later.");
                    }
                    else
                    {
                        // Password is incorrect
                        ModelState.AddModelError(string.Empty, "Invalid password.");
                    }
                }
                else
                {
                    // User not found
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View("Index", model);
        }
        #endregion


        #region Register  Controller 
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    First_Name = model.First_Name,
                    Last_Name = model.Last_Name,
                    Status = model.Status,
                    Date_Of_Birth = model.Date_Of_Birth,
                    Email = model.Email,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.UserName,
                };
                var userWithEmail = await _userManager.FindByEmailAsync(model.Email);
                var userWithUsername = await _userManager.FindByNameAsync(model.UserName);
                ViewBag.emailexistMassage = _localization.Getkey("str-email-exist").Value;
                ViewBag.usernameexistMassage = _localization.Getkey("str-username-exist").Value;
                ViewBag.signupMassage = _localization.Getkey("str-signup").Value;
                if (userWithEmail != null  )
                {
                    ModelState.AddModelError("Email", ViewBag.emailexistMassage);
                    return View(model);
                }
                else if (userWithUsername != null)
                {
                    ModelState.AddModelError("UserName", ViewBag.usernameexistMassage);
                    return View(model);
                }
                //UserManager
                var isVaidData = await _userManager.CreateAsync(user, model.Password);
                if (isVaidData != null)
                {
                    TempData["SuccessMessage"] = ViewBag.signupMassage = _localization.Getkey("str-signup").Value;
                    return RedirectToAction("Index", "Home");
                }
                //All error
                foreach (var item in isVaidData.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

            }
            return View(model);
        }
        #endregion


        #region Logout  Controller
        public async Task<IActionResult> Logout()
        {
            ViewBag.logoutMassage = _localization.Getkey("str-logout").Value;
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Logout successfully!";
            return RedirectToAction("Index", "Home");
        }
        #endregion


        #endregion

        public IActionResult Dashboard()
        {
            
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.TotalAccounts = _context.Accounts.Count();
            ViewBag.TotalTransactions = _context.Transactions.Count();
            ViewBag.TotalRoles =_roleManager.Roles.Count();
            return View();
        }


        #region Role Controller 

        #region  Role list 
        [HttpGet]
        public IActionResult RolesList()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        #endregion

        #region Create role 

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Register created successfully!";
                    return RedirectToAction("RolesList", "Home");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }


        #endregion

        #region Edit role 
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return View("NotFoundRole");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
            };
            //Retrrive all Users
            foreach (var user in _userManager.Users)
            {
                // if user in this role  => add username prop EditRoleViewModel
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMassg = $"No Role with this Id = {model.Id}";
                return View("NotFoundRole");
            }
            else
            {
                role.Name = model.RoleName;
                var updatedStatus = await _roleManager.UpdateAsync(role);
                if (updatedStatus.Succeeded)
                {
                    return RedirectToAction("RolesList", "Home");
                }
                foreach (var item in updatedStatus.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            return View(model);
        }

        #endregion

        #region Not Found role 
        [HttpGet]
        public IActionResult NotFoundRole()
        {
            return View();
        }

        #endregion

        #region update user in role 

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMsg = $"No Role With this ID ={id}";
                return View("NotFoundRole");
            }
            var model = new List<UsersInRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UsersInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UsersInRoleViewModel> model, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMsg = $"No Role with this id ={id}";
                return View("NotFoundRole");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result;
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("RolesList");
                    }
                }
            }
            return RedirectToAction("RolesList");
        }

        #endregion


        #endregion


        #region Access Denied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId)
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("NotFoundUser");
            }
            var model = new EditUserProfile
            {
                Id = user.Id,
                First_Name = user.First_Name,
                Last_Name = user.Last_Name,
                UserName = user.UserName,
                Email = user.Email,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Date_Of_Birth = user.Date_Of_Birth,
                ExistingImage = user.ImageProfile,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UserProfile(EditUserProfile model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        return View("NotFoundUser");
                    }
                    string imageName = "~/Image/" + UploadFile(model);
                   
                    user.First_Name = model.First_Name;
                    user.Last_Name = model.Last_Name;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.Gender = model.Gender;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Date_Of_Birth = model.Date_Of_Birth;
                    user.ImageProfile = imageName;


                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "User Profile Update successfully!";
                        return RedirectToAction("UserProfile", new { id = user.Id });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while updating the user profile.");
            }

            return View(model);
        }

        public string UploadFile(UploadImageViewModel model)
        {
            string uploadFileName = string.Empty;
            try
            {
                if (model.PictureProfile != null)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Image");
                    uploadFileName = Guid.NewGuid().ToString() + "_" + model.PictureProfile.FileName;
                    string fullPath = Path.Combine(uploadFolder, uploadFileName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.PictureProfile.CopyTo(fileStream);
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while uploading the image.");
            }

            return uploadFileName;
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}