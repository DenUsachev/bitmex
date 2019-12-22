using System;

namespace MarketConnectivity.REST.Entities
{
    public class UserObject : BaseRestObject
    {
        public int Id { get; set; }
        public object OwnerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public object Phone { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Country { get; set; }
        public string GeoipCountry { get; set; }
        public object GeoipRegion { get; set; }
        public string Typ { get; set; }
    }
}
