using System;
using System.Collections.Generic;
using System.Text;

namespace LordOfTheWings.DAL.Models
{
    public class Order
    {
        public int tableNumber { get; set; }
        public List<Dish> orderList { get; set; }
        public float totalPrice { get; set; }
    }
}
