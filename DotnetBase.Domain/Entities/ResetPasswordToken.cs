namespace DotnetBase.Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ResetPasswordToken")]
    public class ResetPasswordToken : BaseEntity
    {
        public ResetPasswordToken() : base()
        {

        }

        [Required]
        [MaxLength(50)]
        public string ResetPasswordCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Token { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        public DateTime ExpiredTime { get; set; }
    }
}
