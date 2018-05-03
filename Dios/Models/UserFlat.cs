using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dios.Models
{
    public class UserFlat
    {
        public string UserID { get; set; }
        public User User { get; set; }

        public int FlatID { get; set; }
        public Flat Flat { get; set; }
    }
}
