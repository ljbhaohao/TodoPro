using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoProCore.Dtos.Requests
{
   public class UserRegistDto
    {
        [Required(ErrorMessage ="请填写用户名称")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "请填写Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "请输入密码")]
        [Compare("ConfirmPassWord")]
        public string PassWord { get; set; }
        [Required(ErrorMessage = "请再次输入密码")]
        public string ConfirmPassWord { get; set; }
    }
}
