using R3;
using UnityEngine;

public class BallBounceAnim : MonoBehaviour, IAnim
{
    public GameObject[,,] balls;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int length = 5;
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;

    // 中心ほど速く、端ほど遅くしたい
    [SerializeField] private float minFallSpeed = 0.5f;  // 配列の端のボール
    [SerializeField] private float maxFallSpeed = 2.0f;  // 配列の中心のボール
    [SerializeField] private float resetY = -3f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);

    private CompositeDisposable disposable = new CompositeDisposable();

    // 速度計算用
    private Vector3 centerIndex;     // インデックス空間での中心
    private float maxCenterDistance; // 中心から一番遠いインデックスまでの距離

    public void StartAnimation()
    {
        StopAnimation();
        GenerateBalls();

        Observable.EveryUpdate()
            .Subscribe(_ => BounceBalls())
            .AddTo(disposable);
    }

    private void GenerateBalls()
    {
        balls = new GameObject[length, width, height];

        // インデックス空間の中心を計算
        centerIndex = new Vector3(
            (length - 1) * 0.5f,
            (width  - 1) * 0.5f,
            (height - 1) * 0.5f
        );

        // 中心から一番遠い場所（端のインデックス）までの距離
        maxCenterDistance = centerIndex.magnitude; // (0,0,0) との距離でOK

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    var pos = new Vector3(i, j, k) + offset;
                    balls[i, j, k] = Instantiate(ballPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }

    private void BounceBalls()
    {
        if (balls == null) return;

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    var ball = balls[i, j, k];
                    if (ball == null) continue;

                    var pos = ball.transform.position;

                    if (pos.y < resetY)
                    {
                        ball.transform.position = new Vector3(i, j, k) + offset;
                    }
                    else
                    {
                        // ★ インデックス空間での中心からの距離でスピードを変える
                        Vector3 indexPos = new Vector3(i, j, k);
                        float distFromCenter = Vector3.Distance(indexPos, centerIndex);

                        // 0(端)〜1(中心) に正規化
                        float t = (maxCenterDistance <= 0f)
                            ? 0f
                            : 1f - (distFromCenter / maxCenterDistance);

                        t = Mathf.Clamp01(t);

                        // 端 = minFallSpeed, 中心 = maxFallSpeed
                        float fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, t);

                        ball.transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
                    }
                }
            }
        }
    }

    public void StopAnimation()
    {
        disposable?.Dispose();
        disposable = new CompositeDisposable();

        if (balls != null)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        if (balls[i, j, k] != null)
                        {
                            Destroy(balls[i, j, k]);
                        }
                    }
                }
            }
        }

        balls = null;
    }

    private void OnDestroy()
    {
        StopAnimation();
    }
}