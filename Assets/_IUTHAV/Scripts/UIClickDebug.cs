using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class constra : MonoBehaviour
{
    // Start is called before the first frame update
    private EventSystem _eventSystem;
    [SerializeField] private InputActionAsset _actions;
    void Start() {
        _eventSystem = gameObject.GetComponent<EventSystem>();

        var _action = _actions.FindAction("UI/Click");
        _action.performed += OnClick;
    }

    private void OnClick(InputAction.CallbackContext context) {
        
        Debug.Log(context.action.ToString());
        
    }
}
