using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Models
{
    public class Values
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class testtable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}