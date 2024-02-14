using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatManager
{
    public Action OnDead;
    public Action OnPlayerSetup;
    public Action<string, int> OnPerkSetup;
    public Action<int, int> OnHealing;
    public Action<int, int> OnGetDamaged;
    public Action<Item_SO> OnApplyPerkStat;

    // ### Stats
    public int Hp { get; private set; }
    public float Atk { get; private set; }
    public float FireRate { get; private set; }
    public float MoveSpeed { get; private set; }
    public int AddBullet { get; private set; }
    public int PierceCount { get; private set; }

    public bool IsDead { get; private set; } = false;

    private Dictionary<string, int> _perkDict = new Dictionary<string, int>();

    public void Init()
    {
        Hp = 10;
        MoveSpeed = 5;
        Atk = 1;
        FireRate = 1;
        AddBullet = 0;
        PierceCount = 0;
    }

    public void PlayerSetup()
    {
        // To Do - 플레이어 퍽을 받아서 적용 시키기.        

        string[] keys = Enum.GetNames(typeof(Define.Perks));        
        for (int i = 0; i < keys.Length; i++)
        {
            if (_perkDict.TryGetValue(keys[i], out int value))
                OnPerkSetup?.Invoke(keys[i], value);
        }

        OnPlayerSetup?.Invoke();
    }

    public void Healing()
    {
        int prevHp = Hp;
        Hp++;
        OnHealing?.Invoke(prevHp, Hp);
    }

    public void ApplyPerkStat(Item_SO item)
    {
        if (_perkDict.ContainsKey(item.name))
            _perkDict[item.name]++;
        else
            _perkDict.Add(item.name, 1);

        Atk += item.atk;
        FireRate += item.fireRate;        
        AddBullet += item.addBullet;
        PierceCount += item.pierceCount;
        
        if (item.name == Define.Perks.Item_Injector.ToString() && _perkDict.ContainsKey(item.name) == false)
            MoveSpeed *= item.moveSpeed;

        OnApplyPerkStat?.Invoke(item);
    }

    public void GetDamaged()
    {
        if (IsDead)
            return;

        int prevHp = Hp;

        Hp--;
        OnGetDamaged?.Invoke(prevHp, Hp);

        if (Hp <= 0)
        {
            //To Do - GameOver
            IsDead = true;
            OnDead?.Invoke();
            Init();
            Debug.Log("캐릭터 사망");
        }
    }

    public void Clear()
    {
        IsDead = false;
        OnDead = null;
        OnPlayerSetup = null;
        OnPerkSetup = null;
        OnHealing = null;
        OnGetDamaged = null;
        OnApplyPerkStat = null;        
    }
}
