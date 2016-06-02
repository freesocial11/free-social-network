using mobSocial.Core.Data;

namespace mobSocial.Data.Interfaces
{
    public interface IPicturesSupported<T> where T: BaseEntity
    {
        int Id { get; set; }
    }
}