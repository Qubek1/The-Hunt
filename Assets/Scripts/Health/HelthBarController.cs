using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelthBarController : MonoBehaviour
{
    public GameObject layoutGroup;
    [System.NonSerialized] public int maxHP;
    [System.NonSerialized] public int currentHP;
    [System.NonSerialized] public float hideTime;
    public float showTimeAfterDamage;
    Animator[] animators;

    private void Start()
    {
        animators = layoutGroup.GetComponentsInChildren<Animator>();
    }

    private void Update()
    {
        if(gameObject.active && hideTime < Time.time)
        {
            gameObject.SetActive(false);
        }
    }

    public void Show(float time)
    {
        if(hideTime < Time.time)
        {
            gameObject.SetActive(true);
            for (int i = currentHP; i < maxHP; i++)
            {
                animators[i].SetBool("hide", true);
            }
        }
        hideTime = Mathf.Max(hideTime, Time.time + time);
    }

    public void TakeDamage(int amount)
    {
        Show(showTimeAfterDamage);

        for(int i=currentHP-1; i >= currentHP-amount && i >= 0; i--)
        {
            animators[i].SetTrigger("LostHP");
        }
        currentHP -= amount;
    }
}
