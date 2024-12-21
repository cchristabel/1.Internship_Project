
using System;
using System.Threading;
using SecureMVCApp.Models;

namespace SecureMVCApp.Models
{

    public class Product
    {
        

        public int Id { get; set; }  // Ensure that all properties have both type and name
        public string ProductName { get; set; }
        
        public double Amount { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }

        public override string ToString()
        {
            return "Id: " + Id + ", ProductName: " + ProductName + ", Amount: " + Amount  + ", Description: " + Description + ", ProductType: " + ProductType;
        }

    }


}
