using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenScript : MonoBehaviour
{

    public Text GameOverText;
    public RawImage BackgroundImage;
    public float SpawnSpeed = 0.5f;
    private bool bSpawning = true;

    // Start is called before the first frame update
    void Start()
    {
        if(GameOverText)
        {
            GameOverText.color = new Color(GameOverText.color.r, GameOverText.color.g, GameOverText.color.b, 0.0f);
        }
        if(BackgroundImage)
        {
            BackgroundImage.color = new Color(BackgroundImage.color.r, BackgroundImage.color.g, BackgroundImage.color.b, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bSpawning)
        {
            Color CurrentTextColor = GameOverText.color;
            Color CurrentImageColor = BackgroundImage.color;
            if (CurrentTextColor.a < 1.0f && CurrentImageColor.a < 0.5f)
            {
                CurrentTextColor.a += SpawnSpeed * Time.deltaTime;
                CurrentImageColor.a += SpawnSpeed / 2 * Time.deltaTime;
                GameOverText.color = CurrentTextColor;
                BackgroundImage.color = CurrentImageColor;
            }
            else if (CurrentTextColor.a >= 1.0f)
            {
                CurrentTextColor.a = 1.0f;
                CurrentImageColor.a = 0.5f;
                GameOverText.color = CurrentTextColor;
                BackgroundImage.color = CurrentImageColor;
                bSpawning = false;
            }
        }
    }
}
