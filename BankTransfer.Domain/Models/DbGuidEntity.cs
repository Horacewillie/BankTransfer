using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public abstract class DbEntity<T>
    {
        public T Id { get; set; }
        [JsonIgnore]
        public bool Deleted { get; set; }
    }

    public abstract class DbGuidEntity : DbEntity<Guid>
    {

        public DbGuidEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
