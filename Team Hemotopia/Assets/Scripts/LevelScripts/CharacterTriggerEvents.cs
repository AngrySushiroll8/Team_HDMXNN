using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterTriggerEvents : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent = gameObject.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }
}
