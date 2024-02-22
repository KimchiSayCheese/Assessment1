using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagellanTest.Models;

public class Item
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string ItemName { get; set; }
    
    [ForeignKey("ParentItem")]
    public int? ParentItemId { get; set; }
    public Item? ParentItem { get; set; }
    
    [Required]
    public int Cost { get; set; }
    
    [Required]
    public DateTime ReqDate { get; set; }
}
//
// CREATE TABLE item (
//     id SERIAL PRIMARY KEY,
//     item_name VARCHAR(50) NOT NULL,
//     parent_item INTEGER,
//     cost INTEGER NOT NULL,
//     req_date DATE NOT NULL,
//     CONSTRAINT fk_parent_item FOREIGN KEY (parent_item) REFERENCES item (id)
// );