
using System;
using System.Threading;

namespace SecureMVCApp.Models
{
    // Enum defining possible roles
    public enum PurchaseOrderRole
    {
        boardmember,
        manager,
        employee,
        administrator
    }
    public class PurchaseOrder
    {
        private static int nextId = 0;
        public PurchaseOrder(double amount, string description, string role)
        {
            
            Id = Interlocked.Increment(ref nextId);
            Amount = (amount <= 0) ? throw new ArgumentException("Amount must be greater than zero", nameof(amount)) : amount;
            Description = (string.IsNullOrWhiteSpace(description)) ? throw new ArgumentNullException(nameof(description)) : description;
            Role = (string.IsNullOrWhiteSpace(role)) ? throw new ArgumentException("Role cannot be empty", nameof(role)) :role;
        }
        public int Id { get;  }
        public double Amount { get; set; }
        public string Description { get; set; }

        public string  Department { get;  set;}

        // Role property using Enum
        public string Role { get; set; }
    }
}

