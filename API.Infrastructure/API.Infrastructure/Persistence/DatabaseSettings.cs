using System;
using System.ComponentModel.DataAnnotations;

namespace API.Infrastructure.Persistence
{
    public class DatabaseSettings : IValidatableObject
    {
        public string DBProvider { get; set; } = string.Empty;
        public string MainConnectionstring { get; set; } = string.Empty;
        public string ESBConnectionstring { get; set; } = string.Empty;
        public string Akibaconnectionstring { get; set; } = string.Empty;
        public string Cypher { get; set; } = string.Empty;
        public string? MainConnectionstringEnvironmentVariable { get; set; }
        public string? ESBConnectionstringEnvironmentVariable { get; set; }
        public string? AkibaconnectionstringEnvironmentVariable { get; set; }
        public string? CypherEnvironmentVariable { get; set; }

        public void HydrateFromEnvironment()
        {
            MainConnectionstring = ResolveSecret(MainConnectionstring, MainConnectionstringEnvironmentVariable, nameof(MainConnectionstring));
            ESBConnectionstring = ResolveSecret(ESBConnectionstring, ESBConnectionstringEnvironmentVariable, nameof(ESBConnectionstring));
            Akibaconnectionstring = ResolveSecret(Akibaconnectionstring, AkibaconnectionstringEnvironmentVariable, nameof(Akibaconnectionstring));
            Cypher = ResolveSecret(Cypher, CypherEnvironmentVariable, nameof(Cypher));
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(DBProvider))
            {
                yield return new ValidationResult(
                    $"{nameof(DatabaseSettings)}.{nameof(DBProvider)} is not configured",
                    new[] { nameof(DBProvider) });
            }

            if (string.IsNullOrEmpty(MainConnectionstring) && string.IsNullOrEmpty(MainConnectionstringEnvironmentVariable))
            {
                yield return new ValidationResult(
                    $"{nameof(DatabaseSettings)}.{nameof(MainConnectionstring)} is not configured",
                    new[] { nameof(MainConnectionstring) });
            }
        }

        private static string ResolveSecret(string currentValue, string? environmentVariable, string settingName)
        {
            if (!string.IsNullOrWhiteSpace(currentValue))
            {
                return currentValue;
            }

            if (string.IsNullOrWhiteSpace(environmentVariable))
            {
                return currentValue;
            }

            var resolvedValue = Environment.GetEnvironmentVariable(environmentVariable);

            if (string.IsNullOrWhiteSpace(resolvedValue))
            {
                throw new InvalidOperationException($"Environment variable '{environmentVariable}' for '{settingName}' is not set.");
            }

            return resolvedValue;
        }
    }
}
