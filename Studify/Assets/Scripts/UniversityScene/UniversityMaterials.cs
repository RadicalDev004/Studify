using UnityEngine;
using UnityEngine.UI;
public class UniversityMaterials : MonoBehaviour
{
    public Button TheoryB, GrileB;
    public GameObject[] Theory, Grile;
    public bool GrileStatus, TheoryStatus;

    public void OpenCloseTabs(ref bool status, GameObject[] ToManipulate)
    {
        foreach (GameObject go in ToManipulate)
            go.SetActive(!status);
        status = !status;
    }

    private void Awake()
    {
        TheoryB.onClick.AddListener(delegate
        {
            OpenCloseTabs(ref TheoryStatus, Theory);
        });

        GrileB.onClick.AddListener(delegate
        {
            OpenCloseTabs(ref GrileStatus, Grile);
        });
    }
}
