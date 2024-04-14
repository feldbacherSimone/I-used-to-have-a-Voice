using System;
using Object = System.Object;

namespace _IUTHAV.Scripts.Core.Gamemode.CustomDataTypes {
    public class FloatData : IFinishable {
        private float _floatData;
        private float _targetData;
        private float _backupData;
        private CompareType _compareType;

        public FloatData(float floatData, float targetData = 0, CompareType compareType = CompareType.BiggerThan) {
            this._floatData = floatData;
            _backupData = floatData;
            this._targetData = targetData;
            _compareType = compareType;
        }

        public object GetData() {
            return _floatData;
        }

        public void UpdateData(Object obj) {
            _floatData = (float)obj;
        }

        public bool CheckFinishCondition() {
            switch (_compareType) {
                case CompareType.LowerThan:
                    return _floatData < _targetData;

                case CompareType.Equal:
                    return Math.Abs(_floatData - _targetData) < 0.1f;


                case CompareType.BiggerThan:
                    return _floatData > _targetData;

                default:
                    return false;
            }
        }

        public IFinishable Reset() {
            return new FloatData(_backupData, _targetData);
        }

        public override string ToString() {
            return "IntegerData: " + _floatData + " | TargetData: " + _targetData;
        }
    }
}