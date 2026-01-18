using UnityEngine;

namespace HierarchyColoring
{
    [DisallowMultipleComponent]
    public class HierarchyLabel : MonoBehaviour
    {
        public Color labelColor = new Color(0, 0, 0, 0);
        public bool underlineRed = false;
        public bool applyToChildren = false;
    }
}