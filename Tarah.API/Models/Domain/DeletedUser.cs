using System.ComponentModel.DataAnnotations;

namespace Tarah.API.Models.Domain
{
    public class DeletedUser
    {
        [Key]
        public Guid UserId { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
