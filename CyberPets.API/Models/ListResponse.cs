using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models
{
    public class ListResponse<TItem>
    {
        [Required]
        public IEnumerable<TItem> List { get; set; }
    }
}
