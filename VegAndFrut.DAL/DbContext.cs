using System.Text;
using NpgsqlTypes;
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
            { "id", entity.Id },
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

    public IEnumerable<string> GetFiltered(
        IEnumerable<string>? types = null, IEnumerable<string>? colors = null, decimal? fromCalories = null,
        decimal? toCalories = null)
    {
        if (types is null) types = GetAllTypes();
        if (colors is null) colors = GetAllColors();
        if (fromCalories is null) fromCalories = GetMinCalories();
        if (toCalories is null) toCalories = GetMaxCalories();

        string types_parameter = ConvertCollectionToString(types);
        string colors_parameter = ConvertCollectionToString(colors);
        
        var products = new List<string>();

        _connection.Open();

        const string sql = """
                           SELECT * FROM table_products
                           WHERE (type = ANY (@types)) AND (color = ANY(@colors))
                             AND (calories BETWEEN @fromCalories AND @toCalories);
                           """;
        var command = new NpgsqlCommand(sql, _connection);
        
        command.Parameters.AddWithValue("@types", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, types?.ToArray());
        command.Parameters.AddWithValue("@colors", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, colors?.ToArray());
        command.Parameters.AddWithValue("fromCalories" ,fromCalories);
        command.Parameters.AddWithValue("toCalories", toCalories);
        
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

                products.Add(new Product(id, name, type, color, calories).ToString());
            }
        }

        _connection.Close();

        return products;
    }

    public IEnumerable<string> GetAllTypes()
    {
        var types = new List<string>();

        _connection.Open();

        const string sql = "SELECT DISTINCT type FROM table_products";
        var command = new NpgsqlCommand(sql, _connection);
        var reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                types.Add(reader.GetString("type"));
            }
        }

        _connection.Close();

        return types;
    }

    public IEnumerable<string> GetAllColors()
    {
        var colors = new List<string>();

        _connection.Open();

        const string sql = "SELECT DISTINCT color FROM table_products";
        var command = new NpgsqlCommand(sql, _connection);
        var reader = command.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                colors.Add(reader.GetString("color"));
            }
        }

        _connection.Close();

        return colors;
    }

    public decimal? GetMaxCalories()
    {
        var maxCalories = new List<string>();

        _connection.Open();

        const string sql = "SELECT MAX(calories) FROM table_products";
        var command = new NpgsqlCommand(sql, _connection);
        var calories = command.ExecuteScalar() as decimal?;

        _connection.Close();

        return calories;
    }
    
    public decimal? GetMinCalories()
    {
        var minCalories = new List<string>();

        _connection.Open();

        const string sql = "SELECT MIN(calories) FROM table_products";
        var command = new NpgsqlCommand(sql, _connection);
        var calories = command.ExecuteScalar() as decimal?;

        _connection.Close();

        return calories;
    }

    public string ConvertCollectionToString<T>(IEnumerable<T> collection)
    {
        StringBuilder result = new StringBuilder();
        foreach (var item in collection)
        {
            result.Append("'" + item.ToString() + "', ");
        }

        return result.ToString().TrimEnd(',', ' ');
    }
}