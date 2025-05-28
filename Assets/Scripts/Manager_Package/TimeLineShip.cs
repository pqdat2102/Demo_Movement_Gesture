using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeLineShip : MonoBehaviour
{
    [SerializeField] float delayTime = 0.0f;
    [SerializeField] List<GameObject> desList;
    [SerializeField] List<float> speedTimePos;
    [SerializeField] List<float> speedTimeRot;
    [SerializeField] GameObject target;
    [SerializeField] UnityEvent endTimeLine;
    int id;
    bool isEndTimeLine = false;

    private void Start()
    {
        target.transform.position = desList[0].transform.position;
        id = 1;
    }

    private void Update()
    {
        if (isEndTimeLine) return; 

        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            return;
        }

        float dis = ((target.transform.position - desList[id].transform.position).magnitude);
        target.transform.position = Vector3.MoveTowards(target.transform.position, desList[id].transform.position, speedTimePos[id] );
        target.transform.rotation = Quaternion.Lerp(target.transform.rotation, desList[id].transform.rotation, speedTimeRot[id] * Time.deltaTime);
        if (dis <= 8.0f)
        {
            id++;
            if (id == desList.Count)
            {
                isEndTimeLine = true;
            }
        }

        if (isEndTimeLine) endTimeLine.Invoke();
    }
}
