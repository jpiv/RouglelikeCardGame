using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Creator : MonoBehaviour {
	public GameObject cardPre;
    public GameObject buffPre;
    public GameObject battleRoomPre;
    public GameObject chestRoomPre;
    public GameObject inverseDraftRoomPre;
    public GameObject damageTextPre;
    public GameObject healTextPre;
    public GameObject healAnimationPre;
    public GameObject layoutElementPre;
    public GameObject lootTextPre;
    public GameObject soulRoomPre;
    public GameObject shopRoomPre;
    public GameObject dIDebuffPre;
    public Sprite mysteryRoomPre;
    public static GameObject LootText;
    public static GameObject BattleRoom;
    public static Sprite MysteryRoomSprite;
    public static GameObject ChestRoom;
    public static GameObject InverseDraftRoom;
    public static GameObject LayoutElement;
    public static GameObject SoulRoom;
    public static GameObject DIDebuff;
    public static GameObject ShopRoom;
	private static GameObject Card;
    private static GameObject Buff;
    private static GameObject DamageText;
    private static GameObject HealText;
    private static GameObject HealAnimation;

    void Awake() {
		Creator.Card = cardPre;
        Creator.Buff = buffPre;
        Creator.BattleRoom = battleRoomPre;
        Creator.ChestRoom = chestRoomPre;
        Creator.InverseDraftRoom = inverseDraftRoomPre;
        Creator.DamageText = damageTextPre;
        Creator.HealText = healTextPre;
        Creator.HealAnimation = healAnimationPre;
        Creator.LayoutElement = layoutElementPre;
        Creator.LootText = lootTextPre;
        Creator.SoulRoom = soulRoomPre;
        Creator.MysteryRoomSprite = mysteryRoomPre;
        Creator.DIDebuff = dIDebuffPre;
        Creator.ShopRoom = shopRoomPre;
    }

    public static GameObject CreateDIDebuff(string imageName, Transform transform) {
        Sprite buffImage = Resources.Load<Sprite>("BuffImages/" + imageName);
        GameObject buffObject = Instantiate(Creator.DIDebuff, transform);
        SpriteRenderer sr =  buffObject.GetComponent<SpriteRenderer>();
        sr.sprite = buffImage;
        return buffObject;
    }

    public static GameObject CreateRoom(GraphNode node, Transform transform) {
        GameObject prefab;
        switch (node.type) {
            case NodeTypes.CHEST:
                prefab = Creator.ChestRoom;
                break;
            case NodeTypes.INVERSE_DRAFT:
                prefab = Creator.InverseDraftRoom;
                break;
            case NodeTypes.SOUL:
                prefab = Creator.SoulRoom;
                break;
            case NodeTypes.SHOP:
                prefab = Creator.ShopRoom;
                break;
            default:
                prefab = Creator.BattleRoom;
                break;
        }
        GameObject instance = Instantiate(prefab, transform);
        MapIcon mapIcon = instance.GetComponent<MapIcon>();
        SpriteRenderer sr = mapIcon.GetComponent<SpriteRenderer>();
        if (node.hidden) {
            sr.sprite = Creator.MysteryRoomSprite;
        }
        sr.sortingOrder = 2;
        mapIcon.Init(node);

        return instance;
    }

    public static GameObject CreateLayoutElement(Transform transform) {
        return Instantiate(Creator.LayoutElement, transform);
    }

    public static GameObject CreateCard(Vector3 pos, Vector3 rotation, CardType cardType, Transform transform, int index = 0) {
        GameObject cardObject = Instantiate(Creator.Card, transform);
        cardObject.transform.localPosition = pos;
        cardObject.transform.rotation = Quaternion.Euler(rotation);
        Card card = cardObject.GetComponent<Card>();
        card.SetIndex(index);
        card.SetCard(cardType);
        card.ZIndex(card, index);
        return cardObject;
    }

    public static GameObject CreateArmor(string imageName, Vector3 pos, Vector3 scale, Transform parent) {
        GameObject armorImage = Resources.Load<GameObject>("ItemImages/" + imageName);
        GameObject armor = Instantiate(armorImage, parent);
        armor.transform.localPosition = pos;
        armor.transform.localScale = scale;
        armor.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        armor.GetComponent<SpriteRenderer>().sortingOrder = WindowManager.baseUILayer + 1;
        return armor;
    }

    public static GameObject CreateBuff(string imageName, Vector3 pos, Transform transform) {
        Sprite buffImage = Resources.Load<Sprite>("BuffImages/" + imageName);
        GameObject buffObject = Instantiate(Creator.Buff, transform);
        SpriteRenderer sr =  buffObject.GetComponent<SpriteRenderer>();
        sr.sprite = buffImage;
        return buffObject;
    }

    public static void CreateDamageText(float damage, bool left, Vector3 pos, Transform transform) {
        GameObject damageText = Instantiate(Creator.DamageText, transform);
        TextMeshPro textComponent = damageText.transform.Find("AnimationContainer/Text").GetComponent<TextMeshPro>();
        textComponent.text = damage.ToString();
        damageText.transform.localPosition = pos;
        Animator anim = damageText.GetComponent<Animator>();
        anim.SetBool("left", left);
    }

    public static void CreateHealText(float amount, bool left, Vector3 pos, Transform transform) {
        GameObject healText = Instantiate(Creator.HealText, transform);
        TextMeshPro textComponent = healText.transform.Find("AnimationContainer/Text").GetComponent<TextMeshPro>();
        textComponent.text = amount.ToString();
        healText.transform.localPosition = pos;
        Animator anim = healText.GetComponent<Animator>();
        anim.SetBool("left", left);
    }

    public static void CreateHealAnimation(Vector3 pos, Transform transform) {
        GameObject heal = Instantiate(Creator.HealAnimation, transform);
        heal.transform.localPosition = pos;
        heal.transform.localScale = new Vector3(0.6058745f, 1.663158f, 1.158995f);
    }

    public static GameObject CreateLootText(string text, Transform parent) {
       GameObject lootTextObject = Instantiate(Creator.LootText, parent); 
       lootTextObject.GetComponent<Text>().text = text;
       return lootTextObject;
    }
}
