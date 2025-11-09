using VegAndFrut.Models;
using Vegetable.DAL;

namespace VegAndFrut.BLL;

public class Service
{
    private readonly DbContext _crud;

    public Service()
    {
        _crud = new DbContext();
    }
    
    public IEnumerable<Product> GetAll() => _crud.GetAll();
    
    public IEnumerable<string> GetFiltered(
        IEnumerable<string?>? types = null, IEnumerable<string?>? colors = null, decimal? fromCalories = null,
        decimal? toCalories = null) => _crud.GetFiltered(types , colors , fromCalories ,toCalories);

    public IEnumerable<Product> GetByName(string name,IEnumerable<Product>? products = null)
    {
        if (products is null) products = GetAll();
        return products.Where(product => product.Name.Contains(name));
    }

    public IEnumerable<string> GetTypes(IEnumerable<Product>? products = null) => _crud.GetAllTypes();
    public IEnumerable<string> GetColors(IEnumerable<Product>? products = null)
    {
        if (products is null) products = GetAll();
        return products.Select(x => x.Color).Distinct();
    }

    public decimal GetMax(IEnumerable<Product>? products = null)
    {
        if (products is null) products = GetAll();
        return products.Max(x => x.Calories);
    }
    public decimal GetMin(IEnumerable<Product>? products = null)
    {
        if (products is null) products = GetAll();
        return products.Min(x => x.Calories);
    }
    public decimal GetAvg(IEnumerable<Product>? products = null)
    {
        if (products is null) products = GetAll();
        return products.Average(x => x.Calories);
    }

    public bool Add(Product product) => _crud.Insert(product);
    public bool Delete(Product product) => _crud.Delete(product);
    public bool Update(Product product) => _crud.Update(product);
    
    
}