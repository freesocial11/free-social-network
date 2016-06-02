using mobSocial.Core.Services;
using mobSocial.Data.Entity.ArtistPages;

namespace mobSocial.Services.ArtistPages
{
    public interface IArtistPagePaymentService : IBaseEntityService<ArtistPagePayment>
    {
        void InsertPaymentMethod(ArtistPagePayment artistPagePayment);

        void DeletePaymentMethod(ArtistPagePayment artistPagePayment);

        void UpdatePaymentMethod(ArtistPagePayment artistPagePayment);

        ArtistPagePayment GetPaymentMethod(int artistPageId);
    }
}
