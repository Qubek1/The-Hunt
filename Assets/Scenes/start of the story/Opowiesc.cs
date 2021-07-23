using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opowiesc : MonoBehaviour
{

    [SerializeField] GameObject musicPlayer;

    public void SelfDeactive ()
    {
        this.gameObject.SetActive(false);

    }

    public void SetMusicPlayerActive()
    {
        musicPlayer.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetMusicPlayerActive();
            SelfDeactive();
        }
    }
}
