using Income_and_Expense_Record.Data;
using Income_and_Expense_Record.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Income_and_Expense_Record.Controllers
{
    public class DebtController : Controller
    {
        private decimal used = 0;

        private readonly ApplicationDBContext _db;

        public DebtController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Debt> transactionList = _db.Debts.FromSql($"select * from Debts order by Date");
            foreach (var debt in transactionList)
            {
                used += debt.Amount;
            }
            (IEnumerable<Debt>, decimal) tuple = (transactionList, used);
            return View(tuple);
        }

        public IActionResult Create()
        {
            var obj = new Debt();
            obj.Date = DateTime.Now;

            //Debt? objFromDb = _db.Debts.FromSql($"select top 1 * from Debts order by Date DESC").FirstOrDefault();
            //if (objFromDb != null)
            //{
            //    obj.Date = objFromDb.Date;
            //}
            //else
            //{
            //    obj.Date = DateTime.Now;
            //}

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Debt obj)
        {
            if (ModelState.IsValid)
            {
                _db.Debts.Add(obj);
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
            var obj = _db.Debts.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Debt obj)
        {
            if (ModelState.IsValid)
            {
                _db.Debts.Update(obj);
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
            var obj = _db.Debts.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Debts.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            foreach (var debt in _db.Debts)
            {
                _db.Debts.Remove(debt);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
