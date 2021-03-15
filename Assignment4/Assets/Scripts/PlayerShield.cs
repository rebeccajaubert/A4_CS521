using UnityEngine.UI;
using UnityEngine;
using System;

public class PlayerShield : MonoBehaviour
{
    public Text shieldText;
    public GameObject shield;
    private static float defenseValue=10f;
    private bool isShieldEnabled = false;
    private int numberHits = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//activate shield
        {
            isShieldEnabled = !isShieldEnabled;
        }
        if (isShieldEnabled && defenseValue>0.01) {
            defenseValue -= Time.deltaTime;
            shield.SetActive(true);
            shieldText.text = defenseValue.ToString();
        }    
        else
        {
        shield.SetActive(false);
        }

        if(numberHits >= 2)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Thrown"))
        {
            numberHits++;
        }
    }




}
