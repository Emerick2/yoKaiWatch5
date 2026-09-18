using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.Editor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class PressAndRepeatInteraction : IInputInteraction
{
    // Parameters
    public float holdTime = 0.4f;
    public float repeatTime = 0.2f;
    public bool press = false;

    // Acceleration
    public bool accelerate = false;

    // Amount removed from repeat time after every repeat.
    public float acceleration = 0.01f;

    // Fastest possible repeat interval.
    public float minimumRepeatTime = 0.03f;

    // Private attributes
    private InputInteractionContext ctx;

    private float heldTime;
    private bool firstEventSend;
    private float nextEventTime;

    private float currentRepeatTime;

    private bool doingCancel;

    static PressAndRepeatInteraction()
    {
        InputSystem.RegisterInteraction<PressAndRepeatInteraction>();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
    }

    public void Process(ref InputInteractionContext context)
    {
        ctx = context;

        if (ctx.phase != InputActionPhase.Performed &&
            ctx.phase != InputActionPhase.Started &&
            ctx.ControlIsActuated(0.5f))
        {
            ctx.Started();

            // Initialize repeat speed.
            currentRepeatTime = repeatTime;

            if (press)
                ctx.PerformedAndStayStarted();

            InputSystem.onAfterUpdate -= OnUpdate;
            InputSystem.onAfterUpdate += OnUpdate;
        }
    }

    private void OnUpdate()
    {
        bool isActuated = ctx.ControlIsActuated(0.5f);
        InputActionPhase phase = ctx.phase;

        heldTime += Time.unscaledDeltaTime;

        // Action canceled, disabled, map disabled, or button released.
        if (phase == InputActionPhase.Canceled ||
            phase == InputActionPhase.Disabled ||
            !ctx.action.actionMap.enabled ||
            (!isActuated &&
             (phase == InputActionPhase.Performed ||
              phase == InputActionPhase.Started)))
        {
            Cancel(ref ctx);
            return;
        }

        // Wait until the input becomes a hold.
        if (heldTime < holdTime)
            return;

        // First hold event.
        if (!firstEventSend)
        {
            ctx.PerformedAndStayStarted();

            currentRepeatTime = Mathf.Max(
                minimumRepeatTime,
                repeatTime
            );

            nextEventTime = heldTime + currentRepeatTime;

            firstEventSend = true;
            return;
        }

        if (heldTime < nextEventTime)
            return;

        ctx.PerformedAndStayStarted();

        // Gradually accelerate.
        if (accelerate)
        {
            currentRepeatTime = Mathf.Max(
                minimumRepeatTime,
                currentRepeatTime - acceleration
            );
        }
        else
        {
            currentRepeatTime = repeatTime;
        }

        nextEventTime = heldTime + currentRepeatTime;
    }

    public void Reset()
    {
        Cancel(ref ctx);
    }

    private void Cancel(ref InputInteractionContext context)
    {
        // context.Canceled() may indirectly call Reset().
        if (doingCancel)
            return;

        InputSystem.onAfterUpdate -= OnUpdate;

        heldTime = 0f;
        firstEventSend = false;
        nextEventTime = 0f;
        currentRepeatTime = repeatTime;

        if (context.phase != InputActionPhase.Canceled)
        {
            doingCancel = true;
            context.Canceled();
            doingCancel = false;
        }
    }
}

#if UNITY_EDITOR

internal class PressAndRepeatInteractionEditor
    : InputParameterEditor<PressAndRepeatInteraction>
{
    private static GUIContent holdTimeLabel;
    private static GUIContent repeatTimeLabel;
    private static GUIContent pressLabel;

    private static GUIContent accelerateLabel;
    private static GUIContent accelerationLabel;
    private static GUIContent minimumRepeatTimeLabel;

    protected override void OnEnable()
    {
        holdTimeLabel = new GUIContent(
            "Hold Time",
            "Minimum amount of realtime seconds before the input is considered held."
        );

        repeatTimeLabel = new GUIContent(
            "Repeat Time",
            "Initial time between each repeated Performed event."
        );

        pressLabel = new GUIContent(
            "Press",
            "Immediately sends a Performed event when the input is pressed."
        );

        accelerateLabel = new GUIContent(
            "Accelerate",
            "Makes the repeat speed progressively faster while the input remains held."
        );

        accelerationLabel = new GUIContent(
            "Acceleration",
            "Amount of time removed from the repeat interval after every repeated event."
        );

        minimumRepeatTimeLabel = new GUIContent(
            "Minimum Repeat Time",
            "Minimum interval between repeated events when acceleration is enabled."
        );
    }

    public override void OnGUI()
    {
        target.press = EditorGUILayout.Toggle(
            pressLabel,
            target.press
        );

        target.holdTime = Mathf.Max(
            0f,
            EditorGUILayout.FloatField(
                holdTimeLabel,
                target.holdTime
            )
        );

        target.repeatTime = Mathf.Max(
            0.001f,
            EditorGUILayout.FloatField(
                repeatTimeLabel,
                target.repeatTime
            )
        );

        EditorGUILayout.Space();

        target.accelerate = EditorGUILayout.Toggle(
            accelerateLabel,
            target.accelerate
        );

        if (target.accelerate)
        {
            EditorGUI.indentLevel++;

            target.acceleration = Mathf.Max(
                0f,
                EditorGUILayout.FloatField(
                    accelerationLabel,
                    target.acceleration
                )
            );

            target.minimumRepeatTime = Mathf.Clamp(
                EditorGUILayout.FloatField(
                    minimumRepeatTimeLabel,
                    target.minimumRepeatTime
                ),
                0.001f,
                target.repeatTime
            );

            EditorGUI.indentLevel--;
        }
    }
}

#endif