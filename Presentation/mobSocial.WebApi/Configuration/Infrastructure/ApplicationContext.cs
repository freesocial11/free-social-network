using System.Web;
using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.Currency;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Authentication;
using Microsoft.Owin;

namespace mobSocial.WebApi.Configuration.Infrastructure
{
    public class ApplicationContext
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Stores the current user
        /// </summary>
        private User _user;

        /// <summary>
        /// Stores the active currency
        /// </summary>
        private Currency _currency;

        public ApplicationContext()
        {
            _authenticationService = mobSocialEngine.ActiveEngine.Resolve<IAuthenticationService>();
        }

        /// <summary>
        /// Gets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if (_user != null)
                    return _user;

                _user = _authenticationService.GetCurrentUser();
                return _user;
            }
        }

        public Currency ActiveCurrency
        {
            get
            {
                return _currency;
            }
        }
        /// <summary>
        /// Gets the current httpcontext
        /// </summary>
        public HttpContext CurrentHttpContext => HttpContext.Current;

        /// <summary>
        /// Gets the current owincontext
        /// </summary>
        public IOwinContext CurrentOwinContext => CurrentHttpContext.GetOwinContext();

        /// <summary>
        /// Signs in the user with email 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="isPersistent"></param>
        /// <param name="forceCreate"></param>
        public LoginStatus SignIn(string email, bool isPersistent, bool forceCreate = false)
        {
            return _authenticationService.SignIn(email, isPersistent, forceCreate);
        }

        /// <summary>
        /// Gets the current application context
        /// </summary>
        public static ApplicationContext Current
        {
            get
            {
                var instance = mobSocialEngine.ActiveEngine.IocContainer.Resolve<ApplicationContext>(IfUnresolved.ReturnDefault);
                if (instance == null)
                    instance = mobSocialEngine.ActiveEngine.RegisterAndResolve<ApplicationContext>(new ApplicationContext());

                return instance;
            }
        }

    }
}