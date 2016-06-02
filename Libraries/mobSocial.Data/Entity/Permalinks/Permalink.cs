using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Permalinks
{
    public class Permalink : BaseEntity
    {
        public string Slug { get; set; }

        public bool Active { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        /// <summary>
        /// Returns slug of the permlaink
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Slug;
        }
    }

    public class PermalinkMap: BaseEntityConfiguration<Permalink> { }
}