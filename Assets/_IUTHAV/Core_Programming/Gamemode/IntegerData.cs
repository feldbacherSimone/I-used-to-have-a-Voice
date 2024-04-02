using System;
using UnityEngine;
using Object = System.Object;

namespace _IUTHAV.Core_Programming.Gamemode {
    
    [Serializable]
    public class IntegerData : IFinishable {

        public int integerData;
        [SerializeField] private int targetData;

        public object GetData() {
            return integerData;
        }

        public void UpdateData(Object obj) {
            throw new NotImplementedException();
        }

        public bool CheckFinishCondition() {
            throw new NotImplementedException();
        }

        public IFinishable Reset() {
            throw new NotImplementedException();
        }
    }
}
