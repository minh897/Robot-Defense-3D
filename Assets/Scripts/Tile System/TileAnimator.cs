using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileAnimator : MonoBehaviour
{
    [SerializeField] private float defaultMoveDuration = .1f;

    [Header("Build Slot Movement")]
    [SerializeField] private float buildSlotYOffset = .25f;

    [Header("Grid Animation Details")]
    [SerializeField] private float tileMoveDuration = .1f;
    [SerializeField] private float tileDelay = .1f;
    [SerializeField] private float moveYOffset = 5;
    [Space]

    [SerializeField] private GridBuilder mainSceneGrid;
    [SerializeField] private List<GameObject> mainMenuObjects = new();

    private bool isGridMoving;
    private Coroutine currentActiveCo;

    void Start()
    {
        if (GameManager.instance.IsTestingLevel())
            return;

        CollectMainSceneObjects();
        ShowCurrentGrid(mainSceneGrid, true);
    }

    public void MoveTile(Transform objToMove, Vector3 targetPosition, float? newDuration = null)
    {
        float duration = newDuration ?? defaultMoveDuration;
        StartCoroutine(MoveTileCo(objToMove, targetPosition, duration));
    }

    private void ApplyOffset(List<GameObject> objectsToMove, Vector3 offset)
    {
        foreach (var obj in objectsToMove)
        {
            obj.transform.position += offset;
        }
    }

    public void ShowCurrentGrid(GridBuilder gridToMove, bool isGridShow)
    {
        List<GameObject> objectsToMove = GetObjectsToMove(gridToMove, isGridShow);

        // Only apply offset on the first time the grid was loaded
        // Subsequence times will be ignored
        if (gridToMove.IsOnFirstLoad())
            ApplyOffset(objectsToMove, new Vector3(0, -moveYOffset, 0));

        float newOffset = isGridShow ? moveYOffset : -moveYOffset;

        gridToMove.MakeTileNonInteractable(true);
        currentActiveCo = StartCoroutine(MoveGridCo(objectsToMove, newOffset));
    }

    public void ShowMainGrid(bool isMainGridShow)
    {
        if (mainSceneGrid != null)
            ShowCurrentGrid(mainSceneGrid, isMainGridShow);
    }

    public void CollectMainSceneObjects()
    {
        mainMenuObjects.AddRange(mainSceneGrid.GetTileSetup());
        mainMenuObjects.AddRange(CollectExtraObject());
    }

    public void EnableMainSceneObjects(bool isEnable)
    {
        foreach (var obj in mainMenuObjects)
        {
            obj.SetActive(isEnable);
        }
    }

    // Return a list of all extra object in the scene
    private List<GameObject> CollectExtraObject()
    {
        List<GameObject> extraObjects = new();

        // Find all objects in a scene, get their game object then add them to extraObjects list
        extraObjects.AddRange(FindObjectsByType<EnemyPortal>(FindObjectsSortMode.None).Select(component => component.gameObject));
        extraObjects.AddRange(FindObjectsByType<PlayerCastle>(FindObjectsSortMode.None).Select(component => component.gameObject));

        return extraObjects;
    }

    private List<GameObject> GetObjectsToMove(GridBuilder gridToMove, bool isStartWithTiles)
    {
        List<GameObject> objectsToMove = new();
        List<GameObject> extraObjects = CollectExtraObject();

        // If BringUpMainGrid is true, then add all the tiles to move them first
        // else, add extra objects in order to move them first
        if (isStartWithTiles)
        {
            objectsToMove.AddRange(gridToMove.GetTileSetup());
            objectsToMove.AddRange(extraObjects);
        }
        else
        {
            objectsToMove.AddRange(extraObjects);
            objectsToMove.AddRange(gridToMove.GetTileSetup());
        }

        return objectsToMove;
    }

    public IEnumerator MoveTileCo(Transform objToMove, Vector3 targetPosition, float? newDuration = null)
    {
        float time = 0;
        float duration = newDuration ?? defaultMoveDuration;

        Vector3 startPosition = objToMove.position;

        while (time < duration)
        {
            if (objToMove == null)
                break;

            objToMove.position = Vector3.Lerp(startPosition, targetPosition, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        if (objToMove != null)
            objToMove.position = targetPosition;
    }

    private IEnumerator MoveGridCo(List<GameObject> objectsToMove, float yOffset)
    {
        isGridMoving = true;

        foreach (var tile in objectsToMove)
        {
            if (tile.TryGetComponent<TileSlot>(out var tileSlot))
                tileSlot.MakeNonInteractable(false);
        }

        for (int i = 0; i < objectsToMove.Count; i++)
        {
            yield return new WaitForSeconds(tileDelay);

            // Continue the loop when the all the tiles has been moved
            // Move other game objects like Player Castle and Enemy Portal
            if (objectsToMove[i] == null)
                continue;

            Transform tile = objectsToMove[i].transform;
            Vector3 targetPosition = tile.position + new Vector3(0, yOffset, 0);
            MoveTile(tile, targetPosition, tileMoveDuration);
        }

        isGridMoving = false;
    }

    public bool IsGridMoving() => isGridMoving;

    public float GetBuildOffset() => buildSlotYOffset;

    public float GetYTravelDuration() => defaultMoveDuration;

    public Coroutine GetCurrentActiveRoutine() => currentActiveCo;
}
