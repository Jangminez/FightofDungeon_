using UnityEngine;

public class Potion : MonoBehaviour, IConsumable
{
    public enum PotionType {Hp, MP}
    public PotionType _type;
    public float _value;
    public void UseItem()
    {
        UISoundManager.Instance.PlayClickSound();
        
        switch(_type)
        {
            case PotionType.Hp:
                GameManager.Instance.player.Hp += _value;
                break;
            
            case PotionType.MP:
                GameManager.Instance.player.Mp += _value;
                break;
        }
    }
}
