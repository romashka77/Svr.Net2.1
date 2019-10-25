
namespace Svr.Core.Entities
{
    /// <summary>
    /// связь многие ко многим
    /// </summary>
    public class DistrictPerformer
    {
        public long PerformerId { get; set; }
        public virtual Performer Performer { get; set; }

        public long DistrictId { get; set; }
        public virtual District District { get; set; }

    }
}
