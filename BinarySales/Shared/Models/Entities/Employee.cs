using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySales.Shared.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido"), MaxLength(100), Display(Name = "Nombres")]
        public string Names { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido"), MaxLength(100), Display(Name = "Apellidos")]
        public string LastNames { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido"), MinLength(6, ErrorMessage = Utilities.MSGSTRINGlENGSHORT), MaxLength(30), Display(Name = "DNI")]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "El Campo {0} solo admite letras y números sin espacios o separadores")]
        public string DNI { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido"), MaxLength(30), Display(Name = "Teléfono"), Phone(ErrorMessage = Utilities.MSGFORMATFAIL)]
        public string Phone { get; set; }

        [MaxLength(100), Display(Name = "Email"), EmailAddress(ErrorMessage = "El formato del texto no corresponde a un Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El Campo {0} es requerido"), Display(Name = "Posición Laboral")]
        public string Rol { get; set; }

        [Required, MaxLength(15)]
        public string Status { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}
