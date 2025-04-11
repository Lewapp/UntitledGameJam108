using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInteractions : MonoBehaviour, IWeaponStatusable, IEnemyUseable
{
    #region Properties and References

    public CharacterStats.characterTypes ParentType { get; set; }
    public bool AttackStart { get; set; }
    public bool IsBlocking { get; set; }
    public bool CanBlock { get; set; }
    public float AttackSpeedMultiplier { get; set; }
    public float DelayMultiplier { get; set; }

    public WeaponStats stats;               // The stats of the weapon being used
    public SwingInfo[] swingAnimations;     // Swing animations to cycle through when attacking.
    public SwingInfo[] blockAnimations;     // Block animations for entering and exiting block state.

    private List<AnimationTask> waitingList;    // Tracks the status of started coroutines
    private int attackNo = -1;              // Tracks the current index in the attack animation sequence.
    private bool startUpIgnore = true;      // Ignores the first startup input (useful for input buffering quirks).

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (transform?.parent.GetComponent<IDamageable>() != null)
        {
            ParentType = transform.parent.GetComponent<IDamageable>().GetCharacterType();
        }
        waitingList = new List<AnimationTask>();
        CanBlock = true;
    }

    private void Update()
    {
        CheckWaitingListActive();

        // Automatically break block if no longer able to block while currently blocking.
        if (IsBlocking && !CanBlock)
        {
            BreakShield();
        }
    }

    #endregion

    #region Input Callbacks

    /// <summary>
    /// Input callback for triggering a attack action.
    /// </summary>
    /// <param name="context">The input context from the block input action.</param>
    public void PlayerAttack(InputAction.CallbackContext context)
    {
        Attack();
    }

    /// <summary>
    /// Input callback for triggering a block action.
    /// </summary>
    /// <param name="context">The input context from the block input action.</param>
    public void PlayerBlock(InputAction.CallbackContext context)
    {
        Block();
    }

    /// <summary>
    /// Triggers an attack from an enemy using this weapon. Mirrors the player attack behavior.
    /// </summary>
    public void EnemyAttack()
    {
        Attack();
    }

    #endregion

    #region Blocking Logic

    /// <summary>
    /// Toggles block state with animations, unless already performing an action or block is disabled.
    /// </summary>
    private void Block()
    {
        if (waitingList.Count > 0 || !CanBlock)
            return;

        if (IsBlocking)
        {
            IsBlocking = false;
            AnimationTask task = new AnimationTask();
            task.activeCoroutine = StartCoroutine(DelayAttack(blockAnimations[1], waitingList.Count));
            task.attackAnimation = false;
            waitingList.Add(task);
        }
        else
        {
            IsBlocking = true;
            AnimationTask task = new AnimationTask();
            task.activeCoroutine = StartCoroutine(DelayAttack(blockAnimations[0], waitingList.Count));
            task.attackAnimation = false;
            waitingList.Add(task);
        }
    }

    /// <summary>
    /// Forces the player to exit the blocking state and plays the unblock animation.
    /// </summary>
    public void BreakShield()
    {
        IsBlocking = false;
        AnimationTask task = new AnimationTask();
        task.activeCoroutine = StartCoroutine(DelayAttack(blockAnimations[1], waitingList.Count));
        task.attackAnimation = false;
        waitingList.Add(task);
    }

    #endregion

    #region Attacking Logic

    /// <summary>
    /// Handles the logic for initiating an attack, including animation sequence rotation.
    /// Skips the first input to ignore accidental early inputs after startup.
    /// </summary>
    private void Attack()
    {
        if (startUpIgnore)
        {
            startUpIgnore = false;
            return;
        }

        if (CheckWaitingListActive()) return;

        IsBlocking = false;

        // Return if no swing animations are defined.
        if (swingAnimations.Length <= 0) return;

        // Increment the attack number to select the next animation.
        attackNo++;
        // Loop back to the first swing animation if the end of the array is reached.
        attackNo = (int)Mathf.Repeat(attackNo, swingAnimations.Length);

        //  Claim that the attack has just started
        StartCoroutine(AttackStartedCheck());

        // Start the delay coroutine before doing the current attack sequence.
        AnimationTask task = new AnimationTask();
        task.activeCoroutine = StartCoroutine(DelayAttack(swingAnimations[attackNo], waitingList.Count));
        task.attackAnimation = false;
        waitingList.Add(task);
    }

    /// <summary>
    /// Temporarily sets AttackStart to true for one frame.
    /// </summary>
    private IEnumerator AttackStartedCheck()
    {
        AttackStart = true;
        yield return null;
        AttackStart = false;
    }

    /// <summary>
    /// Returns true if the weapon is in the middle of a movement or rotation animation.
    /// </summary>
    /// <returns>True if any attack animation is active, false otherwise.</returns>
    public bool IsAttacking()
    {
        // If blocking, attacking is not allowed.
        if (IsBlocking) return false;

        // Check if any coroutine is still running.
        foreach (AnimationTask task in waitingList)
        {
            if (task?.activeCoroutine != null && task.attackAnimation)
            {
                return true;
            }
        }

        // No active animations found.
        return false;
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Delays the attack to create anticipation before the actual swing (Purely used for enemies but can be used for player),
    /// applying a scale animation during the delay, then starts movement and rotation coroutines.
    /// </summary>
    /// <param name="swingInfo">The configuration for the swing, including delay duration and scale factor.</param>
    /// <param name="listID">The index in the waiting list this coroutine occupies, used to mark completion.</param>
    private IEnumerator DelayAttack(SwingInfo swingInfo, int listID)
    {
        // Track the elapsed time during the delay.
        float passedTime = 0f;
        // Store the initial scale of the object to revert after the delay.
        Vector3 startingScale = transform.localScale;

        float delay = swingInfo.delay - (swingInfo.delay * DelayMultiplier);

        // Perform the delay, increasing the scale slightly over time to add visual anticipation.
        while (passedTime < delay)
        {
            yield return null; // Wait for the next frame.

            passedTime += Time.deltaTime;
            float passedPercent = passedTime / swingInfo.delay;

            // Scale the object proportionally based on how much time has passed.
            transform.localScale = new Vector3(
                startingScale.x + startingScale.x * passedPercent * swingInfo.delayScale,
                startingScale.y + startingScale.y * passedPercent * swingInfo.delayScale,
                startingScale.z + startingScale.z * passedPercent * swingInfo.delayScale
            );
        }

        // Reset the scale to its original size after the delay.
        transform.localScale = startingScale;

        // After the delay ends, start the actual swing motion:
        // Add rotation animation coroutine.
        AnimationTask rotateTask = new AnimationTask();
        rotateTask.activeCoroutine = StartCoroutine(RotateLerp(swingInfo, waitingList.Count));
        rotateTask.attackAnimation = true;
        waitingList.Add(rotateTask);

        // Add movement animation coroutine.
        AnimationTask moveTask = new AnimationTask();
        moveTask.activeCoroutine = StartCoroutine(MoveLerp(swingInfo, waitingList.Count));
        moveTask.attackAnimation = true;
        waitingList.Add(moveTask);

        // Mark this delay coroutine as completed in the waiting list.
        waitingList[listID] = null;
    }

    /// <summary>
    /// Rotates the weapon over time based on the swing info.
    /// </summary>    
    /// <param name="swingInfo">The configuration for the swing, including delay duration and scale factor.</param>
    /// <param name="listID">The index in the waiting list this coroutine occupies, used to mark completion.</param>
    private IEnumerator RotateLerp(SwingInfo swingInfo, int listID)
    {
        // Get the starting rotation on the Z-axis.
        float startRotation = transform.localRotation.eulerAngles.z;
        float passedTime = 0f;

        // Normalise the target rotation to within 0–360 degrees.
        swingInfo.rotation = Mathf.Repeat(swingInfo.rotation, 360f);

        // Get a multiplier for long-way rotation if applicable.
        float zMultiplier = GetLongWayMultiplier(startRotation, swingInfo.rotation);
        float zRotation = 0f;
        float timePos = 0f;

        while (true)
        {
            timePos = Time.deltaTime / swingInfo.duration;

            // Increase the interpolation time factor based on duration.
            passedTime += timePos + (timePos * AttackSpeedMultiplier);

            // Choose long-way or short-way interpolation.
            zRotation = swingInfo.longWay
                ? Mathf.LerpUnclamped(startRotation, swingInfo.rotation, -(passedTime * zMultiplier))
                : Mathf.LerpUnclamped(startRotation, swingInfo.rotation, passedTime);

            // Normalise the rotation and prevent NaN.
            zRotation = Mathf.Repeat(zRotation, 360f);
            zRotation = float.IsNaN(zRotation) ? 0f : zRotation;

            // Apply the interpolated rotation to the weapon.
            transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

            yield return null;

            // Break once the full interpolation time has passed.
            if (passedTime >= 1f)
                break;
        }

        // Snap to final rotation to ensure exact value.
        transform.localRotation = Quaternion.Euler(0f, 0f, swingInfo.rotation);

        // Mark coroutine as complete.
        waitingList[listID] = null;
    }

    /// <summary>
    /// Moves the weapon over time to the specified local position.
    /// </summary>
    /// <param name="swingInfo">The configuration for the swing, including delay duration and scale factor.</param>
    /// <param name="listID">The index in the waiting list this coroutine occupies, used to mark completion.</param>
    public IEnumerator MoveLerp(SwingInfo swingInfo, int listID)
    {
        // Store the starting local position of the weapon.
        Vector3 startPosition = transform.localPosition;
        float passedTime = 0f;
        float timePos = 0f;

        while (true)
        {
            timePos = Time.deltaTime / swingInfo.duration;

            // Incrementally progress based on duration.
            passedTime += timePos + (timePos * AttackSpeedMultiplier);

            // Linearly interpolate from the start to the target position.
            transform.localPosition = Vector3.Lerp(startPosition, swingInfo.location, passedTime);

            yield return null;

            // Exit once interpolation is complete.
            if (passedTime >= 1f)
                break;
        }

        // Snap to final position to avoid floating-point inaccuracies.
        transform.localPosition = swingInfo.location;

        // Mark coroutine as complete.
        waitingList[listID] = null;
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Returns true if any actions are currently in progress. Resets the list if all coroutines are complete.
    /// </summary>
    private bool CheckWaitingListActive()
    {
        for (int i = 0; i < waitingList.Count; i++)
        {
            if (waitingList[i]?.activeCoroutine != null)
                return true;
        }

        waitingList = new List<AnimationTask>();
        return false;
    }

    /// <summary>
    /// Calculates a multiplier for long-way rotation interpolation.
    /// </summary>
    /// <param name="startZ">The starting rotation angle on the Z-axis.</param>
    /// <param name="endZ">The target rotation angle on the Z-axis.</param>
    /// <returns>A multiplier used to force long way on lerping</returns>
    private float GetLongWayMultiplier(float startZ, float endZ)
    {
        // Normalise both angles to within 0–360 degrees.
        startZ = Mathf.Repeat(startZ, 360f);
        endZ = Mathf.Repeat(endZ, 360f);

        // Calculate the angular difference using Unity’s helper.
        float delta = Mathf.DeltaAngle(startZ, endZ);

        // Determine short and long path distances.
        float shortPathAngle = Mathf.Abs(delta);
        float longPathAngle = 360f - shortPathAngle;

        // Return a multiplier used to scale the long-path interpolation.
        return longPathAngle / shortPathAngle;
    }

    public WeaponStats GetWeaponStats()
    {
        return stats;
    }

    public bool IsInAnimation()
    {
        if (waitingList.Count > 0)
        {
            return true; 
        }

        return false;
    }

    #endregion

    #region Subclasses

    /// <summary>
    /// Defines timing and animation parameters for attacks or blocks.
    /// </summary>
    [Serializable]
    public class SwingInfo
    {
        public float duration;       // Time it takes to perform the animation.
        public Vector3 location;     // Target local position of the weapon.
        public float rotation;       // Target local Z rotation.
        public float delay;          // Time to wait before the animation starts.
        public float delayScale;     // How much the weapon scales during delay.
        public bool longWay;         // Use long rotation path (e.g., clockwise 270° instead of counter-clockwise 90°).
        public bool continues;       // If true, auto-continue to the next attack in sequence.
    }

    public class AnimationTask
    {
        public Coroutine activeCoroutine;
        public bool attackAnimation;
    }

    #endregion
}