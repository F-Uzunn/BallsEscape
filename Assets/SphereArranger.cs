using UnityEngine;
using DG.Tweening;

public class SphereArranger : MonoBehaviour
{
    [field: SerializeField] private GameData gamedata;
    [field: SerializeField] private GameManager gameManager;
    [Space(10)]
    [field: SerializeField] private int rowCount;
    [field: SerializeField] private int columnCount;
    [field: SerializeField] private int depthCount;
    [Space(10)]
    [field: SerializeField] private GameObject box;

    private void OnEnable()
    {
        EventManager.AddHandler(GameEvent.OnBallArrange, OnBallArrange);
    }
    private void OnDisable()
    {
        EventManager.RemoveHandler(GameEvent.OnBallArrange, OnBallArrange);
    }
  
    private void OnBallArrange()
    {
        Bounds cubeBounds = transform.GetComponent<Renderer>().bounds;

        float cubeWidth = cubeBounds.size.x;
        float cubeHeight = cubeBounds.size.y;
        float cubeDepth = cubeBounds.size.z;

        float sphereDiameter = cubeWidth / columnCount;

        for (int i = 0; i < gameManager.TotalBallCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                for (int k = 0; k < depthCount; k++)
                {
                    if (box.transform.childCount == 7)
                        return;

                    float x = transform.position.x - cubeWidth / 2 + (sphereDiameter * j);
                    float y = transform.position.y - cubeHeight / 2 + (sphereDiameter * i);
                    float z = transform.position.z - cubeDepth / 2 + (sphereDiameter * k);

                    Vector3 spherePosition = new Vector3(x + 0.3f, y + 0.3f, z + 0.3f);

                    GameObject sphere = box.transform.GetChild(7).gameObject;
                    sphere.GetComponent<Rigidbody>().isKinematic = true;
                    sphere.transform.localScale = new Vector3(sphereDiameter, sphereDiameter, sphereDiameter);
                    sphere.transform.parent = transform;
                    sphere.transform.DOJump(spherePosition, 2f, 1, 0.5f);
                }
            }
        }
    }
}
