using System;

namespace SaG.GuidReferences
{
    public interface IGuidProvider
    {
        Guid GetGuid();
        string GetStringGuid();
    }
}