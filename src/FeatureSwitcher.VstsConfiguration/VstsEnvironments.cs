namespace FeatureSwitcher.VstsConfiguration
{
    public static class VstsEnvironments
    {
        public static void AddEnvironment(this VstsSettings settings, string environment)
        {
            if (settings.AdditionalFields.ContainsKey("System.Tags"))
            {
                settings.AdditionalFields["System.Tags"] = $"{settings.AdditionalFields["System.Tags"]},{environment}";
            }
            else
            {
                settings.AdditionalFields.Add("System.Tags", environment);
            }

            settings.AdditionalQueryFilter = $"{settings.AdditionalQueryFilter} and [System.Tags] Contains '{environment}'".TrimStart();
        }
    }
}
