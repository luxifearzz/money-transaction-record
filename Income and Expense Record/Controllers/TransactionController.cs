using Income_and_Expense_Record.Data;
using Income_and_Expense_Record.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            decimal sum = 0;
            foreach (var transaction in transactionList)
            {
                sum += transaction.Amount;
            }

            (IEnumerable<Transaction>, decimal) tuple = (transactionList, sum);

            return View(tuple);
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
            IEnumerable<string> labelList = _db.Transactions.Select(t => t.Label).Distinct();

            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "ไม่กำหนด", Value = "" });
            foreach (var label in labelList)
            {
                items.Add(new SelectListItem { Text = label, Value = label });
            }

            ViewBag.LabelList = items;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FilterIndex(Filter obj)
        {
            bool isStartDate = obj.StartDate != DateTime.MinValue;
            bool isEndDate = obj.EndDate != DateTime.MinValue;
            bool isLabel = !obj.Label.IsNullOrEmpty();
            string orderBy = obj.OrderBy == null ? "Date" : obj.OrderBy;
            bool isDesc = obj.IsDesc;
            string orderType = isDesc ? "desc" : "";

            string startDate = "...";
            string endDate = "...";
            string label = isLabel ? obj.Label : "...";
            string sqlStartDate = "...";
            string sqlEndDate = "...";
            bool isAlrCondition = false;
            decimal sum = 0;
       
            string query = "select * from Transactions ";

            if (isStartDate)
            {
                sqlStartDate = "'" + obj.StartDate.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo(culture)) + " 00:00:00.0000000" + "'";
            }
            if (isEndDate)
            {
                sqlEndDate = "'" + obj.EndDate.ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo(culture)) + " 00:00:00.0000000" + "'";
            }
            if (isStartDate && !isEndDate)
            {
                startDate = obj.StartDate.Date.ToString("yyyy/MM/dd");
                query += "where Date >= " + sqlStartDate + " ";
                isAlrCondition = true;
            }
            else if (!isStartDate && isEndDate)
            {
                endDate = obj.EndDate.Date.ToString("yyyy/MM/dd");
                query += "where Date <= " + sqlEndDate + " ";
                isAlrCondition = true;
            }
            else if (isStartDate && isEndDate)
            {
                startDate = obj.StartDate.Date.ToString("yyyy/MM/dd");
                endDate = obj.EndDate.Date.ToString("yyyy/MM/dd");
                query += "where Date >= " + sqlStartDate + " and Date <= " + sqlEndDate + " ";
                isAlrCondition = true;
            }
            if (isLabel)
            {
                if (isAlrCondition)
                {
                    query += "and Label like '%" + obj.Label + "%' ";
                }
                else
                {
                    query += "where Label like '%" + obj.Label + "%' ";
                }
            }
            query += "order by " + orderBy + " ";
            query += orderType;

            var fQuery = FormattableStringFactory.Create(query);
            IEnumerable<Transaction> transactionList = _db.Transactions.FromSql(fQuery);

            foreach (var transaction in transactionList)
            {
                sum += transaction.Amount;
            }

            orderType = isDesc ? "desc" : "normal";

            (IEnumerable<Transaction>, string, string, string, string, string, decimal) tuple = (transactionList, startDate, endDate, label, orderBy, orderType, sum);

            return View(tuple);
        }

        
    }
}
