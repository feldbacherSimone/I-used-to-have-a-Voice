using Object = System.Object;

namespace _IUTHAV.Scripts.Core.Gamemode {
    
    public interface IFinishable {

        public Object GetData();
        public void UpdateData(Object obj);
        public bool CheckFinishCondition();
        public IFinishable Reset();

    }
}
