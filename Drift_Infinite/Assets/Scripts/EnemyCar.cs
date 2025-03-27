using TMPro;
using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private TextMeshProUGUI userNane;
    public Renderer Renderer => renderer;
    public TextMeshProUGUI UserName => userNane;
    private float smoothSpeed = 3f; // —корость интерпол€ции


    public CarTransformInfo transformInfo { get; set; } = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transformInfo != null)
        {
            var pos = transformInfo.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x, pos.y, pos.z), smoothSpeed * Time.deltaTime);

            var rot = transformInfo.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(rot.x, rot.y, rot.z, rot.w), smoothSpeed * Time.deltaTime);
        }
    }
}

//public class EnemyCar : MonoBehaviour
//{
//    [SerializeField] private Renderer renderer;
//    [SerializeField] private TextMeshProUGUI userNane;
//    private Vector3 previousPosition;
//    private Vector3 targetPosition;
//    private Quaternion previousRotation;
//    private Quaternion targetRotation;
//    private float interpolationTime = 0f;
//    private float updateRate = 0.2f; // „астота обновлени€ от сервера (в секундах)

//    public Renderer Renderer => renderer;
//    public TextMeshProUGUI UserName => userNane;
//    void Start()
//    {
//        previousPosition = transform.position;
//        targetPosition = transform.position;
//        previousRotation = transform.rotation;
//        targetRotation = transform.rotation;
//    }

//    void Update()
//    {
//        if (interpolationTime < updateRate)
//        {
//            interpolationTime += Time.deltaTime;
//            float t = interpolationTime / updateRate; // Ќормализуем [0,1]

//            transform.position = Vector3.Lerp(previousPosition, targetPosition, t);
//            transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, t);
//        }
//    }

//    public void UpdateCarTransform(CarTransformInfo newTransform)
//    {
//        // —охран€ем старые позиции и назначаем новые
//        previousPosition = transform.position;
//        previousRotation = transform.rotation;

//        targetPosition = new Vector3(newTransform.position.x, newTransform.position.y, newTransform.position.z);
//        targetRotation = new Quaternion(newTransform.rotation.x, newTransform.rotation.y, newTransform.rotation.z, newTransform.rotation.w);

//        interpolationTime = 0f; // —брасываем интерпол€цию
//    }
//}
