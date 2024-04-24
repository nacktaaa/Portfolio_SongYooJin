using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private string sceneID;
    [SerializeField]
    private CharacterSpriteDB spriteDB;
    [SerializeField]
    private Speaker[] speakers; // 대화에 참여하는 캐릭터들의 UI 배열
    private DialogData[] dialogs;   // 현재 분기의 대사 목록 배열
    [SerializeField]
    Image BGImage;
    [SerializeField]
    Image skipButton;
    [SerializeField]
    int curDialogIdx = -1;  // 현재 대사 순번
    int curSpeakerIdx = 0;     // 현재 화자 순번
    float typingSpeed = 0.05f;
    bool isTypingEffect = false;


    public void StartDialog(string curChapter, string sceneID) 
    {
        // 씬ID 찾기
        // 챕터1을 처음 시작했을 경우, Intro
        // 씬ID 에 따른 대화 목록 들고 있기
        this.sceneID = sceneID;
        if(!DataManager.Instance.dialogDict.ContainsKey(curChapter))
            return;

        if(DataManager.Instance.dialogDict[curChapter].Where(d => d.SceneID == sceneID).Count() == 0)
            return;
        else
            dialogs = new DialogData[DataManager.Instance.dialogDict[curChapter].Where(d => d.SceneID == sceneID).Count()];

        GameManager.Instance.isDialog = true;

        int idx = 0;
        for(int i = 0; i < DataManager.Instance.dialogDict[curChapter].Count; ++i)
        {
            if(DataManager.Instance.dialogDict[curChapter][i].SceneID == sceneID)
            {
                dialogs[idx].speakerName = DataManager.Instance.dialogDict[curChapter][i].Name;
                dialogs[idx].dialogue = DataManager.Instance.dialogDict[curChapter][i].Dialog;
                dialogs[idx].speakerIdx = DataManager.Instance.dialogDict[curChapter][i].SpeakerIdx;
                foreach(var s in spriteDB.speakerImages)
                {
                    if(s.name == DataManager.Instance.dialogDict[curChapter][i].ImageName)
                        dialogs[idx].speakerImage = s;
                }
                idx ++;
            }
        }
        SetUp();
        StartCoroutine("StartDialogCoroutine");

    }

    private IEnumerator StartDialogCoroutine()
    {
        GameManager.Instance.Pause();
        SetNextDialog();
        yield return new WaitUntil(()=>UpdateDialog());
    }

    private void SetUp()
    {
        // 모든 대화 관련 오브젝트 활성화
        BGImage.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);

        for(int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], true);
            if(speakers[i].speakerImage != null)
            {
                speakers[i].speakerImage.gameObject.SetActive(true);
                Color color = speakers[i].speakerImage.color;
                color.a = 0f;
                speakers[i].speakerImage.color = color;
            }
        }
    }

    public void EndDialog()
    {
        GameManager.Instance.isDialog = false;
        StopAllCoroutines();
        BGImage.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        for(int i = 0; i < speakers.Length; ++ i)
        {
            SetActiveObjects(speakers[i], false);
            speakers[i].nameText.text = "";
            speakers[i].dialogueText.text = "";
            if(speakers[i].speakerImage != null)
            {
                speakers[i].speakerImage.sprite = null;
                speakers[i].speakerImage.gameObject.SetActive(false);
            }
        }
        curDialogIdx = -1;
        curSpeakerIdx = 0;
        GameManager.Instance.Resume();
    }

    private void SetActiveObjects(Speaker speaker, bool on)
    {
        speaker.dialogBox.gameObject.SetActive(on);
        speaker.nameText.gameObject.SetActive(on);
        speaker.dialogueText.gameObject.SetActive(on);

        // 화살표는 대사가 종료되었을 때만 활성화하므로 항상 false
        speaker.objectArrow.SetActive(false);

        // 캐릭터 알파값 변경
        if(speaker.speakerImage != null)
        {
            Color color = speaker.speakerImage.color;
            color.a = on == true ? 1 : 0.3f;
            speaker.speakerImage.color = color;
        }
    }

    public bool UpdateDialog()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // 텍스트 타이핑 효과가 재생 중일 때 클릭하면 효과 종료
            if(isTypingEffect)
            {
                isTypingEffect = false;

                StopCoroutine("OnTypingText");
                speakers[curSpeakerIdx].dialogueText.text = dialogs[curDialogIdx].dialogue;
                // 대사 완료 시 출력되는 커서 활성화
                speakers[curSpeakerIdx].objectArrow.SetActive(true);
                return false;
            }
            // 대사가 남아있을 경우 다음 대사 진행
            if(dialogs.Length > curDialogIdx + 1)
            {
                SetNextDialog();
            }
            else // 남은 대사가 없을 경우, 모든 오브젝트 비활성화 후 true 반환
            {
                // 현재 대화에 참여했던 모든 캐릭터, 대화관련 UI를 비활성화
                EndDialog();
                return true;
            }
        }
        return false;
    }


    private void SetNextDialog()
    {
        // 이전 화자의 대화 관련 오브젝트 비활성화
        SetActiveObjects(speakers[curSpeakerIdx], false);
        if(curSpeakerIdx == 3)
            ZeroAlphaImage(speakers[curSpeakerIdx].speakerImage);

        // 다음 대사 진행하도록
        curDialogIdx ++;
        curSpeakerIdx = dialogs[curDialogIdx].speakerIdx;

        // 현재 화자 이름 텍스트 설정
        speakers[curSpeakerIdx].nameText.text = dialogs[curDialogIdx].speakerName;
        
        if(dialogs[curDialogIdx].speakerImage != null)
            speakers[curSpeakerIdx].speakerImage.sprite = dialogs[curDialogIdx].speakerImage;
        
        // 현재 화자의 대화 관련 오브젝트 활성화
        SetActiveObjects(speakers[curSpeakerIdx], true);
        if(curSpeakerIdx == 3)
        {
            ZeroAlphaImage(speakers[1].speakerImage);
            ZeroAlphaImage(speakers[2].speakerImage);
        }
        // 현재 화자 대사 텍스트 설정 (+ 타이핑 효과)
        StartCoroutine("OnTypingText");
    }
    private void ZeroAlphaImage(Image target)
    {
        Color color = target.color;
        color.a = 0;
        target.color = color;
    }
    IEnumerator OnTypingText()
    {
        int idx = 0;
        isTypingEffect = true;

        while(idx < dialogs[curDialogIdx].dialogue.Length)
        {
            speakers[curSpeakerIdx].dialogueText.text = dialogs[curDialogIdx].dialogue.Substring(0, idx);
            idx ++;

            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTypingEffect = false;

        speakers[curSpeakerIdx].objectArrow.SetActive(true);
    }
}


[System.Serializable]
public struct Speaker   // UI 상에 노출되는 정보
{
    public Image dialogBox;
    public Image speakerImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject objectArrow;
}

[System.Serializable]
public struct DialogData    // 실제 대사 데이터
{
    public int speakerIdx;
    public string speakerName;
    public Sprite speakerImage;
    [TextArea(3,5)]
    public string dialogue;
}