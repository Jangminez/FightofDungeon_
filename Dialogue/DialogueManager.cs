using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Transform dialogueBox;
    Queue<string> messages;
    public Text dialogueText;
    public PlayableDirector director;
    public float nextTime;
    private bool isTyping = false;
    private string message;
    void Awake()
    {
        messages = new Queue<string>();
    }

    // 대화 출력 부분
    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("대화 출력");

        messages.Clear();

        foreach (var message in dialogue.messages)
        {
            // 출력할 내용들 큐에 추가
            messages.Enqueue(message);
        }

        DisplayNextSentence();
    }

    // 다음 대화 출력을 위함 Queue가 비어있다면 대화 종료
    public void DisplayNextSentence()
    {
        // 현재 다이얼로그가 출력 중일 때 다음으로 넘어가려 한다면 한번에 모든 메시지 출력
        if (isTyping)
        {
            StopAllCoroutines();
            TypeAllMessage(message);
            return;
        }

        if (messages.Count == 0)
        {
            EndDialogue();
            return;
        }

        // 출력할 내용 큐에서 꺼내기
        message = messages.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeMessage(message));
        isTyping = true;
    }



    // 문자 하나하나 출력
    IEnumerator TypeMessage(string message)
    {
        dialogueText.text = "";

        foreach (var ch in message.ToCharArray())
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
    }

    public void TypeAllMessage(string message)
    {
        UISoundManager.Instance.PlayClickSound();
        dialogueText.text = message;
        isTyping = false;
    }

    // 대화 종료
    void EndDialogue()
    {
        if (nextTime != 0)
        {
            UISoundManager.Instance.PlayExitSound();
            
            director.Stop();
            director.time = nextTime;
            director.Play();
            dialogueBox.gameObject.SetActive(false);
        }
    }
}
