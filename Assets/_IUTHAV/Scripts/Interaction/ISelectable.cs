using UnityEngine;
using UnityEngine.Rendering;

namespace _IUTHAV.Scripts.Interaction
{
    public struct SelectionContext {
    
        public Panel.Panel Panel;

        public SelectionContext(Panel.Panel panel) {
            Panel = panel;
        }
    }
    public interface ISelectable
    {
        public void OnSelect(SelectionContext context);
       

        public void OnDeselect();

    }
}