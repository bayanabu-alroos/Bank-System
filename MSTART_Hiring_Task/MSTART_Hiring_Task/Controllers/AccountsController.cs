using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSTART_Hiring_Task.Data;
using MSTART_Hiring_Task.Models.Currencies;
using MSTART_Hiring_Task.Models.Enum;
using MSTART_Hiring_Task.Models;
using Microsoft.AspNetCore.Authorization;
using MSTART_Hiring_Task.Models.ViewModel;

namespace MSTART_Hiring_Task.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        #region List of Accounts 
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var accountsQuery = _context.Accounts.Include(x => x.User).AsQueryable();
            if (!String.IsNullOrEmpty(searchTerm))
            {
                accountsQuery = accountsQuery.Where(c => c.User_ID.ToString().Contains(searchTerm)
                                                || c.Account_Number.Contains(searchTerm)
                                                || c.ID.ToString().Contains(searchTerm)
                                                || c.User.First_Name.Contains(searchTerm));
            }
            var accountsData = await accountsQuery.Select(c => new Account
            {
                ID = c.ID,
                Account_Number = c.Account_Number,
                Balance = c.Balance,
                Currency = c.Currency,
                User_ID = c.User_ID,
                Status = c.Status,
                User = c.User
            }).ToListAsync();

            return View(accountsData);
        }
        #endregion



        #region Create New Account 
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.UsersName = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.First_Name + " " + u.Last_Name
            }).ToList();
            ViewBag.Currencies = Currencies.GetAll().Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AccountViewModel accountViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var accountData = new Account
                    {
                        Account_Number = accountViewModel.Account_Number,
                        Balance = (decimal)accountViewModel.Balance,
                        Currency = accountViewModel.Currency,
                        Status = (AccountStatus)accountViewModel.Status,
                        User_ID = (string)accountViewModel.User_ID,
                    };
                    var existingAccount_Number = await _context.Accounts.Where(u => u.Account_Number == accountData.Account_Number).FirstOrDefaultAsync();
                    if (existingAccount_Number != null)
                    {
                        ModelState.AddModelError("Account_Number", "This Account Number already exists.");
                        return View(accountViewModel);
                    }
                    _context.Accounts.Add(accountData);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Account created successfully!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the employee.");
            }
            ViewBag.UsersName = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.First_Name + " " + u.Last_Name
            }).ToList();
            ViewBag.Currencies = Currencies.GetAll().Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();
            return View(accountViewModel);
        }

        #endregion



        #region Edit Account 

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }
            var account = _context.Accounts.Find(id);

            if (account != null)
            {
                var accountViewModel = new AccountViewModel
                {
                    ID = account.ID,
                    Account_Number = account.Account_Number,
                    Balance = account.Balance,
                    Currency = account.Currency,
                    Status = account.Status,
                    User_ID = account.User_ID,
                };
                ViewBag.UsersName = _context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.First_Name + " " + u.Last_Name
                }).ToList();
                ViewBag.Currencies = Currencies.GetAll().Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
                return View(accountViewModel);
            }
            return RedirectToAction("AccountNotFound");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AccountViewModel accountViewModel, string action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = await _context.Accounts.FindAsync(accountViewModel.ID);
                    if (account != null)
                    {
                        account.Account_Number = accountViewModel.Account_Number;
                        account.Balance = (decimal)accountViewModel.Balance;
                        account.Currency = accountViewModel.Currency;
                        account.User_ID = (string)accountViewModel.User_ID;
                        account.Status = (AccountStatus)accountViewModel.Status;

                        _context.Accounts.Update(account);
                        await _context.SaveChangesAsync();
                        if (action == "continue")
                        {
                            TempData["SuccessMessage"] = "Account Update successfully!";
                            return RedirectToAction("Edit", new { id = account.ID });
                        }

                        TempData["SuccessMessage"] = "Account Update successfully!";
                        return RedirectToAction("ViewAccount", new { id = account.ID });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while trying to create the Account.");
            }
            ViewBag.UsersName = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.First_Name + " " + u.Last_Name
            }).ToList();
            ViewBag.Currencies = Currencies.GetAll().Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();
            return View(accountViewModel);
        }

        #endregion



        #region Account not found
        [HttpGet]
        public IActionResult AccountNotFound()
        {
            return View();
        }

        #endregion


        #region View Account 

        [HttpGet]
        public IActionResult ViewAccount(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var account = _context.Accounts.Include(x => x.User).SingleOrDefault(e => e.ID == id);
            if (account != null)
            {
                return View(account);
            }
            return RedirectToAction("AccountNotFound");
        }
        #endregion



        #region Delete Selected Accounts
        [HttpPost]
        public async Task<IActionResult> DeleteSelectedAccounts(List<int> accountIds)
        {
            var accounts = await _context.Accounts.Where(x => accountIds.Contains(x.ID)).ToListAsync();

            foreach (var account in accounts)
            {
                if (account.Status == AccountStatus.Deleted)
                {
                    TempData["DangerMessage"] = "Account already Deleted.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Selected accounts were marked as deleted!";
                }
                account.Status = AccountStatus.Deleted;
                _context.Entry(account).State = EntityState.Modified;
            }


            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
