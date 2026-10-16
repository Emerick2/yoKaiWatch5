using UnityEngine;
using UnityEngine.InputSystem;
using YKW.Core.Events;
using YKW.Input;
using Input = YKW.Input.Input;

public sealed class ActionCombat : MonoBehaviour, Input.ICombatActions, Input.ICommonActions
{
    private InputManager inputManager;

    private void OnEnable()
    {
        inputManager = InputManager.Instance;
        inputManager.Input.Common.AddCallbacks(this);
        inputManager.Input.Combat.AddCallbacks(this);
        inputManager.Enable(inputManager.Input.Combat);
    }

    private void OnDisable()
    {
        if (inputManager == null || !InputManager.HasInstance) return;
        inputManager.Input.Common.RemoveCallbacks(this);
        inputManager.Input.Combat.RemoveCallbacks(this);
    }

    public void OnTournerLaRoueGauche(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatRotateRequestedEvent(-1));
    }

    public void OnTournerLaRoueDroite(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatRotateRequestedEvent(1));
    }

    public void OnChangédecibleGauche(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatOptionChangeRequestedEvent(-1));
    }

    public void OnChnagédecibleDroite(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatOptionChangeRequestedEvent(1));
    }

    public void OnAccélérer(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatSpeedToggleRequestedEvent());
    }

    public void OnConfirmer(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatConfirmRequestedEvent());
    }

    public void OnAnnuler(InputAction.CallbackContext context)
    {
        if (context.performed)
            EventBus.Publish(new CombatCancelRequestedEvent());
    }
}
