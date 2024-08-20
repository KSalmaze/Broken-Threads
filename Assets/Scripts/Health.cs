using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
     // private float maxHealth = 100f;
     public int health = 100;
     public GameObject worldSpaceUI;
     private readonly Color orange = new(1.0f, 0.25f, 0.0f);
     int i;
     bool isCrit; //implementar os dois na funcao TakeDamage
     bool isHeadshot;
     
     public void TakeDamage(int damage, float dot)
     {
          GameObject textInstance = Instantiate(worldSpaceUI, transform.position, Quaternion.identity);
          Destroy(textInstance, 0.65f);

          TextMeshProUGUI textMesh = textInstance.GetComponentInChildren<TextMeshProUGUI>();
          textMesh.text = damage.ToString();

          ++i;
          isCrit = i%2 == 0;
          isHeadshot = i%3 == 0;
          int hitType = (isCrit ? 1 : 0) + (isHeadshot ? 2 : 0);
          switch (hitType)
          {
               case 1: //crit
                    textMesh.color = orange;
                    textMesh.text += "!";
                    break;
               case 2: //headshot
                    textMesh.color = Color.yellow;
                    textMesh.text += "!";
                    break;
               case 3: //both
                    textMesh.color = Color.red;
                    textMesh.fontStyle = FontStyles.Bold;
                    textMesh.text += "!!";
                    break;
          }

          
          Rigidbody2D textRB = textInstance.GetComponentInChildren<Rigidbody2D>();
          Vector2 impulse = new Vector2(Random.Range(2f, 5f), Random.Range(2f, 5f));
          impulse.x *= dot > 0 ? 1f : -1f; // changed the direction of the impulse depending on the hit angle
          textRB.AddForce(impulse, ForceMode2D.Impulse);
          
          health -= damage;
          if (health <= 0) Destroy(gameObject);
          Debug.Log(health);
     }
}
