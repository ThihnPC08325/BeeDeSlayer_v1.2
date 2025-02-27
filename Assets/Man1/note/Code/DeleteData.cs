using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteData : MonoBehaviour
{
    [SerializeField] private PasswordData passwordData;

    private void Start()
    {
        DeleteAllData();
    }
    private void DeleteAllData()
    {
        passwordData.collectedNotes.Clear();
        passwordData.passWord.Clear();
    }
}
