using System.ComponentModel.DataAnnotations;

namespace demodockerv2.webapi.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        public string? Image { get; set; }

        [StringLength(20)]
        public string? Class { get; set; }

        public bool Gender { get; set; }
    }
}