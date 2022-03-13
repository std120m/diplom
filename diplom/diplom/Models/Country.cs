namespace diplom.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public Country(string? name, string? code)
        {
            Name = name;
            Code = code;
        }
    }
}
