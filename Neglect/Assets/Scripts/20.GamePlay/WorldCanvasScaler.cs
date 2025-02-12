using System;
using UnityEngine;

public class WorldCanvasScaler : MonoBehaviour
{
    public Camera mainCamera;
    public Canvas canvas;
    public Sprite sprite;

    public void Update()
    {
        AdjustCanvasScale();
    }

    void AdjustCanvasScale()
    {
        float unitToPixel = Screen.height / (Camera.main.orthographicSize * 2); // 1Unit 당 몇 픽셀인지
        RectTransform rt = canvas.GetComponent<RectTransform>();

        // 스프라이트의 실제 크기 (월드 유닛 기준)
        Vector2 spriteSize = sprite.bounds.size;

        // 캔버스의 현재 width, height 가져오기
        Vector2 canvasSize = rt.sizeDelta;

        // 스프라이트 크기에 맞추기 위한 스케일 계산
        float scaleX = spriteSize.x / canvasSize.x;
        float scaleY = spriteSize.y / canvasSize.y;

        // 동일 비율로 스케일 조정 (비율 유지하려면 둘 중 작은 값 사용)
        float uniformScale = Mathf.Min(scaleX, scaleY);

        // 캔버스 스케일 적용
        canvas.transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
    }
}

