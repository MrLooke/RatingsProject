using System.ComponentModel.DataAnnotations;

namespace MediaBoard.Server.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Token { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public virtual AppUser User { get; set; } = null!;
    }
}
