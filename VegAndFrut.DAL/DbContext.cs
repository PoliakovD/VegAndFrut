using VegAndFrut.Models;

namespace Vegetable.DAL;

using Npgsql;
using System.Data;

public class DbContext : ICrud<Product>
{
    private const string ConnectionString =
        "Server=127.0.0.1;Port=5432;Database=veg_and_frut_db;User Id=postgres;Password=1234;";

    private readonly NpgsqlConnection _connection;

    public DbContext()
    {
        _connection = new NpgsqlConnection(ConnectionString);
    }

    public IEnumerable<Product> GetAll()
    {
        var products = new List<Product>();

        _connection.Open();

        const string sql = "SELECT * FROM table_products";
        var command = new NpgsqlCommand(sql, _connection);
        var reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                var id = reader.GetInt32("id");
                var name = reader.GetString("name");
                var type = reader.GetString("type");
                var color = reader.GetString("color");
                var calories = reader.GetDecimal("calories");

                products.Add(new Product(id, name, type, color, calories));
            }
        }

        _connection.Close();

        return products;
    }

    public bool Insert(Product entity)
    {
        const string sql = """
                           INSERT INTO table_products (name,type,color, calories) 
                           VALUES (@name, @type,@color,@calories);
                           """;
        var parameters = new Dictionary<string, object>
        {
            { "name", entity.Name },
            { "type", entity.Type },
            { "color", entity.Color },
            { "calories", entity.Calories }
        };

        return Exec(sql, parameters);
    }
    

    public bool Update(Product entity)
    {
        const string sql = """
                           UPDATE table_products
                           SET name = @name, type = @type, color = @color, calories = @calories
                           WHERE id = @id;
                           """;
        var parameters = new Dictionary<string, object>
        {
            {"id", entity.Id },
            { "name", entity.Name },
            { "type", entity.Type },
            { "color", entity.Color },
            { "calories", entity.Calories }
        };
        return Exec(sql, parameters);
    }

    public bool Delete(Product entity)
    {
        const string sql = "DELETE FROM table_products WHERE id = @id;";

        var parameters = new Dictionary<string, object>
        {
            { "id", entity.Id }
        };

        return Exec(sql, parameters);
    }

    private bool Exec(string sql, Dictionary<string, object> parameters)
    {
        _connection.Open();

        var command = new NpgsqlCommand(sql, _connection);

        foreach (var (key, value) in parameters)
        {
            command.Parameters.AddWithValue(key, value);
        }

        var result = command.ExecuteNonQuery();

        _connection.Close();

        return result > 0;
    }
}