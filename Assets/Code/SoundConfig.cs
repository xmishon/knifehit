using UnityEngine;

namespace knifehit
{
    [CreateAssetMenu(fileName = "New sound config", menuName = "Sound config")]
    class SoundConfig : ScriptableObject
    {
        public AudioClip buttonPressed;
        public AudioClip knifeThrown;
        public AudioClip gameOver;
        public AudioClip backgroundSong;

    }
}
