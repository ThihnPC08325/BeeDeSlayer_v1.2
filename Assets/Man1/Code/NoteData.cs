using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NoteData", menuName = "Game/NoteData")]
public class NoteData : ScriptableObject
{
    [SerializeField]
    private readonly HashSet<string> _collectedNotes = new HashSet<string>();

    public bool IsNoteCollected(string noteID)
    {
        return _collectedNotes.Contains(noteID);
    }

    public void CollectNote(string noteID)
    {
        if (!_collectedNotes.Add(noteID)) return;
        Debug.Log("Note collected: " + noteID);
    }

    public void ResetNotes()
    {
        _collectedNotes.Clear();
        Debug.Log("All notes have been reset.");
    }
}
