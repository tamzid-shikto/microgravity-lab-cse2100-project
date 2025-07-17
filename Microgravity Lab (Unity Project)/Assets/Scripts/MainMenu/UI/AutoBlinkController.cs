using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoBlinkController : MonoBehaviour
{
    private void Start()
    {
        Blink();
    }

    public void Blink()
    {
        gameObject.SetActive(false);
        GameUtility.DoAtNextFrame(() =>
        {
            gameObject.SetActive(true);
            GameUtility.DoAtNextFrame(() =>
            {
                gameObject.SetActive(false);
                GameUtility.DoAtNextFrame(() =>
                {
                    gameObject.SetActive(true);
                });
            });
        });
    }
    public void BlinkSlow()
    {
        gameObject.SetActive(false);
        GameUtility.DoAfterMilliseconds(250, () =>
        {
            gameObject.SetActive(true);
            GameUtility.DoAfterMilliseconds(250, () =>
            {
                gameObject.SetActive(false);
                GameUtility.DoAfterMilliseconds(250, () =>
                {
                    gameObject.SetActive(true);
                });
            });
        });
    }
    public void Blink2()
    {
        gameObject.SetActive(false);
        GameUtility.DoAtNextFrame(() =>
        {
            gameObject.SetActive(true);
            GameUtility.DoAtNextFrame(() =>
            {
                gameObject.SetActive(false);
                GameUtility.DoAtNextFrame(() =>
                {
                    gameObject.SetActive(true);
                    GameUtility.DoAtNextFrame(() =>
                    {
                        gameObject.SetActive(false);
                        GameUtility.DoAtNextFrame(() =>
                        {
                            gameObject.SetActive(true);
                            GameUtility.DoAtNextFrame(() =>
                            {
                                gameObject.SetActive(false);
                                GameUtility.DoAtNextFrame(() =>
                                {
                                    gameObject.SetActive(true);
                                    GameUtility.DoAtNextFrame(() =>
                                    {
                                        gameObject.SetActive(false);
                                        GameUtility.DoAtNextFrame(() =>
                                        {
                                            gameObject.SetActive(true);

                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });
    }
}