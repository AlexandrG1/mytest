
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

ParseJson parse = new ParseJson();
parse.UseJSON();

public class ParseJson
{
    private readonly string _patchJson = "E:\\TestJSON.json";

    public void UseJSON()
    {
        var text = File.ReadAllText(_patchJson);

        var categoryMatches = Regex.Matches(text, @"""name""\s*:\s*""(?<CategoryName>Electronics|Clothing|Books)""");
        var productMatches = Regex.Matches(text, @"""name"":\s*""(?<ProductName>[^""]+)"",\s*""price"":\s*(?<Price>\d+\.\d+),\s*""stock"":\s*(?<Stock>\d+)");
        var namelocationMatch = Regex.Match(text, @"""name""\:\s*""(?<name>\w*)""\,\s*""location""\:\s*""(?<location>\w*)""").Groups;
        var location = namelocationMatch["location"].Value;
        var name = namelocationMatch["name"].Value;


        var storeCategories = new Dictionary<string, List<Product>>();

        foreach (Match categoryMatch in categoryMatches)
        {
            string categoryName = categoryMatch.Groups["CategoryName"].Value;
            storeCategories[categoryName] = new List<Product>();
        }

        int currentCategoryIndex = 0;

        foreach (Match productMatch in productMatches)
        {
            while (currentCategoryIndex < categoryMatches.Count - 1 &&
                   productMatch.Index > categoryMatches[currentCategoryIndex + 1].Index)
            {
                currentCategoryIndex++;
            }

            string currentCategory = categoryMatches[currentCategoryIndex].Groups["CategoryName"].Value;

            Product product = new Product
            {
                name = productMatch.Groups["ProductName"].Value,
                price = float.Parse(productMatch.Groups["Price"].Value, System.Globalization.CultureInfo.InvariantCulture),
                stock = int.Parse(productMatch.Groups["Stock"].Value)
            };

            storeCategories[currentCategory].Add(product);
        }

        Store store = new Store
        {
            storeName = name,
            storeLocation = location,
            storeCategories = storeCategories.Select(kv => new Category
            {
                categoryName = kv.Key,
                categoryProducts = kv.Value
            }).ToList()
        };

        Console.WriteLine($"Store: {store.storeName}, Location: {store.storeLocation}");
        foreach (var category in store.storeCategories)
        {
            Console.WriteLine($"Category: {category.categoryName}");
            foreach (var product in category.categoryProducts)
            {
                Console.WriteLine($"Product: {product.name}, Price: {product.price}, Stock: {product.stock}");
            }
            Console.WriteLine();
        }
    }


    public class Store
    {
        public string storeName { get; set; }
        public string storeLocation { get; set; }
        public List<Category> storeCategories { get; set; }
    }

    public class Category
    {
        public string categoryName { get; set; }
        public List<Product> categoryProducts { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public float price { get; set; }
        public int stock { get; set; }
    }
}
