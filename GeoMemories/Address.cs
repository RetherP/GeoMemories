namespace GeoMemories
{
    public class Address
    {
        public string? Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int? MapPinID { get; set; }
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Street) 
                ? $"{City.Trim()}, {Country.Trim()}" :  
                $"{Street.Trim()}, {City.Trim()}, {Country.Trim()}";
        }
    }
}