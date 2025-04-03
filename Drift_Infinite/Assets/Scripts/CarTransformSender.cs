using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTransformSender : MonoBehaviour
{
    public void StartSendCarTransform(Transform carTransform) => StartCoroutine(SendCarTransform(carTransform));

    public void StopSendCarTransform() => StopAllCoroutines();

    private IEnumerator SendCarTransform(Transform carTransform)
    {
        while (carTransform != null)
        {
            var data = new CarTransformInfo
            {
                userId = GameManager.Instance.SelfId,
                position = new Position
                {
                    x = carTransform.position.x,
                    y = carTransform.position.y,
                    z = carTransform.position.z,
                },
                rotation = new Rotation
                {
                    x = carTransform.rotation.x,
                    y = carTransform.rotation.y,
                    z = carTransform.rotation.z,
                    w = carTransform.rotation.w
                }
            };
            MultiplayerController.sendCarTransform(GameManager.LobbyId, JsonUtility.ToJson(data));

            yield return new WaitForSeconds(0.2f);
        }
    }
}
