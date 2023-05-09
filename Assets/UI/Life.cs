using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ShowEverywhere))]
public class Life : MonoBehaviour {
    public GameObject heart;
    public int max = 6;
    public float invincibilityFrameTime = 2;
    [TextArea(3,10)] public string deathMessage;

    private int level;
    private ShowEverywhere showEverywhere;
    private ThirdPersonCharacter player;
    private Interaction interaction;
    private FlexibleInputDisplay input;

    private bool invincibilityFrames = false;

    void Start() {
        showEverywhere = GetComponent<ShowEverywhere>();
        player = FindObjectOfType<ThirdPersonCharacter>();
        interaction = FindObjectOfType<Interaction>();
        input = FindObjectOfType<FlexibleInputDisplay>();
        level = max;
        for (int i = 0; i < max; i++)
            GameObject.Instantiate(heart, transform);
    }

    public void Decrease() {
        if (invincibilityFrames || level == 0) return;
        level--;
        StartCoroutine(RemoveHeart());
        StartCoroutine(InvincibilityFrames());
        showEverywhere.ActivateTemp();
        if (level == 0) Die();
            
    }

    private IEnumerator RemoveHeart() {
        Transform child = transform.GetChild(transform.childCount - 1);
        child.GetComponentInChildren<ParticleSystem>().Play();
        child.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(child.GetComponentInChildren<ParticleSystem>().main.startLifetime.constant);
        GameObject.Destroy(child.gameObject);
    }

    private IEnumerator InvincibilityFrames() {
        invincibilityFrames = true;
        yield return new WaitForSeconds(invincibilityFrameTime);
        invincibilityFrames = false;
    }

    public void Increase() {
        if (level >= max) return;
        level++;
        GameObject.Instantiate(heart, transform);
    }

    private void Die() {
        player.SetDead(true);
        interaction.mustRelinquish = Resurrect;
        input.SetLayerOrthographic().SetLargeMessage(deathMessage);
    }

    private void Resurrect() {
        player.SetDead(false);
        player.transform.position = Terrain.I.spawnPoint.position;
        player.transform.rotation = Terrain.I.spawnPoint.rotation;
        input.SetLargeMessage(null);
    }
}
