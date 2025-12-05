using R3;
using UnityEngine;

public class BallAnimationStateManager : MonoBehaviour
{
    public ReactiveProperty<States> currentState;

    public GameObject bounce;
    public GameObject spin;

    private IAnim bounceAnim;
    private IAnim spinAnim;
    private IAnim currentAnim;

    private CompositeDisposable disposable;

    private void Awake()
    {
        bounceAnim = bounce.GetComponent<IAnim>();
        spinAnim   = spin.GetComponent<IAnim>();
    }

    private void Start()
    {
        disposable = new CompositeDisposable();
        currentState = new ReactiveProperty<States>(States.idle);
        Bind();

        // 初期状態
        SetState(States.bounce);
    }

    private void Bind()
    {
        currentState
            .DistinctUntilChanged() // 同じstateが連続でセットされたら無視
            .Subscribe(state =>
            {
                // まず前のアニメーションを止める
                currentAnim?.StopAnimation();
                currentAnim = null;

                switch (state)
                {
                    case States.idle:
                        // 何もしない
                        break;

                    case States.bounce:
                        currentAnim = bounceAnim;
                        break;

                    case States.spin:
                        currentAnim = spinAnim;
                        break;
                }

                // 新しいステートのアニメーション開始
                currentAnim?.StartAnimation();
            })
            .AddTo(disposable);
    }

    public void SetState(States newState)
    {
        currentState.Value = newState;
    }

    private void OnDestroy()
    {
        disposable?.Dispose();
        currentAnim?.StopAnimation();
    }
}

public enum States
{
    idle,
    bounce,
    spin,
}