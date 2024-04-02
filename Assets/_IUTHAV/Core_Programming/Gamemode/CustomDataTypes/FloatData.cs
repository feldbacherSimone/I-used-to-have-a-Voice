using System;
using UnityEngine;
using Object = System.Object;

namespace _IUTHAV.Core_Programming.Gamemode.CustomDataTypes {
    [Serializable]
    public class FloatData : IFinishable {

        [SerializeField] private float integerData;
        [SerializeField] private float targetData;
        private float _backupData;

        public FloatData(float integerData, float targetData = 0) {
            this.integerData = integerData;
            _backupData = integerData;
            this.targetData = targetData;
        }
 
        public object GetData() {
            return integerData;
        }

        public void UpdateData(Object obj) {

            integerData = (float)obj;
        }

        public bool CheckFinishCondition() {
        
            return integerData > targetData;
        }

        public IFinishable Reset() {
        
            return new FloatData(_backupData, targetData);
        }

        public override string ToString() {
            return "IntegerData: " + integerData + " | TargetData: " + targetData;
        }
    }
}
