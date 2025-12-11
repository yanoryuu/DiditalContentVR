using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> lowParticleSystems;
    [SerializeField] private List<ParticleSystem> mediumParticleSystems;
    [SerializeField] private List<ParticleSystem> highParticleSystems;

    // ※単一の変数 _delayedCallTween はリスト管理には適さないため削除しました

    public void ChangeParticle(Pitch pitch)
    {
        switch (pitch)
        {
            case Pitch.low:
                PlayParticles(lowParticleSystems);
                break;
            case Pitch.medium:
                PlayParticles(mediumParticleSystems);
                break;
            case Pitch.high:
                PlayParticles(highParticleSystems);
                break;
        }
    }
    private void PlayParticles(List<ParticleSystem> targetParticles)
    {
        // リストが空、またはnullなら何もしない
        if (targetParticles == null) return;

        foreach (var myParticle in targetParticles)
        {
            if (myParticle == null) continue;

            myParticle.transform.DOKill();

            // シーケンス作成
            var sequence = DOTween.Sequence();

            sequence.Append(myParticle.transform.DOScale(Vector3.zero, 0f)) 
                .AppendInterval(0.1f)                                   
                .Append(myParticle.transform.DOScale(Vector3.one, 0f))      
                .OnComplete(() =>
                {
                    // 表示されたタイミングで再生
                    if (!myParticle.isPlaying) myParticle.Play();
                })
                .SetLink(myParticle.gameObject);
        }
    }
}

public enum Pitch
{
    low,
    medium,
    high
}