using UnityEngine;

public class Enemy
{
    public float life = 10;
    public float maxLife = 10;
    public float damage = 2;
    public float defense = 1;
    public float xpGiven = 12;

    public void TakeDamage(int damage)
    {
        life -= damage;
    }
}
