using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public float nextTime;
    
    // 다이얼로그 출력 시작
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().nextTime = nextTime;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
