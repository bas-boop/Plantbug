using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    public sealed class Invoker : MonoBehaviour
    {
        [SerializeField] private float delay = 1;
        [SerializeField] private UnityEvent onInvoke = new();

        public void Start() => Invoke(nameof(CallEvent), delay);

        private void CallEvent() => onInvoke?.Invoke();
    }
}