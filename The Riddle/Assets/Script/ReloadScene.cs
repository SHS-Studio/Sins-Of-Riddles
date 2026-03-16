using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
   public PlayerHealth HP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(reloadscene());
       
    }
    IEnumerator reloadscene()
    {
        if(HP.isDead || HP == null)
        {
            yield return new WaitForSeconds(1.5f); // delay for death animation or effect

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }

}
