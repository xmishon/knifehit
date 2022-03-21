using UnityEngine;

namespace knifehit
{
    public class Timer : IExecute
    {
        public static float levelTime = 0.0f;

        public void Execute()
        {
            levelTime += Time.deltaTime;
        }
    }
}
