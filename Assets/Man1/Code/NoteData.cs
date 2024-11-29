using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NoteData", menuName = "Game/NoteData")]
public class NoteData : ScriptableObject
{
    [SerializeField]
    private HashSet<string> collectedNotes = new HashSet<string>();

    public bool IsNoteCollected(string noteID)
    {
        return collectedNotes.Contains(noteID);
    }

    public void CollectNote(string noteID)
    {
        if (!collectedNotes.Contains(noteID))
        {
            collectedNotes.Add(noteID);
            Debug.Log("Note collected: " + noteID);
        }
    }

    public void ResetNotes()
    {
        collectedNotes.Clear();
        Debug.Log("All notes have been reset.");
    }
}
