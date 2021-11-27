using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySales.Shared.Models
{
    public class UserLogin
    {
        [Display(Name ="Usuario"),Required(ErrorMessage = Utilities.MSGREQUIRED)]
        public string UserName { get; set; }

        [Display(Name = "Password"), Required(ErrorMessage = Utilities.MSGREQUIRED)]
        public string Password { get; set; }
    }
}
