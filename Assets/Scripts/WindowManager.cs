using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour {
    public GameObject canvasElementPre;

    public GameObject startPanelPre;
    public GameObject gameUIPre;
    public GameObject battleUIPre;
    public GameObject discardUIPre;
    public GameObject draftUIPre;
    public GameObject lootDraftPre;
    public GameObject lootUIPre;
    public GameObject cardPackPre;
    public GameObject inverseDraftPre;
    public GameObject discardChoosePre;
    public GameObject soulRoomPre;
    public GameObject shopRoomPre;
    public static KeyValuePair<string, GameObject> startPanel;
    public static KeyValuePair<string, GameObject> battleUI;
    public static KeyValuePair<string, GameObject> discardUI;
    public static KeyValuePair<string, GameObject> draftUI;
    public static KeyValuePair<string, GameObject> lootDraft;
    public static KeyValuePair<string, GameObject> lootUI;
    public static KeyValuePair<string, GameObject> cardPack;
    public static KeyValuePair<string, GameObject> inverseDraft;
    public static KeyValuePair<string, GameObject> discardChoose;
    public static KeyValuePair<string, GameObject> soulRoom;
    public static KeyValuePair<string, GameObject> shopRoom;
    public static int UILayer = 5;
    public static int baseUILayer = 0;
    public static WindowManager instance;

    private static IDictionary<string, GameObject> overlays = new Dictionary<string, GameObject>();
    private GameObject canvasObject;
    void Awake() {
        WindowManager.startPanel = new KeyValuePair<string, GameObject>("StartPanel", startPanelPre);
        WindowManager.battleUI = new KeyValuePair<string, GameObject>("BattleUI", battleUIPre);
        WindowManager.discardUI = new KeyValuePair<string, GameObject>("DiscardUI", discardUIPre);
        WindowManager.draftUI = new KeyValuePair<string, GameObject>("DraftUI", draftUIPre);
        WindowManager.lootDraft = new KeyValuePair<string, GameObject>("LootDraft", lootDraftPre);
        WindowManager.lootUI = new KeyValuePair<string, GameObject>("LootUI", lootUIPre);
        WindowManager.cardPack = new KeyValuePair<string, GameObject>("CardPack", cardPackPre);
        WindowManager.inverseDraft = new KeyValuePair<string, GameObject>("InverseDraft", inverseDraftPre);
        WindowManager.discardChoose = new KeyValuePair<string, GameObject>("DiscardChoose", discardChoosePre);
        WindowManager.soulRoom = new KeyValuePair<string, GameObject>("SoulRoom", soulRoomPre);
        WindowManager.shopRoom = new KeyValuePair<string, GameObject>("ShopRoom", shopRoomPre);

        // Add Panel overlays to Dict
        WindowManager.overlays.Add(startPanel);
        // WindowManager.overlays.Add(battleUI);
        WindowManager.overlays.Add(discardUI);
        WindowManager.overlays.Add(draftUI);
        WindowManager.overlays.Add(lootDraft);
        WindowManager.overlays.Add(lootUI);
        WindowManager.overlays.Add(cardPack);
        WindowManager.overlays.Add(inverseDraft);
        WindowManager.overlays.Add(discardChoose);
        WindowManager.overlays.Add(soulRoom);
        WindowManager.overlays.Add(shopRoom);
        instance = this;
        GameObject canvasObject = Instantiate(WindowManager.instance.canvasElementPre, WindowManager.instance.transform);
        instance.canvasObject = canvasObject;
    }

    void Start() {
        WindowManager.ShowScreen(WindowManager.startPanel);
        AnimationQueue.Clear();
        SceneHelper.CloseAll();
        canvasObject.GetComponent<Canvas>().sortingLayerName = "UI";
        canvasObject.GetComponent<Canvas>().sortingOrder = WindowManager.UILayer;
    }

    public static GameObject ShowScreen(KeyValuePair<string, GameObject> screen) {
        GameObject screenObject = Instantiate(screen.Value, instance.canvasObject.transform);
        screenObject.transform.localScale = screenObject.transform.localScale;
        screenObject.transform.localPosition = screenObject.transform.localPosition + new Vector3(400f, 225f, 0f);

        screenObject.name = screen.Key;
        return screenObject;
    }

    public static void HideScreen(KeyValuePair<string, GameObject> screen) {
        Destroy(GameObject.Find(screen.Key));
    }

    public static void ClearOverlays() {
        foreach (KeyValuePair<string, GameObject> overlay in WindowManager.overlays) {
            WindowManager.HideScreen(overlay);
        }
    }

}
