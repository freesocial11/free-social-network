using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.ArtistPages;

namespace mobSocial.Services.ArtistPages
{
    public class ArtistPagePaymentService: MobSocialEntityService<ArtistPagePayment>, IArtistPagePaymentService
    {
         private readonly IDataRepository<ArtistPagePayment> _paymentMethodRepository;

        public ArtistPagePaymentService(IDataRepository<ArtistPagePayment> pmRepository)
            : base(pmRepository)
        {
            _paymentMethodRepository = pmRepository;
        }

        public void InsertPaymentMethod(ArtistPagePayment artistPagePayment)
        {
            _paymentMethodRepository.Insert(artistPagePayment);
        }

        public void DeletePaymentMethod(ArtistPagePayment artistPagePayment)
        {
            if (artistPagePayment != null)
                _paymentMethodRepository.Delete(artistPagePayment);
        }

        public void UpdatePaymentMethod(ArtistPagePayment artistPagePayment)
        {
            if (artistPagePayment.Id != 0)
                _paymentMethodRepository.Update(artistPagePayment);
        }

        public ArtistPagePayment GetPaymentMethod(int artistPageId)
        {
            return _paymentMethodRepository.Get(x => x.ArtistPageId == artistPageId).FirstOrDefault();
        }
    }
}
