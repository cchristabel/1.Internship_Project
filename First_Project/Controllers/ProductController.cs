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
using Microsoft.AspNetCore.Http;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace SecureMVCApp.Controllers
{

    [Authorize]
    //[EnforcerAuthorization(ResourceType = "Product")]
    public class ProductController : Controller
    {
        private readonly IManageProducts products;

       // private readonly ProductService _productService; // Add ProductService
        
        public ProductController(IManageProducts products)
        {
            this.products = products;
            //this._productService = productService; // Initialize ProductService
        }

        // 1. List all products
        [HttpGet]
        [Route("/Products")]
        
        public IActionResult Index()
        { 
            return View("Products", products.LoadProductsFromJsonDB());
        }
        
        private async Task LoadProductsFromCsv(string filePath, List<Product> newProducts)
        { 
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Product>().ToList();
            
            // Process the records as needed (e.g., save to database or update a list)
            foreach (var product in records)
            {

                var isDuplicate = newProducts.Any(p => p.Id == product.Id && p.ProductName == product.ProductName);
                if (!isDuplicate)
                {
                        // Log or handle duplicates
                    newProducts.Add(product);
                }
                else 
                {
                    System.Console.WriteLine($"CSV - Duplicate found: Id = {product.Id}, ProductName = {product.ProductName}");
                    // Add your logic to handle each record
                    System.Console.WriteLine(product.Id +", "+ product.ProductName +", "+ product.Description +", "+product.Amount +", "+product.ProductType); 
                    //products.Add(product);
                }

            }

        }

        [HttpPost]
        [Route("/Products")]
        //[EnforcerAuthorization(ResourceType = "Product", Action="Upload")]
        [EnforcerAuthorization(ResourceType = "PurchaseOrder",Action="Upload")]
        public async Task<IActionResult> Upload(IFormFile fileUpload)
        { 
            List<Product> newProducts = new List<Product>();
            try
            {
                if (fileUpload == null || fileUpload.Length == 0)
                {
                    ModelState.AddModelError("", "Please select a valid CSV file.");
                    return View("Products", products.LoadProductsFromJsonDB());
                }
                // Process the uploaded CSV file
                var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileUpload.FileName);

                using (var stream = new FileStream(csvPath, FileMode.Create))
                {
                        await fileUpload.CopyToAsync(stream);
                        
                }
                // Load products from the CSV file and check for errors
                
                await LoadProductsFromCsv(csvPath, newProducts);
            }
            catch (Exception ex)
            {
                // Catch any exception (including CSV format errors) and show an error message
                ModelState.AddModelError("", $"An error occurred while processing the CSV file: {ex.Message}");
                return View("Products", products.LoadProductsFromJsonDB());
            }
        
            if (newProducts.Count == 0)
            {
                // Log and notify that no data was found
                ModelState.AddModelError("", "No data found in the uploaded CSV file."); // Adding error message
                return View("Products", products.LoadProductsFromJsonDB());  // Return on success
                
            }
            else 
            {
                // Return the product list after successfully processing the CSV  
                products.LoadProductsToJsonDB(newProducts);
                return View("Products", products.LoadProductsFromJsonDB());  // Return on success
            }

            
                        
        }


               
        
        
    }

}



