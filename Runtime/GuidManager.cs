using System;
using System.Collections.Generic;
using UnityEngine;

// Class to handle registering and accessing objects by GUID
namespace SaG.GuidReferences
{
    public class GuidManager : IGuidManager
    {
        // instance data
        private readonly Dictionary<Guid, GuidInfo> guidToObjectMap;
        
        // for each GUID we need to know the Game Object it references
        // and an event to store all the callbacks that need to know when it is destroyed
        private struct GuidInfo
        {
            public GameObject gameObject;

            public event Action<GameObject> Added;
            public event Action Removed;

            public GuidInfo(GuidComponent comp)
            {
                gameObject = comp.gameObject;
                Removed = null;
                Added = null;
            }

            public void HandleAddCallback()
            {
                Added?.Invoke(gameObject);
            }

            public void HandleRemoveCallback()
            {
                Removed?.Invoke();
            }
        }

        public GuidManager()
        {
            guidToObjectMap = new Dictionary<Guid, GuidInfo>();
        }

        public bool Add(GuidComponent guidComponent)
        {
            Guid guid;
            try
            {
                guid = guidComponent.GetGuid();
            }
            catch (Exception e)
            {
                if (guidComponent == null)
                    throw new ArgumentNullException(nameof(guidComponent), "There was an attempt to add NULL GuidComponent.");
                throw;
            }

            GuidInfo info = new GuidInfo(guidComponent);

            if (!guidToObjectMap.ContainsKey(guid))
            {
                guidToObjectMap.Add(guid, info);
                return true;
            }

            GuidInfo existingInfo = guidToObjectMap[guid];
            if ( existingInfo.gameObject != null && existingInfo.gameObject != guidComponent.gameObject )
            {
                // normally, a duplicate GUID is a big problem, means you won't necessarily be referencing what you expect
                if (Application.isPlaying)
                {
                    Debug.AssertFormat(false, guidComponent, "Guid Collision Detected between {0} and {1}.\nAssigning new Guid. Consider tracking runtime instances using a direct reference or other method.", (guidToObjectMap[guid].gameObject != null ? guidToObjectMap[guid].gameObject.name : "NULL"), (guidComponent != null ? guidComponent.name : "NULL"));
                }
                else
                {
                    // however, at editor time, copying an object with a GUID will duplicate the GUID resulting in a collision and repair.
                    // we warn about this just for pedantry reasons, and so you can detect if you are unexpectedly copying these components
                    Debug.LogWarningFormat(guidComponent, "Guid Collision Detected while creating {0}.\nAssigning new Guid.", (guidComponent != null ? guidComponent.name : "NULL"));
                }
                return false;
            }

            // if we already tried to find this GUID, but haven't set the game object to anything specific, copy any OnAdd callbacks then call them
            existingInfo.gameObject = info.gameObject;
            existingInfo.HandleAddCallback();
            guidToObjectMap[guid] = existingInfo;
            return true;
        }

        public bool Remove(Guid guid)
        {
            GuidInfo info;
            if (guidToObjectMap.TryGetValue(guid, out info))
            {
                // trigger all the destroy delegates that have registered
                info.HandleRemoveCallback();
            }

            return guidToObjectMap.Remove(guid);
        }

        // nice easy api to find a GUID, and if it works, register an on destroy callback
        // this should be used to register functions to cleanup any data you cache on finding
        // your target. Otherwise, you might keep components in memory by referencing them
        public GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            GuidInfo info;
            if (guidToObjectMap.TryGetValue(guid, out info))
            {
                if (onAddCallback != null)
                {
                    info.Added += onAddCallback;
                }

                if (onRemoveCallback != null)
                {
                    info.Removed += onRemoveCallback;
                }
                guidToObjectMap[guid] = info;
                return info.gameObject;
            }

            if (onAddCallback != null)
            {
                info.Added += onAddCallback;
            }

            if (onRemoveCallback != null)
            {
                info.Removed += onRemoveCallback;
            }

            guidToObjectMap.Add(guid, info);
        
            return null;
        }
    }
}
