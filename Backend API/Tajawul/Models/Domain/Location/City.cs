namespace Tajawul.Models.Domain
{
    public class City
    {
        public required string Id { get; set; }
        public required string Name { get; set; }

        public required Country Country { get; set; }
    }
}   
