using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEnemyAttack : IteamSkillBase
{
    private void Start()
    {
        
    }
    public float DamageMultiple;
    public override void SkillEffect(GameObject obj)
    {
        var otherstetas=obj.GetComponent<Stetas>();
        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam)* (DamageMultiple-1), otherstetas.damageDown);

        Destroy(this);


    }
}
