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
    public class DebtController : Controller
    {
        private decimal used = 0;

        private readonly ApplicationDBContext _db;
        private readonly string culture = "en-US";

        public DebtController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Debt> debtList = _db.Debts.FromSql($"select * from Debts order by Date");
            foreach (var debt in debtList)
            {
                used += debt.Amount;
            }
            (IEnumerable<Debt>, decimal) tuple = (debtList, used);
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

        public IActionResult Filter()
        {
            IEnumerable<string?> labelList = _db.Debts.Select(t => t.Name).Distinct();

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
            bool isName = !obj.Name.IsNullOrEmpty();
            bool isLabel = !obj.Label.IsNullOrEmpty();
            string[] orderByDomain = { "Date", "Name", "Label", "Amount" };
            string orderBy = orderByDomain.Contains(obj.OrderBy) ? obj.OrderBy! : "Date";
            bool isDesc = obj.IsDesc;
            string orderType = isDesc ? "desc" : "";

            string startDate = "...";
            string endDate = "...";
            string name = isName ? obj.Name! : "...";
            string label = isLabel ? obj.Label! : "...";
            string sqlStartDate = "...";
            string sqlEndDate = "...";
            bool isAlrCondition = false;
            decimal sum = 0;
            string query = "select * from Debts ";

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
            if (isName)
            {
                if (isAlrCondition)
                {
                    query += "and Name like '%" + obj.Name + "%' ";
                }
                else
                {
                    query += "where Name like '%" + obj.Name + "%' ";
                    isAlrCondition = true;
                }
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
                    isAlrCondition = true;
                }
            }
            query += "order by " + orderBy + " ";
            query += orderType;

            var fQuery = FormattableStringFactory.Create(query);
            IEnumerable<Debt> debtList = _db.Debts.FromSql(fQuery);

            foreach (var debt in debtList)
            {
                sum += debt.Amount;
            }

            orderType = isDesc ? "desc" : "normal";

            (IEnumerable<Debt>, string, string, string, string, string, string, decimal) tuple = (debtList, startDate, endDate, name, label, orderBy, orderType, sum);

            return View(tuple);
        }
    }
}
