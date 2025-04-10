using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConfettiEffect : MonoBehaviour
{
    public GameObject confettiPrefab; // Префаб с компонентом Image
    public RectTransform spawnPoint;  // Точка появления (например, центр экрана)
    public int piecesCount = 50;      // Количество частиц
    public float spread = 500f;       // Разброс частиц
    public float jumpPower = 300f;    // Высота "прыжка"
    public float duration = 1.5f;     // Длительность анимации

    void OnEnable()
    {
        DOTween.SetTweensCapacity(5000, 500);
        CreateConfetti();
    }

    void CreateConfetti()
    {
        for (int i = 0; i < piecesCount; i++)
        {
            // Создание частицы
            GameObject confetti = Instantiate(confettiPrefab, spawnPoint.parent);
            RectTransform rt = confetti.GetComponent<RectTransform>();
            Image img = confetti.GetComponent<Image>();
            
            // Настройка начальных параметров
            rt.anchoredPosition = spawnPoint.anchoredPosition;
            rt.localScale = Vector3.zero;
            img.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f, 1f, 1f);

            // Генерация случайной конечной позиции
            Vector2 endPos = new Vector2(
                spawnPoint.anchoredPosition.x + Random.Range(-spread, spread),
                spawnPoint.anchoredPosition.y + Random.Range(-spread, spread)
            );

            // Настройка анимации
            Sequence seq = DOTween.Sequence();
            
            // Прыжок по параболе
            seq.Append(rt.DOJumpAnchorPos(endPos, jumpPower, 1, duration));
            
            // Вращение
            seq.Join(rt.DORotate(new Vector3(0, 0, Random.Range(-360f, 360f)), duration, RotateMode.FastBeyond360));
            
            // Масштабирование
            seq.Join(rt.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
            seq.Append(rt.DOScale(0f, 0.3f).SetEase(Ease.InBack).SetDelay(duration * 0.7f));
            
            // Исчезновение
            seq.Join(img.DOFade(0, 0.3f).SetDelay(duration * 0.7f));
            
            // Уничтожение после завершения
            seq.OnComplete(() => Destroy(confetti));
        }
    }
}