using UnityEngine;
using UnityEngine.UI;

namespace knifehit
{
    [CreateAssetMenu(fileName = "New Log", menuName = "Game objects/Log")]
    public class LogSO : ScriptableObject
    {
        public string logName = "Log name";
        public Sprite logIcon;
        public Sprite logSprite;
        public Sprite[] destroyedSprites;
    }
}
