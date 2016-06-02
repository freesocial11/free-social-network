using mobSocial.Core.Data;

namespace mobSocial.Data.Interfaces
{
    public interface IHasEntityProperties<T> : IHasEntityProperties where T: BaseEntity
    {
       
    }

    public interface IHasEntityProperties
    {
        int Id { get; set; }
    }
}