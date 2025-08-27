using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    Color color;

    void Start()
    {
        color = model.material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            GameManager.instance.playerSpawnPos.transform.rotation = transform.rotation;
            GameManager.instance.SaveForRespawn();
            StartCoroutine(CheckpointFeedback());
        }
    }

    IEnumerator CheckpointFeedback()
    {
        model.material.color = Color.red;
        //GameManager.instance.checkpointPopup.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        model.material.color = color;
        //GameManager.instance.checkpointPopup.SetActive(false);
    }
}
