using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Color[] transitionRandomColors;
    [SerializeField] private bool transitionRandomColorAffectsNextLevel = true;
    [SerializeField] private float boxWipeShakeMagnitude = 32f;

    private RectTransform[] conveyorPattern;
    private bool transitionPlaying = false;
    private static int lastSceneTransitionType = 0;
    private static Vector2 currentBoxWipeDirection;
    private static Color currentTransitionColor;

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
        if (transitionPlaying)
        {
            return;
        }
        transitionPlaying = true;

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
            currentTransitionColor = Color.white;
            if (transitionRandomColorAffectsNextLevel)
            {
                currentTransitionColor = transitionRandomColors[Random.Range(0, transitionRandomColors.Length)];
            }
            lastSceneTransitionType = 1;

            var wipeTween = fullWipeIn.DOAnchorPosX(0f, 0.4f);
            sequence.Insert(0.6f, wipeTween);
            fullWipeIn.anchoredPosition = new Vector2(-canvasWidth.rect.width - 128f, 0f);
            fullWipeIn.GetComponent<Graphic>().color = currentTransitionColor;
            fullWipeIn.gameObject.SetActive(true);
            fullWipeOut.gameObject.SetActive(false);
            for (var i = 0; i < conveyorPattern.Length; i++)
            {
                var startTime = i * 0.05f;
                var itemIndex = conveyorPattern.Length - 1 - i;
                conveyorPattern[itemIndex].GetComponent<Graphic>().color = currentTransitionColor;
                conveyorPattern[itemIndex].localScale = new Vector3(-0.5f, 0f, 1f);
                sequence.Insert(startTime, conveyorPattern[itemIndex]
                    .DOScale(new Vector3(-0.5f, 0.5f, 1f), 0.1f)
                    .SetEase(Ease.OutCubic));
            }
        }
        else
        {
            var wipeTween = fullWipeOut.DOAnchorPosX(canvasWidth.rect.width + 128f, 0.4f);
            sequence.Insert(0f, wipeTween);
            fullWipeOut.anchoredPosition = new Vector2(-128f, 0f);
            fullWipeOut.GetComponent<Graphic>().color = currentTransitionColor;
            fullWipeIn.gameObject.SetActive(false);
            fullWipeOut.gameObject.SetActive(true);
            wipeTween.OnComplete(() => {
                fullWipeIn.gameObject.SetActive(false);
                fullWipeOut.gameObject.SetActive(false);
            });
            for (var i = 0; i < conveyorPattern.Length; i++)
            {
                var startTime = 0.6f + i * 0.05f;
                var itemIndex = conveyorPattern.Length - 1 - i;
                conveyorPattern[itemIndex].GetComponent<Graphic>().color = currentTransitionColor;
                conveyorPattern[itemIndex].localScale = new Vector3(-0.5f, 0.5f, 1f);
                sequence.Insert(startTime, conveyorPattern[itemIndex]
                    .DOScale(new Vector3(-0.5f, 0f, 1f), 0.1f)
                    .SetEase(Ease.OutCubic));
            }
        }
        sequence.Play();
    }

    public void PlayRestartTransition(bool isSceneStart = false)
    {
        if (transitionPlaying)
        {
            return;
        }
        transitionPlaying = true;

        var sequence = DOTween.Sequence();
        wipeBoxL.gameObject.SetActive(true);
        wipeBoxR.gameObject.SetActive(true);
        if (!isSceneStart)
        {
            currentTransitionColor = transitionRandomColors[Random.Range(0, transitionRandomColors.Length)];
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
        wipeBoxL.GetComponent<Graphic>().color = currentTransitionColor;
        wipeBoxR.GetComponent<Graphic>().color = currentTransitionColor;
        sequence.Play();
    }

    public void ShakeBoxes(float magnitude)
    {
        var vec = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * magnitude;
        wipeBoxL.anchoredPosition = vec;
        wipeBoxR.anchoredPosition = vec;
    }
}
