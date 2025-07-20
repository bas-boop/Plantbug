using System.Collections.Generic;
using UnityEngine;

namespace Framework.Animation
{
    public class SpriteChanger : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<Sprite> sprites;

        public void ChangeSprite(int sprite)
        {
            spriteRenderer.sprite = sprites[sprite];
        }
    }
}