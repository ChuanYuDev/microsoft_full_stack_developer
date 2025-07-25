using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.InMemory
{
    public class ProductsInMemoryRepository : IProductsRepository
    {
        private List<Product> _products = new List<Product>()
        {
            new Product { ProductId = 1, CategoryId = 1, Name = "Iced Tea", Quantity = 100, Price = 1.99 },
            new Product { ProductId = 2, CategoryId = 1, Name = "Canada Dry", Quantity = 200, Price = 1.99 },
            new Product { ProductId = 3, CategoryId = 2, Name = "Whole Wheat Bread", Quantity = 300, Price = 1.50 },
            new Product { ProductId = 4, CategoryId = 2, Name = "White Bread", Quantity = 300, Price = 1.50 }
        };
        private readonly ICategoriesRepository categoriesRepository;

        public ProductsInMemoryRepository(ICategoriesRepository categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public void AddProduct(Product product)
        {
            int maxId = 0;

            if (_products.Count > 0)
            {
                maxId = _products.Max(x => x.ProductId);
            }

            product.ProductId = maxId + 1;
            _products.Add(product);
        }

        // public static List<Product> GetProducts() => _products;

        public IEnumerable<Product> GetProducts(bool loadCategory = false)
        {
            if (loadCategory)
            {
                // Count vs Count()
                //      Count() is an extension method introduced by LINQ
                //      While the Count property is part of the List itself (derived from ICollection)
                //      Internally though, LINQ checks if your IEnumerable implements ICollection and if it does it uses the Count property
                //      So at the end of the day, there's no difference which one you use for a List
                if (_products != null && _products.Count > 0)
                {
                    // ForEach: perform specific action on each element of List<>
                    _products.ForEach(x =>
                    {
                        if (x.CategoryId.HasValue)
                        {
                            x.Category = categoriesRepository.GetCategoryById(x.CategoryId.Value);
                        }
                    });
                }
            }

            // If _products is null, return an empty list of product
            return _products ?? new List<Product>();
        }
        public IEnumerable<Product> GetProductsByCategoryId(int categoryId)
        {
            // Where: filters a sequence of values based on a predicate
            var products = _products.Where(x => x.CategoryId == categoryId);

            if (products != null)
                return products.ToList();

            else
                return new List<Product>();
        }

        public Product? GetProductById(int productId, bool loadCategory = false)
        {
            var product = _products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                var prod = new Product
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    CategoryId = product.CategoryId
                };

                if (loadCategory && prod.CategoryId.HasValue)
                {
                    prod.Category = categoriesRepository.GetCategoryById(prod.CategoryId.Value);
                }

                return prod;
            }

            return null;
        }

        public void UpdateProduct(int productId, Product product)
        {
            if (productId != product.ProductId) return;

            var productToUpdate = _products.FirstOrDefault(x => x.ProductId == productId);
            if (productToUpdate != null)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Quantity = product.Quantity;
                productToUpdate.Price = product.Price;
                productToUpdate.CategoryId = product.CategoryId;
            }
        }

        public void DeleteProduct(int productId)
        {
            var product = _products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                _products.Remove(product);
            }
        }
    }
}