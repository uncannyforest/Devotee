using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour {
    public GameObject heart;
    public int max = 6;

    private int level;

    void Start() {
        level = max;
        for (int i = 0; i < max; i++)
            GameObject.Instantiate(heart, transform);
    }

    public void Decrease() {
        level--;
        StartCoroutine(RemoveHeart());
    }

    private IEnumerator RemoveHeart() {
        Transform child = transform.GetChild(transform.childCount - 1);
        child.GetComponentInChildren<ParticleSystem>().Play();
        child.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(child.GetComponentInChildren<ParticleSystem>().main.startLifetime.constant);
        GameObject.Destroy(child.gameObject);
    }

    public void Increase() {
        if (level >= max) return;
        level++;
        GameObject.Instantiate(heart, transform);
    }
}
