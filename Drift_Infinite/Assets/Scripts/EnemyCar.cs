using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    private Vector3 previousPosition;
    private Vector3 targetPosition;
    private Quaternion previousRotation;
    private Quaternion targetRotation;
    private float interpolationTime = 0f;
    private float updateRate = 0.2f; // Частота обновления от сервера (в секундах)

    void Start()
    {
        previousPosition = transform.position;
        targetPosition = transform.position;
        previousRotation = transform.rotation;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (interpolationTime < updateRate)
        {
            interpolationTime += Time.deltaTime;
            float t = interpolationTime / updateRate; // Нормализуем [0,1]

            transform.position = Vector3.Lerp(previousPosition, targetPosition, t);
            transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, t);
        }
    }

    public void UpdateCarTransform(CarTransformInfo newTransform)
    {
        // Сохраняем старые позиции и назначаем новые
        previousPosition = transform.position;
        previousRotation = transform.rotation;

        targetPosition = new Vector3(newTransform.position.x, newTransform.position.y, newTransform.position.z);
        targetRotation = new Quaternion(newTransform.rotation.x, newTransform.rotation.y, newTransform.rotation.z, newTransform.rotation.w);

        interpolationTime = 0f; // Сбрасываем интерполяцию
    }
}