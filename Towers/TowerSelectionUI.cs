using UnityEngine;

public class TowerSelectionUI : MonoBehaviour
{
    public TowerFactory towerFactory; // Reference to the TowerFactory
    public Camera mainCamera; // Reference to the main camera for mouse position tracking
    public string towerType; // Tower type to spawn
    public int towerIndex; // Index for the tower level
    public float snapRange = 1f; // Maximum distance to snap to a point

    private GameObject currentTowerInstance; // Reference to the tower being placed
    private bool isPlacingTower = false;
    private TowerController towerController; // Reference to the TowerController component
    private TowerSnapOffset towerSnapOffset; // Reference to the TowerSnapOffset component

    public bool IsPlacingTower => isPlacingTower; // Property to check if a tower is being placed

    public void OnButtonPress()
    {
        if (currentTowerInstance != null) {
            Destroy(currentTowerInstance);
        }

        // Spawn the tower at an initial off-screen position
        currentTowerInstance = towerFactory.SpawnTower(towerType, towerIndex, Vector3.zero);

        if (currentTowerInstance != null) {
            // Get the TowerController and TowerSnapOffset components
            towerController = currentTowerInstance.GetComponent<TowerController>();
            towerSnapOffset = currentTowerInstance.GetComponent<TowerSnapOffset>();

            if (towerController != null) {
                towerController.enabled = false;
            }

            SetTowerTransparency(currentTowerInstance, 0.5f); // Make it semi-transparent
            isPlacingTower = true;
        }
    }

    private void Update()
    {
        if (isPlacingTower && currentTowerInstance != null) {
            FollowMousePosition();

            // Place the tower when the left mouse button is clicked
            if (Input.GetMouseButtonDown(0)) {
                if (IsNearSnapPoint(currentTowerInstance.transform.position, out Vector3 snapPosition)) {
                    snapPosition += towerSnapOffset != null ? towerSnapOffset.snapOffset : Vector3.zero; // Apply offset
                    currentTowerInstance.transform.position = snapPosition; // Snap to the closest point with offset
                    PlaceTower();
                }
            }
        }
    }

    private void FollowMousePosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure the tower stays on the 2D plane

        // Check if the current position is within snap range of a point
        if (IsNearSnapPoint(mousePos, out Vector3 snapPosition)) {
            snapPosition += towerSnapOffset != null ? towerSnapOffset.snapOffset : Vector3.zero; // Apply offset
            currentTowerInstance.transform.position = snapPosition; // Snap to the closest point with offset
        } else {
            // If not near a snap point, follow the mouse position freely
            currentTowerInstance.transform.position = mousePos;
        }
    }

    private bool IsNearSnapPoint(Vector3 position, out Vector3 snapPosition)
    {
        snapPosition = Vector3.zero;
        float closestDistance = snapRange;
        bool foundSnapPoint = false;

        GameObject[] snapPoints = GameObject.FindGameObjectsWithTag("SnapPoint");

        foreach (GameObject point in snapPoints) {
            float distance = Vector3.Distance(position, point.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                snapPosition = point.transform.position;
                foundSnapPoint = true;
            }
        }

        return foundSnapPoint;
    }

    private void PlaceTower()
    {
        SetTowerTransparency(currentTowerInstance, 1f); // Set opacity to fully visible

        // Re-enable the TowerController component after placement
        if (towerController != null) {
            towerController.enabled = true;
        }

        isPlacingTower = false;
        currentTowerInstance = null; // Clear the reference after placement
    }

    private void SetTowerTransparency(GameObject tower, float alpha)
    {
        Renderer[] renderers = tower.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            foreach (Material material in renderer.materials) {
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }
    }
}
