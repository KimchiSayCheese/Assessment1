using System.Data;
using MagellanTest.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly string _connectionString;

        public ItemsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PostGresConnection");
        }

        [HttpGet("totalcost/{itemName}")]
        public IActionResult TotalCost(string itemName)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var sqlCommand = new NpgsqlCommand(
                "SELECT Get_Total_Cost(@itemName)", conn);
            sqlCommand.Parameters.AddWithValue("itemName", itemName);
            var totalCost = sqlCommand.ExecuteScalar();
            if (totalCost != null && totalCost != DBNull.Value)
            {
                return Ok(totalCost);
            }
            
            return NotFound($"No total cost for {itemName}. Make sure it is a parent Item.");
            
        }
        
        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var sqlCommand = new NpgsqlCommand(
                "SELECT * FROM item WHERE id = @id", conn);
            sqlCommand.Parameters.AddWithValue("id", id);

            using var reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                int? parentItemId = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                return Ok(new Item
                {
                    Id = id,
                    ItemName = reader.GetString(1),
                    ParentItemId = parentItemId,
                    Cost = reader.GetInt32(3),
                    ReqDate = reader.GetDateTime(4)
                });
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public IActionResult AddItem(Item item)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var sqlCommand = new NpgsqlCommand(
                "INSERT INTO item (item_name, parent_item, cost, req_date) " +
                "VALUES (@itemName, @parentItem, @cost, @reqDate) RETURNING id", conn);
            sqlCommand.Parameters.AddWithValue("itemName", item.ItemName);
            if (item.ParentItemId is not null)
            {
                sqlCommand.Parameters.AddWithValue("parentItem", item.ParentItemId);
            }
            sqlCommand.Parameters.AddWithValue("cost", item.Cost);
            sqlCommand.Parameters.AddWithValue("reqDate", item.ReqDate);

            var id = sqlCommand.ExecuteScalar();
            Console.WriteLine(id);
            Console.WriteLine(id);
            Console.WriteLine(id);
            if (id != null && id != DBNull.Value)
            {
                item.Id = Convert.ToInt32(id);
                return CreatedAtAction(nameof(AddItem), new { id = id }, item);

            }
            return StatusCode(500, "Failed to add new Item.");
        }
        
    }
}
