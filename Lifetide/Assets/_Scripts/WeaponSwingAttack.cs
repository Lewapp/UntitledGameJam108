using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwingAttack : MonoBehaviour
{
    public float swingSpeed = 2f;
    public float s1FinLocation = 120f;

    private bool isAttacking;

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isAttacking)
        {
            StartCoroutine(LightSwing1());
        }
    }

    private IEnumerator LightSwing1()
    {
        float rotPerTick = 80f;

        isAttacking = true;
        Vector3 startRotation = transform.localRotation.eulerAngles;
        float passedTime = 0f;

        float rotationAmount = Mathf.Abs(startRotation.z - s1FinLocation);
        rotationAmount = (rotationAmount <= 180f) ? (360f - rotationAmount) : (rotationAmount);
        rotationAmount = rotationAmount / 80f;

        float rotationPercent = rotationAmount > 1f ? 1f : rotationAmount;

        while (isAttacking)
        {
            if (passedTime >= 1)
            {
                startRotation = transform.localRotation.eulerAngles;
                passedTime = 0f;

                rotationAmount -= 1f;
                rotationPercent = rotationAmount > 1f ? 1f : rotationAmount;
            }

            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(new Vector3(0f, 0f, startRotation.z + rotPerTick * rotationPercent)), passedTime);
            passedTime += Time.deltaTime * swingSpeed * (1 + (1 - rotationPercent));
            yield return null;

            if (rotationAmount <= 0)
            {
                isAttacking = false;
            }
        }
    }
}
