using System.Collections.Generic;

namespace mobSocial.Core.Services.Configuration
{
    public interface IConfigurationFileService
    {
        IDictionary<string, string> ReadFile(string filePath, bool throwExceptionIfFileNotFound = false);

        void WriteFile(string savePath, IDictionary<string, string> configurationValues, bool overwriteIfFileExists = true);
    }
}