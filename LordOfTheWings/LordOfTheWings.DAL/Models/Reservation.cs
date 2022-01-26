using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LordOfTheWings.DAL.Models
{
    public class Reservation
    {
        [Display(Name = "Table Number")]
        public int TableNumber { get; set; }
        [Display(Name = "Reserved by")]
        public string ReservedBy { get; set; }
        [Display(Name = "Reservation date")]
        public string ReservationDate { get; set; }
        [Display(Name = "Reservation time")]
        public string ReservationTime { get; set; }
    }
}
