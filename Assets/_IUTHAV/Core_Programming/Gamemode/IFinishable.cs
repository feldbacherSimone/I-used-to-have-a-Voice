using Object = System.Object;

namespace _IUTHAV.Core_Programming.Gamemode {
    
    public interface IFinishable {

        public abstract Object GetData();
        public abstract void UpdateData(Object obj);
        public abstract bool CheckFinishCondition();
        public abstract IFinishable Reset();

    }
}
