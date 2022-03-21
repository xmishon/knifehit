using UnityEngine;
using UnityEngine.UI;

namespace knifehit
{
    [CreateAssetMenu(fileName = "New Knife", menuName = "Game objects/Knife")]
    public class KnifeSO : ScriptableObject
    {
        public string knifeName = "Knife name";
        public Sprite knifeIcon;
        public Sprite knifeSprite;
        public float knifeSpeed = 1.0f;
    }
}
