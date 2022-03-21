using UnityEngine;

namespace knifehit
{
    [CreateAssetMenu(fileName = "New rotation curve", menuName = "Game objects/Rotation curve")]
    public class RotationCurve : ScriptableObject
    {
        public string curveName = "Rotation curve name";
        public AnimationCurve curve;
    }
}
