using System.ComponentModel.DataAnnotations;
using Ozmosis;

namespace Web.Models
{
    public class Title : Entity<Title>
    {
        [Required]
        public virtual string Name { get; set; }
        public virtual string Author { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}