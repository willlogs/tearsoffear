using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MultiplayerTools : MonoBehaviour
{
    // if it's a player => PlayerController if not => PredatorController
    public Controller myController;

    // Lists of Objects
    public List<GameObject> dummies = new List<GameObject>();
    public List<TransformData> transformData = new List<TransformData>();

    // Prefabs and SpawnPoses
    public GameObject playerPrefab, dummyPrefab, predatorPref, dummyPredatorPref;
    public PositionKeeper spawnPositions;
    public Transform predSpawnPos;

    public float smoothMoveDuration = 0.1f;
    public float maxErr = 0.5f;

    public void UpdateTransforms(bool isCli, int conIndex)
    {
        int i = 0;
        foreach (TransformData td in transformData)
        {
            bool shouldSkip = (isCli && i == conIndex + 1) || (!isCli && i == 0);

            if (td.isSet)
            {
                Vector3 diff = td.position - dummies[i].transform.position;

                if (!shouldSkip)
                {
                    dummies[i].transform.rotation = td.rotation;
                    if (td.hasTween)
                    {
                        td.tween.Kill();
                    }

                    td.tween = dummies[i].transform.DOMove(td.position, smoothMoveDuration);
                    td.hasTween = true;

                    // TODO: optimize this with a list of dummies
                    dummies[i].GetComponent<DummyAnimations>().sfx.Walk();
                }

                diff = td.position - dummies[i].transform.position;

                if (diff.magnitude < maxErr)
                    td.isSet = false;
            }

            i++;
        }
    }

    public void AddDummy(int input)
    {
        int positionIndex = dummies.Count;
        GameObject temp = Instantiate(dummyPrefab, spawnPositions.poses[positionIndex].position, Quaternion.identity);
        dummies.Add(temp);
        temp.AddComponent<Dummy>().index = dummies.Count - 1;
        transformData.Add(new TransformData());
    }

    public void AddDummyMonster(int input)
    {
        GameObject temp = Instantiate(dummyPredatorPref, predSpawnPos.position, Quaternion.identity);
        dummies.Add(temp);
        temp.AddComponent<Dummy>().index = dummies.Count - 1;
        transformData.Add(new TransformData());
    }

    public void AddPlayer(int input)
    {
        GameObject temp = Instantiate(playerPrefab, spawnPositions.poses[dummies.Count].position, Quaternion.identity);
        dummies.Add(temp);
        myController = temp.GetComponent<Controller>();
        transformData.Add(new TransformData());
    }

    public void AddMonster(int input)
    {
        GameObject temp = Instantiate(predatorPref, predSpawnPos.position, Quaternion.identity);
        dummies.Add(temp);
        myController = temp.GetComponent<Controller>();
        transformData.Add(new TransformData());
    }
}
