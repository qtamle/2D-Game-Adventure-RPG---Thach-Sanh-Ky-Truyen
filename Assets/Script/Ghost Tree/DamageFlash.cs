using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;
    [SerializeField] private AnimationCurve flashCurve;

    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;

    public Coroutine damageFlashCoroutine;
    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        Init();
    }

    private void Init()
    {
        materials = new Material[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }

    public void CallDamageFlash()
    {
        damageFlashCoroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        SetFlashColor();

        float currentFlashAmout = 0f;
        float esapsedTime = 0f;
        while (esapsedTime < flashTime) 
        { 
            esapsedTime += Time.deltaTime;

            currentFlashAmout = Mathf.Lerp(1f, flashCurve.Evaluate(esapsedTime), (esapsedTime / flashTime));
            SetFlashAmount(currentFlashAmout);
            yield return null;
        }
    }

    private void SetFlashColor()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_flashcolor", flashColor);
        }
    }

    private void SetFlashAmount(float amount)
    {
        for (int i = 0; i< materials.Length; i++)
        {
            materials[i].SetFloat("_FlashAmount", amount);
        }
    }
}
