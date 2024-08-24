using UnityEngine;
using System.Collections;
using MoreMountains.TopDownEngine;

/*
     01.【TopDownEngine.Health】设定在“血量将至0，则会触发死亡”
     02.【Minos_Health】则改为“血量将至0以下，才会触发死亡”
*/

public class Minos_Health : Health
{
    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public override void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration)
    {
        // if the object is invulnerable, we do nothing and exit
        if (Invulnerable)
        {
            return;
        }

        // if we're already below zero, we do nothing and exit
        if (CurrentHealth < 0)
        {
            return;
        }

        // we decrease the character's health by the damage
        float previousHealth = CurrentHealth;
        CurrentHealth -= damage;

        if (OnHit != null)
        {
            OnHit();
        }

        bool isDead = false;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            isDead = true;
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabled(invincibilityDuration));
        }

        // we trigger a damage taken event
        MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        if (_animator != null)
        {
            _animator.SetTrigger("Damage");
        }

        if (instigator != null)
        {
            DamageMMFeedbacks?.PlayFeedbacks(this.transform.position);
        }

        // we update the health bar
        UpdateHealthBar(true);

        // if health has reached zero
        if (isDead)
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;

            Kill();
        }
    }
}
