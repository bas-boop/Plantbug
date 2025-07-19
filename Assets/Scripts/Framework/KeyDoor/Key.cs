using UnityEngine;

namespace Framework.KeyDoor
{
    public sealed class Key : MonoBehaviour
    {
        public bool IsCollected { get; private set; }

        public void Collect()
        {
            IsCollected = true;
            gameObject.SetActive(false);
        }
    }
}