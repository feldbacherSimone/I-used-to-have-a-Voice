namespace _IUTHAV.Scripts.Panel.Interaction
{
    public struct SelectionContext {
    
        public Scripts.Panel.Panel Panel;

        public SelectionContext(Scripts.Panel.Panel panel) {
            Panel = panel;
        }
    }
    public interface ISelectable
    {
        public void OnSelect(SelectionContext context);
       

        public void OnDeselect();

    }
}