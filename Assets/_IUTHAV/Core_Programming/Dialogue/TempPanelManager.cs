using UnityEngine;

namespace _IUTHAV.Core_Programming.Dialogue {
    public class TempPanelManager : MonoBehaviour {

        public TempPanelParameters[] panels;

        private int _mIndex;

        public void NextPanel() {
            if (_mIndex < panels.Length - 1) {
                _mIndex++;
            }
        }

        public TempPanelParameters GetCurrentPanel() {
            return panels[_mIndex];
        }
        
    }
}
