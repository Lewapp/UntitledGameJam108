using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInteractions : MonoBehaviour, IWeaponStatusable, IEnemyUseable
{
    public bool AttackStart { get; set; }

    public WeaponStats stats;
    public SwingInfo[] swingAnimations; // Array holding information for each swing animation.
    public SwingInfo blockAnimation;

    private List<Coroutine> waitingList = new List<Coroutine>();
    private int attackNo = -1; // Counter to track the current attack sequence.
    private bool isBlocking = false;
    private bool startUpIngore = true;


    public void PlayerBlock(InputAction.CallbackContext context)
    {
        Block();
    }

    private void Block()
    {
        if (waitingList.Count > 0) return;

        isBlocking = true;
    }

    /// <summary>
    /// Initiates an attack based on input context, starting the rotation animation if not already attacking.
    /// </summary>
    /// <param name="context">The input context from the attack input action.</param>
    public void PlayerAttack(InputAction.CallbackContext context)
    {
        Attack();
    }

    public void EnemyAttack()
    {
        Attack();
    }

    private void Attack()
    {
        if (startUpIngore)
        {
            startUpIngore = false;
            return;
        }

        if (isBlocking) return;

        for (int i = 0; i < waitingList.Count; i++)
        {
            if (waitingList[i] != null)
            {
                return;
            }
        }

        waitingList = new List<Coroutine>();

        // Return if no swing animations are defined.
        if (swingAnimations.Length <= 0) return;

        //  Claim that the attack has just started
        StartCoroutine(AttackStartedCheck());

        // Increment the attack number to select the next animation.
        attackNo++;
        // Loop back to the first swing animation if the end of the array is reached.
        attackNo = (int)Mathf.Repeat(attackNo, swingAnimations.Length);

        // Start the delay coroutinebefore doing the current attack sequence.
        waitingList.Add(StartCoroutine(DelayAttack(swingAnimations[attackNo], waitingList.Count)));
    }

    private IEnumerator AttackStartedCheck()
    {
        AttackStart = true;
        yield return null;
        AttackStart = false;
    }

    private IEnumerator DelayAttack(SwingInfo swingInfo, int listID)
    {
        float passedTime = 0f;
        float passedPercent = 0f;
        Vector3 startingScale = transform.localScale;

        while (passedTime < swingInfo.delay)
        {
            yield return null;
            passedTime += Time.deltaTime;
            passedPercent = passedTime / swingInfo.delay;

            transform.localScale = new Vector3(
                startingScale.x + startingScale.x * passedPercent * swingInfo.delayScale,
                startingScale.y + startingScale.y * passedPercent * swingInfo.delayScale,
                startingScale.z + startingScale.z * passedPercent * swingInfo.delayScale);
            
        }

        transform.localScale = startingScale;

        // Start the rotation animation coroutine for the current attack sequence.
        waitingList.Add(StartCoroutine(RotateLerp(swingAnimations[attackNo], waitingList.Count)));
        // Start the movement animation coroutine simultaneously.
        waitingList.Add(StartCoroutine(MoveLerp(swingAnimations[attackNo], waitingList.Count)));

        // Mark the delay as complete by resetting the coroutine reference.
        waitingList[listID] = null;
    }

    /// <summary>
    /// Smoothly interpolates the weapon's rotation over time based on the swing information provided.
    /// </summary>
    /// <param name="swingInfo">The data structure containing information about the swing, including duration, target location, and whether to take the long way.</param>
    /// <returns>Coroutine for smooth animation of the weapon's rotation.</returns>
    private IEnumerator RotateLerp(SwingInfo swingInfo, int listID)
    {
        // Record the current rotation of the weapon (Z-axis).
        float startRotation = transform.localRotation.eulerAngles.z;
        float passedTime = 0f;

        // Ensure the target rotation is within the valid range of 0 to 360 degrees.
        swingInfo.rotation = Mathf.Repeat(swingInfo.rotation, 360f);

        // Calculate the multiplier for the long way rotation (if needed).
        float zMultiplier = GetLongWayMultiplier(startRotation, swingInfo.rotation);

        float zRotation = 0f;

        // Continuously interpolate the rotation until the animation completes.
        while (true)
        {
            // Increment the elapsed time, adjusting by the animation duration.
            passedTime += Time.deltaTime / swingInfo.duration;

            // Depending on whether the long way is chosen, adjust the interpolation.
            if (swingInfo.longWay)
            {
                // Perform the rotation using the long path (negative multiplier).
                zRotation = Mathf.LerpUnclamped(startRotation, swingInfo.rotation, -(passedTime * zMultiplier));
            }
            else
            {
                // Standard interpolation for the short path.
                zRotation = Mathf.LerpUnclamped(startRotation, swingInfo.rotation, passedTime);
            }

            // If the calculated rotation is invalid (NaN), reset to 0 degrees to avoid errors.
            zRotation = float.IsNaN(zRotation) ? 0f : zRotation;

            // Apply the calculated rotation to the weapon on the Z-axis.
            transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

            // Yield until the next frame to continue the animation.
            yield return null;

            // Exit the loop when the animation has completed.
            if (passedTime >= 1f)
            {
                break;
            }
        }

        // After the animation finishes, set the final rotation directly to the target location.
        transform.localRotation = Quaternion.Euler(0f, 0f, swingInfo.rotation);

        // Mark the attack as complete by resetting the coroutine reference.
        waitingList[listID] = null;
    }

    /// <summary>
    /// Smoothly interpolates the weapon's position over time from the current position to the target position.
    /// </summary>
    /// <param name="swingInfo">The data structure containing information about the swing, including duration, target location, and whether to take the long way.</param>
    /// <returns>Coroutine for smooth animation of the weapon's movement.</returns>
    public IEnumerator MoveLerp(SwingInfo swingInfo, int listID)
    {
        // Record the current local position of the weapon.
        Vector3 startPosition = transform.localPosition;
        float passedTime = 0f;

        // Smoothly interpolate the position using Mathf.Lerp.
        while (true)
        {
            passedTime += Time.deltaTime / swingInfo.duration;

            // Interpolate the position between the start and target positions.
            transform.localPosition = Vector3.Lerp(startPosition, swingInfo.location, passedTime);

            // Yield to the next frame to continue the movement.
            yield return null;

            // Exit the loop once the movement is complete.
            if (passedTime >= 1f)
            {
                break;
            }
        }

        // Ensure the final position is exactly the target position.
        transform.localPosition = swingInfo.location;

        // Mark the movement as complete by resetting the coroutine reference.
        waitingList[listID] = null;
    }

    /// <summary>
    /// Calculates the multiplier for the "long way" rotation between two angles, used when a full 360-degree rotation is necessary.
    /// </summary>
    /// <param name="startZ">The current rotation on the Z-axis.</param>
    /// <param name="endZ">The target rotation on the Z-axis.</param>
    /// <returns>The multiplier value for the long way rotation.</returns>
    float GetLongWayMultiplier(float startZ, float endZ)
    {
        // Ensure both angles are within the 0 to 360 degree range.
        startZ = Mathf.Repeat(startZ, 360f);
        endZ = Mathf.Repeat(endZ, 360f);

        // Compute the difference between the start and end angles.
        float delta = Mathf.DeltaAngle(startZ, endZ);

        // Calculate the angle of the shorter path.
        float shortPathAngle = Mathf.Abs(delta);

        // Calculate the long path as the remainder of 360 degrees.
        float longPathAngle = 360f - shortPathAngle;

        // The multiplier is the ratio between the long path and the short path angles.
        float multiplier = longPathAngle / shortPathAngle;

        return multiplier;
    }

    /// <summary>
    /// Returns true if the weapon is currently performing an attack (either moving or rotating).
    /// </summary>
    public bool IsAttacking()
    {
        bool isAttacking = false;

        // Check whether either of the animation coroutines is active.
        for (int i = 0; i < waitingList.Count; i++)
        {
            if (waitingList[i] != null)
            {
                isAttacking = true;
                break;
            }
        }
        return isAttacking;
    }

    public WeaponStats GetWeaponStats()
    {
        return stats;
    }

    /// <summary>
    /// Contains information for each swing animation, including duration, final rotation, location, and whether to take the long way around.
    /// </summary>
    [Serializable]
    public class SwingInfo
    {
        public float duration;      // The duration for the swing animation.
        public Vector3 location;    // The target location for the swing.
        public float rotation;      // The target Z-axis rotation.
        public float delay;
        public float delayScale;
        public bool longWay;        // A flag indicating whether to take the long way for rotation (e.g., clockwise vs counter-clockwise).
        public bool continues;      // If true, the swing automatically continues to the next one in sequence.
    }
}