using System.Collections.Generic;
using SecureMVCApp.Models; // Ensure the correct models are referenced

namespace SecureMVCApp.ViewModels // Change to .Models if placed in the Models folder
{
     public class MyMultipleModelInOneView
    {
        public IEnumerable<PurchaseOrder> PurchaseOrders { get; set; }
        public IEnumerable<Product> AllProducts { get; set; }
        public double TotalCartAmount  { get; set; }
        // Add more properties as needed
    }
}



