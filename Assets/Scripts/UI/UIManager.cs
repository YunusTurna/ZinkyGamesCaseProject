using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Settings")]
    [SerializeField] private float _menuFadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    public void FadePanelIn(CanvasGroup cg)
    {
        if (cg == null) return; 

        cg.gameObject.SetActive(true);
        cg.alpha = 0f; 

        cg.DOKill(); 
        cg.DOFade(1f, _menuFadeDuration)
          .SetUpdate(true); 

        cg.blocksRaycasts = true;
        cg.interactable = true;
    }
    public void FadePanelOut(CanvasGroup cg)
    {
        if (cg == null) return;

        cg.blocksRaycasts = false;
        cg.interactable = false;

        cg.DOKill();
        cg.DOFade(0f, _menuFadeDuration)
          .SetUpdate(true)
          .OnComplete(() =>
          {
              cg.gameObject.SetActive(false);
          });
    }
}