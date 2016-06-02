namespace mobSocial.Services.Installation
{
    public interface IInstallationService
    {
        void Install();

        void Install(string connectionString, string providerName);

        void FillRequiredSeedData(string defaultUserEmail, string defaultUserPassword);
    }
}