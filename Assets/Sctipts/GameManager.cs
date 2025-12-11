using UnityEngine;
using R3;

public class GameManager : MonoBehaviour
{
    [SerializeField]private PitchEventDetector pitchEventDetector;

    private CompositeDisposable disposable;
    
    [SerializeField] private AnimationController animationController;
    [SerializeField] private ParticleController particleController;

    [SerializeField] private string lowAnim;
    [SerializeField] private string midAnim;
    [SerializeField] private string highAnim;

    private void Awake()
    {
        disposable = new CompositeDisposable();
        
        Bind();
    }

    private void Bind()
    {
        //低音
        pitchEventDetector.lowEvent.Subscribe(_ =>
            {
                animationController.ChangeAnim(lowAnim);
                particleController.ChangeParticle(Pitch.low);
            })
            .AddTo(disposable);

        //中音
        pitchEventDetector.midEvent.Subscribe(_ =>
            {
                animationController.ChangeAnim(midAnim);
                particleController.ChangeParticle(Pitch.medium);
            })
            .AddTo(disposable);

        //高音
        pitchEventDetector.highEvent.Subscribe(_ =>
            {
                animationController.ChangeAnim(highAnim);
                particleController.ChangeParticle(Pitch.high);
            })
            .AddTo(disposable);

        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                pitchEventDetector.DetectPitchAndInvokeEvents();
            })
            .AddTo(disposable);
    }
}
