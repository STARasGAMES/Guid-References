using System;
using UnityEngine;

namespace SaG.GuidReferences
{
    public interface IGuidManager
    {
        bool Add(Guid guid, GameObject gameObject);

        bool Remove(Guid guid);

        GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback);
    }
}