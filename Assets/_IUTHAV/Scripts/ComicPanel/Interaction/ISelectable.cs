namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    public struct SelectionContext {
    
        public ComicPanel.Panel Panel;

        public SelectionContext(ComicPanel.Panel panel) {
            Panel = panel;
        }
    }
    public interface ISelectable
    {
        public void OnSelect(SelectionContext context);
       

        public void OnDeselect();

    }
}