using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSTART_Hiring_Task.Data;
using MSTART_Hiring_Task.Models;
using MSTART_Hiring_Task.Models.Enum;
using MSTART_Hiring_Task.Models.ViewModel;

namespace MSTART_Hiring_Task.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        #region List of Transactions

        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions
                .Include(x => x.User)
                .Include(x => x.Account)
                .Select(x => new Transaction
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Account_Id = x.Account_Id,
                    User_Id = x.User_Id,
                    Status = x.Status,
                    User = x.User,
                    Account = x.Account,
                }).ToListAsync();
            return View(transactions);
        }

        #endregion

        #region Create New Transactions
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel transactionViewModel)
        {
            if (ModelState.IsValid)
            {

                var transactionData = new Transaction
                {
                    Amount = (decimal)transactionViewModel.Amount,
                    User_Id = (string)transactionViewModel.User_Id,
                    Account_Id = (int)transactionViewModel.Account_Id,
                    Status = (TransactionStatus)transactionViewModel.Status,
                    Type = (TransactionType)transactionViewModel.Type,
                };

                await _context.Transactions.AddAsync(transactionData);
                var accountUser = await _context.Accounts.FindAsync(transactionViewModel.Account_Id);
                if (transactionViewModel.Type == TransactionType.Credit)
                {
                    accountUser.Balance += (decimal)transactionViewModel.Amount;
                }
                else
                {
                    accountUser.Balance -= (decimal)transactionViewModel.Amount;
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction created successfully!";
                return RedirectToAction("Index");
            }
            return View(transactionViewModel);
        }


        #endregion

        #region  Get User Name By Account Number
        [HttpGet]
        public async Task<IActionResult> GetUserNameByAccountNumber(string accountNumber)
        {
            var account = await _context.Accounts.Include(a => a.User)
                                    .FirstOrDefaultAsync(a => a.Account_Number == accountNumber);

            if (account != null && account.User != null)
            {
                return Json(new
                {
                    success = true,
                    userName = account.User.First_Name + " " + account.User.Last_Name,
                    userId = account.User.Id,
                    accountId = account.ID
                });
            }
            return Json(new { success = false, message = "No user found for this account number." });
        }
        #endregion

        #region Transaction Not Found
        public IActionResult TransactionNotFound()
        {
            return View();
        }
        #endregion
        #region Reverse Transaction
        [HttpGet]
        public IActionResult ReverseTransaction(int? id)
        {
            if (id == null)
            {
                return Redirect("Index");
            }
            var transaction = _context.Transactions.Include(x => x.User).Include(x => x.Account).SingleOrDefault(e => e.Id == id);

            if (transaction != null)
            {
                return View(transaction);
            }
            return RedirectToAction("TransactionNotFound");
        }


        [HttpPost]
        public async Task<IActionResult> ReverseTransaction(TransactionViewModel model)
        {
            var transaction = await _context.Transactions.FindAsync(model.Id);
            if (transaction != null)
            {
                transaction.Status = TransactionStatus.Reversed;

                var account = await _context.Accounts.FindAsync(transaction.Account_Id);
                if (transaction.Type == TransactionType.Credit)
                {
                    account.Balance -= transaction.Amount;
                    transaction.Type = TransactionType.Debit; // Change the type after reversing
                    TempData["SuccessMessage"] = "Reversed Transaction from Credit to Debit.";
                }
                else if (transaction.Type == TransactionType.Debit)
                {
                    account.Balance += transaction.Amount;
                    transaction.Type = TransactionType.Credit; // Change the type after reversing
                    TempData["SuccessMessage"] = "Reversed Transaction from Debit to Credit.";
                }

                _context.Update(transaction);
                await _context.SaveChangesAsync();

                // Redirect to the "Index" action
                return RedirectToAction("Index");
            }
            else
            {
                TempData["DangerMessage"] = "Transaction is already reversed or not found.";
                return RedirectToAction("Index");
            }
        }

        #endregion



    }
}
