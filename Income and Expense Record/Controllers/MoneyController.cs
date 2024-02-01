using Income_and_Expense_Record.Data;
using Income_and_Expense_Record.Models;
using Microsoft.AspNetCore.Mvc;

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
            IEnumerable<Money> transactionList = _db.Moneys;
            foreach (var money in transactionList)
            {
                left -= money.Amount;
            }
            (IEnumerable<Money>, decimal, decimal) tuple = (transactionList, weekly_used, left);
            return View(tuple);
        }

        public IActionResult Create()
        {
            return View();
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
    }
}
