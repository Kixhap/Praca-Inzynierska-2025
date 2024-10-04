using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] GameObject[] Menus;
    [SerializeField] Animator Logo;
    [SerializeField] Animator Lines;
    [SerializeField] AudioSource buttonsound;

    public void ChangeMenu(int x)
    {
        foreach (var menu in Menus)
        {
            menu.SetActive(false);
        }
        Menus[x].SetActive(true);
        buttonsound.Play();
    }
    public void exit()
    {
        Application.Quit();
    }

    public void playAnimation(int id)
    {
        switch (id)
        {
            case 0:
                Logo.SetTrigger("zdd");
                Lines.SetTrigger("zdd");
                break;
            case 1:
                Logo.SetTrigger("ydd");
                Lines.SetTrigger("ydd");
                break;
        default: break;
        }
    }

}

