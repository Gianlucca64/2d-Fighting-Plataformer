using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fox_Save : MonoBehaviour
{
    [SerializeField] TMP_Text saveWarning;
    //Guardar la posición del personaje del jugador
    public void SavePosition(Vector3 playerPos)
    {
        // Guardar la posición del personaje del jugador en todos los ejes en diferentes espacios de PlayerPrefs)
        PlayerPrefs.SetFloat("posX", playerPos.x);
        PlayerPrefs.SetFloat("posY", playerPos.y);
        PlayerPrefs.SetFloat("posZ", playerPos.z);
        // Guardando los datos
        PlayerPrefs.Save();
        saveWarning.text = "Posición guardada";
        Invoke("DeleteText", 2f);
    }
    public void DeleteText()
    {
        saveWarning.text = "";
    }
    private void OnTriggerEnter(Collider other)
    {
        // Si el trigger del portal se cruzó con el objeto con la etiqueta Player, entonces
        if (other.CompareTag("Player"))
        {
            // Obtener la posición del objeto y pasarlo al método SavePosition
            Vector3 pos = other.transform.position;
            SavePosition(pos);
        }
    }
}
