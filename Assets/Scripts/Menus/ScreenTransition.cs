using DG.Tweening;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    public float SingleArrowDuration = 0.2f;
    public float PatternInterval = 200f;

    [Header("Setup")]
    [SerializeField] private RectTransform canvasWidth;
    [SerializeField] private RectTransform conveyorPatternArrow;
    [SerializeField] private RectTransform fullWipeIn;
    [SerializeField] private RectTransform fullWipeOut;
    [SerializeField] private RectTransform wipeBoxL;
    [SerializeField] private RectTransform wipeBoxR;
    [SerializeField] private Vector2[] boxWipeDirections;
    [SerializeField] private float boxWipeShakeMagnitude = 32f;

    private RectTransform[] conveyorPattern;
    private static int lastSceneTransitionType = 0;
    private static Vector2 currentBoxWipeDirection;

    public void Start()
    {
        fullWipeIn.gameObject.SetActive(false);
        fullWipeOut.gameObject.SetActive(false);
        conveyorPatternArrow.gameObject.SetActive(false);
        if (lastSceneTransitionType == 1)
        {
            lastSceneTransitionType = 0;
            PlayNextLevelTransition(true);
        }
        if (lastSceneTransitionType == 2)
        {
            lastSceneTransitionType = 0;
            PlayRestartTransition(true);
        }
    }

    public void PlayNextLevelTransition(bool isSceneStart = false)
    {
        if (conveyorPattern == null || conveyorPattern.Length == 0)
        {
            var patternItemCount = Mathf.CeilToInt(canvasWidth.rect.width / PatternInterval);
            conveyorPattern = new RectTransform[patternItemCount];
            for (var i = 0; i < patternItemCount; i++)
            {
                conveyorPattern[i] = Instantiate(conveyorPatternArrow.gameObject, Vector2.zero, Quaternion.identity, conveyorPatternArrow.parent).transform as RectTransform;
                conveyorPattern[i].anchoredPosition = new Vector2((i + 0.25f) * PatternInterval - canvasWidth.rect.width * 0.5f, 0f);
                conveyorPattern[i].gameObject.SetActive(true);
            }
            conveyorPatternArrow.gameObject.SetActive(false);
        }
        var sequence = DOTween.Sequence();
        if (!isSceneStart)
        {
            lastSceneTransitionType = 1;

            var wipeTween = fullWipeIn.DOAnchorPosX(0f, 0.4f);
            sequence.Insert(0.6f, wipeTween);
            fullWipeIn.anchoredPosition = new Vector2(-canvasWidth.rect.width - 128f, 0f);
            fullWipeIn.gameObject.SetActive(true);
            fullWipeOut.gameObject.SetActive(false);
            for (var i = 0; i < conveyorPattern.Length; i++)
            {
                float startTime = i * 0.05f;
                conveyorPattern[i].localScale = new Vector3(-0.5f, 0f, 1f);
                sequence.Insert(startTime, conveyorPattern[conveyorPattern.Length - 1 - i]
                    .DOScale(new Vector3(-0.5f, 0.5f, 1f), 0.1f)
                    .SetEase(Ease.OutCubic));
            }
        }
        else
        {
            var wipeTween = fullWipeOut.DOAnchorPosX(canvasWidth.rect.width + 128f, 0.4f);
            sequence.Insert(0f, wipeTween);
            fullWipeOut.anchoredPosition = new Vector2(-128f, 0f);
            fullWipeIn.gameObject.SetActive(false);
            fullWipeOut.gameObject.SetActive(true);
            wipeTween.OnComplete(() => {
                fullWipeIn.gameObject.SetActive(false);
                fullWipeOut.gameObject.SetActive(false);
            });
            for (var i = 0; i < conveyorPattern.Length; i++)
            {
                float startTime = 0.6f + i * 0.05f;
                conveyorPattern[i].localScale = new Vector3(-0.5f, 0.5f, 1f);
                sequence.Insert(startTime, conveyorPattern[conveyorPattern.Length - 1 - i]
                    .DOScale(new Vector3(-0.5f, 0f, 1f), 0.1f)
                    .SetEase(Ease.OutCubic));
            }
        }
        sequence.Play();
    }

    public void PlayRestartTransition(bool isSceneStart = false)
    {
        var sequence = DOTween.Sequence();
        wipeBoxL.gameObject.SetActive(true);
        wipeBoxR.gameObject.SetActive(true);
        if (!isSceneStart)
        {
            currentBoxWipeDirection = boxWipeDirections[Random.Range(0, boxWipeDirections.Length)];
            lastSceneTransitionType = 2;

            wipeBoxL.anchoredPosition = Vector2.Scale(currentBoxWipeDirection, new Vector2(canvasWidth.rect.width * -0.5f, +canvasWidth.rect.height));
            wipeBoxR.anchoredPosition = Vector2.Scale(currentBoxWipeDirection, new Vector2(canvasWidth.rect.width * +0.5f, -canvasWidth.rect.height));
            sequence.Insert(0f, wipeBoxL.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.InCubic));
            sequence.Insert(0f, wipeBoxR.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.InCubic));
            sequence.Insert(0.6f, DOTween.To(() => boxWipeShakeMagnitude, ShakeBoxes, 0f, 0.4f));
        }
        else
        {
            wipeBoxL.anchoredPosition = new Vector2(0f, 0f);
            wipeBoxR.anchoredPosition = new Vector2(0f, 0f);
            sequence.Insert(0f, wipeBoxL.DOAnchorPos(Vector2.Scale(currentBoxWipeDirection, new Vector2(canvasWidth.rect.width * -0.5f, -canvasWidth.rect.height)), 0.4f).SetEase(Ease.InOutCubic));
            sequence.Insert(0f, wipeBoxR.DOAnchorPos(Vector2.Scale(currentBoxWipeDirection, new Vector2(canvasWidth.rect.width * +0.5f, +canvasWidth.rect.height)), 0.4f).SetEase(Ease.InOutCubic));
        }
        sequence.Play();
    }

    public void ShakeBoxes(float magnitude)
    {
        var vec = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * magnitude;
        wipeBoxL.anchoredPosition = vec;
        wipeBoxR.anchoredPosition = vec;
    }
}
