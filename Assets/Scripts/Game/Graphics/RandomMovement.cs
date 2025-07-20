using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomMovement : MonoBehaviour
{
    /*Tester tester = new Tester();

    private void Start()
    {
        
    }


    private void Update()
    {
        ChangePosition(tester);
        ChangeFloat(ref tester.position2);

        Debug.Log(tester.position1);
        Debug.Log(tester.position2);
    }

    private void ChangePosition(Tester tester)
    {
        tester.position1 += Time.deltaTime;
    }

    private void ChangeFloat(ref float _float)
    {
        _float += Time.deltaTime;
    }*/

    Vector2 a = new Vector2(0, 0);
    Vector2 b;

    Tester testerA = new Tester();
    Tester testerB;

    private void Start()
    {
        /*b = a;
        Debug.Log(b.x);

        a.x = 1f;
        Debug.Log(b.x);

        a= new Vector2(3f, 3f);
        Debug.Log(b.x);*/

        Debug.Log("Tester");

        testerB = testerA;
        Debug.Log(testerB.position1);

        testerA.position1++;
        Debug.Log(testerB.position1);

        testerB.position1++;
        Debug.Log(testerB.position1);

    }

}


public class Tester
{
    public float position1 = 0f;
    public float position2 = 0f;

}