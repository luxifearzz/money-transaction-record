using Income_and_Expense_Record.Data;
using Income_and_Expense_Record.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Income_and_Expense_Record.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly string culture = "en-US";

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

        public IActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterIndex(Transaction obj)
        {
            bool isDate = obj.Date != DateTime.MinValue;
            bool isLabel = !obj.Label.IsNullOrEmpty();
            string date = "";
            string label = "";
            string query = "";
            decimal sum = 0;
            string sqlDate = "";

            if (isDate)
            {
                sqlDate = "'" + obj.Date.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo(culture)) + " 00:00:00.0000000" + "'";
            }
            if (isDate && isLabel)
            {
                date = obj.Date.Date.ToString("yyyy/MM/dd");
                label = obj.Label;
                query = "select * from Transactions where Date = " + sqlDate + " and Label like '%" + obj.Label + "%' order by Date";
            }
            else if (isDate && !isLabel) 
            {
                date = obj.Date.Date.ToString("yyyy/MM/dd");
                label = "...";
                query = "select * from Transactions where Date = " + sqlDate + " order by Date";
            }
            else if (!isDate && isLabel)
            {
                date = "...";
                label = obj.Label;
                query = "select * from Transactions where Label like '%" + obj.Label + "%' order by Date";
            }
            else if (!isDate && !isLabel)
            {
                date = "...";
                label = "...";
                query = "select * from Transactions order by date";
            }

            var fQuery = FormattableStringFactory.Create(query);
            IEnumerable<Transaction> transactionList = _db.Transactions.FromSql(fQuery);

            foreach (var transaction in transactionList)
            {
                sum += transaction.Amount;
            }

            (IEnumerable<Transaction>, string, string, decimal) tuple = (transactionList, date,  label, sum);

            return View(tuple);
        }
    }
}
