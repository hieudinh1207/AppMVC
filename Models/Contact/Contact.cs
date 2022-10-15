using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_01.Models.Contacts
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [DisplayName("Họ Tên")]
        public string FullName { get; set; }
        [Required]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Phải là địa chỉ email")]
        [DisplayName("Địa chỉ email")]
        public string Email { get; set; }
        public DateTime DateSent { get; set; }
        [DisplayName("Nội dung")]
        public string Message { get; set; }
        [StringLength(50)]
        [Phone(ErrorMessage = "Phải là số điện thoại")]
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
    }
}


