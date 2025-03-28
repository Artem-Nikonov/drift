using TMPro;
using UnityEngine;

//public class EnemyCar : MonoBehaviour
//{
//    private CarTransformInfo transformInfo = null;
//    private CarTransformInfo previousTransformInfo = null;
//    private float lerpTime = 0.2f; // ¬рем€ интерпол€ции (должно совпадать с частотой обновлени€ сервера)
//    private float currentLerpTime = 0f;

//    [SerializeField] private Renderer renderer;
//    [SerializeField] private TextMeshProUGUI userNane;
//    public Renderer Renderer => renderer;
//    public TextMeshProUGUI UserName => userNane;

//    void Update()
//    {
//        if (transformInfo != null)
//        {
//            if (previousTransformInfo == null)
//            {
//                // ≈сли это первое обновление, сразу устанавливаем позицию
//                previousTransformInfo = transformInfo;
//                transform.position = new Vector3(transformInfo.position.x, transformInfo.position.y, transformInfo.position.z);
//                transform.rotation = new Quaternion(transformInfo.rotation.x, transformInfo.rotation.y, transformInfo.rotation.z, transformInfo.rotation.w);
//            }
//            else
//            {
//                // »нтерпол€ци€ между предыдущей и текущей позицией
//                currentLerpTime += Time.deltaTime;
//                float t = Mathf.Clamp01(currentLerpTime / lerpTime);

//                // »нтерпол€ци€ позиции
//                Vector3 targetPos = new Vector3(transformInfo.position.x, transformInfo.position.y, transformInfo.position.z);
//                Vector3 prevPos = new Vector3(previousTransformInfo.position.x, previousTransformInfo.position.y, previousTransformInfo.position.z);
//                transform.position = Vector3.Lerp(prevPos, targetPos, t);

//                // »нтерпол€ци€ вращени€
//                Quaternion targetRot = new Quaternion(transformInfo.rotation.x, transformInfo.rotation.y, transformInfo.rotation.z, transformInfo.rotation.w);
//                Quaternion prevRot = new Quaternion(previousTransformInfo.rotation.x, previousTransformInfo.rotation.y, previousTransformInfo.rotation.z, previousTransformInfo.rotation.w);
//                transform.rotation = Quaternion.Slerp(prevRot, targetRot, t);

//                // ≈сли интерпол€ци€ завершена и пришли новые данные, обновл€ем
//                if (t >= 1f && transformInfo != previousTransformInfo)
//                {
//                    previousTransformInfo = transformInfo;
//                    currentLerpTime = 0f;
//                }
//            }
//        }
//    }

//    public void SetTransformInfo(CarTransformInfo newInfo)
//    {
//        transformInfo = newInfo;
//    }
//}

//public class EnemyCar : MonoBehaviour
//{
//    [SerializeField] private Renderer renderer;
//    [SerializeField] private TextMeshProUGUI userNane;
//    public Renderer Renderer => renderer;
//    public TextMeshProUGUI UserName => userNane;
//    private float smoothSpeed = 1f; // —корость интерпол€ции


//    public CarTransformInfo transformInfo { get; set; } = null;
//    // Start is called before the first frame update
//    void Start()
//    {
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (transformInfo != null)
//        {
//            var pos = transformInfo.position;
//            transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, pos.y, pos.z), smoothSpeed * Time.deltaTime);

//            var rot = transformInfo.rotation;
//            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(rot.x, rot.y, rot.z, rot.w), smoothSpeed * Time.deltaTime);
//        }
//    }
//}

public class EnemyCar : MonoBehaviour
{

    [SerializeField] private Renderer renderer;
    [SerializeField] private TextMeshProUGUI userNane;
    public Renderer Renderer => renderer;
    public TextMeshProUGUI UserName => userNane;
    private float smoothSpeed = 3f; // —корость интерпол€ции

    public CarTransformInfo previousTransformInfo { get; private set; }
    public CarTransformInfo currentTransformInfo { get; private set; }

    private float interpolationTime; // ¬рем€ интерпол€ции между состо€ни€ми
    private float timeSinceLastUpdate; // ¬рем€ с момента получени€ последнего состо€ни€

    void Update()
    {
        if (currentTransformInfo != null && previousTransformInfo != null)
        {
            // ”величиваем счетчик времени
            timeSinceLastUpdate += Time.deltaTime;

            // ¬ычисл€ем коэффициент интерпол€ции (от 0 до 1)
            float lerpFactor = timeSinceLastUpdate / interpolationTime;
            lerpFactor = Mathf.Clamp01(lerpFactor); // ќграничиваем значение в диапазоне [0, 1]

            // »нтерпол€ци€ позиции
            var pos = Vector3.Lerp(
                new Vector3(previousTransformInfo.position.x, previousTransformInfo.position.y, previousTransformInfo.position.z),
                new Vector3(currentTransformInfo.position.x, currentTransformInfo.position.y, currentTransformInfo.position.z),
                lerpFactor
            );
            transform.position = pos;

            // »нтерпол€ци€ поворота
            var rot = Quaternion.Slerp(
                new Quaternion(previousTransformInfo.rotation.x, previousTransformInfo.rotation.y, previousTransformInfo.rotation.z, previousTransformInfo.rotation.w),
                new Quaternion(currentTransformInfo.rotation.x, currentTransformInfo.rotation.y, currentTransformInfo.rotation.z, currentTransformInfo.rotation.w),
                lerpFactor
            );
            transform.rotation = rot;
        }
    }

    // ћетод дл€ обновлени€ состо€ни€ объекта
    public void UpdateTransformInfo(CarTransformInfo newTransformInfo)
    {
        if (currentTransformInfo == null)
        {
            // ≈сли это первое обновление, просто устанавливаем начальное состо€ние
            currentTransformInfo = newTransformInfo;
            timeSinceLastUpdate = 0f;
        }
        else
        {
            // ѕеремещаем текущее состо€ние в предыдущее
            previousTransformInfo = currentTransformInfo;
            currentTransformInfo = newTransformInfo;

            // —брасываем таймер
            timeSinceLastUpdate = 0f;

            // ”станавливаем врем€ интерпол€ции (например, 0.2 секунды - частота отправки данных)
            interpolationTime = 0.1f;
        }
    }
}
