namespace mobSocial.Data.Database
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; }

        string ProviderName { get; }

        void LoadSettings();

        void WriteSettings(string connectionString, string providerName);
    }
}