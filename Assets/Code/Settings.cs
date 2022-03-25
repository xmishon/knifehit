using UnityEngine;

namespace knifehit
{
    [CreateAssetMenu(fileName = "New Settings", menuName = "Game Settings")]
    class Settings : ScriptableObject
    {
        public Vector3 logPosition = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 knifePosition = new Vector3(0.0f, -3.5f, 0.0f);
        public float appleBonusProbability = 0.25f;
        public int logKnifesMaxNumberAtStart = 3;
        public int knifeNumMin = 3;
        public int knifeNumMax = 6;
        public int knifeNumMinBoss = 5;
        public int knifeNumMaxBoss = 7;
        public float knifeSpawnDelay = 0.1f;
    }
}
