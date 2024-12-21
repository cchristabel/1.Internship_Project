using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

public class EditPurchaseOrderAuthorizationContext : AuthorizationContext<EditPurchaseOrderAuthorizationContext>
{
    public EditPurchaseOrderAuthorizationContext(string action) :base("PurchaseOrder",action)
    {

    }
    [PolicyAttributeValue(PolicyAttributeCategories.Resource,"PurchaseOrderDepartment")]
    public string PurchaseOrderDepartment { get; set; }

    [PolicyAttributeValue(PolicyAttributeCategories.Action,"PurchaseOrderTotal")]
    public double? PurchaseOrderAmount { get; set; }

    [PolicyAttributeValue(PolicyAttributeCategories.Resource, "PurchaseOrderRole")]
    public string PurchaseOrderRole { get; set; }  // Role of the user or entity in context
    public double PurchaseOrderTotal { get; internal set; }
}

