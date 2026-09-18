using UnityEngine;
using UnityEngine.InputSystem;
using YKW.Input;
using Input = YKW.Input.Input;

public sealed class ActionCombat : MonoBehaviour, Input.ICombatActions, Input.ICommonActions
{
    private GestionnaireCombat gestionnaireCombat;
    private InputManager inputManager;

    private void Awake()
    {
        gestionnaireCombat = GetComponent<GestionnaireCombat>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        inputManager.Input.Common.AddCallbacks(this);
        inputManager.Input.Combat.AddCallbacks(this);
        inputManager.Enable(inputManager.Input.Combat);
    }

    private void OnDestroy()
    {
        if (!InputManager.HasInstance)
            return;

        inputManager ??= InputManager.Instance;
        inputManager.Input.Common.RemoveCallbacks(this);
        inputManager.Input.Combat.RemoveCallbacks(this);
    }

    public void OnTournerLaRoueGauche(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.DemanderRotationRoue(-1);
    }

    public void OnTournerLaRoueDroite(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.DemanderRotationRoue(1);
    }

    public void OnChangédecibleGauche(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.ChangerOption(-1);
    }

    public void OnChnagédecibleDroite(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.ChangerOption(1);
    }

    public void OnAccélérer(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.BasculerVitesseAccélérée();
    }

    public void OnConfirmer(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.ConfirmerAction();
    }

    public void OnAnnuler(InputAction.CallbackContext context)
    {
        if (context.performed)
            gestionnaireCombat.AnnulerAction();
    }
}
