using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    private static MenuMusicManager instance;

    void Awake()
    {
        // Para hindi ma-duplicate ang music kapag bumalik sa menu
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
    }
}