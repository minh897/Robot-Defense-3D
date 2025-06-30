using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStealth : Enemy
{
    [Header("Stealth Enemy Details")]
    [SerializeField] private float hideDuration = .5f;
    [SerializeField] private ParticleSystem smokeScreenFX;
    [SerializeField] private List<Enemy> enemiesToHide;
    private bool canHideEnemy = true;

    public void EnableSmokeScreen(bool isEnable)
    {
        if (smokeScreenFX.isPlaying == false && isEnable)
            smokeScreenFX.Play();
        else if (smokeScreenFX.isPlaying == true && isEnable == false)
            smokeScreenFX.Stop();
    }

    private void HideItSelf()
    {
        HideEnemy(hideDuration);
    }

    private void HideEnemies()
    {
        if (canHideEnemy == false)
            return;
            
        foreach (Enemy enemy in enemiesToHide)
        {
            enemy.HideEnemy(hideDuration);
        }
    }

    protected override IEnumerator DisableHideCo(float duration)
    {
        canBeHidden = false;
        canHideEnemy = false;
        EnableSmokeScreen(false);

        yield return new WaitForSeconds(duration);

        EnableSmokeScreen(true);
        canBeHidden = true;
        canHideEnemy = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InvokeRepeating(nameof(HideItSelf), .1f, hideDuration);
        InvokeRepeating(nameof(HideEnemies), .1f, hideDuration);
    }

    public List<Enemy> GetEnemiesToHide() => enemiesToHide;
}
