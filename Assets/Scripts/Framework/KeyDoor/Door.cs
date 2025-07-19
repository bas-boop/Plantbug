using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.KeyDoor
{
    public sealed class Door : MonoBehaviour
    {
        [SerializeField] private List<Key> keys;
        [SerializeField] private UnityEvent onEnterDoor = new();

        public void Open()
        {
            if (keys.Any(key => !key.IsCollected))
                return;
            
            onEnterDoor?.Invoke();
        }
    }
}