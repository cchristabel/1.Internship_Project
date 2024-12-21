using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

public class PurchaseDepartmentLimits
{
    [PolicyAttributeValue(PolicyAttributeCategories.Resource, "MaxPurchaseOrderValue")]
    public double? MaxPurchaseOrder { get; set; }
}

public class PurchaseDepartmentAttributeProvider : RecordAttributeValueProvider<PurchaseDepartmentLimits>
{
    // Define the role attribute for resolution
    private static readonly PolicyAttribute Role =
             new PolicyAttribute("role",
                        PolicyValueType.String,
                        PolicyAttributeCategories.Subject);

        protected override async Task<PurchaseDepartmentLimits> GetRecordValue(IAttributeResolver attributeResolver, CancellationToken ct)
    {
        // Retrieve the roles from the evaluation context
        IReadOnlyCollection<string> roles = await attributeResolver.Resolve<string>(Role, ct);
        
        foreach (var role in roles)
        // Validate role data
        if (roles == null || !roles.Any())
        {
            throw new ArgumentException("No role found.");
        }

        // For multiple roles, we can define a strategy, e.g., select the highest limit or throw a warning.
        double purchaseOrderLimit = 0;

        // Choose a method to resolve multiple roles
        foreach (var role in roles)
        {
            var normalizedRole = role.ToLower();
            switch (normalizedRole)
            {
                case "boardmember":
                    purchaseOrderLimit = Math.Max(purchaseOrderLimit, 2000);
                    break;
                case "manager":
                    purchaseOrderLimit = Math.Max(purchaseOrderLimit, 1500);
                    break;
                case "administrator":
                    purchaseOrderLimit = Math.Max(purchaseOrderLimit, 1300);
                    break;
                case "employee":
                    purchaseOrderLimit = Math.Max(purchaseOrderLimit, 1000);
                
                    break;
                default:
                    throw new ArgumentException($"Unknown role: {normalizedRole}");
            }
        }

        // Print the final purchase order limit before returning the result
        
        return new PurchaseDepartmentLimits()
        {
            MaxPurchaseOrder = purchaseOrderLimit
        };
    }
}
