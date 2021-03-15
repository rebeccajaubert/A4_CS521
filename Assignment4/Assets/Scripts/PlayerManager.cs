using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    private bool hasTresor = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Tresor"))
        {
            hasTresor = true;
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Equals("WallEntrance") && hasTresor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

    }

}
