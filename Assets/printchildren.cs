using UnityEngine;

public class printchildren : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // write names of direct children to text file
        System.IO.StreamWriter file = new System.IO.StreamWriter(gameObject.name + "children.txt");
        int i = 0;
        foreach (Transform child in transform)
        {


            if (i % 2 == 0)
            {
                file.Write('\n');

            }
            else
            {
                file.Write(",");

            }

            file.Write(child.name);


            i += 1;
        }
        file.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
