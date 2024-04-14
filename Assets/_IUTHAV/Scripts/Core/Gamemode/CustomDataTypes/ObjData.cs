
using System;

namespace _IUTHAV.Scripts.Core.Gamemode.CustomDataTypes {
    public class ObjData : IFinishable {

        private Object _data;
        private Object _targetObj;
        private Object _backupData;

        public ObjData(Object data, Object targetData) {
            _data = data;
            _backupData = data;
            _targetObj = targetData;
        }
 
        public object GetData() {
            return _data;
        }

        public void UpdateData(Object obj) {

            _data = obj;
        }

        public bool CheckFinishCondition() {
        
            if (_targetObj == null) return false;
            
            return _data.Equals(_targetObj);
        }

        public IFinishable Reset() {
        
            return new ObjData(_backupData, _targetObj);
        }
        
        public override string ToString() {
            return _data.ToString();
        }
    }
}
