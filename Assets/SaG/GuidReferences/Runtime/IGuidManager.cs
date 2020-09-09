using System;
using UnityEngine;

namespace SaG.GuidReferences
{
    public interface IGuidManager
    {
        bool Add(GuidComponent guidComponent);

        bool Remove(Guid guid);

        GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback);
    }
}