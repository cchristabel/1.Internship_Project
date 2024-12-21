using System.Collections.Generic;
using System.Linq;
using SecureMVCApp.Models;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace SecureMVCApp.Services
{
    public interface IManageProducts
    {
        Product FindById(int id);

        List<Product> FindSelected(List<int> selectedProductIds);

        List<Product> FindAll();

        List<Product> LoadProductsFromJsonDB();
        void LoadProductsToJsonDB(List<Product> products);
        
    }
    public class ProductService : IManageProducts
    {
        private string _productsJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "products.json");

        public Product FindById(int id)
        {
            
            System.Console.WriteLine("------kkkk----1-----");
            List<Product> allProducts = LoadProductsFromJsonDB();
            System.Console.WriteLine(allProducts.Count);

            Product product  = allProducts.Find(x => x.Id == id);
            System.Console.WriteLine(product);

           return product;
        }

        public List<Product> FindSelected(List<int> selectedProductIds)
        {
            List<Product> allProducts = LoadProductsFromJsonDB();
            List<Product> selectedProducts = new List<Product>();
            foreach (int id in selectedProductIds)
            {
                selectedProducts.Add(allProducts.Find(x => x.Id == id));               
            }
            return selectedProducts;
        }
        public List<Product> FindAll()
        {
            return LoadProductsFromJsonDB();
        }

        public List<Product> LoadProductsFromJsonDB()
        {   
            System.Console.WriteLine(_productsJsonFilePath);
            var jsonOld = File.ReadAllText(_productsJsonFilePath);
            List<Product> existingProducts = JsonSerializer.Deserialize<List<Product>>(jsonOld);
            
            return existingProducts;
            
        }
        public void LoadProductsToJsonDB(List<Product> products)
        {   
            
            var jsonOld = File.ReadAllText(_productsJsonFilePath);
            List<Product> existingProducts = JsonSerializer.Deserialize<List<Product>>(jsonOld);

            foreach (var product in products)
            {

                var isDuplicate = existingProducts.Any(p => p.Id == product.Id && p.ProductName == product.ProductName);
                if (!isDuplicate)
                {
                        // Log or handle duplicates
                    existingProducts.Add(product);
                }
                else 
                {
                    System.Console.WriteLine($"JSON - Duplicate found: Id = {product.Id}, ProductName = {product.ProductName}");
                    // Add your logic to handle each record
                    System.Console.WriteLine(product.Id +", "+ product.ProductName +", "+ product.Description +", "+product.Amount +", "+product.ProductType); 
                }

            }
            System.IO.File.WriteAllText(_productsJsonFilePath, string.Empty);
            string json = JsonSerializer.Serialize(existingProducts);
            File.WriteAllText(_productsJsonFilePath, json);
            
        }    
        
    }
}


