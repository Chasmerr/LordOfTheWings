using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LordOfTheWings.DAL.Models
{
    public class Opinion
    {
        [Display(Name = "Contents")]
        [DataType(DataType.MultilineText)]
        public string content { get; set; }
        [Display(AutoGenerateField = false)]
        public string date { get; set; }
        [Display(AutoGenerateField = false)]
        public float positivity { get; set; }
    }
}
