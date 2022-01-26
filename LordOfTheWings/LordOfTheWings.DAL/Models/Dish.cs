using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LordOfTheWings.DAL.Models
{
    public class Dish
    {
        public string dishName { get; set; }
        public bool isItWithMeat { get; set; }
        public float price { get; set; }
    }
}
