
namespace TotalConnect.Extensions
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Model;

    public static class ObservableExtensions
    {
        /// <summary>
        /// Adds a unique zone entry to the given collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="zone"></param>
        public static void AddUnique(this ObservableCollection<ZoneEntry> collection, ZoneEntry zone)
        {
            var z = collection.SingleOrDefault(zz => zz.ZoneId == zone.ZoneId);
            if (z == null)
            {
                collection.Add(zone);
            }
            else
            {
                z.ZoneId = zone.ZoneId;
                z.ZoneName = zone.ZoneName;
                z.ZoneStatus = zone.ZoneStatus;
                z.ZoneStatusId = zone.ZoneStatusId;
            }
        }
    }
}
