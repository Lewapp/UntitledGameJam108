using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputSettings;

public class WeaponInteractions : MonoBehaviour
{
    public float swingSpeed = 2f;
    public float s1FinLocation = 120f;
    public float s2FinLocation = -150f;

    private float rotationPerTick = 80f;
    private bool isAttacking;
    private int attackNo = 0;
    private float GetRotationCount(float startRotation, float finalZRot, float rotPerTick)
    {
        float rotationAmount = Mathf.Abs(startRotation - finalZRot);
        rotationAmount = (rotationAmount <= 180f) ? (360f - rotationAmount) : (rotationAmount);
        return rotationAmount = rotationAmount / rotPerTick;
    }

    private float GetRotationPercent(float rotationAmount)
    {
        float rotationPercent = rotationAmount > 1f ? 1f : rotationAmount;
        return rotationPercent;
    }

    private Quaternion GetRotation(Vector3 startRotation, float rotPerTick, float rotationPercent, float stagePercent)
    {
        return Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(new Vector3(0f, 0f, startRotation.z + rotPerTick * rotationPercent)), stagePercent);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!isAttacking)
        {
            switch(attackNo)
            {
                case 0:
                    attackNo++;
                    StartCoroutine(LightSwing(rotationPerTick, s1FinLocation));
                    break;
                case 1:
                    StartCoroutine(LightSwing(-rotationPerTick, -s2FinLocation));
                    attackNo = 0;
                    break;
            }
        }
    }

    private IEnumerator LightSwing(float rotPerTick, float finalZRot)
    {
        isAttacking = true;
        Vector3 startRotation = transform.localRotation.eulerAngles;
        float passedTime = 0f;

        float rotationAmount = GetRotationCount(startRotation.z, finalZRot, rotPerTick);
        float rotationPercent = GetRotationPercent(rotationAmount);

        while (isAttacking)
        {
            if (passedTime >= 1)
            {
                startRotation = transform.localRotation.eulerAngles;
                passedTime = 0f;

                rotationAmount -= 1f;
                rotationPercent = rotationAmount > 1f ? 1f : rotationAmount;
            }

            transform.localRotation = GetRotation(startRotation, rotPerTick, rotationPercent, passedTime);
            passedTime += Time.deltaTime * swingSpeed * (1 + (1 - rotationPercent));
            yield return null;

            if (rotationAmount <= 0)
            {
                isAttacking = false;
            }
        }
    }

 /*   private IEnumerator PrepareWeapon(Vector3 finalRot, float rotPerTick)
    {
        bool isSetting = true;
        Vector3 startRotation = transform.localRotation.eulerAngles;
        float passedTime = 0f;

        float rotationAmount = GetRotationCount(startRotation.z, finalRot.z);
        float rotationPercent = GetRotationPercent(rotationAmount);

        while (isSetting)
        {
            if (passedTime >= 1)
            {
                startRotation = transform.localRotation.eulerAngles;
                passedTime = 0f;

                rotationAmount -= 1f;
                rotationPercent = rotationAmount > 1f ? 1f : rotationAmount;
            }

            transform.localRotation = GetRotation(startRotation, rotPerTick, rotationPercent, passedTime);
            passedTime += Time.deltaTime * swingSpeed * (1 + (1 - rotationPercent));
            yield return null;

            if (rotationAmount <= 0)
            {
                isSetting = false;
            }
        }

    }*/
}
