using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInteractions : MonoBehaviour, IWeaponStatusable
{
    // Array holding information for each swing animation.
    public SwingInfo[] swingAnimations;

    // Coroutine reference to track the state of the attack animation.
    public Coroutine isRotating;
    // Coroutine reference for handling movement animation.
    public Coroutine isMoving;

    // Counter to track the current attack sequence.
    public int attackNo = -1;

    /// <summary>
    /// Initiates an attack based on input context, starting the rotation animation if not already attacking.
    /// </summary>
    /// <param name="context">The input context from the attack input action.</param>
    public void Attack(InputAction.CallbackContext context)
    {
        // Proceed only if an attack is not already in progress.
        if (isRotating == null && isMoving == null)
        {
            // Return if no swing animations are defined.
            if (swingAnimations.Length <= 0) return;

            // Increment the attack number to select the next animation.
            attackNo++;
            // Loop back to the first swing animation if the end of the array is reached.
            attackNo = (int)Mathf.Repeat(attackNo, swingAnimations.Length);

            // Start the rotation animation coroutine for the current attack sequence.
            isRotating = StartCoroutine(RotateLerp(swingAnimations[attackNo]));
            isMoving = StartCoroutine(MoveLerp(swingAnimations[attackNo]));
        }
    }

    /// <summary>
    /// Smoothly interpolates the weapon's rotation over time based on the swing information provided.
    /// </summary>
    /// <param name="swingInfo">The data structure containing information about the swing, including duration, target location, and whether to take the long way.</param>
    /// <returns>Coroutine for smooth animation of the weapon's rotation.</returns>
    private IEnumerator RotateLerp(SwingInfo swingInfo)
    {
        // Record the current rotation of the weapon (Z-axis).
        float startRotation = transform.localRotation.eulerAngles.z;
        float passedTime = 0f;

        // Ensure the target rotation is within the valid range of 0 to 360 degrees.
        swingInfo.rotation = Mathf.Repeat(swingInfo.rotation, 360f);

        // Calculate the multiplier for the "long way" rotation (if needed).
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
        isRotating = null;

        if (swingInfo.continues)
        {
            attackNo++;
            // Loop back to the first swing animation if the end of the array is reached.
            attackNo = (int)Mathf.Repeat(attackNo, swingAnimations.Length);
            isRotating = StartCoroutine(RotateLerp(swingAnimations[attackNo]));
        }
    }

    /// <summary>
    /// Smoothly interpolates the weapon's position over time from the current position to the target position.
    /// </summary>
    /// <param name="swingInfo">The data structure containing information about the swing, including duration, target location, and whether to take the long way.</param>
    /// <returns>Coroutine for smooth animation of the weapon's rotation.</returns>
    public IEnumerator MoveLerp(SwingInfo swingInfo)
    {
        Vector3 startPosition = transform.localPosition; // Record the current position.
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

        // Mark the rotate as complete by resetting the coroutine reference.
        isMoving = null;

        if (swingInfo.continues)
        {
            attackNo++;
            // Loop back to the first swing animation if the end of the array is reached.
            attackNo = (int)Mathf.Repeat(attackNo, swingAnimations.Length);
            isMoving = StartCoroutine(MoveLerp(swingAnimations[attackNo]));
        }
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

    public bool IsAttacking()
    {
        bool isAttacking = true;
        if (isMoving == null && isRotating == null)
        {
            isAttacking = false;
        }

        return isAttacking;
    }

    /// <summary>
    /// Contains information for each swing animation, including duration, final rotation location, and whether to take the long way around.
    /// </summary>
    [Serializable]
    public class SwingInfo
    {
        public float duration;  // The duration for the swing animation.
        public Vector3 location;  // The target location location 
        public float rotation;  // The target rotation rotation (Z-axis).
        public bool longWay;    // A flag indicating whether to take the long way for rotation (360 degrees path).
        public bool continues; // Continues to the next swing animation if true
    }
}