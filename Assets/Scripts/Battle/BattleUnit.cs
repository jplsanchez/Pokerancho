using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    Image _image;
    Vector3 _originalPosition;
    Color _originalColor;

    public BattleHud Hud => hud;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _originalPosition = _image.transform.localPosition;
        _originalColor = _image.color;
    }

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit => isPlayerUnit;

    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        if (isPlayerUnit) _image.sprite = Pokemon.BackSprite;
        else _image.sprite = Pokemon.FrontSprite;

        _image.color = _originalColor;

        hud.SetData(Pokemon);

        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit) _image.transform.localPosition = new Vector3(-550f, _originalPosition.y);
        else _image.transform.localPosition = new Vector3(550f, _originalPosition.y);

        _image.transform.DOLocalMoveX(_originalPosition.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();

        if (isPlayerUnit) sequence.Append(_image.transform.DOLocalMoveX(_originalPosition.x + 50f, 0.25f));
        else sequence.Append(_image.transform.DOLocalMoveX(_originalPosition.x - 50f, 0.25f));
        
        sequence.Append(_image.transform.DOLocalMoveX(_originalPosition.x, 0.25f));
    }
    
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_image.DOColor(Color.gray, 0.1f));
        sequence.Append(_image.DOColor(_originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_image.transform.DOLocalMoveY(_originalPosition.y - 150f, 0.5f))    ;
        sequence.Join(_image.DOFade(0f, 0.5f));
    }
}
