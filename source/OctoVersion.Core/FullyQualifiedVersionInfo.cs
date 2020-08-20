namespace OctoVersion.Core
{
    public class FullyQualifiedVersionInfo
    {
        public FullyQualifiedVersionInfo(VersionInfo versionInfo, string preReleaseTag, string buildMetadata)
        {
            VersionInfo = versionInfo;
            PreReleaseTag = preReleaseTag;
            BuildMetadata = buildMetadata;
        }

        public VersionInfo VersionInfo { get; }
        public string PreReleaseTag { get; }
        public string BuildMetadata { get; }

        public override string ToString()
        {
            var structuredOutput = new StructuredOutput(VersionInfo.Major, VersionInfo.Minor, VersionInfo.Patch,
                PreReleaseTag, BuildMetadata);
            return structuredOutput.ToString();
        }
    }
}