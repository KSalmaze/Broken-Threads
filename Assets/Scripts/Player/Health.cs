using System;
using System.ComponentModel;
using Tests.NetworkTest.Serializers;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
     private Serializer _serializer;
     // private float maxHealth = 100f;
     public int health = 100;
     public GameObject worldSpaceUI;
     private readonly Color orange = new(1.0f, 0.25f, 0.0f);
     bool isCrit; //implementar os dois na funcao TakeDamage
     bool isHeadshot;
     
     public void TakeDamage(int damage, bool dot, Vector3 position)
     {
          GameObject textInstance = Instantiate(worldSpaceUI, position, Quaternion.identity);
          Destroy(textInstance, 0.65f);
          
          TextMeshProUGUI textMesh = textInstance.GetComponentInChildren<TextMeshProUGUI>();
          textMesh.text = damage.ToString();

          isCrit = true;
          isHeadshot = true;
          switch ((isCrit ? 1 : 0) + (isHeadshot ? 2 : 0))
          {
               case 1: textMesh.color = orange;         textMesh.text += "!"; break; //crit
               case 2: textMesh.color = Color.yellow; textMesh.text += "!"; break; //headshot
               case 3: textMesh.color = Color.red;    textMesh.text += "!!";
                       textMesh.fontStyle = FontStyles.Bold; break;
          }
          
          Rigidbody2D textRB = textInstance.GetComponentInChildren<Rigidbody2D>();
          Vector2 impulse = new Vector2(Random.Range(2f, 5f), Random.Range(2f, 5f));
          impulse.x *= dot ? 1f : -1f; // changed the direction of the impulse depending on the hit angle
          textRB.AddForce(impulse, ForceMode2D.Impulse);
          
          health -= damage;
          if (health <= 0)
          {
               Morreu();
          }
          Debug.Log(health);
     }

     public void Morreu()
     {
          ConnectionSingleton.Instance.Connection.UDP_Send_Message(
               new Message("DIE", new byte[]{0}));
     }
}
