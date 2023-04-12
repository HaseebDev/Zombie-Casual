using System;
using System.Collections;
using UnityEngine;

public class HitColorVisualisation : MonoBehaviour
{
    private void Start()
    {
    }

    public void Reset()
    {
        //this.particelObj.Stop();
        //this.particelObj.Clear();
    }

    public void PlayHitEffect()
    {
        //if (this.onVisualNeedPlay)
        //{
        //	return;
        //}
        //this.onVisualNeedPlay = true;
        //base.StartCoroutine(this.playEffect(0.2f));
    }

    private IEnumerator playEffect(float time)
    {
        //this.Reset();
        //yield return new WaitForSeconds(time);
        //this.particelObj.Play();
        //this.onVisualNeedPlay = false;
        //yield break;
        yield break;
    }

    [SerializeField]
    private ParticleSystem particelObj;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private bool onVisualNeedPlay;
}
