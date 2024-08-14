using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Income_and_Expense_Record.Models
{
    public class Debt
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "กรอกชื่อผู้ยืม")]
        [DisplayName("ชื่อผู้ยืม")]
        public string? Name {  get; set; }
        
        [Required(ErrorMessage = "กรอกวันที่ที่ถูกต้อง")]
        [DisplayName("วันที่")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "กรอกจำนวนเงินที่ถูกต้อง(ทศนิยมไม่เกิน2ตำแหน่ง)")]
        [DisplayName("จำนวนเงิน")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "ระบุบันทึก")]
        [DisplayName("บันทึกช่วยจำ")]
        public string? Label { get; set; }

        override
        public string ToString()
        {
            return $"{Id} {Date} {Amount} {Label}";
        }
    }
}
