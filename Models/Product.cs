using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUISqlLite.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public Product Clone() => MemberwiseClone() as Product;

        // validations
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return (false, $"{nameof(Name)} is required");
            }
            else if(Price <= 0)
            {
                return (false, $"{nameof(Price)} should be greater than 0");
            }
            return (true, null);
        }
    }
}
