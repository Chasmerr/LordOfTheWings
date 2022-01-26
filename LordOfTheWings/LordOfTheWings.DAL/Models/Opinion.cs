using System;
using System.Collections.Generic;
using System.Text;

namespace LordOfTheWings.DAL.Models
{
    internal class Opinion
    {
        public string content { get; set; }
        public string date { get; set; }
        public float positivity { get; set; }
    }
}
