using Income_and_Expense_Record.Data;
using Income_and_Expense_Record.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Income_and_Expense_Record.Controllers
{
    public class MoneyController : Controller
    {
        private decimal weekly_used = 1400;
        private decimal left = 1400;

        private readonly ApplicationDBContext _db;

        public MoneyController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Money> transactionList = _db.Moneys.FromSql($"select * from Moneys order by Date");
            foreach (var money in transactionList)
            {
                left -= money.Amount;
            }
            (IEnumerable<Money>, decimal, decimal) tuple = (transactionList, weekly_used, left);
            return View(tuple);
        }

        public IActionResult Create()
        {
            var obj = new Money();
            Money? objFromDb = _db.Moneys.FromSql($"select top 1 * from Moneys order by Date DESC").FirstOrDefault();
            if (objFromDb != null)
            {
                obj.Date = objFromDb.Date;
            }
            else
            {
                obj.Date = DateTime.Now;
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Money obj)
        {
            if (ModelState.IsValid)
            {
                _db.Moneys.Add(obj);
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
            var obj = _db.Moneys.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Money obj)
        {
            if (ModelState.IsValid)
            {
                _db.Moneys.Update(obj);
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
            var obj = _db.Moneys.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Moneys.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult TransferData()
        {
            foreach (var item in _db.Moneys)
            {
                var transaction = new Transaction();
                transaction.Date = item.Date;
                transaction.Amount = item.Amount;
                transaction.Label = item.Label;
                _db.Transactions.Add(transaction);
                _db.Moneys.Remove(item);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            foreach (var money in _db.Moneys)
            {
                _db.Moneys.Remove(money);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
