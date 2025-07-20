using UnityEngine;

namespace Framework
{
    public sealed class PlayerUnstucker : MonoBehaviour
    {
        [SerializeField] private Transform unstuckPosition;

        public void Unstuck() => transform.position = unstuckPosition.position;
    }
}