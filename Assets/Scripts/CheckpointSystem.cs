#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 respawnPosition;
    private Rigidbody rigidbody;
    private BallMaterial playerBallMaterial;
    public Transform player;
    public GameObject heart3D; 
    private Vector3 originalHeartPosition; 

    //to delete PlayerPrefs if using the unity editor and not the build version
#if UNITY_EDITOR
    [InitializeOnLoad]
    public class ClearPlayerPrefsOnExitPlayMode
    {
        static ClearPlayerPrefsOnExitPlayMode()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        static void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Delete PlayerPrefs when exiting Play mode
                PlayerPrefs.DeleteAll();
            }
        }
    }
#endif
	
	private void OnSceneUnloaded(Scene scene)
	{
		PlayerPrefs.DeleteAll();
	}

    public void SaveBallMaterial()
    {
        if (playerBallMaterial != null)
        {
            // saves the path of the BallMaterial
            string path = "BallMaterials/" + playerBallMaterial.name;
            PlayerPrefs.SetString("BallMaterialPath", path);
            //PlayerPrefs.Save();

            Debug.Log("Ball material path saved: " + path);
        }
        else
        {
            Debug.LogWarning("No BallMaterial assigned to save.");
        }
    }

    public void LoadBallMaterial()
    {
        if (PlayerPrefs.HasKey("BallMaterialPath"))
        {
            string path = PlayerPrefs.GetString("BallMaterialPath");
            BallMaterial loadedMaterial = Resources.Load<BallMaterial>(path);

            if (loadedMaterial != null)
            {
                // playerBallMaterial = loadedMaterial;
                // sets the BallMaterial to the one in the prefs/the one that was just loaded
                rigidbody.GetComponent<MaterialController>().SetBallMaterial(loadedMaterial);
                Debug.Log("Ball material loaded: " + loadedMaterial.name);
            }
            else
            {
                Debug.LogError("Failed to load BallMaterial from path: " + path);
            }
        }
        else
        {
            Debug.LogWarning("No saved BallMaterial path found in PlayerPrefs.");
        }
    }

    /*    void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }*/

    void Start()
    {
        /*		// Load respawn position from PlayerPrefs
                float x = PlayerPrefs.GetFloat("RespawnX");
                float y = PlayerPrefs.GetFloat("RespawnY");
                float z = PlayerPrefs.GetFloat("RespawnZ");
                respawnPosition = new Vector3(x, y, z);*/

        // getting current rigidbody (for position)
        // and ballmaterial of player
        rigidbody = player.GetComponent<Rigidbody>();
        playerBallMaterial = rigidbody.GetComponent<MaterialController>().ballMaterial;

        float x = PlayerPrefs.GetFloat("RespawnX");
        float y = PlayerPrefs.GetFloat("RespawnY");
        float z = PlayerPrefs.GetFloat("RespawnZ");

        LoadBallMaterial();

        //if PlayerPrefs are not set, no checkpoint has been reached, no changes made to the scene reload
        //if they are player position is changed after scene has been reloaded

        if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY") && PlayerPrefs.HasKey("RespawnZ"))
        {
            respawnPosition = new Vector3(x, y, z);
            player.position = respawnPosition;
            Debug.Log($"Player respawned at: {respawnPosition}");
        }

        if (heart3D != null)
        {
            originalHeartPosition = heart3D.transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // reading the ball material of the player again
        playerBallMaterial = rigidbody.GetComponent<MaterialController>().ballMaterial;

        if (other.CompareTag("Checkpoint"))
        {
            respawnPosition = other.transform.position;

            // Save the new respawn position to PlayerPrefs
            PlayerPrefs.SetFloat("RespawnX", respawnPosition.x);
            PlayerPrefs.SetFloat("RespawnY", respawnPosition.y);
            PlayerPrefs.SetFloat("RespawnZ", respawnPosition.z);
            SaveBallMaterial();
            PlayerPrefs.Save();

            float x = PlayerPrefs.GetFloat("RespawnX");
            float y = PlayerPrefs.GetFloat("RespawnY");
            float z = PlayerPrefs.GetFloat("RespawnZ");

            Debug.Log("Checkpoint reached!");
            Debug.Log($"Player respawn point set to: {respawnPosition}");
            Debug.Log($"PlayerPrefs: {x}, {y}, {z}");
        }
    }

    public void RespawnPlayer()
    {
        LifeSystem lifeSystem = FindObjectOfType<LifeSystem>();

        if (lifeSystem != null)
        {
            if (LifeSystem.lives <= 1) //if last life is being lost
            {
                PlayerPrefs.DeleteAll(); // clear checkpoint data
                ResetHeart();
                LifeSystem.lives = lifeSystem.startLives; 
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
                return; //avoid checkpoint logic
            }
            else
            {
                lifeSystem.LoseLife(); 
            }
        }

        // else respawn at the last checkpoint
        if (respawnPosition != Vector3.zero)
        {
            player.position = respawnPosition; 
            rigidbody.velocity = Vector3.zero; 
        }
        else
        {
            // if no checkpoint is set, reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }



     private void ResetHeart()
    {
        if (heart3D != null)
        {
            heart3D.SetActive(true); 
            heart3D.transform.position = originalHeartPosition; 
        }

        // Remove the heart from the collected list
        if (LifeSystem.collectedHearts.Contains(heart3D.name))
        {
            LifeSystem.collectedHearts.Remove(heart3D.name);
        }
    }



    /*    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            float x = PlayerPrefs.GetFloat("RespawnX"); 
            float y = PlayerPrefs.GetFloat("RespawnY"); 
            float z = PlayerPrefs.GetFloat("RespawnZ");  

            //if PlayerPrefs are not set, no checkpoint has been reached, no changes made to the scene reload
            //if they are player position is changed after scene has been reloaded
            if (PlayerPrefs.HasKey("RespawnX") && PlayerPrefs.HasKey("RespawnY") && PlayerPrefs.HasKey("RespawnZ"))
            {
                respawnPosition = new Vector3(x, y, z);
                player.position = respawnPosition;
                Debug.Log($"Player respawned at: {respawnPosition}");
            }
        }*/
}
