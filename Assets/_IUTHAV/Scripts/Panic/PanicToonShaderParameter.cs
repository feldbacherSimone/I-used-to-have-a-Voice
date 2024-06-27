using UnityEngine;

namespace _IUTHAV.Scripts.Panic {
    public class PanicToonShaderParameter : PanicParameter<Material> {

        [SerializeField] private Material toonMaterial;


        private const string RimColorName = "_RimColor";
        private const string AmbientColorName = "_AmbientColor";
        public void Awake() {
            Configure();
        }

        public void Configure() {
            
            _baseParameterValue = new Material(toonMaterial.shader);
            _currentParameterValue = new Material(toonMaterial.shader);
            _targetParameterValue = new Material(toonMaterial.shader);
            
            _currentParameterValue.color = minPanicParameter.color;
            _currentParameterValue.SetColor(RimColorName, minPanicParameter.GetColor(RimColorName));
            _currentParameterValue.SetColor(AmbientColorName, minPanicParameter.GetColor(AmbientColorName));
            SetDesiredParameter();
            
            _baseParameterValue.color = toonMaterial.color;
            _baseParameterValue.SetColor(RimColorName, toonMaterial.GetColor(RimColorName));
            _baseParameterValue.SetColor(AmbientColorName, toonMaterial.GetColor(AmbientColorName));
            
            _targetParameterValue.color = toonMaterial.color;
            _targetParameterValue.SetColor(RimColorName, toonMaterial.GetColor(RimColorName));
            _targetParameterValue.SetColor(AmbientColorName, toonMaterial.GetColor(AmbientColorName));
            
        }

        public override void SetBaseParameter() {
            
            _baseParameterValue.color = toonMaterial.color;
            _baseParameterValue.SetColor(RimColorName, toonMaterial.GetColor(RimColorName));
            _baseParameterValue.SetColor(AmbientColorName, toonMaterial.GetColor(AmbientColorName));
            
        }
        
        public override void SetTargetParameter(float targetValue) {
            _targetParameterValue.color = Vector4.Lerp(minPanicParameter.color, maxPanicParameter.color, targetValue);
            _targetParameterValue.SetColor(RimColorName, Vector4.Lerp(minPanicParameter.GetColor(RimColorName), maxPanicParameter.GetColor(RimColorName), targetValue));
            _targetParameterValue.SetColor(AmbientColorName, Vector4.Lerp(minPanicParameter.GetColor(AmbientColorName), maxPanicParameter.GetColor(AmbientColorName),
                targetValue));
                
        }

        public override void SetDesiredParameter() {

            toonMaterial.color = _currentParameterValue.color;
            toonMaterial.SetColor(RimColorName, _currentParameterValue.GetColor(RimColorName));
            toonMaterial.SetColor(AmbientColorName, _currentParameterValue.GetColor(AmbientColorName));
            
        }

        public override void LerpByPanicValue(float targetValue) {
           _currentParameterValue.color = Vector4.Lerp(_baseParameterValue.color, _targetParameterValue.color, targetValue);
           _currentParameterValue.SetColor(RimColorName, Vector4.Lerp(_baseParameterValue.GetColor(RimColorName), _targetParameterValue.GetColor(RimColorName), targetValue));
           _currentParameterValue.SetColor(AmbientColorName, Vector4.Lerp(_baseParameterValue.GetColor(AmbientColorName), _targetParameterValue.GetColor(AmbientColorName),
                targetValue));
            
        }
        
    }
}