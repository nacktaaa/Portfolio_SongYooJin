using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 인벤토리UI에 사용될 슬롯 객체의 컴포넌트 
public class Slot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public bool isEmpty = true;         // 비어있는가?
    public bool isSelect = false;       // 슬롯이 선택되었는가?
    public Item item;                   // 슬롯에 들어있을 아이템 캐싱
    public GameObject equipIcon;        // 장착 표시를 위한 오브젝트 캐싱
    Image icon;                         // 슬롯에 표시할 아이템의 아이콘이미지 캐싱

    public Define.InvenType invenType;  // 슬롯이 소속된 인벤타입
    CanvasGroup group;                  // 슬롯 드래그 시 불투명 처리를 위한 캔버스 그룹 캐싱

    Color trasparentColor = new Color(0,0,0,0); // 아이콘이미지의 투명화를 위한 컬러 변수

    // 슬롯 초기화
    private void Awake()
    {
        icon = transform.GetChild(0).GetComponent<Image>();
        icon.color = trasparentColor;
        group = GetComponent<CanvasGroup>();
        equipIcon.SetActive(false);
    }

    // Item 데이터로 해당 Slot을 세팅
    public void SetSlot(Item _item)
    {
        this.item = _item;

        if(_item != null)
        {
            icon.sprite = _item.iconImage;
            icon.color = Color.white;
            isEmpty = false;

            if(_item is Weapon)
            {
                Weapon temp = (Weapon)_item;
                if(temp.isEquiped)
                    equipIcon.SetActive(true);
                else
                    equipIcon.SetActive(false);
            }
        }
    }

    // 슬롯 비우기
    public void ClearSlot()
    {
        item = null;
        icon.color = trasparentColor;
        equipIcon.SetActive(false);
        isEmpty = true;
    }

    // 드래그 시작 시 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!isEmpty) // 슬롯이 비어있지 않을 때만 실행
        {
            if(isSelect)
            {
                Managers.UI.HideInspectorUI();
                isSelect = false;
            }
            
            Managers.UI.dragSlot.CopySlot(this); // dragSlot 게임오브젝트에 이 Slot을 카피
            Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
            group.alpha = 0.4f; // 이 Slot은 불투명 처리
        }
    }

    // 드래그 중 계속 호출되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        if(!isEmpty) // 슬롯이 비어 있지 않을 때만 실행
        {
            Managers.UI.dragSlot.transform.position = eventData.position; // dragSlot을 마우스포인터를 따라 이동시킴
        }
    }

    // 드래그 종료 시 호출되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        Managers.UI.dragSlot.ClearDragSlot(); // dragSlot 비우기
        Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
        group.alpha = 1f; // 원본 알파값 돌려놓기   
    }

    // 자기자신 위에서 뭔가 드롭된다면 호출되는 함수
    public void OnDrop(PointerEventData eventData)
    {
        if(Managers.UI.dragSlot.slot != null) // dragSlot이 비어있지 않다면,
        {
            // 덮어쓸 아이템 전달하여 스왑
            Item temp = item;
            Slot otherSlot = Managers.UI.dragSlot.slot;

            if(this.invenType != otherSlot.invenType)
            {
                // 옮기기 가능 여부 먼저 체크
                if(this.invenType == Define.InvenType.World) // 여기가 스토리지슬롯인 경우
                {
                    if(temp != null)
                    {
                        if(Managers.Inven.selectedStorage.curWeight - temp.weight + otherSlot.item.weight 
                            > Managers.Inven.selectedStorage.maxWeight)
                        {
                            return;   
                        }
                    }
                    else
                    {
                        if(Managers.Inven.selectedStorage.curWeight + otherSlot.item.weight > Managers.Inven.selectedStorage.maxWeight)
                        {
                            return;
                        }
                    }
                }
                else // 여기가 플레이어슬롯인 경우
                {
                    if(temp != null)
                    {
                        if(Managers.Inven.selectedStorage.curWeight - otherSlot.item.weight + temp.weight > Managers.Inven.selectedStorage.maxWeight)
                        {
                            return;
                        }
                    }      
                }

                if(temp != null)
                {
                    CheckEquipedWeapon(item);
                    CheckEquipedWeapon(otherSlot.item);
                    Managers.Inven.SwapBetweenInvens(otherSlot, this);
                }
                else
                {
                    CheckEquipedWeapon(otherSlot.item);
                    Managers.Inven.AddItem(otherSlot.item, invenType);
                    Managers.Inven.RemoveItem(otherSlot.item, otherSlot.invenType);
                }
            }
            else
            {
                equipIcon.SetActive(false);
                SetSlot(otherSlot.item);
                if(temp != null)
                {
                    otherSlot.equipIcon.SetActive(false);
                    otherSlot.SetSlot(temp);
                }
                else
                {
                    otherSlot.ClearSlot();
                }
            }
        }
    }

    public void CheckEquipedWeapon(Item item)
    {
        Weapon tempWeapon = item as Weapon;
        if(tempWeapon == null)
            return;
        
        if(tempWeapon.isEquiped)
            Managers.Inven.TakeOffItem();
    }

    //슬롯 클릭 시 호출되는 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isEmpty)
        {
            if(eventData.clickCount >= 1)
            {
                isSelect = true;
                Managers.UI.slotOutline.SelectImage(this.transform.position);
                Managers.UI.ShowInspectorUI(this, invenType);
                // 클릭하여 인스펙터창 열기
            }
            Managers.Sound.PlayEffectSound(Define.EffectSound.Click);
        }
        else
        {
            // 인스펙터창 켜져있다면 해제 
            isSelect = false;
            Managers.UI.HideInspectorUI();
        }
    }
}
