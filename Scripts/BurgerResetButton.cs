using UnityEngine;

public class BurgerResetButton : MonoBehaviour
{
    public BurgerJudgeSystem judgeSystem;

    public void ResetBurger()
    {
        if (judgeSystem != null)
        {
            judgeSystem.ResetBurger();
          
        }
    }
}
