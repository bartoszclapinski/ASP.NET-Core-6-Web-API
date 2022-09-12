namespace DemoApp.Models
{
    public class CitiesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public ICollection<PointOfInterestDTO> PointsOfInterest { get; set; } 
            = new List<PointOfInterestDTO>();
        public int NumberOfPointsOfInterest
        {
            get { return PointsOfInterest.Count; }
        }
    }
}
