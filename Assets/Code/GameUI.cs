using UnityEngine;

namespace knifehit
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _knifePanel;

        public void SetupKnifeCount(int count)
        {
            for (int i = 0; i < _knifePanel.transform.childCount; i++)
            {
                Destroy(_knifePanel.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < count; i++)
            {
                GameObject knifeIcon = Instantiate(Resources.Load<GameObject>("KnifeIcon"));
                knifeIcon.transform.SetParent(_knifePanel.transform);
            }
        }

    }
}
