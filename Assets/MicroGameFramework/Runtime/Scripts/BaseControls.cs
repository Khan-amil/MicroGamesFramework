// GENERATED AUTOMATICALLY FROM 'Packages/com.khanamil.microgamesframework/Runtime/Scripts/BaseControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @BaseControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @BaseControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BaseControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""f01bdff7-4353-468a-ab81-266667e42934"",
            ""actions"": [
                {
                    ""name"": ""MainAction"",
                    ""type"": ""Button"",
                    ""id"": ""59ef9d8d-69d4-4bd4-a736-9647c7fa7d9b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""340de7f1-9354-4b63-85d9-29e79b5905ed"",
                    ""path"": ""<Keyboard>/#(A)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_MainAction = m_Gameplay.FindAction("MainAction", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_MainAction;
    public struct GameplayActions
    {
        private @BaseControls m_Wrapper;
        public GameplayActions(@BaseControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MainAction => m_Wrapper.m_Gameplay_MainAction;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @MainAction.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMainAction;
                @MainAction.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMainAction;
                @MainAction.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMainAction;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MainAction.started += instance.OnMainAction;
                @MainAction.performed += instance.OnMainAction;
                @MainAction.canceled += instance.OnMainAction;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMainAction(InputAction.CallbackContext context);
    }
}
