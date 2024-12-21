using System.Collections.Generic;
using System.Linq;
using SecureMVCApp.Models;

namespace SecureMVCApp.Services
{
    public interface IManagePurchaseOrders
    {
        void Add(PurchaseOrder purchaseOrder);
        PurchaseOrder FindById(int id);
        
        IEnumerable<PurchaseOrder> All { get; }

        void ClearAll();
        
        
    }
    public class PurchaseOrderService : IManagePurchaseOrders
    {
        private readonly List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
        
        public void Add(PurchaseOrder purchaseOrder)
        {
           purchaseOrders.Add(purchaseOrder);
        }

        public PurchaseOrder FindById(int id)
        {
            return purchaseOrders.First(po => po.Id == id);
        }

        public void Update(PurchaseOrder purchaseOrder)
        {
            var existingOrder = FindById(purchaseOrder.Id); // Find the existing order
            if (existingOrder != null)
            {
                existingOrder.Description = purchaseOrder.Description; // Update properties
                existingOrder.Amount = purchaseOrder.Amount;
                // Add more properties to update as needed
            }
        }
        public IEnumerable<PurchaseOrder> All => purchaseOrders;

        public void ClearAll()
        {
            purchaseOrders.Clear();
        }
    }
}