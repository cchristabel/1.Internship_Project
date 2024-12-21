using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using SecureMVCApp.Models;
using SecureMVCApp.Services;



namespace SecureMVCApp.Controllers
{
    [Authorize]
    [EnforcerAuthorization(ResourceType = "PurchaseOrder")]
    public class PurchaseOrdersController : Controller
    {
        private readonly IManagePurchaseOrders purchaseOrders;

        private readonly IManageProducts products;

        private readonly IPolicyEnforcementPoint pep;

        public PurchaseOrdersController(IManagePurchaseOrders purchaseOrders, 
            IManageProducts products,
            IPolicyEnforcementPoint pep)
        {
            this.purchaseOrders = purchaseOrders;
            this.products = products;
            this.pep = pep;
        }

        [HttpGet]
        [Route("/PurchaseOrders", Name = "Index")]
        public IActionResult ListAllPurchaseOrders()
        {
            System.Console.WriteLine("------ListAllPurchaseOrders--------");
            SecureMVCApp.ViewModels.MyMultipleModelInOneView mymodel = new SecureMVCApp.ViewModels.MyMultipleModelInOneView();
            mymodel.PurchaseOrders = purchaseOrders.All;
            mymodel.AllProducts = products.LoadProductsFromJsonDB();
            return View("PurchaseOrders", mymodel);
        }

        [HttpPost]
        [Route("/PurchaseOrders")]
        public async Task<IActionResult> CreatePurchaseOrder(string action, List<int> selectedProductIds, double TotalCartAmount)
        {
            SecureMVCApp.ViewModels.MyMultipleModelInOneView mymodel = new SecureMVCApp.ViewModels.MyMultipleModelInOneView();

            System.Console.WriteLine("------CreatePurchaseOrder---1-----");
            System.Console.WriteLine(action);
               
            string role = User.Claims.First(c => c.Type == "role").Value;
            System.Console.WriteLine(role);
            System.Console.WriteLine(selectedProductIds);
            
            
            List<Product> selectedProducts = products.FindSelected(selectedProductIds);
            foreach (var product in selectedProducts)
            {   
                System.Console.WriteLine(product);
                purchaseOrders.Add(new PurchaseOrder((double)product.Amount, product.ProductName, role));
                
                TotalCartAmount += product.Amount;
            }
            mymodel.PurchaseOrders = purchaseOrders.All;
            mymodel.AllProducts = products.LoadProductsFromJsonDB();
            mymodel.TotalCartAmount = TotalCartAmount;
            
            if (action.Equals("Confirm Cart")) {
                System.Console.WriteLine(TotalCartAmount);
                IAttributeValueProvider context = new EditPurchaseOrderAuthorizationContext("CreatePurchaseOrder")
                {
                    PurchaseOrderRole = role,
                    PurchaseOrderAmount = TotalCartAmount
                };

                var authorizationResult = await pep.Evaluate(context);
                System.Console.WriteLine(authorizationResult.Outcome);

                if (authorizationResult.Outcome != PolicyOutcome.Permit)
                {
                    return AuthorizationFailed(authorizationResult);
                }
                else
                {
                    return AuthorizationSuccess(TotalCartAmount);
                }
            }
            return View("PurchaseOrders", mymodel);
        }
        

        private IActionResult AuthorizationFailed(PolicyEvaluationOutcome po)
        {
            IEnumerable<AuthorizationFailureAdvice>failureReason =
            po.UnresolvedAdvice.Find<AuthorizationFailureAdvice>("AuthorizationFailure");

            var view = View("NotAuthorized", failureReason);
            view.StatusCode = (int) HttpStatusCode.Forbidden;
            purchaseOrders.ClearAll();
            return view;
        }
        
        private IActionResult AuthorizationSuccess(double PurchaseOrderTotal)
        {
            string Message = "Your purchase order for $" + PurchaseOrderTotal + " confirmed.";
            List<string> list = new List<string>();
            list.Add(Message);
            var view = View("CardAuthorized", list);
            view.StatusCode = (int) HttpStatusCode.Forbidden;
            purchaseOrders.ClearAll();
            return view;
        }
    
    }
}
