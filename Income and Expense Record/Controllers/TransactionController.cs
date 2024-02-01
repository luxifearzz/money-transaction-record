using Income_and_Expense_Record.Data;
using Income_and_Expense_Record.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Income_and_Expense_Record.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDBContext _db;

        public TransactionController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Transaction> transactionList = _db.Transactions.FromSql($"select * from Transactions order by Date");
            return View(transactionList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Transaction obj)
        {
            if (ModelState.IsValid)
            {
                _db.Transactions.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Transactions.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Transaction obj)
        {
            if (ModelState.IsValid)
            {
                _db.Transactions.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Transactions.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Transactions.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            foreach (var transaction in _db.Transactions)
            {
                _db.Transactions.Remove(transaction);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
