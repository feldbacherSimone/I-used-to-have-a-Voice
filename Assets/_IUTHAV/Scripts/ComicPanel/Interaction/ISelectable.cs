namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    
    public interface ISelectable
    {
        public void OnSelect(SelectionContext context);
        
        public void OnDeselect();

    }
    
}