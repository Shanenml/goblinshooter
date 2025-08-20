using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //handle to text
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;
    [SerializeField]
    private Slider _sprintBoostSlider;
    [SerializeField]
    private Image[] _livesImage;
        //0 = Heart Image 1
        //1 = Heart Image 2
        //2 = Heart Image 3

    void Start()
    {
        _scoreText.text = "Enemy's Slain: 0";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

    }

    void Update()
    {
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Enemy's Slain: " + score;
    }

    public void UpdateLives(int lives)
    {
        switch (lives)
        {
            case 0: //dead
                _livesImage[0].color = Color.black;
                _livesImage[1].color = Color.black;
                _livesImage[2].color = Color.black;

                _restartText.gameObject.SetActive(true);
                StartCoroutine(GameOverTextRoutine());

                break;
            case 1: //one life
                _livesImage[0].color = Color.white;
                _livesImage[1].color = Color.black;
                _livesImage[2].color = Color.black;
                break;
            case 2: //two lives
                _livesImage[0].color = Color.white;
                _livesImage[1].color = Color.white;
                _livesImage[2].color = Color.black;
                break;
            case 3: //full life
                _livesImage[0].color = Color.white;
                _livesImage[1].color = Color.white;
                _livesImage[2].color = Color.white;
                break;
            default:
                Debug.Log("Default Lives Value");
                break;

        }
    }

    public void UpdateSprintSlider(float sprintvalue)
    {
        _sprintBoostSlider.value = sprintvalue;
    }

    IEnumerator GameOverTextRoutine()
    {
        while(true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.0f);
        }
    }


}
