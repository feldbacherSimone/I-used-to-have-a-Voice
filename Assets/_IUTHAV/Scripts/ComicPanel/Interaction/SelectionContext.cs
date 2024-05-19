using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel.Interaction {
    public class SelectionContext {
        
        public readonly ComicPanel.Panel panel;

        public SelectionContext(ComicPanel.Panel panel) {
            this.panel = panel;
        }
        
        public bool IsSelectableContext(SelectionContext requiredContext) {

            return panel == requiredContext.panel;
        }

        public bool IsValidPanelExists(Panel[] validPanels) {
            
            //return true if no explicit panel was given
            if (validPanels == null || validPanels.Length == 0) return true;
            
            foreach (var _panel in validPanels) {

                if (this.panel == _panel) {
                    return true;
                }
            }

            return false;
        }
        
    }
}