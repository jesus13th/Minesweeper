using UnityEngine;
using UnityEngine.UI;

public class LogContainer : MonoBehaviour {
    [SerializeField] private Registro register;
    public Registro Register {
        set {
            register = value;
            difficultyTxt.text = register.Dificultad;
            timeTxt.text = register.Tiempo.ToString();
            dateTxt.text = register.Fecha;
        }
    }
    [SerializeField] private Text difficultyTxt;
    [SerializeField] private Text timeTxt;
    [SerializeField] private Text dateTxt;
}