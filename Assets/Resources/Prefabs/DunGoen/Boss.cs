using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public RoomDataSO roomData;
    public GameObject boosObj;
    public GameObject playerObj;

    public Transform spawnPot;
    public Transform playerSpawnPot;

    public Image fill;


    public AudioClip bossMusic;
    
    private void Start()
    {
        SoundManager.Instance.ChangeBackGroundMusic(bossMusic,0.2f);
        StartCoroutine(BossAppearCo());
    }


  
  

    IEnumerator BossAppearCo()
    {
        yield return new WaitForSeconds(1f);
        BossMonster boss = Instantiate(boosObj, spawnPot.position, Quaternion.identity).GetComponent<BossMonster>();
        yield return new WaitForSeconds(0.1f);
        HealthSystem bossHelathSystem = boss.GetComponent<HealthSystem>();
        float curhealth = bossHelathSystem.CurrentHealth;
        Debug.Log($"{curhealth}");
        //float curhealth = boss.bossHealth;

        while (bossHelathSystem.CurrentHealth > 0)
        {
            if(bossHelathSystem.CurrentHealth != curhealth)
            {
                curhealth = bossHelathSystem.CurrentHealth;
                fill.fillAmount = curhealth / bossHelathSystem.MaxHealth;
            }

            yield return new WaitForSeconds(0.1f);
        }
        DunGoenManager.Instance.CreatePortal(transform);
        fill.fillAmount = 0;
    }

}
