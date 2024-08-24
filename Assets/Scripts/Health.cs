using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
     // private float maxHealth = 100f;
     public int health = 100;
     
     
     public void TakeDamage(int damage)
     {
          health -= damage;
          if (health <= 0) Destroy(gameObject);
          Debug.Log(health);
     }
}
