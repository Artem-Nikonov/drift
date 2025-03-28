using TMPro;
using UnityEngine;

//public class EnemyCar : MonoBehaviour
//{
//    private CarTransformInfo transformInfo = null;
//    private CarTransformInfo previousTransformInfo = null;
//    private float lerpTime = 0.2f; // ����� ������������ (������ ��������� � �������� ���������� �������)
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
//                // ���� ��� ������ ����������, ����� ������������� �������
//                previousTransformInfo = transformInfo;
//                transform.position = new Vector3(transformInfo.position.x, transformInfo.position.y, transformInfo.position.z);
//                transform.rotation = new Quaternion(transformInfo.rotation.x, transformInfo.rotation.y, transformInfo.rotation.z, transformInfo.rotation.w);
//            }
//            else
//            {
//                // ������������ ����� ���������� � ������� ��������
//                currentLerpTime += Time.deltaTime;
//                float t = Mathf.Clamp01(currentLerpTime / lerpTime);

//                // ������������ �������
//                Vector3 targetPos = new Vector3(transformInfo.position.x, transformInfo.position.y, transformInfo.position.z);
//                Vector3 prevPos = new Vector3(previousTransformInfo.position.x, previousTransformInfo.position.y, previousTransformInfo.position.z);
//                transform.position = Vector3.Lerp(prevPos, targetPos, t);

//                // ������������ ��������
//                Quaternion targetRot = new Quaternion(transformInfo.rotation.x, transformInfo.rotation.y, transformInfo.rotation.z, transformInfo.rotation.w);
//                Quaternion prevRot = new Quaternion(previousTransformInfo.rotation.x, previousTransformInfo.rotation.y, previousTransformInfo.rotation.z, previousTransformInfo.rotation.w);
//                transform.rotation = Quaternion.Slerp(prevRot, targetRot, t);

//                // ���� ������������ ��������� � ������ ����� ������, ���������
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
//    private float smoothSpeed = 1f; // �������� ������������


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
    private float smoothSpeed = 3f; // �������� ������������

    public CarTransformInfo previousTransformInfo { get; private set; }
    public CarTransformInfo currentTransformInfo { get; private set; }

    private float interpolationTime; // ����� ������������ ����� �����������
    private float timeSinceLastUpdate; // ����� � ������� ��������� ���������� ���������

    void Update()
    {
        if (currentTransformInfo != null && previousTransformInfo != null)
        {
            // ����������� ������� �������
            timeSinceLastUpdate += Time.deltaTime;

            // ��������� ����������� ������������ (�� 0 �� 1)
            float lerpFactor = timeSinceLastUpdate / interpolationTime;
            lerpFactor = Mathf.Clamp01(lerpFactor); // ������������ �������� � ��������� [0, 1]

            // ������������ �������
            var pos = Vector3.Lerp(
                new Vector3(previousTransformInfo.position.x, previousTransformInfo.position.y, previousTransformInfo.position.z),
                new Vector3(currentTransformInfo.position.x, currentTransformInfo.position.y, currentTransformInfo.position.z),
                lerpFactor
            );
            transform.position = pos;

            // ������������ ��������
            var rot = Quaternion.Slerp(
                new Quaternion(previousTransformInfo.rotation.x, previousTransformInfo.rotation.y, previousTransformInfo.rotation.z, previousTransformInfo.rotation.w),
                new Quaternion(currentTransformInfo.rotation.x, currentTransformInfo.rotation.y, currentTransformInfo.rotation.z, currentTransformInfo.rotation.w),
                lerpFactor
            );
            transform.rotation = rot;
        }
    }

    // ����� ��� ���������� ��������� �������
    public void UpdateTransformInfo(CarTransformInfo newTransformInfo)
    {
        if (currentTransformInfo == null)
        {
            // ���� ��� ������ ����������, ������ ������������� ��������� ���������
            currentTransformInfo = newTransformInfo;
            timeSinceLastUpdate = 0f;
        }
        else
        {
            // ���������� ������� ��������� � ����������
            previousTransformInfo = currentTransformInfo;
            currentTransformInfo = newTransformInfo;

            // ���������� ������
            timeSinceLastUpdate = 0f;

            // ������������� ����� ������������ (��������, 0.2 ������� - ������� �������� ������)
            interpolationTime = 0.1f;
        }
    }
}
