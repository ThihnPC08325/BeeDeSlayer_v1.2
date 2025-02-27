using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "PasswordData", menuName = "Game/PasswordData")]

public class PasswordData : ScriptableObject
{
    public List<string> collectedNotes = new List<string>(); // Danh sách note đã nhặt
    [SerializeField] public List<string> passWord;

}
