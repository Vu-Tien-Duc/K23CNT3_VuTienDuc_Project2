using System.ComponentModel.DataAnnotations;

namespace QLBanSachWeb.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        [StringLength(500, ErrorMessage = "Nội dung tối đa 500 ký tự")]
        public string Message { get; set; }
    }
}
