using System;
using System.ComponentModel;
using Tests.NetworkTest.Serializers;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
     // private float maxHealth = 100f;
     public int health = 100;
     public GameObject worldSpaceUI;
     private readonly Color orange = new(1.0f, 0.25f, 0.0f);
     bool isCrit; //implementar os dois na funcao TakeDamage
     bool isHeadshot;
     private GameRules gameRule;

     void Start()
     {
          gameRule = GameObject.Find("GameManager").GetComponent<GameRules>();
     }

     public void TakeDamage(int damage, Vector3 position, Transform textRotateTarget)
     {
          GameObject textInstance = Instantiate(worldSpaceUI, position, textRotateTarget.rotation); //Quaternion.identity);
          Destroy(textInstance, 0.65f);
          
          TextMeshProUGUI textMesh = textInstance.GetComponentInChildren<TextMeshProUGUI>();
          textMesh.text = damage.ToString();
          textInstance.GetComponentInChildren<RotateText>().textRotateTarget = textRotateTarget;
          textInstance.transform.rotation = textRotateTarget.rotation;
          
          Rigidbody2D textRB = textInstance.GetComponentInChildren<Rigidbody2D>();
          Vector2 impulse = new Vector2(Random.Range(2f, 5f), Random.Range(2f, 5f));
          impulse.x *= Random.Range(0,1f)>0.5f ? 1f : -1f;
          textRB.AddForce(impulse, ForceMode2D.Impulse);
          
          // Vector3 perpendicularDirection = Vector3.Cross(transform.forward, Vector3.up).normalized;
          // textRB.AddForce(perpendicularDirection * impulse, ForceMode.Impulse);

          
          
          isCrit = true;
          isHeadshot = true;
          switch ((isCrit ? 1 : 0) + (isHeadshot ? 2 : 0))
          {
               case 1: textMesh.color = orange;         textMesh.text += "!"; break; //crit
               case 2: textMesh.color = Color.yellow; textMesh.text += "!"; break; //headshot
               case 3: textMesh.color = Color.red;    textMesh.text += "!!";
                       textMesh.fontStyle = FontStyles.Bold; break;
          }
          
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
          gameRule.pontuacao++;
     }
}
